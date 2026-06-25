using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// UIGooMeter
    /// </summary>
    public sealed class UIGooMeter : GameObject
    {
        private enum GooMeterPart { TopEmpty, MiddleEmpty, BottomEmpty, TopFilled, MiddleFilled, BottomFilled };

        #region Private fields

        private const float fillSpeed = 8;
        private readonly List<Sprite> parts = [];
        private readonly Vector2 position = new(6, 2);
        private float visualValue;

        #endregion

        #region Private members

        // Refresh
        private void Refresh()
        {
            parts.Clear();

            if (Actor == null)
                return;

            visualValue = Actor.Energy;

            while (parts.Count < Actor.MaxEnergy)
            {
                parts.Add(new Sprite(Atlases.UI.GooMeter[(int)GooMeterPart.MiddleEmpty]) { X = position.X });
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (Actor == null || parts.Count == 0)
                return;

            Game.SpriteBatch.Begin(Game.Camera);

            float currentLocalY = 0;

            for (int i = 0; i < parts.Count; i++)
            {
                var segment = parts[i];

                // Invertimos la evaluación: los índices más altos (el fondo) se llenan primero.
                int fillIndex = parts.Count - 1 - i;
                bool isSegmentFilled = visualValue > fillIndex;

                // Determinar la parte del enum correspondiente
                GooMeterPart part;

                if (i == 0)
                {
                    part = isSegmentFilled ? GooMeterPart.TopFilled : GooMeterPart.TopEmpty;
                }
                else if (i == parts.Count - 1)
                {
                    part = isSegmentFilled ? GooMeterPart.BottomFilled : GooMeterPart.BottomEmpty;
                }
                else
                {
                    part = isSegmentFilled ? GooMeterPart.MiddleFilled : GooMeterPart.MiddleEmpty;
                }

                segment.RenderImage = Atlases.UI.GooMeter[(int)part];
                segment.Y = position.Y + currentLocalY;
                segment.Draw(gameTime);

                currentLocalY += segment.BoundingBox.Height - 1;
            }

            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Actor == null)
                return;

            if (Actor.MaxEnergy != parts.Count)
                Refresh();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float targetValue = Actor.Energy;

            if (visualValue != targetValue)
            {
                if (visualValue < targetValue)
                {
                    visualValue = Math.Min(visualValue + (fillSpeed * dt), targetValue);
                }
                else
                {
                    visualValue = Math.Max(visualValue - (fillSpeed * dt), targetValue);
                }
            }
        }

        #endregion

        // Actor
        public Actor? Actor
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    if (field != null)
                        Refresh();
                }
            }
        }
    }
}