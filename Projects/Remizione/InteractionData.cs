using Adberration.Scripting;
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

        private readonly CombatCommand combatCommand = new();
        private readonly InteractionCommand[] commandChain;
        private readonly ItemCommand itemCommand = new();
        private readonly LiftCommand liftCommand = new();
        private readonly ScriptCommand scriptCommand = new();
        private readonly ThrowCommand throwCommand = new();

        #endregion

        // Constructor
        public InteractionData(GameSession session)
        {
            this.Session = session;
            this.commandChain = [throwCommand, combatCommand, liftCommand, scriptCommand, itemCommand];
        }

        // CanExecute
        public bool CanExecute => Target != null;

        // Clear
        public void Clear()
        {
            Target = null;
            TargetPosition = Vector2.Zero;
            IsAttack = false;
        }

        // Execute
        public bool Execute()
        {
            if (Session.Player == null || Target == null)
                return false;

            var target = Target;
            var executed = false;

            for (int i = 0; i < commandChain.Length; i++)
            {
                if (commandChain[i].Execute(this, target))
                {
                    executed = true;
                    break;
                }
            }

            Clear();
            return executed;
        }

        // IsAttack
        public bool IsAttack { get; private set; }

        // Prepare
        public void Prepare()
        {
            Clear();

            var context = Session.InteractionContext;
            var target = context.Target;
            var player = Session.Player;
            var heldItem = context.HeldItem;

            if (target == null) return;

            // Filtros iniciales de validación
            if (player?.ActiveThrowable != null && target.Verb != Verb.Attack && !target.IsGoToVerb)
                return;

            if (heldItem != null && !target.IsGoToVerb)
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

            Target = target;
            TargetPosition = target.Position;

            // IsAttack directo
            IsAttack = (heldItem == null && target.Verb == Verb.Attack) ||
                       (heldItem != null && !target.IsGoToVerb && player?.CombatBehavior?.Intents.Find(heldItem.Name) != null);
        }

        // Session
        public GameSession Session { get; }

        // Target
        public GameThing? Target { get; private set; }

        // TargetPosition
        public Vector2 TargetPosition { get; private set; }
    }
}