using Adberration.Scripting;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// InteractionData
    /// </summary>
    public sealed class InteractionData
    {
        #region Private fields

        private CombatIntent? combatIntent;
        private Item? item;
        private Script? script;
        private readonly GameSession session;
        private Prop? throwable;

        #endregion

        // Constructor
        public InteractionData(GameSession session)
        {
            this.session = session;
        }

        // CanExecute
        public bool CanExecute => combatIntent != null || script != null || item != null || throwable != null;

        // Clear
        public void Clear()
        {
            combatIntent = null;
            item = null;
            script = null;
            Target = null;
            TargetPosition = Vector2.Zero;
            throwable = null;
        }

        // Execute
        public bool Execute()
        {
            if (session.Player == null || Target == null)
                return false;

            var result = false;

            if (combatIntent != null)
            {
                session.Player.ExecuteAction(combatIntent, Target);
                result = true;
            }
            else if (throwable != null && Target.Verb == Verb.Attack)
            {
                session.Player.StopMoving();
                session.Player.ThrowActiveTrowable(Target);
                result = true;
            }
            else if (Target.Verb == Verb.Lift && session.InteractionContext.HeldItem == null)
            {
                if (Target is Prop prop && prop.IsLiftable)
                {
                    session.Player.Lift(prop);
                    result = true;
                }
            }
            else if (script != null)
            {
                session.Player.StopMoving();

                if (Target != null)
                {
                    if (Target.Verb == Verb.PickUp && !session.InventoryEnabled)
                    {
                        session.AwaitRoutine(RoutineNames.NoSack);
                    }
                    else if (Vector2.Distance(Target.Position, TargetPosition) > 1)
                    {
                        session.HUD?.Message.Show(MessageKind.OutOfReach);
                    }
                    else
                    {
                        session.Player.FaceTo(Target);
                        if (script != null)
                        {
                            session.BeginOutcome(script, Target);
                            result = true;
                        }
                    }
                }
            }
            else if (item != null)
            {
                session.Player.ExecuteAction(item, Target);
                result = true;
            }

            Clear();

            return result;
        }

        // IsAttack
        public bool IsAttack => combatIntent != null;

        // Prepare
        public void Prepare()
        {
            Clear();

            var context = session.InteractionContext;

            if (context.Target == null)
                return;

            if (context.Session.Player?.ActiveThrowable != null)
            {
                if (context.Target.Verb != Verb.Attack && !context.Target.IsGoToVerb)
                    return;
            }

            if (context.HeldItem != null && !context.Target.IsGoToVerb)
            {
                if (context.Session.Player == context.Target)
                {
                    if (context.HeldItem.Definition.ActionKind is ActionKind.Projectile or ActionKind.Proximity)
                        return;
                }
                else if (context.HeldItem.Definition.ActionKind == ActionKind.Self)
                {
                    return;
                }
            }

            this.Target = context.Target;
            this.TargetPosition = context.Target.Position;

            if (context.HeldItem == null)
            {
                if (context.Session.Player?.ActiveThrowable is Prop activeThrowable && !Target.IsGoToVerb)
                {
                    this.throwable = activeThrowable;
                }
                else if (Target.Verb == Verb.Attack)
                {
                    // TODO: update here if player can use different intents.
                    this.combatIntent = context.Session.Player?.CombatBehavior?.Intents[0];
                }
                else if (Target.Verb == Verb.Lift)
                {
                    this.throwable = Target as Prop;
                }
                else
                {
                    this.script = Target.OutcomeScript;
                }
            }
            else
            {
                if (Target.IsGoToVerb)
                {
                    this.script = Target.OutcomeScript;
                }
                else
                {
                    if (context.HeldItem.Definition.ActionKind == ActionKind.Script)
                    {
                        if (Target.Session.ScriptLibrary.FindOutcomeOverload(Target.DeclaredName, context.HeldItem.Name) is Script script)
                        {
                            this.script = script;
                            this.item = context.HeldItem;
                        }
                    }
                    else
                    {
                        this.item = context.HeldItem;
                        this.combatIntent = context.Session.Player?.CombatBehavior?.Intents.Find(context.HeldItem.Name);
                    }
                }
            }
        }

        // Target
        public GameThing? Target { get; private set; }

        // TargetPosition
        public Vector2 TargetPosition { get; private set; }
    }
}