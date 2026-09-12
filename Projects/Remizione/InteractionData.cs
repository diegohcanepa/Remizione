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
        public bool CanExecute => Session.Player != null && Target != null && activeCommand != null;

        // Clear
        public void Clear()
        {
            Target = null;
            TargetPosition = Vector2.Zero;
            activeCommand = null;
        }

        // Execute
        public void Execute()
        {
            var player = Session.Player;

            if (!CanExecute || player == null || Target == null)
                return;

            activeCommand!.Execute(this, player, Target);

            Clear();
        }

        // IsAttack
        public bool IsAttack => activeCommand == combatCommand;

        // Prepare
        public void Prepare()
        {
            Clear();

            var context = Session.InteractionContext;
            var heldItem = context.HeldItem;

            if (context.Target is not GameThing target || Session.Player is not Actor player)
                return;

            if (player.ActiveThrowable != null && target.Verb != Verb.Attack && !target.IsGoToVerb)
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

            for (int i = 0; i < commandChain.Length; i++)
            {
                if (commandChain[i].CanExecute(this, player, target))
                {
                    activeCommand = commandChain[i];
                    break;
                }
            }
        }

        // Session
        public GameSession Session { get; }

        // Target
        public GameThing? Target { get; private set; }

        // TargetPosition
        public Vector2 TargetPosition { get; private set; }
    }
}