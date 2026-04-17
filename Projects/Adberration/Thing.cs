using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;

namespace Adberration
{
    /// <summary>
    /// Thing
    /// </summary>
    public abstract class Thing : Entity
    {
        #region Private Fields

        private Vector2 moveDirection;
        private readonly PathSegment moveSegment = new();

        #endregion

        #region Constructor

        // Constructor
        protected Thing(Session session, string name)
            : base(session, name)
        {
            PivotOrigin = RectanglePoint.Bottom;
            Sprite.SoundEmitter = this;

            // Cache outcome script
            //if (InstanceKind != InstanceKind.Anonymous)
            {
                if (!string.IsNullOrWhiteSpace(name))
                    OutcomeScript = session.ScriptLibrary.FindOutcome(name);

                OutcomeScript ??= session.ScriptLibrary.FindOutcome(DeclaredName);
            }
        }

        #endregion

        #region Private members

        // MoveEndPositionReached
        private bool MoveEndPositionReached()
        {
            return moveSegment.IsEmpty || Vector2.Distance(moveSegment.Start, Position) > Vector2.Distance(moveSegment.Start, moveSegment.End);
        }

        #endregion

        #region Protected members

        // CalculateSpeed
        protected virtual float CalculateSpeed()
        {
            return Speed;
        }

        // OnActivate
        protected virtual void OnActivate()
        {
        }

        // OnDeactivate
        protected virtual void OnDeactivate()
        {
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (Room == null)
                return;

            if (ParallaxDepth == 0)
            {
                base.OnDraw(gameTime);
            }
            else
            {
                var pos = this.Position;

                float parallaxScale = 1f / (1 + ParallaxDepth);
                this.Position = Position - (Session.Camera.Position * (ParallaxFactor * parallaxScale));

                base.OnDraw(gameTime);

                this.Position = pos;
            }
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();

            // If thing has no atlas
            Atlas ??= Session.Room?.Atlas;
        }

        // OnMoveToCompleted
        protected virtual void OnMoveToCompleted()
        {
        }

        // OnStartMoving
        protected virtual void OnStartMoving()
        {
        }

        // OnStopMoving
        protected virtual void OnStopMoving()
        {
        }

        // OnUnload
        protected override void OnUnload()
        {
            StopMoving();
            base.OnUnload();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (IsMoving)
                Sprite.Velocity = moveDirection * CalculateSpeed();

            base.OnUpdate(gameTime);

            if (IsMoving)
            {
                if (!moveSegment.IsEmpty && MoveEndPositionReached())
                {
                    Position = moveSegment.End;
                    moveSegment.Reset();
                    OnMoveToCompleted();

                    // if not reroute
                    //if (moveSegment.IsEmpty)
                    //    StopMoving();
                }
            }
        }

        #endregion

        #region Internal members

        // Activate
        internal void Activate()
        {
            OnActivate();

            for (var i = 0; i < Children.Count; i++)
            {
                Children[i].Activate();
            }
        }

        // Deactivate
        internal void Deactivate()
        {
            OnDeactivate();

            for (var i = 0; i < Children.Count; i++)
            {
                Children[i].Deactivate();
            }
        }

        #endregion

        // Altitude
        [ScriptProperty]
        public float Altitude
        {
            get => Sprite.Altitude;
            set => Sprite.Altitude = value;
        }

        // BottomPosition
        public Vector2 BottomPosition => PivotOrigin == RectanglePoint.Bottom ? Position : BoundingBox.GetPoint(RectanglePoint.Bottom);

        // CanMove
        public virtual bool CanMove => IsInCurrentRoom && Speed > 0;

        // CanParent
        public override bool CanParent(Entity child)
        {
            return child is Thing && base.CanParent(child);
        }

        // Depth
        public override float Depth => BottomPosition.Y + DepthOffset + Altitude;

        // DepthOffset
        [ScriptProperty]
        public int DepthOffset { get; set; }

        // Direction
        [ScriptProperty]
        public FacingDirection Direction
        {
            get => IsFlippedHorizontally ? FacingDirection.Left : FacingDirection.Right;
            set
            {
                if (value == FacingDirection.Right)
                    Sprite.FlipRight();
                else
                    Sprite.FlipLeft();
            }
        }

        // IgnoreCulling
        [ScriptProperty]
        public bool IgnoreCulling { get; set; }

        // IsActiveInGameLoop
        public bool IsActiveInGameLoop => IgnoreCulling || IsInCullingBox || Tweens.IsTweeningPosition || IsMoving;

        // IsInCullingBox
        public virtual bool IsInCullingBox => Session.Camera.CullingBox.Contains(Position) || BoundingBox.Intersects(Session.Camera.CullingBox);

        // IsInCurrentRoom
        [ScriptProperty]
        public bool IsInCurrentRoom => Room != null && Room == Session.Room;

        // IsInViewport
        [ScriptProperty(CodingContext.Execution)]
        public bool IsInViewport => IsInCurrentRoom && RectangleF.Intersects(BoundingBox, Session.Viewport) != RectangleF.Empty;

        // IsMoving
        [ScriptProperty]
        public bool IsMoving => Sprite.Velocity != Vector2.Zero;

        protected virtual Vector2 OnAdjustMoveDirection(Vector2 direction)
        {
            return direction;
        }

        // Move
        public bool Move(Vector2 direction)
        {
            if (!CanMove)
                return false;

            if (!IsMoving && direction == Vector2.Zero)
                return false;

            direction = OnAdjustMoveDirection(direction);

            var previousVelocity = Sprite.Velocity;
            Sprite.Velocity = direction * CalculateSpeed();
            moveDirection = direction;

            if (previousVelocity != Vector2.Zero && Sprite.Velocity == Vector2.Zero)
            {
                OnStopMoving();
            }
            else if (previousVelocity == Vector2.Zero && Sprite.Velocity != Vector2.Zero)
            {
                OnStartMoving();
            }

            return Sprite.Velocity != Vector2.Zero;
        }

        // MoveTo
        public virtual bool MoveTo(Vector2 destination)
        {
            if (!CanMove || destination == Position)
                return false;

            moveSegment.SetPath(Position, destination);

            Move(Vector2.Normalize(destination - Position));

            return true;
        }

        // OutcomeScript
        public Script? OutcomeScript { get; }

        // ParallaxDepth
        [ScriptProperty]
        public float ParallaxDepth { get; set; }

        // ParallaxFactor
        [ScriptProperty]
        public Vector2 ParallaxFactor { get; set; }

        // PerformOutcome
        [ScriptMethod]
        public Script? PerformOutcome()
        {
            if (OutcomeScript == null)
                return null;

            if (OutcomeScript.HasCapability(ScriptCapability.SetTargetEntity))
                OutcomeScript.SetTargetEntity(Name);

            Session.BeginOutcome(OutcomeScript, this);

            return OutcomeScript;
        }

        // Room
        public virtual Room? Room => Parent as Room;

        // Speed
        [ScriptProperty]
        public float Speed
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    if (field == 0)
                        StopMoving();
                }
            }
        }

        // StopMoving
        [ScriptMethod(CodingContext.Any)]
        public void StopMoving()
        {
            Sprite.Velocity = Vector2.Zero;
            moveDirection = Vector2.Zero;
            moveSegment.Reset();
            OnStopMoving();
        }

        // TimeScale
        [ScriptProperty]
        public float TimeScale
        {
            get => Sprite.TimeScale;
            set => Sprite.TimeScale = value;
        }

        // ToString
        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Name) ? "[" + GetType().Name + "]" : Name;
        }

        // Unparent
        public sealed override void Unparent()
        {
            Parent?.Children.Remove(this);
        }
    }
}
