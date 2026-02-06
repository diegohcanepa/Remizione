using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    public sealed class UIContextualHealthMeter : GameObject
    {
        #region Private fields

        private Vector2 anchorPosition;
        private int autoHideCooldown;
        private readonly List<ImageSprite> hearts = [];
        private int lastKnownHP;
        private readonly int maxHp;
        private readonly int totalHeartCount;

        #endregion

        #region Constructor

        // Constructor
        public UIContextualHealthMeter(Actor actor)
            : base(actor.Game)
        {
            this.Actor = actor;
            this.maxHp = actor.MaxHP;
            this.HP = actor.HP;

            // Calculamos cantidad fija basada en MaxHP
            totalHeartCount = (maxHp + 1) / 2;
            lastKnownHP = actor.HP;

            // 1. Instanciación única (Object Pooling)
            CreateHeartPool();

            // 2. Estado inicial: Oculto o Visible según prefieras.
            // Lo dejaremos oculto hasta que alguien llame a Show(), o visible si el enemigo empieza quieto.
            // Para asegurar consistencia, llamamos a Show() una vez para posicionar todo.
            Show();
        }

        #endregion

        #region Private members

        // ArrangeHearts
        private void ArrangeHearts()
        {
            if (hearts.Count == 0)
                return;

            // Obtenemos dimensiones de referencia del primer corazón (asumiendo todos iguales)
            float w = hearts[0].BoundingBox.Width;
            float h = hearts[0].BoundingBox.Height;

            // Lista temporal para cálculos de colisión (Structs o objetos ligeros)
            // Es local al método, se limpia sola al terminar el scope, pero mejor ser explícitos.
            var placedRects = new List<RectangleF>(totalHeartCount);

            // Dirección actual del actor
            float dirMultiplier = (Actor.Direction == FacingDirection.Left) ? -1f : 1f;

            for (int i = 0; i < hearts.Count; i++)
            {
                var heart = hearts[i];

                // --- Lógica de Posicionamiento (Copiada y adaptada) ---

                Vector2 idealPos = GetOrganizedOffset(i, w, h);
                idealPos.X *= dirMultiplier; // Espejo según dirección

                // Jitter aleatorio para que cada vez que frenes se vea orgánico y distinto
                float jitterX = (float)((Random.Shared.NextDouble() * 8.0) - 4.0);
                float jitterY = (float)((Random.Shared.NextDouble() * 6.0) - 3.0);

                Vector2 attemptPos = anchorPosition + idealPos + new Vector2(jitterX, jitterY);

                // Resolución de superposición
                attemptPos = SolveOverlap(attemptPos, w, h, placedRects);

                // Guardamos el rect para el siguiente corazón
                placedRects.Add(new RectangleF(attemptPos.X, attemptPos.Y, w, h));

                // --- Asignación de Estado ---

                heart.Position = attemptPos;

                // Reseteamos tweens para dar efecto de "Aparición"

                // 1. Reset Escala para Pop-in
                heart.Scale = Vector2.Zero;
                var scaleTween = new Vector2Tween() { StartDelay = 50 + (i * 20) }; // Pequeño stagger por índice queda bien
                scaleTween.Start(TweenStyle.CubicInOut, Vector2.Zero, ScaleInfo.UIElement.Medium, Random.Shared.Next(200, 350), 1);
                heart.Tweens.ScaleTween = scaleTween;

                // 2. Reset Flotación
                // Forzamos el Y inicial para que el tween no salte bruscamente
                heart.Tweens.YTween = FloatTween.Create(TweenStyle.CubicInOut, attemptPos.Y, attemptPos.Y + Random.Shared.Next(-2f, 2f), Random.Shared.Next(500, 800), -1);
            }
        }

        // CreateHeartPool
        private void CreateHeartPool()
        {
            hearts.Clear();
            for (int i = 0; i < totalHeartCount; i++)
            {
                var heart = new ImageSprite(Game, Atlases.UI.HeartFull)
                {
                    PivotOrigin = RectanglePoint.Center
                };
                hearts.Add(heart);
            }
        }

        // GetOrganizedOffset
        private static Vector2 GetOrganizedOffset(int index, float w, float h)
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

        // RefreshImages
        private void RefreshImages()
        {
            if (hearts.Count == 0) return;

            int fullHeartsCount = HP / 2;
            bool hasHalfHeart = (HP % 2) == 1;

            for (int i = 0; i < hearts.Count; i++)
            {
                var currentImage = hearts[i].Image;
                var targetImage = Atlases.UI.HeartEmpty;

                if (i < fullHeartsCount)
                {
                    targetImage = Atlases.UI.HeartFull;
                }
                else if (i == fullHeartsCount && hasHalfHeart)
                {
                    targetImage = Atlases.UI.HeartHalf;
                }

                hearts[i].Image = targetImage;

                // Solo hacemos tween de golpe si YA estamos visibles y cambia la vida.
                // Si estamos en medio del proceso de Show(), el tween de escala inicial (Pop-in) tiene prioridad.
                // Verificamos hearts[i].Scale.X > 0.1f para saber si ya "apareció".
                if (IsVisible && currentImage != targetImage && hearts[i].Scale.X > 0.1f)
                {
                    // Efecto de latido al recibir daño/curación
                    hearts[i].Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicInOut, ScaleInfo.UIElement.Medium, ScaleInfo.UIElement.Medium * 1.4f, 150, 2);
                }
            }
        }

        // SolveOverlap
        private static Vector2 SolveOverlap(Vector2 pos, float w, float h, List<RectangleF> others)
        {
            // (Misma lógica de antes)
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

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (IsVisible && !Actor.IsDead)
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
            // Sincronización de datos (siempre ocurre, visible o no)
            if (lastKnownHP != Actor.HP)
            {
                this.HP = Actor.HP;
                lastKnownHP = Actor.HP;
            }

            // Actualización visual (solo si visible)
            if (IsVisible)
            {
                for (var i = 0; i < hearts.Count; i++)
                {
                    hearts[i].Update(gameTime);
                }
            }

            if (autoHideCooldown > 0)
            {
                autoHideCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                if (autoHideCooldown <= 0)
                    Hide();
            }
        }

        #endregion

        // Actor
        public Actor Actor { get; }

        // Hide
        public void Hide()
        {
            IsVisible = false;
        }

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
                    RefreshImages();
                }
            }
        }

        // IsVisible
        public bool IsVisible { get; private set; }

        // Show
        public void Show()
        {
            // 1. Actualizar anclaje al Actor (que se movió)
            this.anchorPosition = Actor.RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.Top, 0, -4);

            // 2. Reciclar: Recalcular posiciones usando los objetos existentes
            ArrangeHearts();

            // 3. Actualizar texturas (Lleno/Vacío) según HP actual
            RefreshImages();

            IsVisible = true;

            autoHideCooldown = 8000;
        }
    }
}