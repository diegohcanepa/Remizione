using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione.UI
{
    /// <summary>
    /// InteractionMenu
    /// </summary>
    public sealed class InteractionMenu : GameObject
    {
        private readonly UIContextMenu contextMenu;
        private RectangleF frame;

        // Constructor
        public InteractionMenu(GameSession session)
            : base(session.Game)
        {
            this.contextMenu = new UIContextMenu(session.Game, session.Camera, Fonts.CommonOutline)
            {
                OptionTextScale = ScaleInfo.InteractionMenu.Option,
                TitleTextScale = ScaleInfo.InteractionMenu.Title,
                //ShowSelector = false
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (Thing == null)
                return;

            Game.SpriteBatch.Begin(Thing.Session.Camera);
            Game.Shapes.DrawRectangle(frame, ColorPalette.Text.DarkRed * .5f);
            Game.SpriteBatch.End();

            contextMenu.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Thing != null)
                contextMenu.Update(gameTime);
        }

        #endregion

        // Hide
        public void Hide()
        {
            Thing = null;
        }

        // Show
        public void Show(GameThing thing)
        {
            this.Thing = thing;
            contextMenu.Clear();
            contextMenu.Title = thing.GetLocalizedDisplayName();
            contextMenu.TitleColor = new(125, 56, 51);
            contextMenu.AddOption("Talk", "Talk");
            contextMenu.AddOption("Attack", "Attack");
            contextMenu.AddOption("Threat", "Threat");
            contextMenu.Position = thing.GetOverheadPosition();
            contextMenu.X -= contextMenu.BoundingBox.Width / 2;
            contextMenu.Y -= contextMenu.BoundingBox.Height + 4;

            var bbox = contextMenu.BoundingBox;
            frame = new(bbox.Left - 1, bbox.Top - 1, bbox.Width + 2, bbox.Height + 2);
        }

        // Thing
        public GameThing? Thing { get; private set; }
    }
}
