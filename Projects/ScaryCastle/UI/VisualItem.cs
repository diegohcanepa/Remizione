using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScaryCastle
{
    /// <summary>
    /// VisualItem
    /// </summary>
    public sealed class VisualItem : GameObject
    {
        private readonly ImageSprite image;

        // Constructor
        public VisualItem(ScaryCastleGame game, Item item)
            : base(game)
        {
            this.Item = item;

            // Image
            this.image = new(Game, item.MetaItem.Image)
            {
                PivotOrigin = RectanglePoint.Center
            };
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            image.Draw(gameTime);
        }

        // BoundingBox
        public RectangleF BoundingBox => image.BoundingBox;

        // Item
        public Item Item { get; }

        // Opacity
        public float Opacity
        {
            get => image.Opacity;
            set => image.Opacity = value;
        }

        // Position
        public Vector2 Position
        {
            get => image.Position;
            set => image.Position = value;
        }

        // Scale
        public Vector2 Scale
        {
            get => image.Scale;
            set => image.Scale = value;
        }
    }
}
