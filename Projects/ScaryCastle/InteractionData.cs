using Adberration.Scripting;
using Microsoft.Xna.Framework;

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
            CombatIntent = null;
            LiftProp = null;
            Item = null;
            Script = null;
            Target = null;
            TargetPosition = Vector2.Zero;
        }

        // CombatIntent
        public CombatIntent? CombatIntent { get; private set; }

        // Execute
        public void Execute(GameSession session)
        {
            if (session.Player == null)
                return;

            if (LiftProp != null)
            {
                session.Player.Lift(LiftProp);
            }
            else if (CombatIntent != null)
            {
                session.Player.PerformAction(CombatIntent, Target);
            }
            else if (Script != null)
            {
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
                                    session.Player.PerformAction(session.Player.DefaultCombatIntent, Target);
                            }
                            else
                            {
                                session.BeginOutcome(Script, Target);
                            }
                        }
                    }
                }
            }

            Clear();
        }

        // Item
        public Item? Item { get; private set; }

        // LiftProp
        public Prop? LiftProp { get; private set; }

        // Script
        public Script? Script { get; private set; }

        // Session
        public GameSession Session { get; }

        // SetCombatIntent
        public void SetCombatIntent(CombatIntent combatIntent, GameThing target)
        {
            Clear();
            this.CloseAttack = true;
            this.CombatIntent = combatIntent;
            this.Target = target;
        }

        // SetLiftTarget
        public void SetLiftTarget(Prop prop)
        {
            Clear();
            this.LiftProp = prop;
        }

        // SetOutcome
        public Script? SetOutcome(GameThing target)
        {
            if (target.OutcomeScript != null)
                SetOutcomeCore(target, target.OutcomeScript);

            return target.OutcomeScript;
        }

        // SetUseWithOutcome
        public Script? SetUseWithOutcome(GameThing target, Item item)
        {
            var script = target.Session.ScriptLibrary.FindOutcomeOverload(target.DeclaredName, item.Name);

            if (script != null)
            {
                SetOutcomeCore(target, script);
                Item = item;
            }

            return script;
        }

        // Target
        public GameThing? Target { get; private set; }

        // TargetPosition
        public Vector2 TargetPosition { get; private set; }
    }
}