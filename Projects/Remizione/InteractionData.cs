using Adberration;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Remizione.InteractionCommands;

namespace Remizione
{
    /// <summary>
    /// InteractionData
    /// </summary>
    public sealed class InteractionData
    {
        #region Private fields

        private InteractionCommand? activeCommand;
        private readonly CombatCommand combatCommand = new();
        private readonly InteractionCommand[] commandChain;
        private readonly DropCarriedPropCommand endLiftCommand = new();
        private readonly ItemCommand itemCommand = new();
        private readonly LiftCommand liftCommand = new();
        private readonly ScriptCommand scriptCommand = new();
        private readonly ThrowCommand throwCommand = new();

        #endregion

        // Constructor
        public InteractionData()
        {
            this.commandChain = [throwCommand, combatCommand, endLiftCommand, liftCommand, scriptCommand, itemCommand];
        }

        #region Private members

        // ApproachAndExecute
        private void ApproachAndExecute(Actor player, GameThing target)
        {
            ApproachBehavior? behavior = this.IsAttack || player.CarriedProp != null ? ApproachBehavior.ClosestSide : null;
            var destination = target.GetApproachPosition(player, behavior);

            // Ajustes manuales de distancia para objetos levantados y proyectiles
            if (destination != Vector2.Zero)
            {
                if (player.CarriedProp != null)
                {
                    if (target.X < player.X)
                        destination.X += 32;
                    else
                        destination.X -= 32;
                }
                else if (player.Session.InteractionContext.HeldItem?.Definition.ActionKind == ActionKind.Projectile)
                {
                    destination.X = player.X;
                }
            }

            var walkThreshold = this.IsAttack ? 0 : GameSettings.WalkThreshold;
            var moveToResult = destination == Vector2.Zero ? MoveToResult.NoPath : player.MoveTo(destination, walkThreshold);

            if (destination != Vector2.Zero && moveToResult == MoveToResult.NoPath)
            {
                player.FaceTo(target);
                Clear();
            }
            else if (moveToResult == MoveToResult.LessThan1px || destination == Vector2.Zero)
            {
                ExecutePending(player);
            }
        }

        // Prepare
        private void Prepare(Actor player, GameThing target, Verb verb)
        {
            Clear();

            var heldItem = player.Session.InteractionContext.HeldItem;

            if (player.CarriedProp != null)
            {
                if (verb is not Verb.Attack and not Verb.Drop)
                    return;
            }

            if (heldItem != null && !Utils.IsGoToVerb(verb))
            {
                if (player == target)
                {
                    if (heldItem.Definition.ActionKind is ActionKind.Projectile or ActionKind.Proximity)
                        return;
                }
                else if (heldItem.Definition.ActionKind == ActionKind.Self)
                {
                    return;
                }
            }

            this.Target = target;
            this.TargetPosition = target.Position;
            this.Verb = verb;

            for (int i = 0; i < commandChain.Length; i++)
            {
                if (commandChain[i].CanExecute(this, player, target, Verb))
                {
                    activeCommand = commandChain[i];
                    break;
                }
            }
        }

        #endregion

        // Clear
        public void Clear()
        {
            Target = null;
            TargetPosition = Vector2.Zero;
            Verb = Verb.None;
            activeCommand = null;
        }

        // ExecutePending
        public void ExecutePending(Actor player)
        {
            // Al validar las variables locales, el compilador sabe que no son nulas
            if (Target == null || activeCommand == null)
                return;

            if (player.Session.State != GameSessionState.Idle)
                return;

            activeCommand.Execute(this, player, Target, Verb);

            Clear();
        }

        // IsAttack
        public bool IsAttack => activeCommand == combatCommand;

        // ProcessPrimaryAction
        public void ProcessPrimaryAction(Actor player)
        {
            if (player.IsDead)
                return;

            var context = player.Session.InteractionContext;

            // 1. Walk to (no target)
            if (context.Target == null)
            {
                Clear();
                player.EnforceTurn = true;
                var destination = InputManager.DefaultPlayer.Mouse.WorldPosition(player.Session.Camera);
                var walkThreshold = player.HasHostilesNearby() ? 0 : GameSettings.WalkThreshold;
                player.MoveTo(destination, walkThreshold);
                return;
            }

            // 2. Prepare interaction
            Prepare(player, context.Target, context.Target.Verb);

            if (Target == null || activeCommand == null)
            {
                MouseCursor.Shake();
                return;
            }

            // Evaluar si la acción debe ejecutarse en el lugar (InPlace, consumibles, o auto-click)
            bool executeInPlace = (context.HeldItem == null && Target == player) ||
                                  (context.HeldItem?.Definition.ActionKind is ActionKind.InPlace or ActionKind.Self);

            if (executeInPlace)
            {
                ExecutePending(player);
            }
            else
            {
                ApproachAndExecute(player, Target);
            }
        }

        // ProcessSecondaryAction
        public void ProcessSecondaryAction(Actor player)
        {
            if (player.IsDead)
                return;

            var context = player.Session.InteractionContext;

            // Remove held item
            if (context.HeldItem != null)
            {
                player.StopMoving();
                context.HeldItem = null;
                Clear();
                return;
            }

            // Attack
            if (context.Target != null && !context.Target.IsPlayer)
            {
                Prepare(player, context.Target, Verb.Attack);

                if (Target != null && activeCommand != null)
                {
                    ApproachAndExecute(player, Target);
                    return;
                }
            }

            MouseCursor.Shake();
        }

        // Target
        public GameThing? Target { get; private set; }

        // TargetPosition
        public Vector2 TargetPosition { get; private set; }

        // Verb
        public Verb Verb { get; private set; }
    }
}