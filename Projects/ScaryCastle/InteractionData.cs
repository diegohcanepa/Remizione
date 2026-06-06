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
        private GameThing? target;
        private Vector2 targetPosition;

        #endregion

        // CanExecute
        public bool CanExecute => combatIntent != null || script != null || item != null;

        // Clear
        public void Clear()
        {
            combatIntent = null;
            item = null;
            script = null;
            target = null;
            targetPosition = Vector2.Zero;
        }

        // Execute
        public bool Execute(GameSession session)
        {
            if (session.Player == null || target == null)
                return false;

            var result = false;

            if (combatIntent != null)
            {
                session.Player.ExecuteAction(combatIntent, target);
                result = true;
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
                        {
                            session.BeginOutcome(script, target);
                            result = true;
                        }
                    }
                }
            }
            else if (item != null && !MouseCursor.IsArrow)
            {
                if (item.Name == ItemNames.Lift)
                {
                    if (target is Prop prop && prop.IsLiftable)
                    {
                        session.Player.Lift(prop);
                        result = true;
                    }
                }
                else
                {
                    session.Player.ExecuteAction(item, target);
                    result = true;
                }
            }

            Clear();

            return result;
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
                this.script = target.OutcomeScript;
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
                        this.combatIntent = context.Session.Player?.CombatBehavior?.Intents.Find(context.HeldItem.Name);
                    }
                }
            }
        }
    }
}