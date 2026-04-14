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

        private readonly BrokenPieces brokenPieces;
        private float depth;
        private float floorY;               // Cuánto se frena en horizontal al chocar
        private readonly float gravity;     // gravedad base
        private GameThing? ignoreThing;
        private readonly Vector2 initialVelocity;     // gravedad base
        private bool isGrounded;
        private object? lastThingCollisioned;
        private readonly Actor owner;
        private readonly Polygon testPoly = new();
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
            this.brokenPieces = new BrokenPieces(prop);

            var animation = AddAnimation(AnimationNames.Default);
            animation.AddFrame(prop.DeclaredName, 1000);
        }

        #endregion

        #region Private members

        // Break
        private void Break()
        {
            if (Prop.DeathSound != null)
                PlaySound(Prop.DeathSound);

            RenderLayer = RenderLayer.Background;
            velocity = Vector2.Zero;
            DepthOffset = 0;
            isGrounded = true;
            Prop.Position = this.Position;
            brokenPieces.Launch();
        }

        // CheckCollision
        private GameThing? CheckCollision(bool appyDamage)
        {
            if (Room == null || Prop.Definition == null)
                return null;

            for (var i = 0; i < Room.Walls.Count; i++)
            {
                if (Room.Walls[i] != lastThingCollisioned && Room.Walls[i].Contains(Position))
                {
                    lastThingCollisioned = Room.Walls[i];
                    Break();
                    return null;
                }
            }

            for (var i = 0; i < Room.CulledThings.Count; i++)
            {
                if (Room.CulledThings[i] == this || Room.CulledThings[i] == owner || Room.CulledThings[i] == ignoreThing)
                    continue;

                if (Room.CulledThings[i] is GameThing target && target.CanBeHit && target.CollisionDetection && target != lastThingCollisioned && !target.IsDead)
                {
                    if (target.HitTest(Position))
                    {
                        if (lastThingCollisioned == null && appyDamage)
                        {
                            EffectDescriptor.Apply(Prop.Definition.EffectDescriptors, owner, target, EffectContext.Contact);
                            Break();
                        }

                        return target;
                    }
                }
            }

            return null;
        }

        // UpdateFloorCollision
        private void UpdateFloorCollision()
        {
            if (isGrounded)
                return;

            if (Position.Y >= floorY)
            {
                Position = new Vector2(Position.X, floorY);
                Break();
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (isGrounded)
                brokenPieces.Draw(gameTime);
            else
                base.OnDraw(gameTime);
        }

        // OnUnload
        protected override void OnUnload()
        {
            base.OnUnload();
            brokenPieces.Dispose();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (isGrounded)
            {
                brokenPieces.Update(gameTime);
                return;
            }

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Apply gravity
            if (!isGrounded)
                velocity += new Vector2(0, gravity * weight * dt);

            // Movement
            Position += velocity * dt;

            // Floor collision
            UpdateFloorCollision();

            // Object collision
            CheckCollision(true);
        }

        #endregion

        // Depth
        public override float Depth => depth;

        // Launch
        public void Launch()
        {
            if (owner.Room == null || owner.GetActiveThrowablePosition() == null)
                return;

            this.ignoreThing = null;
            this.isGrounded = false;
            this.velocity = initialVelocity;
            this.lastThingCollisioned = null;
            this.RenderLayer = RenderLayer.Default;

            depth = owner.Depth + .01f;

            this.Position = owner.GetActiveThrowablePosition() ?? Vector2.Zero;
            this.floorY = owner.Y - (BoundingBox.Height / 2);

            if (owner.IsFlippedHorizontally)
                velocity.X *= -1;

            owner.Room.Children.Add(this);

            ignoreThing = CheckCollision(false);
            var y = float.MinValue;
            if (ignoreThing is IHoleArea holeArea)
            {
                testPoly.SetVertices(holeArea.Polygon.GetVertices(), -2);

                for (var i = 0; i < testPoly.Vertices.Count; i++)
                {
                    if (owner.IsFlippedHorizontally)
                    {
                        if (testPoly.Vertices[i].X > owner.X)
                            continue;
                    }
                    else if (testPoly.Vertices[i].X < owner.X)
                    {
                        continue;
                    }

                    if (testPoly.Vertices[i].Y > y)
                        y = testPoly.Vertices[i].Y;
                }

                if (owner.Y <= y)
                    ignoreThing = null;
            }
        }

        // Prop
        public Prop Prop { get; }
    }
}
