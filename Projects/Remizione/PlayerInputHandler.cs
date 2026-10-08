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
        private const float ActionWindowDuration = 1.2f; // Tiempo sin clics para resetear el contador
        private const int MaxConsecutiveActions = 3;     // Límite de acciones continuas

        // Constructor
        public PlayerInputHandler(T owner, PlayerIndex playerIndex)
            : base(playerIndex)
        {
            Owner = owner;
        }

        #region Private members

        // ProcessPlayerAction
        private HandleInputResult ProcessPlayerAction(System.Action action)
        {
            // Si el jugador superó el límite de spam, bloqueamos la acción y forzamos fatiga
            if (consecutiveActionCount >= MaxConsecutiveActions)
            {
                consecutiveActionCount = 0;
                Owner.Fatigue(); // Activa BodyFatigueState en el Actor
                return HandleInputResult.Handled;
            }

            // Ejecutamos la acción (ataque, movimiento o interacción)
            action();

            // Incrementamos el contador y reiniciamos la ventana de recuperación
            consecutiveActionCount++;
            actionWindowTimer = ActionWindowDuration;

            return HandleInputResult.Handled;
        }

        #endregion

        // HandleInput
        public override HandleInputResult HandleInput(GameTime gameTime)
        {
            // 1. Temporizador de ventana de recuperación
            if (actionWindowTimer > 0f)
            {
                actionWindowTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (actionWindowTimer <= 0f)
                {
                    consecutiveActionCount = 0; // El jugador hizo una pausa táctica, resetea
                }
            }

            var mouse = InputManager.DefaultPlayer.Mouse;

            // Left button
            if (mouse.IsLeftButtonPressed())
            {
                return ProcessPlayerAction(() => Owner.Session.InteractionData.ProcessPrimaryAction(Owner));
            }

            // Right button
            if (mouse.IsRightButtonPressed())
            {
                return ProcessPlayerAction(() => Owner.Session.InteractionData.ProcessSecondaryAction(Owner));
            }

            if (InputBindings.Map.IsPressed(0))
            {
                Owner.Session.ShowMiniMap();
                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // Owner
        public T Owner { get; }
    }
}