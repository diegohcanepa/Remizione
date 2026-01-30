using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// DrawHandState
    /// </summary>
    public sealed class DrawHandState(Arena arena) : ArenaState(arena)
    {
        // Enter
        public override void Enter()
        {
            base.Enter();
            Arena.Session.Deck.DrawHand();
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Arena.Session.Deck.IsBusy)
                Arena.TransitionTo(new PlayerInputState(Arena));
        }
    }
}
