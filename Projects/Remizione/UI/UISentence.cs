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
        private readonly TextSprite sentenceText;

        // Constructor
        public UISentence(EngendroGame game)
            : base(game)
        {
            this.sentenceText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                MaximumWidth = (int)(Screen.NativeWidth * .8f),
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.SafeArea.GetPoint(RectanglePoint.Bottom, 0, -9),
                Scale = ScaleInfo.Text.Large
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            sentenceText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        #endregion

        // IsEmpty
        public bool IsEmpty => sentenceText.IsEmpty;

        // Tag
        public object? Tag { get; set; }

        // Text
        public string? Text
        {
            get => sentenceText.Text;
            set => sentenceText.Text = value;
        }
    }
}
