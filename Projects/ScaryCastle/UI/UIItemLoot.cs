using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// UINPCInfo
    /// </summary>
    public sealed class UIItemLoot : GameObject
    {
        private readonly Sprite icon;
        private readonly TextSprite label;

        // Constructor
        public UIItemLoot()
        {
            // Icon
            this.icon = new()
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom, 0, -16),
                Scale = ScaleInfo.UIElement.Medium
            };

            // Label
            this.label = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Green,
                Opacity = .6f,
                PivotOrigin = RectanglePoint.Top,
                Scale = ScaleInfo.Text.Large
            };
        }

        #region Private members

        // Refresh
        private void Refresh()
        {
            if (Actor == null)
                return;

            label.Text = Actor.ItemReward?.DisplayName;
            icon.RenderImage = Actor.ItemReward?.Image;

            if (icon.RenderImage != null)
                label.Position = icon.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, 1);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (Actor == null)
                return;

            Game.SpriteBatch.Begin(Game.Camera);
            icon.Draw(gameTime);
            label.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Actor == null)
                return;

            if (Actor.IsDead)
                Refresh();
        }

        #endregion

        // Actor
        public Actor? Actor
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Refresh();
                }
            }
        }
    }
}
