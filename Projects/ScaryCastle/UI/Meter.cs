/*

using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ScaryCastle
{
    /// <summary>
    /// Meter
    /// </summary>
    public sealed class Meter : GameObject
    {
        #region Private fields

        private enum MeterPart { LeftEmpty, MiddleEmpty, RightEmpty, LeftFilled, MiddleFilled, RightFilled };
        private readonly ReadOnlyCollection<AtlasImage> images;
        private readonly List<Sprite> parts = [];

        #endregion

        // Constructor
        public Meter(float x, float y, RunModifierKind modifierKind)
        {
            if (modifierKind == RunModifierKind.Darkness)
                this.images = new(Atlases.UI.PoisonMeter);
            else
                throw new InvalidOperationException();

            for (var i = 0; i < MaximumValue; i++)
            {
                var part = new Sprite(images[(int)MeterPart.MiddleEmpty]) { X = x, Y = y };
                parts.Add(part);
                x += part.BoundingBox.Width - 1;
            }

            Refresh();

            Value = 3;
        }

        #region Private members

        // Refresh
        private void Refresh()
        {
            for (int i = 0; i < parts.Count; i++)
            {
                var segment = parts[i];
                bool isSegmentFilled = Value > i;

                MeterPart part;

                if (i == 0)
                {
                    part = isSegmentFilled ? MeterPart.LeftFilled : MeterPart.LeftEmpty;
                }
                else if (i == parts.Count - 1)
                {
                    part = isSegmentFilled ? MeterPart.RightFilled : MeterPart.RightEmpty;
                }
                else
                {
                    part = isSegmentFilled ? MeterPart.MiddleFilled : MeterPart.MiddleEmpty;
                }

                segment.RenderImage = images[(int)part];
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            for (int i = 0; i < parts.Count; i++)
            {
                var segment = parts[i];
                bool isSegmentFilled = Value > i;

                // Determinar la parte del enum correspondiente
                MeterPart part;

                if (i == 0)
                {
                    part = isSegmentFilled ? MeterPart.LeftFilled : MeterPart.LeftEmpty;
                }
                else if (i == parts.Count - 1)
                {
                    part = isSegmentFilled ? MeterPart.RightFilled : MeterPart.RightEmpty;
                }
                else
                {
                    part = isSegmentFilled ? MeterPart.MiddleFilled : MeterPart.MiddleEmpty;
                }

                segment.RenderImage = images[(int)part];
                segment.Draw(gameTime);
            }
        }

        #endregion

        // IsFull
        public bool IsFull => Value == MaximumValue;

        // MaximumValue
        public int MaximumValue { get; } = 10;

        // Value
        public int Value
        {
            get;
            set
            {
                if (Math.Clamp(value, 0, MaximumValue) != field)
                {
                    field = Math.Clamp(value, 0, MaximumValue);
                    Refresh();
                }
            }
        }
    }
}

*/