using Adberration.Scripting;
using Microsoft.Xna.Framework;

namespace ScaryCastle
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
        private GameThing? target;
        private Vector2 targetPosition;
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
            target = null;
            targetPosition = Vector2.Zero;
            throwable = null;
        }

        // Execute
        public bool Execute()
        {
            if (session.Player == null || target == null)
                return false;

            var result = false;

            if (combatIntent != null)
            {
                session.Player.ExecuteAction(combatIntent, target);
                result = true;
            }
            else if (throwable != null && target.Verb == Verb.Attack)
            {
                session.Player.StopMoving();
                session.Player.ThrowActiveTrowable(target);
                result = true;
            }
            else if (target.Verb == Verb.Lift)
            {
                if (target is Prop prop && prop.IsLiftable)
                {
                    session.Player.Lift(prop);
                    result = true;
                }
            }
            else if (script != null)
            {
                session.Player.StopMoving();

                if (target != null)
                {
                    if (Vector2.Distance(target.Position, targetPosition) > 1)
                    {
                        session.HUD.Message.Show(MessageKind.OutOfReach);
                    }
                    else
                    {
                        session.Player.FaceTo(target);
                        if (script != null)
                        {
                            session.BeginOutcome(script, target);
                            result = true;
                        }
                    }
                }
            }
            else if (item != null)
            {
                session.Player.ExecuteAction(item, target);
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

            this.target = context.Target;
            this.targetPosition = context.Target.Position;

            if (context.HeldItem == null)
            {
                if (context.Session.Player?.ActiveThrowable is Prop activeThrowable && !target.IsGoToVerb)
                {
                    this.throwable = activeThrowable;
                }
                else if (target.Verb == Verb.Attack)
                {
                    // TODO: update here if player can use different intents.
                    this.combatIntent = context.Session.Player?.CombatBehavior?.Intents[0];
                }
                else if (target.Verb == Verb.Lift)
                {
                    this.throwable = target as Prop;
                }
                else
                {
                    this.script = target.OutcomeScript;
                }
            }
            else
            {
                if (target.IsGoToVerb)
                {
                    this.script = target.OutcomeScript;
                }
                else
                {
                    if (context.HeldItem.Definition.ActionKind == ActionKind.Script)
                    {
                        if (target.Session.ScriptLibrary.FindOutcomeOverload(target.DeclaredName, context.HeldItem.Name) is Script script)
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
    }
}