using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// Meter
    /// </summary>
    public sealed class Meter : GameObject
    {
        #region Private fields

        private enum MeterPart { LeftEmpty, MiddleEmpty, RightEmpty, LeftFilled, MiddleFilled, RightFilled };
        private readonly ReadOnlyCollection<AtlasImage> images;
        private readonly ReadOnlyCollection<Sprite> parts = [];

        #endregion

        #region Constructors

        // Constructor
        public Meter(Vector2 position, MeterColor color)
            : this(position.X, position.Y, color)
        {
        }

        // Constructor
        public Meter(float x, float y, MeterColor color)
        {
            this.Position = new Vector2(x, y);
            this.Color = color;

            this.images = color switch
            {
                MeterColor.Green => Atlases.UI.MeterGreen,
                MeterColor.Orange => Atlases.UI.MeterOrange,
                MeterColor.Purple => Atlases.UI.MeterPurple,
                MeterColor.SkyBlue => Atlases.UI.MeterSkyBlue,
                MeterColor.White => Atlases.UI.MeterWhite,

                _ => throw new NotImplementedException(),
            };

            var list = new List<Sprite>();
            for (var i = 0; i < 10; i++)
            {
                list.Add(new(images[(int)MeterPart.MiddleEmpty]) { X = x, Y = Position.Y });
            }

            parts = new(list);

            Refresh();
        }

        #endregion

        #region Private members

        // Refresh
        private void Refresh()
        {
            for (int i = 0; i < MaximumValue; i++)
            {
                var segment = parts[i];
                bool isSegmentFilled = Value > i;

                MeterPart part;

                if (i == 0)
                {
                    part = isSegmentFilled ? MeterPart.LeftFilled : MeterPart.LeftEmpty;
                }
                else if (i == MaximumValue - 1)
                {
                    part = isSegmentFilled ? MeterPart.RightFilled : MeterPart.RightEmpty;
                }
                else
                {
                    part = isSegmentFilled ? MeterPart.MiddleFilled : MeterPart.MiddleEmpty;
                }

                segment.RenderImage = images[(int)part];
            }

            BoundingBox = new(Position, new(MaximumValue * parts[0].BoundingBox.Width, parts[0].BoundingBox.Height));
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (MaximumValue == 0)
                return;

            for (int i = 0; i < MaximumValue; i++)
            {
                parts[i].Draw(gameTime);
            }
        }

        #endregion

        // BoundingBox
        public RectangleF BoundingBox { get; private set; }

        // Color
        public MeterColor Color { get; }

        // IsFull
        public bool IsFull => Value == MaximumValue;

        // MaximumValue
        public int MaximumValue
        {
            get;
            set
            {
                if (value != field)
                {
                    field = int.Clamp(value, 0, parts.Count);

                    var x = this.Position.X;
                    for (var i = 0; i < field; i++)
                    {
                        parts[i].X = x;
                        parts[i].Y = Position.Y;
                        x += parts[i].BoundingBox.Width - 1;
                    }

                    Refresh();
                }
            }
        }

        // Position
        public Vector2 Position { get; }

        // Value
        public int Value
        {
            get;
            set
            {
                if (value != field && Math.Clamp(value, 0, MaximumValue) != field)
                {
                    field = Math.Clamp(value, 0, MaximumValue);
                    Refresh();
                }
            }
        }
    }
}