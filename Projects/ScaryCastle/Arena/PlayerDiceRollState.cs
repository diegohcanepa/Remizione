using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// PlayerDiceRollState
    /// </summary>
    public sealed class PlayerDiceRollState(Arena arena, Card card) : ArenaState(arena)
    {
        private readonly Card _card = card;
        private Dice? dice;
        private bool diceRolled;

        // Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Discard card
            if (!diceRolled && TimeInState > .5f)
            {
                diceRolled = true;
                dice = Arena.Session.GetEntity<Dice>("Dice");
                if (dice != null)
                    Arena.PlayerInfo.DiceRollResult = dice.Roll(Arena.PlayerInfo.Actor, _card.Definition.DiceThreshold);
                return;
            }

            if (TimeInState > 3)
            {
                if (dice == null || !dice.IsRolling)
                    Arena.TransitionTo(new PlayerCardResolutionState(Arena));
            }
        }
    }
}