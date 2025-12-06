using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engendro
{
    /// <summary>
    /// ViewportAdapter
    /// </summary>
    public class ViewportAdapter
    {
        #region Private fields

        private readonly EngendroGame game;
        private Matrix transformationMatrix;
        private Matrix transformationMatrixForInput;
        private bool invertScale;

        #endregion

        #region Constructor

        // Constructor
        internal ViewportAdapter(EngendroGame game, int virtualWidth, int virtualHeight)
        {
            this.game = game;

            this.VirtualWidth = virtualWidth;
            this.VirtualHeight = virtualHeight;
            this.VirtualAspectRatio = VirtualWidth / (float)VirtualHeight;

            SetDisplaySize(virtualWidth, virtualHeight);
        }

        #endregion

        #region Private members

        // RecreateTransformationMatrix
        private void RecreateTransformationMatrix()
        {
            var scaleX = (float)DisplayWidth / VirtualWidth;
            var scaleY = (float)DisplayHeight / VirtualHeight;

            if (invertScale)
            {
                Matrix.CreateScale(scaleY, scaleX, 1, out transformationMatrix);
            }
            else
            {
                Matrix.CreateScale(scaleX, scaleY, 1, out transformationMatrix);
            }
        }

        // SetupDestinationRectangle
        private void SetupDestinationRectangle()
        {
            if (DisplayAspectRatio <= VirtualAspectRatio)
            {
                // Output is wider than it is tall, bars left/right
                var presentHeight = (int)((DisplayWidth / VirtualAspectRatio) + .5f);
                var destHeight = Math.Min(presentHeight, DisplayHeight);
                var barHeight = (DisplayHeight - destHeight) / 2;
                DestinationRectangle = new Rectangle(0, barHeight, DisplayWidth, destHeight);
            }
            else
            {
                // Output is taller than it is wider, bars on top/bottom
                var presentWidth = (int)((DisplayHeight * VirtualAspectRatio) + .5f);
                var destWidth = Math.Min(presentWidth, DisplayWidth);
                var barWidth = (DisplayWidth - presentWidth) / 2;
                DestinationRectangle = new Rectangle(barWidth, 0, destWidth, DisplayHeight);
            }

            var scaleX = (float)DestinationRectangle.Width / VirtualWidth;
            var scaleY = (float)DestinationRectangle.Height / VirtualHeight;
            transformationMatrixForInput = Matrix.Invert(Matrix.CreateScale(scaleX, scaleY, 1));
        }

        // SetupVirtualViewport
        private void SetupVirtualViewport()
        {
            // Figure out the largest area that fits in this resolution at the desired aspect ratio
            var width = DisplayWidth;
            var height = (int)((width / DisplayAspectRatio) + .5f);

            if (height > DisplayHeight)
            {
                invertScale = true;
                height = DisplayHeight;
                width = (int)((height * DisplayAspectRatio) + .5f);
            }
            else
            {
                invertScale = false;
            }

            if (game.GraphicsDevice != null)
            {
                game.GraphicsDevice.Viewport = new Viewport
                {
                    X = (DisplayWidth / 2) - (width / 2),
                    Y = (DisplayHeight / 2) - (height / 2),
                    Width = width,
                    Height = height
                };
            }
        }

        #endregion

        // DestinationRectangle
        public Rectangle DestinationRectangle { get; private set; }

        // DisplayAspectRatio
        public float DisplayAspectRatio { get; private set; }

        // DisplayHeight
        public int DisplayHeight { get; private set; }

        // DisplayWidth
        public int DisplayWidth { get; private set; }

        // SetDisplaySize
        public void SetDisplaySize(int width, int height)
        {
            this.DisplayAspectRatio = width / (float)height;
            this.DisplayWidth = width;
            this.DisplayHeight = height;

            SetupVirtualViewport();
            SetupDestinationRectangle();
            RecreateTransformationMatrix();
        }

        // ToVirtual
        public Vector2 ToVirtual(Vector2 displayPosition)
        {
            if (!DestinationRectangle.Contains(displayPosition))
                return Vector2.Zero;

            displayPosition.X -= DestinationRectangle.X;
            displayPosition.Y -= DestinationRectangle.Y;

            return Vector2.Transform(displayPosition, transformationMatrixForInput);
        }

        // ToVirtual
        public Vector2 ToVirtual(Point displayPosition)
        {
            return ToVirtual(displayPosition.ToVector2());
        }

        // TransformationMatrix
        public Matrix TransformationMatrix => transformationMatrix;

        // VirtualAspectRatio
        public float VirtualAspectRatio { get; }

        // VirtualHeight
        public int VirtualHeight { get; }

        // VirtualWidth 
        public int VirtualWidth { get; }
    }
}