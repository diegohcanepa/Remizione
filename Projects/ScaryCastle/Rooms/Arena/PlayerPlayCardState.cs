using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// PlayerPlayCardState
    /// </summary>
    public sealed class PlayerPlayCardState(Arena arena, Card card) : ArenaState(arena)
    {
        private readonly Card _cardToPlay = card;

        // Enter
        public override void Enter()
        {
            base.Enter();

            // Animación visual
            MouseCursor.AnimateClick();
            _cardToPlay.Scale = 1;
            _cardToPlay.Float = true;
            // Mover al slot del jugador
            _cardToPlay.MoveTo(Arena.PlayerCardSlotPosition - new Vector2(_cardToPlay.BoundingBox.Width, 0), 0, false);
            Sound.Play(SoundNames.CardFlap);
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Esperamos a que la carta deje de moverse
            if (!_cardToPlay.IsMoving)
            {
                // Pequeña pausa extra y pasamos a resolver
                if (TimeInState > 0.3f)
                {
                    Arena.TransitionTo(new TurnResolutionState(Arena));
                }
            }
        }
    }
}
