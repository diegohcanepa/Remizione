using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione.UI
{
    /// <summary>
    /// HUDButton
    /// </summary>
    public sealed class HUDButton : GameObject
    {
        private readonly UIControl control;

        // Constructor
        public HUDButton(EngendroGame game, string imageName, string displayText)
            : base(game)
        {
            this.DisplayText = displayText;

            // Bag icon
            this.control = new UIControl(Game)
            {
                DisplayMode = UIControlDisplayMode.ImageOnly,
                ImageName = imageName,
                ImageScale = ScaleInfo.UIElement.Large,
                PivotOrigin = RectanglePoint.LeftBottom
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            control.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            control.Update(gameTime);
        }

        #endregion

        // BoundingBox
        public RectangleF BoundingBox => control.BoundingBox;

        // Container
        public ItemContainer? Container { get; }

        // DisplayText
        public string DisplayText { get; }

        // IsMouseOver
        public bool IsMouseOver => control.IsMouseOver;

        // Position
        public Vector2 Position
        {
            get => control.Position;
            set => control.Position = value;
        }

        // TestPressed
        public bool TestPressed(PlayerIndex playerIndex) => control.TestPressed(playerIndex);
    }
}
