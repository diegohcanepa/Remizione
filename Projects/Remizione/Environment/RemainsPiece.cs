using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    public class RemainsPiece : GameObject, IPoolable
    {
        #region Private fields

        private float arcHeight;
        private Vector2 direction;
        private float duration;
        private float elapsed;
        private Vector2 groundPos;
        private readonly Sprite image = new() { PivotOrigin = RectanglePoint.Center };
        private bool isActive;
        private bool isLaunched;
        private bool isOutOfWalkArea;
        private float launchDelay;
        private GameRoom? room;
        private float rotationSpeed;
        private float speed;
        private Vector2 startPos;
        private Vector2 targetPos;

        #endregion

        #region Private members

        // CalculateNextArc
        private void CalculateNextArc(float minHeight, float maxHeight)
        {
            elapsed = 0;
            arcHeight = Random.Shared.NextSingle() * (maxHeight - minHeight) + minHeight;

            // Duración adaptada a distancias más cortas
            duration = MathF.Max(0.08f, (arcHeight * 0.035f) + (speed * 0.004f) + (Random.Shared.NextSingle() * 0.04f));

            Vector2 tentativeTarget = startPos + (direction * speed * duration);

            if (room?.WalkArea is { } walkArea)
            {
                var bounds = walkArea.Polygon.BoundingRectangleF;

                tentativeTarget.X = MathHelper.Clamp(tentativeTarget.X, bounds.Left + 2, bounds.Right - 2);
                tentativeTarget.Y = MathHelper.Clamp(tentativeTarget.Y, bounds.Top + 2, bounds.Bottom - 2);

                if (!walkArea.Contains(tentativeTarget))
                {
                    if (tentativeTarget.Y < bounds.Top + 15)
                    {
                        direction.Y = Math.Abs(direction.Y);
                        direction.X = -direction.X;
                    }
                    else if (tentativeTarget.Y > bounds.Bottom - 10)
                    {
                        direction.Y = -Math.Abs(direction.Y);
                    }
                    else
                    {
                        direction.X = -direction.X;
                    }

                    int safety = 0;
                    while (!walkArea.Contains(tentativeTarget) && safety < 10)
                    {
                        tentativeTarget = Vector2.Lerp(tentativeTarget, startPos, 0.5f);
                        safety++;
                    }
                }
            }

            targetPos = tentativeTarget;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (isOutOfWalkArea)
                return;

            if (Shadow)
            {
                Vector2 originalPos = image.Position;

                image.Position = new Vector2(groundPos.X, groundPos.Y + 1f);
                image.Color = Colors.Shadow;
                image.Draw(gameTime);

                image.Position = originalPos;
                image.Color = Color.White;
            }

            image.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (!isActive)
                return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!isLaunched)
            {
                launchDelay -= dt;
                if (launchDelay <= 0) isLaunched = true;
                return;
            }

            elapsed += dt;
            float t = MathHelper.Clamp(elapsed / duration, 0, 1);

            groundPos = Vector2.Lerp(startPos, targetPos, t);
            float height = 4 * arcHeight * t * (1 - t);

            image.Position = new Vector2(groundPos.X, groundPos.Y - height);
            image.Rotation += dt * rotationSpeed;

            if (t >= 1)
            {
                startPos = targetPos;

                float speedRestitution = 0.30f + (Random.Shared.NextSingle() * 0.15f);
                float heightRestitution = 0.30f + (Random.Shared.NextSingle() * 0.15f);

                speed *= speedRestitution;
                arcHeight *= heightRestitution;
                rotationSpeed *= 0.5f;

                // Umbrales reducidos acorde a la baja velocidad
                if (arcHeight < 0.5f || speed < 2.5f)
                {
                    isActive = false;

                    if (room?.Walls != null)
                    {
                        for (var i = 0; i < room.Walls.Count; i++)
                        {
                            if (room.Walls[i].ContainsVertex(image.BoundingBox))
                            {
                                isOutOfWalkArea = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    CalculateNextArc(arcHeight * 0.75f, arcHeight);
                }
            }
        }

        #endregion

        // Image
        public AtlasImage? Image
        {
            get => image.RenderImage;
            set => image.RenderImage = value;
        }

        // Launch
        public void Launch(GameThing owner)
        {
            room = owner.Session.Room;
            if (room == null)
                return;

            startPos = new Vector2(owner.X, owner.Y);
            groundPos = startPos;

            float angle = Random.Shared.NextSingle() * MathF.Tau;
            float flattenFactor = 0.35f + (Random.Shared.NextSingle() * 0.25f);
            direction = new Vector2(MathF.Cos(angle), MathF.Sin(angle) * flattenFactor);

            // Velocidad reducida ~50% (rango previo 50-90, ahora 22-42)
            speed = Random.Shared.NextSingle() * 20f + 22f;
            rotationSpeed = (Random.Shared.NextSingle() * 10f) - 5f;

            isActive = true;
            isLaunched = false;
            isOutOfWalkArea = false;

            launchDelay = Random.Shared.NextSingle() * 0.12f;

            // Altura inicial ligeramente recortada para no perder proporción visual en 135p
            CalculateNextArc(4, 9);
        }

        // Opacity
        public float Opacity
        {
            get => image.Opacity;
            set => image.Opacity = value;
        }

        // Reset
        public void Reset()
        {
            Image = null;
            Scale = Vector2.One;
            Shadow = false;
        }

        // Scale
        public Vector2 Scale
        {
            get => image.Scale;
            set => image.Scale = value;
        }

        // Shadow
        public bool Shadow { get; set; }
    }
}