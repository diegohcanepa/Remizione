using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// HUD
    /// </summary>
    public sealed class HUD : GameObject, IInputHandler
    {
        #region Private fields

        private readonly GadgetSlot gadgetSlot;
        private readonly UIHealthMeter healthMeter;
        private readonly JunkSlot junkSlot;
        private readonly SackSlot sackSlot;
        private readonly GameSession session;
        private readonly UITicketMeter ticketMeter;
        private readonly TrinketSlot trinketSlot;

        #endregion

        #region Constructor

        // Constructor
        public HUD(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            this.healthMeter = new(session.Game);
            this.Log = new(Game);
            this.Message = new(Game);
            this.TargetMeter = new(Game);

            // Junk slot
            this.junkSlot = new(session);

            // Gadget slot
            this.gadgetSlot = new(session);

            // Sack slot
            this.sackSlot = new(session);

            // Ticket meter
            this.ticketMeter = new(Game);

            // Trincket slot
            this.trinketSlot = new(session);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (session.IsCurrentScene)
            {
                sackSlot.Draw(gameTime);
                if (session.IsRunInProgress)
                    TargetMeter.Draw(gameTime);
                Log.Draw(gameTime);
                Message.Draw(gameTime);
            }

            junkSlot.Draw(gameTime);
            gadgetSlot.Draw(gameTime);
            trinketSlot.Draw(gameTime);
            healthMeter.Draw(gameTime);
            ticketMeter.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            TargetMeter.Update(gameTime);
            sackSlot.Update(gameTime);
            junkSlot.Update(gameTime);
            gadgetSlot.Update(gameTime);
            trinketSlot.Update(gameTime);
            healthMeter.Update(gameTime);

            Log.Update(gameTime);
            Message.Update(gameTime);

            if (session.Player != null)
            {
                ticketMeter.Value = session.Tickets;
                ticketMeter.Update(gameTime);
            }
        }

        #endregion

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (session.IsAwaiting)
                return HandleInputResult.Unhandled;

            if (session.IsConsoleVisible || session.GameplayMode == GameplayMode.Adventure)
                return HandleInputResult.Unhandled;

            if (junkSlot.HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            if (gadgetSlot.HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            if (sackSlot.HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            return HandleInputResult.Unhandled;
        }

        // Log
        public UILog Log { get; }

        // Message
        public HUDMessage Message { get; }

        // Reset
        public void Reset()
        {
            healthMeter.Actor = session.Player;
        }

        // TargetMeter
        public UITargetMeter TargetMeter { get; }
    }
}
