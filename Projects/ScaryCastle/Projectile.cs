using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// Projectile
    /// </summary>
    public sealed class Projectile : GameThing
    {
        #region Private fields

        private Vector2 direction;
        private readonly List<EffectDescriptor> effects = [];
        private GameThing? emitter;
        private float gravity;
        private bool impactDone;
        private readonly ParticlePopEffect particles = new();
        private Sound? ricochetSound;
        private RectangleF roomBounds;
        private ProjectileTrajectoryType trajectory;
        private float yVelocity;

        #endregion

        // Constructor
        public Projectile(GameSession session)
            : base(session, string.Empty)
        {
        }

        #region Private members

        // CheckImpacts
        private bool CheckImpacts(Vector2 from, Vector2 to)
        {
            if (Room == null)
                return false;

            for (var i = 0; i < Room.Children.Count; i++)
            {
                if (Room.Children[i] is not GameThing thing || thing == this || thing == this.emitter || !thing.CanBeHit || thing.Hotspot.IsEmpty)
                    continue;

                if (!thing.IsDead && thing.IsInViewport)
                {
                    if (thing.RuntimeHotspot.BoundingRectangleF.Intersects(from, to))
                    {
                        if (effects != null)
                            EffectDescriptor.Apply(effects, this, thing, EffectContext.ProjectileHit);
                        Impact();
                        return true;
                    }
                }
            }

            return false;
        }

        // CheckWallImpact
        private bool CheckWallImpact(Vector2 position)
        {
            if (Room == null)
                return false;

            for (var i = 0; i < Room.Walls.Count; i++)
            {
                if (Room.Walls[i].Contains(position))
                {
                    if (ricochetSound != null)
                        PlaySound(ricochetSound);

                    Impact();
                    return true;
                }
            }

            return false;
        }

        // Destroy
        private void Destroy()
        {
            this.effects.Clear();
            this.emitter = null;
            Unparent();
            Session.ObjectPools.Projectiles.Return(this);
        }

        // Impact
        private void Impact()
        {
            particles.Spawn(this.Position, Color.Gray);
            impactDone = true;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (particles.IsActive)
                particles.Draw(gameTime);
            else if (!impactDone)
                base.OnDraw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            // Si por alguna razón la entidad ya se destruyó en este frame, abortamos inmediatamente
            if (Room == null)
                return;

            if (impactDone)
            {
                if (particles.IsActive)
                {
                    particles.Update(gameTime);
                }
                else
                {
                    Destroy();
                }

                return;
            }

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 previousPosition = this.Position;
            Vector2 nextPosition = previousPosition;

            // 1. Cálculo del movimiento simulado (Posición futura)
            if (this.trajectory == ProjectileTrajectoryType.Linear)
            {
                nextPosition += this.direction * this.Speed * deltaTime;
            }
            else if (this.trajectory == ProjectileTrajectoryType.Parabolic)
            {
                float moveX = this.direction.X * this.Speed * deltaTime;

                this.yVelocity += this.gravity * deltaTime;
                float moveY = this.yVelocity * deltaTime;

                nextPosition += new Vector2(moveX, moveY);
            }

            // 2. Control de salida de los límites absolutos de la habitación
            if (!this.roomBounds.Contains(nextPosition))
            {
                Destroy();
                return;
            }

            // 3. Verificación de impactos ambientales (Paredes) PRIMERO
            // Si la posición futura se metió en una pared, muere acá y no procesa daño a entidades ocultas
            if (CheckWallImpact(nextPosition))
                return;

            // 4. Verificación de impactos contra entidades usando el barrido anti-tunneling
            if (CheckImpacts(previousPosition, nextPosition))
                return;

            // 5. Si no chocó con nada, aplicamos el movimiento real de forma segura
            this.Position = nextPosition;
        }

        #endregion

        // Launch
        public void Launch(GameThing emitter, Vector2 spawnPosition, Vector2 direction, ProjectileDescriptor descriptor)
        {
            if (emitter.Room == null)
                return;

            Sprite.ClearAnimations();

            this.Atlas = Atlases.Environment;
            this.emitter = emitter;
            this.trajectory = descriptor.Trajectory;
            this.Position = spawnPosition;
            this.direction = direction != Vector2.Zero ? Vector2.Normalize(direction) : Vector2.Zero;
            this.Speed = descriptor.Speed;
            this.yVelocity = descriptor.InitialYVelocity;
            this.gravity = descriptor.Gravity;
            this.roomBounds = emitter.Room.BoundingBox;
            this.ricochetSound = descriptor.RicochetSound;
            this.impactDone = false;

            var anim = AddAnimation("Default");
            anim.AddFrame("PistolBullet", 1000);

            this.effects.Clear();
            this.effects.AddRange(descriptor.EffectDescriptors);

            emitter.Room.Children.Add(this);
        }
    }
}
