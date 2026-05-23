using Adberration.Scripting;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// InteractionData
    /// </summary>
    public sealed class InteractionData
    {
        // Constructor
        public InteractionData(GameSession session)
        {
            this.Session = session;
        }

        #region Private members

        // SetOutcomeCore
        private void SetOutcomeCore(GameThing target, Script script)
        {
            Clear();
            Target = target;
            TargetPosition = target.Position;
            Script = script;
        }

        #endregion

        // CloseAttack
        public bool CloseAttack { get; private set; }

        // Clear
        public void Clear()
        {
            CloseAttack = false;
            Item = null;
            Script = null;
            Target = null;
            TargetPosition = Vector2.Zero;
        }

        // Execute
        public void Execute(GameSession session)
        {
            if (Script == null || session.Player == null)
                return;

            session.Player.StopMoving();

            if (Target != null)
            {
                if (Vector2.Distance(Target.Position, TargetPosition) > 1)
                {
                    session.TextHUD.Message.Show(MessageKind.OutOfReach);
                }
                else
                {
                    session.Player.FaceTo(Target);

                    if (Script != null)
                    {
                        if (CloseAttack)
                        {
                            if (session.Player.DefaultCombatIntent != null)
                                session.Player.PerformCloseAttack(session.Player.DefaultCombatIntent, Target);
                        }
                        else
                        {
                            session.BeginOutcome(Script, Target);
                        }
                    }
                }
            }

            Clear();
        }

        // Item
        public Item? Item { get; private set; }

        // Script
        public Script? Script { get; private set; }

        // Session
        public GameSession Session { get; }

        // SetCloseAttackOutcome
        public void SetCloseAttackOutcome(GameThing target, CombatIntent combatIntent)
        {
            if (Session.ScriptLibrary.FindRoutine(combatIntent.Name) is Script script)
            {
                CloseAttack = true;
                SetOutcomeCore(target, script);
            }
        }

        // SetDefaultOutcome
        public void SetDefaultOutcome(GameThing target)
        {
            if (target.OutcomeScript != null)
                SetOutcomeCore(target, target.OutcomeScript);
        }

        // SetLiftOutcome
        public void SetLiftOutcome(Prop prop)
        {
            if (Session.ScriptLibrary.FindRoutine(RoutineNames.LiftOutcomeTarget) is Script script)
                SetOutcomeCore(prop, script);
        }

        // SetUseWithOutcome
        public void SetUseWithOutcome(GameThing target, Item item)
        {
            var script = target.Session.ScriptLibrary.FindOutcomeOverload(target.DeclaredName, item.Name);
            script ??= item.Script;

            if (script != null)
            {
                SetOutcomeCore(target, script);
                Item = item;
            }
        }

        // Target
        public GameThing? Target { get; private set; }

        // TargetPosition
        public Vector2 TargetPosition { get; private set; }
    }
}