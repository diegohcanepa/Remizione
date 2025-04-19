using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engendro
{
    /// <summary>
    /// Shapes
    /// </summary>
    public sealed class Shapes
    {
        private readonly EngendroGame game;
        private Texture2D? pixel;

        // Constructor
        internal Shapes(EngendroGame game)
        {
            this.game = game;
        }

        // DrawFrame
        public void DrawFrame(Rectangle rect, Color color, int thickness)
        {
            // Top
            DrawRectangle(new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);

            // Bottom
            DrawRectangle(new Rectangle(rect.X, rect.Y + rect.Height - thickness, rect.Width, thickness), color);

            // Left
            DrawRectangle(new Rectangle(rect.X, rect.Y + thickness, thickness, rect.Height - thickness * 2), color);

            // Right
            DrawRectangle(new Rectangle(rect.X + rect.Width - thickness, rect.Y + thickness, thickness, rect.Height - thickness * 2), color);
        }

        // DrawFrame
        public void DrawFrame(RectangleF rect, Color color, float thickness)
        {
            // Top
            DrawRectangle(new RectangleF(rect.X, rect.Y, rect.Width, thickness), color);

            // Bottom
            DrawRectangle(new RectangleF(rect.X, rect.Y + rect.Height - thickness, rect.Width, thickness), color);

            // Left
            DrawRectangle(new RectangleF(rect.X, rect.Y + thickness, thickness, rect.Height - thickness * 2), color);

            // Right
            DrawRectangle(new RectangleF(rect.X + rect.Width - thickness, rect.Y + thickness, thickness, rect.Height - thickness * 2), color);
        }

        // DrawRectangle
        public void DrawRectangle(RectangleF rect, Color color, float rotation = 0)
        {
            game.SpriteBatch.Draw(Pixel, rect.Location, null, color, rotation, Vector2.Zero, new Vector2(rect.Width, rect.Height), SpriteEffects.None, 0);
        }

        // DrawRectangle
        public void DrawRectangle(Rectangle rect, Color color, float rotation = 0)
        {
            game.SpriteBatch.Draw(Pixel, rect, null, color, rotation, Vector2.Zero, SpriteEffects.None, 0);
        }

        // Pixel
        public Texture2D Pixel
        {
            get
            {
                if (pixel == null)
                {
                    pixel = new Texture2D(game.GraphicsDevice, 1, 1);
                    pixel.SetData([Color.White]);
                }

                return pixel;
            }
        }
    }
}
