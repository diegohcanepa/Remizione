using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// UIArenaHealthMeter
    /// </summary>
    public sealed class UIArenaHealthMeter(EngendroGame game) : GameObject(game)
    {
        #region Private fields

        private Vector2 anchorPosition;
        private readonly List<ImageSprite> hearts = [];
        private int maxHp;
        private int totalHeartCount;

        #endregion

        #region Private members

        // GenerateHearts
        private void GenerateHearts()
        {
            if (totalHeartCount <= 0)
                return;

            // Sprites para obtener dimensiones
            var tempSprite = new ImageSprite(Game, Atlases.UI.HeartFull);
            var w = tempSprite.BoundingBox.Width;
            var h = tempSprite.BoundingBox.Height;

            // LISTA DE COLISIONES: Usamos esto para que no se superpongan demasiado
            var placedRects = new List<RectangleF>();

            for (int i = 0; i < totalHeartCount; i++)
            {
                var heart = new ImageSprite(Game, Atlases.UI.HeartFull)
                {
                    PivotOrigin = RectanglePoint.Center
                };

                // --- ALGORITMO DE NUBE ORGANIZADA ---

                // 1. Calcular posición IDEAL (Simetría base)
                // Intentamos crear una estructura piramidal/nube
                Vector2 idealPos = GetOrganizedOffset(i, w, h);

                // 2. Añadir JITTER (Imperfección)
                // Desplazamos un poquito para que no sea una grilla perfecta
                // +/- 4 pixeles de variación random
                float jitterX = (float)((Random.Shared.NextDouble() * 8.0) - 4.0);
                float jitterY = (float)((Random.Shared.NextDouble() * 6.0) - 3.0);

                Vector2 attemptPos = anchorPosition + idealPos + new Vector2(jitterX, jitterY);

                // 3. Resolución de Superposición (Physics light)
                // Si la posición calculada (con jitter) choca con otro corazón, 
                // la empujamos un poquito hacia afuera hasta que entre.
                attemptPos = SolveOverlap(attemptPos, w, h, placedRects);

                // Guardamos
                placedRects.Add(new RectangleF(attemptPos.X, attemptPos.Y, w, h));
                heart.Position = attemptPos;
                hearts.Add(heart);
            }

            foreach (var heart in hearts)
            {
                heart.Tweens.YTween = FloatTween.Create(TweenStyle.CubicInOut, heart.Y, heart.Y + Random.Shared.Next(-1f, 1f), Random.Shared.Next(400, 700), -1);

                var scaleTween = new Vector2Tween() { StartDelay = 400 };
                scaleTween.Start(TweenStyle.CubicInOut, Vector2.One, Vector2.One * 1.2f, Random.Shared.Next(200, 400), 2);
                heart.Tweens.ScaleTween = scaleTween;
            }
        }

        /// <summary>
        /// Calcula una posición base basada en un índice para formar una estructura
        /// de racimo o pirámide invertida (crece hacia arriba).
        /// </summary>
        private Vector2 GetOrganizedOffset(int index, float w, float h)
        {
            // CONFIGURACIÓN DE ESPACIADO
            float spacingX = w * 0.8f; // Un poco pegados horizontalmente
            float spacingY = h * 0.7f; // Un poco pegados verticalmente (stacking)

            if (index == 0) return new Vector2(-w / 2, -h); // El primero centrado justo arriba del anchor

            // Para los siguientes, alternamos Izquierda / Derecha
            // Nivel 1 (índices 1, 2): A los costados y un poco arriba
            // Nivel 2 (índices 3, 4, 5): Más arriba

            // Lógica de "Capas":
            // Capa 0: 1 corazón
            // Capa 1: 2 corazones
            // Capa 2: 3 corazones...

            int layer = 0;
            int itemsInLayer = 1;
            int currentCount = 0;

            // Buscamos en qué capa cae este índice
            while (index >= currentCount + itemsInLayer)
            {
                currentCount += itemsInLayer;
                itemsInLayer++; // Cada capa superior soporta un corazón más (pirámide)
                layer++;
            }

            // Índice dentro de la capa actual (0..itemsInLayer-1)
            int indexInLayer = index - currentCount;

            // Centramos la capa horizontalmente
            // Si la capa tiene 2 items, offset es: -0.5, 0.5
            // Si la capa tiene 3 items, offset es: -1, 0, 1
            float centerOffset = (itemsInLayer - 1) / 2.0f;
            float xPos = (indexInLayer - centerOffset) * spacingX;

            // La Y sube según la capa (negativo es arriba)
            float yPos = -h - (layer * spacingY);

            // Ajuste fino: Las capas superiores se abren un poco más en abanico
            xPos *= 1.0f + (layer * 0.1f);

            // Centramos el sprite (el origen del sprite suele ser topleft, ajustamos para que xPos sea el centro)
            return new Vector2(xPos - (w / 2), yPos);
        }

        /// <summary>
        /// Intenta corregir la posición si se superpone con los anteriores.
        /// </summary>
        private static Vector2 SolveOverlap(Vector2 pos, float w, float h, List<RectangleF> others)
        {
            Vector2 currentPos = pos;
            var myRect = new RectangleF(currentPos.X, currentPos.Y, w, h);
            bool collided = true;
            int safetyBreak = 0;

            // Hacemos unos pequeños "nudges" (empujones) si hay colisión
            while (collided && safetyBreak < 10)
            {
                collided = false;
                foreach (var other in others)
                {
                    if (myRect.Intersects(other))
                    {
                        collided = true;

                        // Vector de empuje: alejarse del centro del otro corazón
                        Vector2 dir = currentPos - new Vector2(other.X, other.Y);
                        if (dir == Vector2.Zero) dir = new Vector2(0, -1); // Evitar NaN
                        dir.Normalize();

                        // Empujar 2 pixeles en esa dirección
                        currentPos += dir * 2f;
                        myRect = new RectangleF(currentPos.X, currentPos.Y, w, h);
                    }
                }
                safetyBreak++;
            }

            return currentPos;
        }

        // RefreshVisuals
        private void RefreshVisuals()
        {
            int fullHeartsCount = HP / 2;
            bool hasHalfHeart = (HP % 2) == 1;

            // Ordenamos visualmente el update para que se llene de abajo hacia arriba (por índice)
            for (int i = 0; i < hearts.Count; i++)
            {
                if (i < fullHeartsCount)
                    hearts[i].Image = Atlases.UI.HeartFull;
                else if (i == fullHeartsCount && hasHalfHeart)
                    hearts[i].Image = Atlases.UI.HeartHalf;
                else
                    hearts[i].Image = Atlases.UI.HeartEmpty;
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            for (var i = 0; i < hearts.Count; i++)
            {
                hearts[i].Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            for (var i = 0; i < hearts.Count; i++)
            {
                hearts[i].Update(gameTime);
            }
        }

        #endregion

        // HP
        public int HP
        {
            get;
            set
            {
                var clampedValue = MathHelper.Clamp(value, 0, maxHp);
                if (field != clampedValue)
                {
                    field = clampedValue;
                    RefreshVisuals();
                }
            }
        }

        // Prepare
        public void Prepare(Actor actor)
        {
            this.maxHp = actor.MaxHP;
            this.HP = actor.HP;
            this.anchorPosition = actor.GetOverheadPosition();
            totalHeartCount = (maxHp + 1) / 2;

            hearts.Clear();
            GenerateHearts();
            RefreshVisuals();
        }
    }
}