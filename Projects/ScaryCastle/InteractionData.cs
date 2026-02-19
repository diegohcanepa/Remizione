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
                    session.BeginOutcome(Script, Target);
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
            if (item.Definition.UsageMode != ItemUsageMode.Cast)
                throw new InvalidOperationException($"Item '{item.Definition.Name}' cannot be casted.");

            Clear();
            Script = Session.ScriptLibrary.FindRoutine($"{item.Name}Outcome");
            InteractionType = InteractionType.Cast;
            LastKnownCastPosition = castPosition;
        }

        // SetPlaceOutcome
        public void SetPlaceOutcome(Item item)
        {
            if (item.Definition.UsageMode != ItemUsageMode.Place)
                throw new InvalidOperationException($"Item '{item.Definition.Name}' cannot be placed.");

            Clear();
            Script = Session.ScriptLibrary.FindRoutine($"{item.Name}Outcome");
            InteractionType = InteractionType.Place;
        }

        // SetOutcome
        public void SetOutcome(GameThing target)
        {
            Clear();
            Target = target;
            TargetPosition = target.Position;
            Script = target.OutcomeScript;
            InteractionType = InteractionType.Outcome;
        }

        // SetUseWithOutcome
        public void SetUseWithOutcome(GameThing target, Item item)
        {
            if (item.Definition.UsageMode != ItemUsageMode.Default)
                throw new InvalidOperationException($"Item '{item.Definition.Name}' cannot be used with other items.");

            Clear();
            Item = item;
            Target = target;
            TargetPosition = target.Position;
            Script = target.Session.ScriptLibrary.FindOverload(Target.DeclaredName, Item.Name);

            if (Script == null && item.Definition.SelfTarget && target.Session.Player == target)
                Script = target.Session.ScriptLibrary.FindRoutine($"{item.Name}Outcome");

            InteractionType = InteractionType.UseWithOutcome;
        }

        // Target
        public GameThing? Target { get; private set; }

        // TargetPosition
        public Vector2 TargetPosition { get; private set; }
    }
}