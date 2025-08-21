using Engendro;
using Microsoft.Xna.Framework;
using Remizione.UI;

namespace Remizione
{
    /// <summary>
    /// UITicketsMeter
    /// </summary>
    public sealed class UITicketsMeter : GameObject
    {
        private Actor? actor;
        private readonly UIScore[] tickets;

        // Constructor
        public UITicketsMeter(EngendroGame game)
            : base(game)
        {
            tickets = new UIScore[3];

            tickets[0] = new UIScore(game, Atlases.UI.RedTicket, ColorPalette.Text.Default)
            {
            };

            tickets[1] = new UIScore(game, Atlases.UI.GoldenTicket, ColorPalette.Text.Default)
            {
            };

            tickets[2] = new UIScore(game, Atlases.UI.WhiteTicket, ColorPalette.Text.Default)
            {
            };

            Layout();
        }

        #region Private members

        // ApplyValues
        private void ApplyValues()
        {
            if (actor == null)
                return;

            tickets[0].Score = actor.RedTickets;
            tickets[1].Score = actor.GoldenTickets;
            tickets[2].Score = actor.WhiteTickets;

            if (actor.RedTickets == 0 || actor.GoldenTickets == 0)
                Layout();
        }

        // Layout
        private void Layout()
        {
            if (actor == null)
                return;

            var pos = Screen.HUDArea.GetPoint(RectanglePoint.RightTop, -4, 0);

            tickets[0].Position = pos;

            pos.X -= 12;
            tickets[1].Position = pos;

            pos.X -= 12;
            tickets[2].Position = pos;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (actor == null)
                return;

            tickets[0].Draw(gameTime);
            tickets[1].Draw(gameTime);
            tickets[2].Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            tickets[0].Update(gameTime);
            tickets[1].Update(gameTime);
            tickets[2].Update(gameTime);
            ApplyValues();
        }

        #endregion

        // Actor
        public Actor? Actor
        {
            get => actor;
            set
            {
                if (value != actor)
                {
                    actor = value;
                    ApplyValues();
                    Layout();
                }
            }
        }
    }
}
