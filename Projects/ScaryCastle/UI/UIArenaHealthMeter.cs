using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// UIArenaHealthMeter
    /// </summary>
    public sealed class UIArenaHealthMeter : GameObject
    {
        #region Private fields

        private Vector2 anchorPosition;
        private readonly List<ImageSprite> hearts = [];
        private int lastKnownHP;
        private readonly int maxHp;
        private readonly int totalHeartCount;

        #endregion

        #region Constructor

        public UIArenaHealthMeter(Actor actor)
            : base(actor.Game)
        {
            this.Actor = actor;

            this.maxHp = actor.MaxHP;
            this.HP = actor.HP;
            this.anchorPosition = actor.BoundingBox.Center;

            if (actor.Direction == FacingDirection.Left)
                this.anchorPosition.X += 6;
            else
                this.anchorPosition.X -= 6;

            anchorPosition.Y -= 15;

            totalHeartCount = (maxHp + 1) / 2;

            hearts.Clear();
            GenerateHearts();
            RefreshVisuals();

            lastKnownHP = actor.HP;
        }

        #endregion

        #region Private members

        // GenerateHearts
        private void GenerateHearts()
        {
            if (totalHeartCount <= 0)
                return;

            var tempSprite = new ImageSprite(Game, Atlases.UI.HeartFull);
            var w = tempSprite.BoundingBox.Width;
            var h = tempSprite.BoundingBox.Height;

            var placedRects = new List<RectangleF>();

            // --- CORRECCIÓN: Lógica invertida ---
            // Antes usamos (Left ? 1 : -1). Como salían "hacia adelante", invertimos los valores.
            // Ahora: Si mira a la Izquierda (-1), los corazones se invierten hacia la Derecha (Atrás).
            // Si mira a la Derecha (1), los corazones se mantienen normales (o se invierten según tu sistema de coordenadas).
            float dirMultiplier = (Actor.Direction == FacingDirection.Left) ? -1f : 1f;

            for (int i = 0; i < totalHeartCount; i++)
            {
                var heart = new ImageSprite(Game, Atlases.UI.HeartFull)
                {
                    PivotOrigin = RectanglePoint.Center
                };

                // 1. Calcular posición IDEAL
                Vector2 idealPos = GetOrganizedOffset(i, w, h);

                // 2. APLICAR ESPEJO (FLIP)
                // Esto invierte la coordenada X de la estructura de la nube para que salga
                // hacia el lado contrario a donde mira el personaje.
                idealPos.X *= dirMultiplier;

                // 3. JITTER
                float jitterX = (float)((Random.Shared.NextDouble() * 8.0) - 4.0);
                float jitterY = (float)((Random.Shared.NextDouble() * 6.0) - 3.0);

                Vector2 attemptPos = anchorPosition + idealPos + new Vector2(jitterX, jitterY);

                // 4. Resolución de Superposición
                attemptPos = SolveOverlap(attemptPos, w, h, placedRects);

                placedRects.Add(new RectangleF(attemptPos.X, attemptPos.Y, w, h));
                heart.Position = attemptPos;
                hearts.Add(heart);
            }

            foreach (var heart in hearts)
            {
                // Tween de flotación vertical
                heart.Tweens.YTween = FloatTween.Create(TweenStyle.CubicInOut, heart.Y, heart.Y + Random.Shared.Next(-1f, 1f), Random.Shared.Next(400, 700), -1);

                // Tween de escala inicial
                var scaleTween = new Vector2Tween() { StartDelay = 400 };
                scaleTween.Start(TweenStyle.CubicInOut, Vector2.One, Vector2.One * 1.2f, Random.Shared.Next(200, 400), 2);
                heart.Tweens.ScaleTween = scaleTween;
            }
        }

        private Vector2 GetOrganizedOffset(int index, float w, float h)
        {
            float spacingX = w * 0.8f;
            float spacingY = h * 0.7f;

            if (index == 0) return new Vector2(-w / 2, -h);

            int layer = 0;
            int itemsInLayer = 1;
            int currentCount = 0;

            while (index >= currentCount + itemsInLayer)
            {
                currentCount += itemsInLayer;
                itemsInLayer++;
                layer++;
            }

            int indexInLayer = index - currentCount;
            float centerOffset = (itemsInLayer - 1) / 2.0f;
            float xPos = (indexInLayer - centerOffset) * spacingX;
            float yPos = -h - (layer * spacingY);

            xPos *= 1.0f + (layer * 0.1f);

            return new Vector2(xPos - (w / 2), yPos);
        }

        private static Vector2 SolveOverlap(Vector2 pos, float w, float h, List<RectangleF> others)
        {
            Vector2 currentPos = pos;
            var myRect = new RectangleF(currentPos.X, currentPos.Y, w, h);
            bool collided = true;
            int safetyBreak = 0;

            while (collided && safetyBreak < 10)
            {
                collided = false;
                foreach (var other in others)
                {
                    if (myRect.Intersects(other))
                    {
                        collided = true;
                        Vector2 dir = currentPos - new Vector2(other.X, other.Y);
                        if (dir == Vector2.Zero) dir = new Vector2(0, -1);
                        dir.Normalize();

                        currentPos += dir * 2f;
                        myRect = new RectangleF(currentPos.X, currentPos.Y, w, h);
                    }
                }
                safetyBreak++;
            }
            return currentPos;
        }

        private void RefreshVisuals()
        {
            int fullHeartsCount = HP / 2;
            bool hasHalfHeart = (HP % 2) == 1;

            for (int i = 0; i < hearts.Count; i++)
            {
                var currentImage = hearts[i].Image;

                if (i < fullHeartsCount)
                {
                    hearts[i].Image = Atlases.UI.HeartFull;
                }
                else if (i == fullHeartsCount && hasHalfHeart)
                {
                    hearts[i].Image = Atlases.UI.HeartHalf;
                }
                else
                {
                    hearts[i].Image = Atlases.UI.HeartEmpty;
                }

                if (currentImage != hearts[i].Image)
                {
                    hearts[i].Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicInOut, hearts[i].Scale, hearts[i].Scale * 1.3f, 300, 2);
                }
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!Actor.IsDead)
            {
                for (var i = 0; i < hearts.Count; i++)
                {
                    hearts[i].Draw(gameTime);
                }
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (lastKnownHP != Actor.HP)
            {
                this.HP = Actor.HP;
                lastKnownHP = Actor.HP;
            }

            for (var i = 0; i < hearts.Count; i++)
            {
                hearts[i].Update(gameTime);
            }
        }

        #endregion

        // Actor
        public Actor Actor { get; }

        // HP
        public int HP
        {
            get;
            private set
            {
                var clampedValue = MathHelper.Clamp(value, 0, maxHp);
                if (field != clampedValue)
                {
                    field = clampedValue;
                    RefreshVisuals();
                }
            }
        }
    }
}