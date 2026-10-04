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
        private readonly ItemCommand itemCommand = new();
        private readonly LiftCommand liftCommand = new();
        private readonly ScriptOutcomeCommand scriptCommand = new();
        private readonly ThrowHelpPropCommand throwCommand = new();

        #endregion

        // Constructor
        public InteractionData()
        {
            this.commandChain = [throwCommand, combatCommand, liftCommand, scriptCommand, itemCommand];
        }

        #region Private members

        // ApproachAndExecute
        private void ApproachAndExecute(Actor player, GameThing target)
        {
            ApproachBehavior? behavior = this.IsAttack || player.HeldProp != null ? ApproachBehavior.ClosestSide : null;

            var destination = target.GetApproachPosition(player, behavior);

            var fastMove = this.IsAttack || Vector2.Distance(player.Position, destination) > GameSettings.WalkThreshold;
            var moveToResult = destination == Vector2.Zero ? MoveToResult.NoPath : player.MoveTo(destination, fastMove);

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

            if (player.HeldProp != null)
                verb = Verb.Attack;

            var heldItem = player.Session.InteractionContext.HeldItem;
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
            if (player.IsDead || Target == null || activeCommand == null)
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
            if (context.Target == null || (context.Target is Actor actor && actor.HeldProp != null))
            {
                Clear();
                var destination = InputManager.DefaultPlayer.Mouse.WorldPosition(player.Session.Camera);
                var fastMove = Vector2.Distance(player.Position, destination) > GameSettings.WalkThreshold;
                player.MoveTo(destination, fastMove);
                return;
            }

            // 2. Prepare interaction
            Prepare(player, context.Target, context.Target.Verb);

            if (Target == null || activeCommand == null)
            {
                MouseCursor.Shake();
                return;
            }

            // In-place action?
            bool executeInPlace = (player.HeldProp != null) ||
                                  (context.HeldItem == null && Target == player) ||
                                  (Verb == Verb.Examine && context.HeldItem == null) ||
                                  (context.HeldItem?.Definition.ActionKind is ActionKind.Self);

            if (executeInPlace)
                ExecutePending(player);
            else
                ApproachAndExecute(player, Target);
        }

        // ProcessSecondaryAction
        public void ProcessSecondaryAction(Actor player)
        {
            if (player.IsDead)
                return;

            var context = player.Session.InteractionContext;

            Clear();

            if (player.HeldProp != null)
            {
                player.ReleaseHeldProp();
            }
            else if (context.HeldItem == null)
            {
                context.HeldItem = player.Session.PlayerData.Inventory.Find(ItemNames.GadlingKnuckle);
            }
            else
            {
                player.StopMoving();
                context.HeldItem = null;
            }

            Sound.Play(SoundNames.Interact);
        }

        // Target
        public GameThing? Target { get; private set; }

        // TargetPosition
        public Vector2 TargetPosition { get; private set; }

        // Verb
        public Verb Verb { get; private set; }
    }
}