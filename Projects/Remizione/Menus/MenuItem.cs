using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Remizione.Menus
{
    /// <summary>
    /// MenuItem
    /// </summary>
    public sealed class MenuItem : UIComponent
    {
        #region Private members

        private readonly ImageSprite highlightSprite;
        private readonly ImageSprite icon;
        private readonly Action? onPress;
        private readonly TextSprite textSprite;

        #endregion

        #region Constructor

        // Constructor
        public MenuItem(Menu menu, MenuItemName name, AtlasImage? iconImage, Action? onPress)
            : base(menu.Game)
        {
            this.Menu = menu;
            this.Name = name;
            this.onPress = onPress;

            this.icon = new ImageSprite(Game, iconImage) { PivotOrigin = RectanglePoint.Left, Scale = new Vector2(.2f) };
            this.textSprite = new TextSprite(Game, Fonts.Regular)
            {
                PivotOrigin = RectanglePoint.Middle,
                Text = $"@Menu.Items.{Name}"
            };

            highlightSprite = new ImageSprite(Game, Atlases.Menu.MenuItemHighlight)
            {
                Opacity = .5f,
                PivotOrigin = RectanglePoint.Middle
            };
            highlightSprite.Tweens.OpacityTween = FloatTween.Create(TweenStyle.QuadraticInOut, .5f, .8f, 800, -1);

            Invalidate();
        }

        #endregion

        #region Private members

        // DrawText
        private void DrawText(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);

            var color = textSprite.Color;
            var pos = textSprite.Position;
            textSprite.Position += Vector2.One / 2;
            textSprite.Color = Color.Black * .5f;
            textSprite.Draw(gameTime);

            textSprite.Color = color;
            textSprite.Position = pos;
            textSprite.Draw(gameTime);

            Game.SpriteBatch.End();
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (IsSelected)
            {
                Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
                highlightSprite.Position = BoundingBox.Center;
                highlightSprite.Y = BoundingBox.Center.Y - 1;
                highlightSprite.Draw(gameTime);
                Game.SpriteBatch.End();
            }

            if (!icon.IsEmpty)
            {
                Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
                icon.Draw(gameTime);
                Game.SpriteBatch.End();
            }

            DrawText(gameTime);
        }

        // OnInvalidate
        protected override void OnInvalidate()
        {
            textSprite.Tweens.ColorTween = null;

            textSprite.Color = IsSelected ? ColorPalette.MenuItemTextActive : ColorPalette.MenuItemTextInactive;
            textSprite.Scale = IsSelected ? ScaleInfo.MenuItemTextActive : ScaleInfo.MenuItemTextInactive;
            icon.Color = textSprite.Color;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            textSprite.Update(gameTime);

            if (IsSelected)
            {
                highlightSprite.Update(gameTime);
            }

            icon.Position = textSprite.BoundingBox.GetPoint(RectanglePoint.Right, 4, 0);
        }

        #endregion

        // BoundingBox
        public RectangleF BoundingBox => icon.IsEmpty ? textSprite.BoundingBox : RectangleF.Union(icon.BoundingBox, textSprite.BoundingBox);

        // Color
        public Color Color => textSprite.Color;

        // IsSelected
        public bool IsSelected => Menu.SelectedItem == this;

        // Menu
        public Menu Menu { get; }

        // Name
        public MenuItemName Name { get; }

        // Position
        public Vector2 Position
        {
            get => textSprite.Position;
            set
            {
                textSprite.Position = value;
                icon.Position = textSprite.BoundingBox.GetPoint(RectanglePoint.Right, 1, 0);
            }
        }

        // Press
        public void Press()
        {
            onPress?.Invoke();
            Invalidate();
        }
    }
}
