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
            if (cardUnderMouse != Arena.HoveredCard)
            {
                // Desmarcar anterior
                if (Arena.HoveredCard != null)
                    Arena.HoveredCard.IsHovered = false;

                // Marcar nueva
                Arena.HoveredCard = cardUnderMouse;

                if (Arena.HoveredCard != null)
                {
                    Arena.HoveredCard.IsHovered = true;
                    Sound.Play(SoundNames.UISelectC);
                }
            }
        }

        #endregion

        // Enter
        public override void Enter()
        {
            base.Enter();
            MouseCursor.State = MouseCursorState.Hand;
        }

        // Exit
        public override void Exit()
        {
            MouseCursor.State = MouseCursorState.Wait;

            // Limpiamos hover visual al salir
            if (Arena.HoveredCard != null)
            {
                Arena.HoveredCard.IsHovered = false;
                Arena.HoveredCard = null;
            }
        }

        // HandleInput
        public override HandleInputResult HandleInput(GameTime gameTime)
        {
            // Si el mazo se está moviendo, no permitimos input
            if (Arena.Session.Deck.IsBusy)
                return HandleInputResult.Unhandled;

            UpdateHoverLogic();

            // Detectar Click
            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
            {
                if (Arena.HoveredCard != null)
                {
                    // Transición: Jugar la carta
                    Arena.TransitionTo(new PlayerPlayCardState(Arena, Arena.HoveredCard));
                    return HandleInputResult.Handled;
                }
            }

            return HandleInputResult.Unhandled;
        }
    }
}
