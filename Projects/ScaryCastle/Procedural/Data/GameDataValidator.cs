using Adberration.Scripting;
using ScaryCastle.Scripting;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// GameDataValidator
    /// </summary>
    public static class GameDataValidator
    {
        #region Private members

        // AssertInterruptible
        private static void AssertInterruptibleClause(Script script)
        {
            if (!script.Interruptible)
                throw new InvalidOperationException($"{script.Name} must be interruptible.");
        }

        // ExtractItemName
        private static string? ExtractItemName(Script script)
        {
            if (script.Name.StartsWith("Item-", StringComparison.Ordinal))
            {
                string[] parts = script.Name.Split('-');
                return parts.Length >= 3 ? parts[1] : string.Empty;
            }

            return null;
        }

        #endregion

        // Validate
        public static void Validate(ScriptLibrary scriptLibrary)
        {
            if (!GameData.IsLoaded)
                throw new InvalidOperationException("Game data not loaded.");

            // Check overloads
            foreach (var script in scriptLibrary.AllScripts)
            {
                string? itemName = null;

                // The overload part must be the item name
                if (script.ScriptType == ScriptType.Outcome && script.OverloadName.Length > 0)
                {
                    itemName = script.OverloadName;
                    AssertInterruptibleClause(script);
                }

                // If script name is an item verb
                else if (script.Name.StartsWith("Item-", StringComparison.Ordinal))
                {
                    itemName = ExtractItemName(script);
                }

                if (itemName != null && GameData.Items.Find(itemName) == null)
                    throw new InvalidOperationException($"The item definition supplied in [{script.Name}] does not exist.");
            }

            // Check descriptions for all sack items
            foreach (var item in GameData.Items)
            {
                if (item.Behavior != ItemBehavior.Sack)
                    continue;

                if (scriptLibrary.FindItemRoutine(item.Name, Verb.Examine) is not Script examineRoutine)
                {
                    throw new InvalidOperationException($"Missing examine routine for {item.Name} sack item.");
                }
                else if (!examineRoutine.Interruptible)
                {
                    AssertInterruptibleClause(examineRoutine);
                }
            }
        }
    }
}
