using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// ThrownItem
    /// </summary>
    public abstract class ThrownItem : GameThing
    {
        #region Private fields

        private int bounceCount;
        private readonly float bounciness;  // Cuánto rebota (0=sin rebote, 1=rebotar igual de fuerte)
        private bool checkWalkArea;
        private int collectCooldown = -1;
        private float floorY;               // Cuánto se frena en horizontal al chocar
        private readonly float gravity;     // gravedad base
        private readonly float horizontalDamping = 0.7f; // cuánto se reduce X en cada golpe
        private GameThing? ignoreThing;
        private readonly Vector2 initialVelocity;     // gravedad base
        private bool isGrounded;
        private Item? item;
        private GameThing? lastThingCollisioned;
        private readonly int maxBounces = 3;    // gravedad base
        private readonly float radius;      // "tamaño" del objeto en píxeles
        private readonly Vector2Tween scaleTween = new();
        private readonly Polygon testPoly = new();
        private Vector2 velocity;
        private readonly float weight;      // Masa relativa (afecta la gravedad)

        #endregion

        #region Constructor

        // Constructor
        protected ThrownItem(GameSession session, Vector2 initialVelocity, float weight, float bounciness, float gravity, float radius)
            : base(session, string.Empty)
        {
            this.Atlas = Atlases.Environment;
            this.PivotOrigin = RectanglePoint.Center;
            this.initialVelocity = initialVelocity;
            this.weight = weight;
            this.bounciness = bounciness;
            this.gravity = gravity;
            this.radius = radius;

            this.Shadow = new ImageSprite(session.Game)
            {
                Opacity = ColorPalette.ShadowOpacity,
                PivotOrigin = RectanglePoint.Center,
            };
        }

        #endregion

        #region Private members

        // CheckCollision
        private GameThing? CheckCollision(bool applyDamage)
        {
            if (Room == null || item == null)
                return null;

            for (var i = 0; i < Room.CulledThings.Count; i++)
            {
                if (Room.CulledThings[i] == this || Room.CulledThings[i] == item.Owner || Room.CulledThings[i] == ignoreThing)
                    continue;

                if (Room.CulledThings[i] is GameThing target && !target.IgnoreThrowables && target != lastThingCollisioned && !target.IsDead)
                {
                    if (target.HitTest(Position))
                    {
                        if (lastThingCollisioned == null && applyDamage)
                        {
                            item.ApplyDamage(target);
                            if (ImpactSound != null)
                                PlaySound(ImpactSound);
                            velocity = new Vector2(-velocity.X, velocity.Y) * Randomizer.Next(.2f, .5f);
                            lastThingCollisioned = target;
                        }

                        return target;
                    }
                }
            }

            return null;
        }

        // CheckWalkAreaCollision
        private bool CheckWalkAreaCollision()
        {
            if (item?.Owner.Room?.WalkArea is WalkArea walkArea)
            {
                if (Y >= walkArea.Polygon.BoundingRectangleF.Top && !walkArea.Contains(Position))
                {
                    velocity = new Vector2(-velocity.X, velocity.Y) * Randomizer.Next(.2f, .5f);
                    return true;
                }
            }

            return false;
        }

        // ReturnToOwner
        private void ReturnToOwner()
        {
            if (HasParent && item?.Owner is Actor actor)
            {
                Unparent();
                Session.ObjectPools.ReturnThrownItem(this);
                actor.Inventory.Junk.Add(item.Name, 1);
            }
        }

        // UpdateCollectState
        private void UpdateCollectState(GameTime gameTime)
        {
            if (item == null)
                return;

            if (collectCooldown > 0)
            {
                collectCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                if (collectCooldown < 0)
                    collectCooldown = 0;
            }
            else if (scaleTween.IsRunning || DistanceTo(item.Owner) <= 10)
            {
                if (!scaleTween.IsRunning)
                {
                    scaleTween.Start(TweenStyle.Linear, Scale, Vector2.Zero, 100);
                }
                else
                {
                    scaleTween.Update(gameTime);

                    Scale = scaleTween.CurrentValue;

                    if (!scaleTween.IsRunning)
                    {
                        item.Owner.PlaySound(SoundNames.ThrowablePickup);
                        ReturnToOwner();
                    }
                }
            }
        }

        // UpdateFloorCollision
        private void UpdateFloorCollision()
        {
            if (Position.Y >= floorY)
            {
                Position = new Vector2(Position.X, floorY);

                if (MathF.Abs(velocity.Y) > 10f && bounceCount < maxBounces)
                {
                    // Rebote vertical
                    velocity = new Vector2(velocity.X * horizontalDamping, -velocity.Y * bounciness);

                    // Agregar variación a la rotación para que no termine siempre igual
                    float randomTwist = (float)(Randomizer.Random.NextDouble() - 0.5) * 0.4f;
                    Rotation += randomTwist; // pequeño “sacudón”

                    bounceCount++;
                }
                else
                {
                    // Se queda quieto después de usar sus rebotes
                    velocity = Vector2.Zero;
                    collectCooldown = 600;
                    DepthOffset = 0;
                    isGrounded = true;
                }
            }
        }

        #endregion

        #region Protected members

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

            if (scaleTween.IsRunning)
                ReturnToOwner();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Shadow.Image != null)
                Shadow.X = X;

            base.OnUpdate(gameTime);

            // Collect phase
            if (collectCooldown >= 0)
            {
                UpdateCollectState(gameTime);
                return;
            }

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Apply gravity
            if (!isGrounded)
                velocity += new Vector2(0, gravity * weight * dt);

            // Movement
            Position += velocity * dt;

            // Rotation based on horizontal speed (ω = v/r)
            Rotation += (velocity.X / radius) * dt;

            // Floor collision
            UpdateFloorCollision();

            // Object collision
            if (CheckCollision(true) == null)
            {
                if (checkWalkArea && CheckWalkAreaCollision())
                    checkWalkArea = false;
            }
        }

        // Shadow
        protected ImageSprite Shadow { get; }

        #endregion

        // Launch
        public void Launch(Item item)
        {
            this.bounceCount = 0;
            this.checkWalkArea = true;
            this.collectCooldown = -1;
            this.ignoreThing = null;
            this.isGrounded = false;
            this.Rotation = 0;
            this.Scale = Vector2.One;
            this.scaleTween.Stop();
            this.velocity = initialVelocity;
            this.lastThingCollisioned = null;

            if (item.Owner.Room == null)
            {
                ReturnToOwner();
                return;
            }
            
            item.Use();

            this.item = item;
            this.Position = item.Owner.GetThrowableSpawnPosition();
            this.floorY = item.Owner.Y;

            if (item.Owner.IsFlippedHorizontally)
                velocity.X *= -1;

            item.Owner.Room?.Children.Add(this);

            ignoreThing = CheckCollision(false);
            var y = float.MinValue;
            if (ignoreThing is IHoleArea holeArea)
            {
                testPoly.SetVertices(holeArea.Polygon.GetVertices(), -2);

                for (var i = 0; i < testPoly.Vertices.Count; i++)
                {
                    if (item.Owner.IsFlippedHorizontally)
                    {
                        if (testPoly.Vertices[i].X > item.Owner.X)
                            continue;
                    }
                    else if (testPoly.Vertices[i].X < item.Owner.X)
                        continue;

                    if (testPoly.Vertices[i].Y > y)
                        y = testPoly.Vertices[i].Y;
                }

                if (item.Owner.Y <= y)
                    ignoreThing = null;
            }
        }
    }
}
