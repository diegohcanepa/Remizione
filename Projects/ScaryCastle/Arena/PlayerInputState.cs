using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// PlayerInputState
    /// </summary>
    public sealed class PlayerInputState(Arena arena) : ArenaState(arena)
    {
        private bool cardPlayed;
        private Card? hoveredCard;

        #region Private members

        // UpdateHoverLogic
        private void UpdateHoverLogic()
        {
            // Busca la carta bajo el mouse (orden inverso para respetar Z-index)
            Card? cardUnderMouse = null;
            var cards = Arena.Session.Deck.DrawnCards;

            for (int i = cards.Count - 1; i >= 0; i--)
            {
                if (cards[i] is Card c && c.IsMouseOver())
                {
                    cardUnderMouse = c;
                    break;
                }
            }

            // Si cambia la carta bajo el mouse
            if (cardUnderMouse != hoveredCard)
            {
                // Desmarcar anterior
                hoveredCard?.IsHovered = false;

                // Marcar nueva
                hoveredCard = cardUnderMouse;

                if (hoveredCard  != null)
                {
                   hoveredCard.IsHovered = true;
                    Sound.Play(SoundNames.UISelectC);
                }
            }
        }

        #endregion

        // Exit
        public override void Exit()
        {
            cardPlayed = false;
            if (hoveredCard != null)
            {
                hoveredCard.IsHovered = false;
                hoveredCard = null;
            }
        }

        // HandleInput
        public override HandleInputResult HandleInput(GameTime gameTime)
        {
            if (cardPlayed || Arena.Session.Deck.IsBusy)
                return HandleInputResult.Unhandled;

            UpdateHoverLogic();

            // Left click
            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
            {
                if (hoveredCard != null)
                {
                    MouseCursor.AnimateClick();
                    Arena.PlayPlayerCard(hoveredCard);
                    cardPlayed = true;
                    return HandleInputResult.Handled;
                }
            }

            return HandleInputResult.Unhandled;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (cardPlayed && hoveredCard != null && !hoveredCard.IsMoving)
                Arena.TransitionTo(new PlayerCardResolutionState(Arena));
        }
    }
}
