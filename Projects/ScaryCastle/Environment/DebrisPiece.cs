using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// DebrisPiece
    /// </summary>
    public class DebrisPiece : GameObject, IPoolable
    {
        #region Private fields

        private float arcHeight;
        private Vector2 direction;
        private float duration;
        private float elapsed;
        private readonly Sprite image = new() { PivotOrigin = RectanglePoint.Center };
        private bool isActive;
        private bool isFirstBounce;
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
            arcHeight = Random.Shared.Next((int)minHeight, (int)maxHeight);

            // 5. Duración aleatoria: esto es lo que evita que todas aterricen a la vez
            float baseDuration = isFirstBounce ? 0.3f : 0.15f;
            duration = baseDuration + ((float)Random.Shared.NextDouble() * 0.25f);

            Vector2 tentativeTarget = startPos + (direction * speed * duration);

            if (room?.WalkArea is { } walkArea)
            {
                var bounds = walkArea.Polygon.BoundingRectangleF;

                tentativeTarget.X = MathHelper.Clamp(tentativeTarget.X, bounds.Left + 2, bounds.Right - 2);
                tentativeTarget.Y = MathHelper.Clamp(tentativeTarget.Y, bounds.Top + 2, bounds.Bottom - 2);

                if (!walkArea.Contains(tentativeTarget))
                {
                    // Lógica de rebote simple contra bordes del WalkArea
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
                image.Color = ColorPalette.Shadow;
                image.Y += 1f;
                image.Draw(gameTime);
                image.Y -= 1f;
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

            // Interpolación de posición en "suelo"
            Vector2 groundPos = Vector2.Lerp(startPos, targetPos, t);

            // Parábola de altura
            float height = 4 * arcHeight * t * (1 - t);

            image.Position = new Vector2(groundPos.X, groundPos.Y - height);

            // Usamos la velocidad de rotación calculada en Launch
            image.Rotation += dt * rotationSpeed;

            if (t >= 1)
            {
                if (isFirstBounce)
                {
                    isFirstBounce = false;
                    startPos = targetPos;

                    // 6. Fricción aleatoria para que no todas se deslicen igual al final
                    float friction = 0.15f + ((float)Random.Shared.NextDouble() * 0.25f);
                    speed *= friction;

                    CalculateNextArc(4, 9);
                }
                else
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

            // 1. Variación de ángulo y deformación de perspectiva (Y)
            float angle = (float)(Random.Shared.NextDouble() * Math.PI * 2);
            float flattenFactor = 0.35f + ((float)Random.Shared.NextDouble() * 0.25f);
            direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle) * flattenFactor);

            // 2. Velocidad con rango más amplio
            speed = Random.Shared.Next(35, 65);

            // 3. Rotación única (algunas giran hacia atrás, otras rápido, otras lento)
            rotationSpeed = (float)((Random.Shared.NextDouble() * 12) - 6);

            isFirstBounce = true;
            isActive = true;
            isLaunched = false;
            isOutOfWalkArea = false;

            // 4. Delay de salida más generoso para romper el "bloque" inicial
            launchDelay = (float)Random.Shared.NextDouble() * 0.2f;

            CalculateNextArc(12, 22);
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