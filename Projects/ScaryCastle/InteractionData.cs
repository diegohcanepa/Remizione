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

        // SetOutcomeCore
        private void SetOutcomeCore(GameThing target, InteractionType interactionType, Script script)
        {
            Clear();
            Target = target;
            TargetPosition = target.Position;
            InteractionType = interactionType;
            Script = script;
        }

        // Clear
        public void Clear()
        {
            InteractionType = InteractionType.None;
            Item = null;
            Script = null;
            Target = null;
            TargetPosition = Vector2.Zero;
        }

        // Execute
        public void Execute(GameSession session)
        {
            if (InteractionType == InteractionType.None || session.Player == null)
                return;

            session.Player.StopMoving();

            if (Target != null)
            {
                if (Vector2.Distance(Target.Position, TargetPosition) > 1)
                {
                    session.HUD.Message.Show(MessageKind.OutOfReach);
                }
                else
                {
                    session.Player.FaceTo(Target);
                    if (Script != null)
                    {
                        if (InteractionType == InteractionType.Attack)
                        {
                            if (session.Player.DefaultCombatIntent != null)
                                session.Player.PerformAttack(session.Player.DefaultCombatIntent, Target);
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

        // InteractionType
        public InteractionType InteractionType { get; private set; }

        // Item
        public Item? Item { get; private set; }

        // Script
        public Script? Script { get; private set; }

        // Session
        public GameSession Session { get; }

        // SetAttackOutcome
        public void SetAttackOutcome(GameThing target, CombatIntent combatIntent)
        {
            if (Session.ScriptLibrary.FindRoutine(combatIntent.Name) is Script script)
                SetOutcomeCore(target, InteractionType.Attack, script);
        }

        // SetCastOutcome
        public void SetCastOutcome(GameThing target, Item item)
        {
            if (item.Definition.GooCost == 0)
                throw new InvalidOperationException($"Item '{item.Definition.Name}' cannot be casted.");

            if (Session.ScriptLibrary.FindRoutine($"{item.Name}Outcome") is Script script)
                SetOutcomeCore(target, InteractionType.Cast, script);
        }

        // SetDefaultOutcome
        public void SetDefaultOutcome(GameThing target)
        {
            if (target.OutcomeScript != null)
                SetOutcomeCore(target, InteractionType.Outcome, target.OutcomeScript);
        }

        // SetLiftOutcome
        public void SetLiftOutcome(Prop prop)
        {
            if (Session.ScriptLibrary.FindRoutine(RoutineNames.LiftOutcomeTarget) is Script script)
                SetOutcomeCore(prop, InteractionType.Lift, script);
        }

        // SetUseWithOutcome
        public void SetUseWithOutcome(GameThing target, Item item)
        {
            var script = target.Session.ScriptLibrary.FindOverload(target.DeclaredName, item.Name);
            script ??= target.Session.ScriptLibrary.FindRoutine($"{item.Name}Outcome");

            if (script != null)
            {
                SetOutcomeCore(target, InteractionType.UseWithOutcome, script);
                Item = item;
                InteractionType = InteractionType.UseWithOutcome;
            }
        }

        // Target
        public GameThing? Target { get; private set; }

        // TargetPosition
        public Vector2 TargetPosition { get; private set; }
    }
}