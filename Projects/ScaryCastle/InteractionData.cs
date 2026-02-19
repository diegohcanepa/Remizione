using Adberration.Scripting;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// InteractionData
    /// </summary>
    public static class InteractionData
    {
        // Clear
        public static void Clear()
        {
            HasData = false;
            Item = null;
            Script = null;
            Target = null;
            TargetPosition = Vector2.Zero;
        }

        // Execute
        public static bool Execute(GameSession session)
        {
            if (!HasData)
                return false;

            var result = false;

            if (Target != null)
            {
                session.Player?.StopMoving();
                session.Player?.FaceTo(Target);
                if (Script != null)
                    session.BeginOutcome(Script, Target);
            }

            /*
            if (Script != null)
            {
                if (Target == null)
                {
                    session.AwaitScript(Script);
                    result = true;
                }
                else if (Target.Position == TargetPosition)
                {
                    session.InteractionContext.HeldItem = null;
                    session.Player?.StopMoving();
                    session.Player?.FaceTo(Target);
                    session.BeginOutcome(Script, Target);
                    result = true;
                }
            }
            */

            Clear();

            return result;
        }

        // HasData
        public static bool HasData { get; private set; }

        // Item
        public static Item? Item { get; private set; }

        // Script
        public static Script? Script { get; private set; }

        // SetOutcome
        public static void SetOutcome(GameThing target)
        {
            Clear();
            Target = target;
            TargetPosition = target.Position;
            Script = target.OutcomeScript;
            HasData = true;
        }

        // SetUseWithOutcome
        public static void SetUseWithOutcome(GameThing target, Item item)
        {
            if (item.Definition.UsageMode != ItemUsageMode.Default)
                throw new InvalidOperationException($"Item '{item.Definition.Name}' cannot be used with other items.");

            Clear();
            Item = item;
            Target = target;
            TargetPosition = target.Position;
            Script = target.Session.ScriptLibrary.FindOverload(Target.DeclaredName, Item.Name);
            HasData = true;
        }

        // Target
        public static GameThing? Target { get; private set; }

        // TargetPosition
        public static Vector2 TargetPosition { get; private set; }
    }
}

