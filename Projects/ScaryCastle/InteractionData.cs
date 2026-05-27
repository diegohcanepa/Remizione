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
        private Prop? liftProp;
        private Script? script;
        private GameThing? target;
        private Vector2 targetPosition;

        #endregion

        // CanExecute
        public bool CanExecute => liftProp != null || combatIntent != null || script != null || item != null;

        // Clear
        public void Clear()
        {
            combatIntent = null;
            liftProp = null;
            item = null;
            script = null;
            target = null;
            targetPosition = Vector2.Zero;
        }

        // Execute
        public void Execute(GameSession session)
        {
            if (session.Player == null || target == null)
                return;

            if (liftProp != null)
            {
                session.Player.Lift(liftProp);
            }
            else if (combatIntent != null)
            {
                session.Player.ExecuteAction(combatIntent, target);
            }
            else if (script != null)
            {
                session.Player.StopMoving();

                if (target != null)
                {
                    if (Vector2.Distance(target.Position, targetPosition) > 1)
                    {
                        session.TextHUD.Message.Show(MessageKind.OutOfReach);
                    }
                    else
                    {
                        session.Player.FaceTo(target);

                        if (script != null)
                            session.BeginOutcome(script, target);
                    }
                }
            }
            else if (item != null && !MouseCursor.IsArrow)
            {
                session.Player.ExecuteAction(item, target);
            }

            Clear();
        }

        // IsAttack
        public bool IsAttack => combatIntent != null;

        // Refresh
        public void Refresh(InteractionContext context)
        {
            Clear();

            if (context.Target == null)
                return;

            if (context.HeldItem != null && context.Target.Cursor == MouseCursorState.Cross)
            {
                if (context.Session.Player == context.Target)
                {
                    if (context.HeldItem.Definition.UsageMode is ItemUsageMode.ProjectileAction or ItemUsageMode.ProximityAction)
                        return;
                }
                else if (context.HeldItem.Definition.UsageMode == ItemUsageMode.SelfAction)
                {
                    return;
                }
            }

            this.target = context.Target;
            this.targetPosition = context.Target.Position;

            if (context.HeldItem == null)
            {
                // Lift
                if (context.LiftTarget != null)
                {
                    this.liftProp = context.LiftTarget;
                }

                // Headbutt
                else if (context.Target.Faction == Faction.Evil)
                {
                    if (context.Session.Player?.DefaultCombatIntent is CombatIntent combatIntent)
                        this.combatIntent = combatIntent;
                }

                else
                {
                    this.script = target.OutcomeScript;
                }
            }
            else
            {
                if (MouseCursor.IsArrow)
                {
                    this.script = target.OutcomeScript;
                }
                else
                {
                    if (context.HeldItem.Definition.UsageMode == ItemUsageMode.Script)
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
                    }
                }
            }
        }
    }
}