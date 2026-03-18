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
        public bool Execute(GameSession session)
        {
            if (InteractionType == InteractionType.None)
                return false;

            var result = false;

            session.Player?.StopMoving();

            if (Target != null)
            {
                session.Player?.FaceTo(Target);
                if (Script != null)
                {
                    if (Vector2.Distance(Target.Position, TargetPosition) <= 1)
                        session.BeginOutcome(Script, Target);
                }
            }
            else if (Script != null)
            {
                session.InteractionContext.HeldItem = null;

                if (InteractionType == InteractionType.Cast)
                    session.Player?.FaceTo(LastKnownCastPosition);

                session.AwaitScript(Script);
            }

            Clear();

            return result;
        }

        // InteractionType
        public InteractionType InteractionType { get; private set; }

        // Item
        public Item? Item { get; private set; }

        // LastKnownCastPosition
        public Vector2 LastKnownCastPosition { get; private set; }

        // Script
        public Script? Script { get; private set; }

        // Session
        public GameSession Session { get; }

        // SetCastOutcome
        public void SetCastOutcome(Item item, Vector2 castPosition)
        {
            if (!item.Definition.IsMagical)
                throw new InvalidOperationException($"Item '{item.Definition.Name}' cannot be casted.");

            Clear();
            Script = Session.ScriptLibrary.FindRoutine($"{item.Name}Outcome");
            InteractionType = InteractionType.Cast;
            LastKnownCastPosition = castPosition;
        }

        // SetOutcome
        public void SetOutcome(GameThing target, InteractionType interactionType)
        {
            Clear();
            Target = target;
            TargetPosition = target.Position;
            InteractionType = interactionType;

            Script = interactionType switch
            {
                InteractionType.Headbutt => Session.ScriptLibrary.FindRoutine(RoutineNames.Headbutt),
                InteractionType.Outcome => target.OutcomeScript,
                _ => null
            };
        }

        // SetUseWithOutcome
        public void SetUseWithOutcome(GameThing target, Item item)
        {
            if (item.Definition.IsMagical)
                throw new InvalidOperationException($"Item '{item.Definition.Name}' cannot be used with other items.");

            Clear();
            Item = item;
            Target = target;
            TargetPosition = target.Position;
            Script = target.Session.ScriptLibrary.FindOverload(Target.DeclaredName, Item.Name);

            if (Script == null && item.Definition.Verb != ItemVerb.None && target.Session.Player == target)
                Script = target.Session.ScriptLibrary.FindRoutine($"{item.Name}Outcome");

            InteractionType = InteractionType.UseWithOutcome;
        }

        // Target
        public GameThing? Target { get; private set; }

        // TargetPosition
        public Vector2 TargetPosition { get; private set; }
    }
}