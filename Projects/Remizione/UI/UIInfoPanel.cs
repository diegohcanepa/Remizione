using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione.UI
{
    /// <summary>
    /// UIInfoPanel
    /// </summary>
    public sealed class UIInfoPanel : GameObject
    {
        private readonly ImageSprite gradient;
        private readonly TextSprite textSprite;
        private readonly TextSprite titleSprite;

        // Constructor
        public UIInfoPanel(EngendroGame game)
            : base(game)
        {
            this.textSprite = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                MaximumWidth = (int)(Screen.NativeWidth * .8f),
                PivotOrigin = RectanglePoint.Top,
                Position = Screen.SafeArea.GetPoint(RectanglePoint.Bottom, 0, -22),
                Scale = ScaleInfo.Text.Large
            };

            this.titleSprite = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Terra,
                MaximumWidth = (int)(Screen.NativeWidth * .8f),
                PivotOrigin = RectanglePoint.Bottom,
                Scale = ScaleInfo.Text.Large
            };

            this.gradient = new(game, Atlases.UI.BottomGradient)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom)
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            
            if (ShowGradient)
                gradient.Draw(gameTime);

            if (!textSprite.IsEmpty)
            {
                titleSprite.Draw(gameTime);
                textSprite.Draw(gameTime);
            }

            Game.SpriteBatch.End();
        }

        #endregion

        // IsEmpty
        public bool IsEmpty => textSprite.IsEmpty;

        // ShowGradient
        public bool ShowGradient { get; set; }

        // Tag
        public object? Tag { get; set; }

        // Text
        public string? Text
        {
            get => textSprite.Text;
            set
            {
                textSprite.Text = value;
                titleSprite.Position = textSprite.BoundingBox.GetPoint(RectanglePoint.Top, 0, 1);
            }
        }

        // Title
        public string? Title
        {
            get => titleSprite.Text;
            set => titleSprite.Text = value;
        }
    }
}
