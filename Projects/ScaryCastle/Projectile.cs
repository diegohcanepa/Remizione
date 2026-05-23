using Engendro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// Projectile
    /// </summary>
    public sealed class Projectile : GameThing
    {
        private Vector2 direction;
        private readonly List<EffectDescriptor> effects = [];
        private GameThing? emitter;
        private float gravity;
        private RectangleF roomBounds;
        private ProjectileTrajectoryType trajectory;
        private float yVelocity;

        // Constructor
        public Projectile(GameSession session)
            : base(session, string.Empty)
        {
        }

        #region Private members

        // ApplyEffectsTo
        private void ApplyEffectsTo(GameThing target)
        {
            if (effects == null)
                return;

            EffectDescriptor.Apply(effects, this, target, EffectContext.ProjectileHit);
        }

        // CheckImpacts
        private void CheckImpacts(Vector2 from, Vector2 to)
        {
            if (Room == null)
                return;

            for (var i = 0; i < Room.Children.Count; i++)
            {
                if (Room.Children[i] is not GameThing thing || thing == this || thing == this.emitter || !thing.CanBeHit || thing.Hotspot.IsEmpty)
                    continue;

                if (thing.RuntimeHotspot.BoundingRectangleF.Intersects(from, to))
                {
                    ApplyEffectsTo(thing);
                    Destroy();
                    break;
                }
            }
        }

        // Destroy
        private void Destroy()
        {
            this.effects.Clear();
            this.emitter = null;
            Unparent();
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 previousPosition = this.Position;

            // 1. Cálculo del movimiento según el tipo de trayectoria
            if (this.trajectory == ProjectileTrajectoryType.Linear)
            {
                this.Position += this.direction * this.Speed * deltaTime;
            }
            else if (this.trajectory == ProjectileTrajectoryType.Parabolic)
            {
                // El avance horizontal (X) sigue siendo constante basado en la dirección
                float moveX = this.direction.X * this.Speed * deltaTime;

                // Aplicamos gravedad al impulso vertical (MonoGame: Y positivo es hacia abajo)
                this.yVelocity += this.gravity * deltaTime;
                float moveY = this.yVelocity * deltaTime;

                this.Position += new Vector2(moveX, moveY);
            }

            // 2. Control de salida de los límites de la habitación
            if (!this.roomBounds.Contains(this.Position))
            {
                Destroy();
                return;
            }

            // 3. Verificación de impactos barriendo el segmento recorrido (Anti-Tunneling)
            CheckImpacts(previousPosition, this.Position);
        }

        #endregion

        // Launch
        public void Launch(GameThing emitter, Vector2 spawnPosition, Vector2 direction, ProjectileDescriptor descriptor)
        {
            Launch(emitter, descriptor.Trajectory, spawnPosition, direction, descriptor.Speed, descriptor.InitialYVelocity, descriptor.Gravity, Atlases.Environment.FindImage(descriptor.ImageName), descriptor.EffectDescriptors);
        }

        // Launch
        public void Launch(GameThing emitter, ProjectileTrajectoryType trajectory, Vector2 spawnPosition, Vector2 direction, float speed, float initialYVelocity, float gravity, AtlasImage? image, IList<EffectDescriptor> effects)
        {
            if (emitter.Room == null)
                return;

            this.Atlas = Atlases.Environment;
            this.emitter = emitter;
            this.trajectory = trajectory;
            this.Position = spawnPosition;
            this.direction = direction != Vector2.Zero ? Vector2.Normalize(direction) : Vector2.Zero;
            this.Speed = speed;
            this.yVelocity = initialYVelocity;
            this.gravity = gravity;
            this.roomBounds = emitter.Room.BoundingBox;

            var anim = AddAnimation("Default");
            anim.AddFrame("PistolBullet", 1000);

            this.effects.Clear();
            this.effects.AddRange(effects);

            emitter.Room.Children.Add(this);
        }
    }
}
