using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle.Menus
{
    /// <summary>
    /// ButtonOption
    /// </summary>
    public abstract class ButtonOption<T> : UIComponent, IOption
    {
        private readonly TextSprite textSprite;

        #region Constructor

        // Constructor
        protected ButtonOption(ScaryCastleGame game, string label)
            : base(game)
        {
            this.Game = game;

            if (TextRepository.IsKeyReference(label))
            {
                // Compose platform specific key
                var key = string.Concat(label.AsSpan(1), ".", EngendroGame.RunningPlatform.ToString());
                if (TextRepository.ContainsKey(key))
                {
                    label = TextRepository.KeyReferenceSymbol + key;
                }
            }

            this.textSprite = new TextSprite(Game, Fonts.Common)
            {
                Color = ColorPalette.TextWhite,
                PivotOrigin = RectanglePoint.Center,
                Scale = ScaleInfo.MenuOption,
                Text = label
            };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            textSprite.Draw(gameTime);
        }

        // OnPerformClick
        protected virtual void OnPerformClick()
        {
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            textSprite.Update(gameTime);
        }

        #endregion

        // Description
        public string Description => string.Empty;

        // Game
        public new ScaryCastleGame Game { get; }

        // IsEnabled
        public bool IsEnabled => true;

        // LabelBoundingBox
        public virtual RectangleF LabelBoundingBox => textSprite.BoundingBox;

        // LeftArrowBoundingBox
        public RectangleF LeftArrowBoundingBox => Rectangle.Empty;

        // NextValue
        public bool NextValue()
        {
            return false;
        }

        // PerformClick
        public void PerformClick()
        {
            OnPerformClick();
        }

        // Position
        public Vector2 Position
        {
            get => textSprite.Position;
            set => textSprite.Position = value;
        }

        // PreviousValue
        public bool PreviousValue()
        {
            return false;
        }

        // RightArrowBoundingBox
        public RectangleF RightArrowBoundingBox => Rectangle.Empty;
    }
}
