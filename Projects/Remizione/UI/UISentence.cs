using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione.UI
{
    /// <summary>
    /// UISentence
    /// </summary>
    public sealed class UISentence : GameObject
    {
        private readonly ImageSprite gradient;
        private readonly TextSprite textSprite;

        // Constructor
        public UISentence(EngendroGame game)
            : base(game)
        {
            this.textSprite = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                MaximumWidth = (int)(Screen.NativeWidth * .8f),
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.SafeArea.GetPoint(RectanglePoint.Bottom, 0, -6),
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

            textSprite.Draw(gameTime);

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
            set => textSprite.Text = value;
        }
    }
}
