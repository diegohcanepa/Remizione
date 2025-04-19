using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace Remizione.UI
{
    /// <summary>
    /// QuickSlots
    /// </summary>
    public sealed class QuickSlots : GameObject, IInputHandler
    {
        private readonly GameSession session;
        private readonly QuickSlot[] slots;

        // Constructor
        public QuickSlots(GameSession session)
            : base(session.Game)
        {
            this.session = session;
            slots = new QuickSlot[4];

            slots[0] = new QuickSlot(session.Game, InputBindings.QuickSlotTop) { Position = new(26, 110) };
            slots[1] = new QuickSlot(session.Game, InputBindings.QuickSlotRight) { Position = new(38, 117) };
            slots[2] = new QuickSlot(session.Game, InputBindings.QuickSlotBottom) { Position = new(26, 123) };
            slots[3] = new QuickSlot(session.Game, InputBindings.QuickSlotLeft) { Position = new(14, 117) };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
            slots[0].Draw(gameTime);
            slots[1].Draw(gameTime);
            slots[2].Draw(gameTime);
            slots[3].Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            slots[0].Update(gameTime);
            slots[1].Update(gameTime);
            slots[2].Update(gameTime);
            slots[3].Update(gameTime);
        }

        #endregion

        // CanHandleInput
        public bool CanHandleInput => session.Player != null;

        // Invalidate
        public void Invalidate()
        {
            //slots[0].Items = session.Player?.Spells;
            //slots[1].Items = session.Player?.Throwables;
            //slots[2].Items = session.Player?.Consumables;
            //?slots[3].Items = session.Player?.Tools;
        }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (slots[0].HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            if (slots[1].HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            if (slots[2].HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            if (slots[3].HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            return HandleInputResult.Unhandled;
        }
    }
}
