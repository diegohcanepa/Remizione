using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// HUDFaithMeter
    /// </summary>
    public sealed class HUDFaithMeter : SessionGameObject<GameSession>
    {
        #region Private fields

        private enum MeterPart { TopEmpty, MiddleEmpty, BottomEmpty, TopFilled, MiddleFilled, BottomFilled };

        private Actor? actor;
        private const float fillSpeed = 8;
        private readonly Sprite icon = new(Atlases.UI.FaithIcon) { PivotOrigin = RectanglePoint.Top };
        private readonly List<Sprite> parts = [];
        private float visualValue;

        #endregion

        // Constructor
        public HUDFaithMeter(GameSession session)
            : base(session)
        {
        }

        #region Private members

        // Refresh
        private void Refresh()
        {
            parts.Clear();

            if (actor == null)
                return;

            visualValue = actor.Energy;

            icon.Position = new(9, 3);

            float x = 6.75f;
            float y = 13;
            while (parts.Count < actor.MaxEnergy)
            {
                var part = new Sprite(Atlases.UI.FaithMeter[(int)MeterPart.MiddleEmpty]) { X = x, Y = y };
                parts.Add(part);
                y += part.BoundingBox.Height - 1;
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (actor == null || parts.Count == 0)
                return;

            Game.SpriteBatch.Begin(Game.Camera);

            for (int i = 0; i < parts.Count; i++)
            {
                var segment = parts[i];

                // Invertimos la evaluación: los índices más altos (el fondo) se llenan primero.
                int fillIndex = parts.Count - 1 - i;
                bool isSegmentFilled = visualValue > fillIndex;

                // Determinar la parte del enum correspondiente
                MeterPart part;

                if (i == 0)
                {
                    part = isSegmentFilled ? MeterPart.TopFilled : MeterPart.TopEmpty;
                }
                else if (i == parts.Count - 1)
                {
                    part = isSegmentFilled ? MeterPart.BottomFilled : MeterPart.BottomEmpty;
                }
                else
                {
                    part = isSegmentFilled ? MeterPart.MiddleFilled : MeterPart.MiddleEmpty;
                }

                segment.RenderImage = Atlases.UI.FaithMeter[(int)part];
                segment.Draw(gameTime);
            }

            icon.Draw(gameTime);

            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Session.Player != actor)
            {
                actor = Session.Player;
                if (actor != null)
                    Refresh();
            }

            if (actor == null)
                return;

            if (actor.MaxEnergy != parts.Count)
                Refresh();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float targetValue = actor.Energy;

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
    }
}