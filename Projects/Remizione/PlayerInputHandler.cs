using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// PlayerInputHandler
    /// </summary>
    public sealed class PlayerInputHandler<T> : InputHandler where T : Actor
    {
        private int consecutiveActionCount;
        private float actionWindowTimer;
        private const float ActionWindowDuration = 1.2f;
        private const int MaxConsecutiveActions = 3;

        // Constructor
        public PlayerInputHandler(PlayerIndex playerIndex)
            : base(playerIndex)
        {
        }

        // HandleInput
        public override HandleInputResult HandleInput(GameTime gameTime)
        {
            if (Owner == null || !Owner.IsInCurrentRoom || Owner.IsDead || Owner.IsFatigued)
                return HandleInputResult.Unhandled;

            // Timer para limpiar el contador si el jugador hace una pausa
            if (actionWindowTimer > 0f)
            {
                actionWindowTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (actionWindowTimer <= 0f)
                {
                    consecutiveActionCount = 0;
                }
            }

            var mouse = InputManager.DefaultPlayer.Mouse;

            bool isPrimary = mouse.IsLeftButtonPressed();
            bool isSecondary = mouse.IsRightButtonPressed();

            if (isPrimary || isSecondary)
            {
                // Evaluamos el límite ANTES de procesar la nueva orden
                if (consecutiveActionCount >= MaxConsecutiveActions)
                {
                    consecutiveActionCount = 0;
                    actionWindowTimer = 0f;
                    Owner.Fatigue(); // Cancela la orden actual y aplica la penalización por spam de movilidad
                    return HandleInputResult.Handled;
                }

                // Ejecutamos la interacción
                if (isPrimary)
                    Owner.Session.InteractionData.ProcessPrimaryAction(Owner);
                else
                    Owner.Session.InteractionData.ProcessSecondaryAction(Owner);

                // Solo incrementamos el contador si la orden resultó en un movimiento activo del Actor
                if (Owner.IsMoving)
                {
                    consecutiveActionCount++;
                    actionWindowTimer = ActionWindowDuration;
                }

                return HandleInputResult.Handled;
            }

            if (InputBindings.Map.IsPressed(0))
            {
                Owner.Session.ShowMiniMap();
                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // Owner
        public T? Owner { get; set; }
    }
}