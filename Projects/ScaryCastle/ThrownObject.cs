using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// ThrownObject
    /// </summary>
    public abstract class ThrownObject : GameThing
    {
        #region Private fields

        private int bounceCount;
        private readonly float bounciness;  // Cuánto rebota (0=sin rebote, 1=rebotar igual de fuerte)
        private CombatIntent? combatIntent;
        private float depth;
        private float floorY;               // Cuánto se frena en horizontal al chocar
        private readonly float gravity;     // gravedad base
        private readonly float horizontalDamping = 0.7f; // cuánto se reduce X en cada golpe
        private GameThing? ignoreThing;
        private readonly Vector2 initialVelocity;     // gravedad base
        private bool isGrounded;
        private object? lastThingCollisioned;
        private readonly int maxBounces = 3;    // gravedad base
        private readonly FloatTween opacityTween = new();
        private GameThing? owner;
        private readonly float radius;      // "tamaño" del objeto en píxeles
        private readonly Vector2Tween scaleTween = new();
        private readonly Polygon testPoly = new();
        private Vector2 velocity;
        private readonly float weight;      // Masa relativa (afecta la gravedad)

        #endregion

        #region Constructor

        // Constructor
        protected ThrownObject(GameSession session, float defaultScale, Vector2 initialVelocity, float weight, float bounciness, float gravity, float radius)
            : base(session, string.Empty)
        {
            this.DefaultScale = new Vector2(defaultScale);
            this.Atlas = Atlases.Environment;
            this.PivotOrigin = RectanglePoint.Center;
            this.initialVelocity = initialVelocity;
            this.IgnoreWalkArea = true;
            this.weight = weight;
            this.bounciness = bounciness;
            this.gravity = gravity;
            this.radius = radius;

            this.Shadow = new ImageSprite(session.Game)
            {
                Opacity = ColorPalette.ShadowOpacity,
                PivotOrigin = RectanglePoint.Center,
                Scale = DefaultScale
            };
        }

        #endregion

        #region Private members

        // CheckCollision
        private GameThing? CheckCollision(bool applyDamage)
        {
            void Bounce()
            {
                if (ImpactSound != null)
                    PlaySound(ImpactSound);
                velocity = new Vector2(-velocity.X, velocity.Y) * RandomHelper.Next(Random.Shared, .2f, .5f);
            }

            if (Room == null || owner == null)
                return null;

            for (var i = 0; i < Room.Walls.Count; i++)
            {
                if (Room.Walls[i] != lastThingCollisioned && Room.Walls[i].Contains(Position))
                {
                    lastThingCollisioned = Room.Walls[i];
                    Bounce();
                    return null;
                }
            }

            for (var i = 0; i < Room.CulledThings.Count; i++)
            {
                if (Room.CulledThings[i] == this || Room.CulledThings[i] == owner || Room.CulledThings[i] == ignoreThing)
                    continue;

                if (Room.CulledThings[i] is GameThing target && !target.IgnoreThrowables && target.CollisionDetection && target != lastThingCollisioned && !target.IsDead)
                {
                    if (target.HitTest(Position))
                    {
                        if (lastThingCollisioned == null && applyDamage)
                        {
                            if (combatIntent != null)
                                EffectDescriptor.Apply(combatIntent.EffectDescriptors, owner, target);
                            Bounce();
                            lastThingCollisioned = target;
                        }

                        return target;
                    }
                }
            }

            return null;
        }

        // ReturnToObjectPool
        private void ReturnToObjectPool()
        {
            if (HasParent)
            {
                Unparent();
                Session.ObjectPools.ReturnThrownObject(this);
                Session.BibleCount++;
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

                if (MathF.Abs(velocity.Y) > 10f && bounceCount < maxBounces)
                {
                    // Rebote vertical
                    velocity = new Vector2(velocity.X * horizontalDamping, -velocity.Y * bounciness);

                    // Agregar variación a la rotación para que no termine siempre igual
                    float randomTwist = (float)(Random.Shared.NextDouble() - 0.5) * 0.4f;
                    Rotation += randomTwist; // pequeño “sacudón”

                    bounceCount++;
                }
                else
                {
                    // Se queda quieto después de usar sus rebotes
                    velocity = Vector2.Zero;
                    DepthOffset = 0;
                    isGrounded = true;
                    opacityTween.Start(TweenStyle.CubicIn, Opacity, 0, 500, ReturnToObjectPool);
                    scaleTween.Start(TweenStyle.CubicIn, Scale, Scale * .3f, 500);
                    
                    Tweens.OpacityTween = opacityTween;
                    Tweens.ScaleTween = scaleTween;
                }
            }
        }

        #endregion

        #region Protected members

        // DefaultScale
        protected Vector2 DefaultScale { get; }

        // ImpactSound
        protected Sound? ImpactSound { get; set; }

        // OnDrawShadow
        protected override void OnDrawShadow(GameTime gameTime)
        {
            Shadow.Y = Y + 1;
            Shadow.Rotation = Rotation;
            Shadow.Draw(gameTime);
        }

        // OnUnload
        protected override void OnUnload()
        {
            base.OnUnload();
            ReturnToObjectPool();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (Shadow.Image != null)
            {
                Shadow.X = X;
                Shadow.Scale = Scale;
                Shadow.Opacity = Opacity * ColorPalette.ShadowOpacity;
            }

            if (isGrounded)
                return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Apply gravity
            if (!isGrounded)
                velocity += new Vector2(0, gravity * weight * dt);

            // Movement
            Position += velocity * dt;

            // Rotation based on horizontal speed (ω = v/r)
            Rotation += velocity.X / radius * dt;

            // Floor collision
            UpdateFloorCollision();

            // Object collision
            CheckCollision(true);
        }

        // Shadow
        protected ImageSprite Shadow { get; }

        #endregion

        // Depth
        public override float Depth => depth;

        // Launch
        public void Launch(Actor owner, CombatIntent combatIntent)
        {
            if (owner.Room == null)
            {
                ReturnToObjectPool();
                return;
            }

            this.combatIntent = combatIntent;
            this.owner = owner;
            this.bounceCount = 0;
            this.ignoreThing = null;
            this.isGrounded = false;
            this.Opacity = 1;
            this.Rotation = 0;
            this.Scale = DefaultScale;
            this.opacityTween.Stop();
            this.scaleTween.Stop();
            this.velocity = initialVelocity;
            this.lastThingCollisioned = null;

            depth = owner.Depth + .01f;

            //this.item = item;
            this.Position = owner.GetThrowableSpawnPosition();
            this.floorY = owner.Y;

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
                        continue;

                    if (testPoly.Vertices[i].Y > y)
                        y = testPoly.Vertices[i].Y;
                }

                if (owner.Y <= y)
                    ignoreThing = null;
            }
        }
    }
}
