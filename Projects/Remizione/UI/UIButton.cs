using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione
{
    /// <summary>
    /// UIButton
    /// </summary>
    public sealed class UIButton : UIControl
    {
        #region Private fields

        private string? imageName;
        private readonly ImageSprite image;

        #endregion

        #region Constructors

        // Constructor
        public UIButton(EngendroGame game, InputBinding? inputBinding = null)
            : base(game, inputBinding)
        {
            this.image = new ImageSprite(game);
        }

        #endregion

        #region Protected members

        // Invalidate
        protected override void Invalidate()
        {
            image.Position = Position;
            image.Image = GetInputBindingImage(ImageName, InputBinding);
            image.Scale = Small ? ScaleInfo.UIElement.Tiny : ScaleInfo.UIElement.Medium;
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (BoundingBox.IsEmpty)
                return;

            if (!image.IsEmpty)
            {
                Effect? shader = null;

                if (IsMouseOver)
                {
                    RemizioneGame.Effects.ColorSaturation.SetColor(.7f, .7f, .7f, 1);
                    shader = RemizioneGame.Effects.ColorSaturation.Effect;
                }

                Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp, shader);
                image.Draw(gameTime);
                Game.SpriteBatch.End();
            }
        }

        #endregion

        // BoundingBox
        public override RectangleF BoundingBox => image.BoundingBox;

        // ImageName
        public string? ImageName
        {
            get => imageName;
            set
            {
                if (value != imageName)
                {
                    imageName = value;
                    Invalidate();
                }
            }
        }

        // PivotOrigin
        public RectanglePoint PivotOrigin
        {
            get => image.PivotOrigin;
            set
            {
                if (value != image.PivotOrigin)
                {
                    image.PivotOrigin = value;
                    Invalidate();
                }
            }
        }
    }
}
