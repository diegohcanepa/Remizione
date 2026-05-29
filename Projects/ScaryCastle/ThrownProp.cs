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
        private GameThing? ignoreThing;
        private readonly Vector2 initialVelocity;     // gravedad base
        private bool isGrounded;
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
            this.debris = new Debris(prop);

            var animation = AddAnimation(AnimationNames.Default);
            animation.AddFrame(prop.GetThrowableImageName(), 1000);
        }

        #endregion

        #region Private members

        // CheckCollision
        private GameThing? CheckCollision(bool appyDamage)
        {
            if (Room == null || Prop.Definition == null)
                return null;

            for (var i = 0; i < Room.CulledThings.Count; i++)
            {
                if (Room.CulledThings[i] == this || Room.CulledThings[i] == owner || Room.CulledThings[i] == ignoreThing)
                    continue;

                if (Room.CulledThings[i] is GameThing target && target.CanBeHit() && !target.IsDead)
                {
                    if (target.RuntimeHotspot.BoundingRectangleF.Intersects(BoundingBox))
                    {
                        if (appyDamage)
                        {
                            EffectDescriptor.Apply(Prop.Definition.EffectDescriptors, owner, target, EffectContext.Contact);
                            Break();
                        }

                        return target;
                    }
                }
            }

            for (var i = 0; i < Room.Walls.Count; i++)
            {
                if (Room.Walls[i].Contains(Position))
                {
                    Break();
                    return null;
                }
            }

            return null;
        }

        // Launch
        private void Launch(bool drop)
        {
            if (owner.Room == null || owner.GetActiveThrowablePosition() == null)
                return;

            this.ignoreThing = null;
            this.isGrounded = false;
            this.velocity = initialVelocity;
            this.RenderLayer = RenderLayer.Default;

            depth = owner.Depth + .01f;

            this.Position = owner.GetActiveThrowablePosition() ?? Vector2.Zero;
            this.floorY = owner.Y;

            if (drop)
            {
                velocity.X = 0;
            }
            else if (owner.IsFlippedHorizontally)
            {
                velocity.X *= -1;
            }

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

            // Floor collision
            UpdateFloorCollision();

            // Object collision
            CheckCollision(true);
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
            Launch(true);
        }

        // Throw
        public void Throw()
        {
            Launch(false);
        }

        // Prop
        public Prop Prop { get; }
    }
}
