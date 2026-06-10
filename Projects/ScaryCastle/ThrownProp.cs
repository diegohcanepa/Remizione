using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ThrownProp
    /// </summary>
    public sealed class ThrownProp : GameThing
    {
        #region Private fields

        private float depth;
        private readonly Debris debris;
        private float floorY;               // Cuánto se frena en horizontal al chocar
        private readonly float gravity;     // gravedad base
        private readonly Vector2 initialVelocity;     // gravedad base
        private bool isGrounded;
        private readonly Actor owner;
        private GameThing? target;
        private Vector2 velocity;
        private readonly float weight;      // Masa relativa (afecta la gravedad)

        #endregion

        #region Constructor

        // Constructor
        public ThrownProp(Actor owner, Prop prop)
            : base(prop.Session, string.Empty)
        {
            this.owner = owner;
            this.Prop = prop;
            this.Atlas = Atlases.Environment;
            this.PivotOrigin = RectanglePoint.Center;
            this.initialVelocity = new(110, -50);
            this.IgnoreWalkArea = true;
            this.weight = .8f;
            this.gravity = 500;
            this.debris = new Debris(prop);

            var animation = AddAnimation(AnimationNames.Default);
            animation.AddFrame(prop.GetThrowableImageName(), 1000);
        }

        #endregion

        #region Private members

        // CheckCollision
        private void CheckCollision()
        {
            if (Prop.Definition == null || target == null)
                return;

            if (target.CanBeHit() && !target.IsDead)
            {
                if (target.RuntimeHotspot.BoundingRectangleF.Intersects(BoundingBox))
                {
                    EffectDescriptor.Apply(Prop.Definition.EffectDescriptors, owner, target, EffectContext.Contact);
                    Break();
                }
            }
        }

        // CheckFloorCollision
        private void CheckFloorCollision()
        {
            if (isGrounded)
                return;

            if (Position.Y >= floorY)
            {
                Position = new Vector2(Position.X, floorY);
                Break();
            }
        }

        // Launch
        private void Launch(GameThing? target)
        {
            if (owner.Room == null || owner.GetActiveThrowablePosition() == null)
                return;

            this.target = target;
            this.isGrounded = false;
            this.velocity = initialVelocity;
            this.RenderLayer = RenderLayer.Default;

            depth = owner.Depth + .01f;

            this.Position = owner.GetActiveThrowablePosition() ?? Vector2.Zero;
            this.floorY = owner.Y;

            if (target == null)
            {
                velocity.X = 0;
            }
            else if (owner.IsFlippedHorizontally)
            {
                velocity.X *= -1;
            }

            owner.Room.Children.Add(this);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (isGrounded)
                debris.Draw(gameTime);
            else
                base.OnDraw(gameTime);
        }

        // OnUnload
        protected override void OnUnload()
        {
            base.OnUnload();
            debris.Release();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (isGrounded)
            {
                debris.Update(gameTime);
                return;
            }

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Apply gravity
            if (!isGrounded)
                velocity += new Vector2(0, gravity * weight * dt);

            // Movement
            Position += velocity * dt;

            // Target collision
            CheckCollision();

            // Floor collision
            CheckFloorCollision();
        }

        #endregion

        // Break
        public void Break()
        {
            if (Prop.DeathSound != null)
                PlaySound(Prop.DeathSound);

            RenderLayer = RenderLayer.Background;
            velocity = Vector2.Zero;
            DepthOffset = 0;
            isGrounded = true;
            Prop.Position = this.Position;
            debris.Launch();

            Session.Camera.Shake(TweenStyle.Linear, Vector2.One, 40, 6);
        }

        // Depth
        public override float Depth => depth;

        // Drop
        public void Drop()
        {
            Launch(null);
        }

        // Throw
        public void Throw(GameThing target)
        {
            Launch(target);
        }

        // Prop
        public Prop Prop { get; }
    }
}
