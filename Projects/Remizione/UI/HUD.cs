using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Remizione.UI;

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
        private readonly LeftHandSlot leftHandSlot;
        private readonly RightHandSlot rightHandSlot;
        private readonly SackSlot sackSlot;
        private readonly GameSession session;
        private readonly UITicketMeter ticketMeter;

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
            this.PlayerSelector = new(session);
            this.TargetMeter = new(Game);

            // Left hand slot
            this.leftHandSlot = new(session);

            // Right hand slot
            this.rightHandSlot = new(session);

            // Gadget slot
            this.gadgetSlot = new(session);

            // Sack slot
            this.sackSlot = new(session);

            // Ticket meter
            this.ticketMeter = new(Game);

            // Mini map
            this.MiniMap = new(Game);
        }

        #endregion

        #region Private members

        // DrawForActionMode
        private void DrawForActionMode(GameTime gameTime)
        {
            if (session.IsCurrentScene)
            {
                ticketMeter.Draw(gameTime);
                sackSlot.Draw(gameTime);
                if (RunManager.HasContent)
                    TargetMeter.Draw(gameTime);
                Log.Draw(gameTime);
                Message.Draw(gameTime);

                if (session.Room is ProceduralRoom)
                    MiniMap.Draw(gameTime);
            }

            leftHandSlot.Draw(gameTime);
            rightHandSlot.Draw(gameTime);
            gadgetSlot.Draw(gameTime);
            healthMeter.Draw(gameTime);
        }

        // DrawForAdventureMode
        private void DrawForAdventureMode(GameTime gameTime)
        {
            if (session.AllowPlayerSelector)
                PlayerSelector.Draw(gameTime);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (session.GameplayMode == GameplayMode.Adventure)
                DrawForAdventureMode(gameTime);
            else
                DrawForActionMode(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (session.GameplayMode == GameplayMode.Action)
            {
                TargetMeter.Update(gameTime);
                sackSlot.Update(gameTime);
                leftHandSlot.Update(gameTime);
                rightHandSlot.Update(gameTime);
                gadgetSlot.Update(gameTime);
                healthMeter.Update(gameTime);
                MiniMap.Update(gameTime);
                Log.Update(gameTime);
                Message.Update(gameTime);

                if (session.Player != null)
                {
                    ticketMeter.Value = session.Tickets;
                    ticketMeter.Update(gameTime);
                }
            }
        }

        #endregion

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (session.IsAwaiting)
                return HandleInputResult.Unhandled;

            if (session.IsConsoleVisible)
                return HandleInputResult.Unhandled;

            if (session.GameplayMode == GameplayMode.Adventure)
            {
                if (session.AllowPlayerSelector)
                    return PlayerSelector.HandleInput(gameTime);
            }
            else
            {
                if (leftHandSlot.HandleInput(gameTime) == HandleInputResult.Handled)
                    return HandleInputResult.Handled;

                if (rightHandSlot.HandleInput(gameTime) == HandleInputResult.Handled)
                    return HandleInputResult.Handled;

                if (sackSlot.HandleInput(gameTime) == HandleInputResult.Handled)
                    return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // Log
        public UILog Log { get; }

        // MiniMap
        public UIMiniMap MiniMap { get; }

        // Message
        public HUDMessage Message { get; }

        // PlayerSelector
        public UIPlayerSelector PlayerSelector { get; }

        // Reset
        public void Reset()
        {
            healthMeter.Actor = session.Player;
            PlayerSelector.Invalidate();
        }

        // TargetMeter
        public UITargetMeter TargetMeter { get; }
    }
}
