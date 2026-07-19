using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ScaryCastle
{
    /// <summary>
    /// RunModifierManager
    /// </summary>
    public sealed class RunModifierManager
    {
        private readonly List<RunModifier> modifiers = [];
        private readonly Dictionary<RunModifierKind, RunModifier> modifiersDict = [];

        // Constructor
        public RunModifierManager()
        {
            Modifiers = modifiers.AsReadOnly();
        }

        #region Private members

        // InvalidateContentVersion
        private void InvalidateContentVersion()
        {
            unchecked { ContentVersion++; }
        }

        #endregion

        // Add
        public void Add(RunModifier modifier)
        {
            // Avoid duplicates
            if (modifiersDict.ContainsKey(modifier.Kind))
                return;

            modifiers.Add(modifier);
            modifiersDict.Add(modifier.Kind, modifier);

            InvalidateContentVersion();
        }

        // Clear
        public void Clear()
        {
            modifiers.Clear();
            modifiersDict.Clear();
            InvalidateContentVersion();
        }

        // ClearRoomModifiers
        public void ClearRoomModifiers()
        {
            var invalidateContent = false;

            for (var i = modifiers.Count - 1; i >= 0; i--)
            {
                if (modifiers[i].Scope == RunModifierScope.Room)
                {
                    modifiersDict.Remove(modifiers[i].Kind);
                    modifiers.Remove(modifiers[i]);
                    invalidateContent = true;
                }
            }

            if (invalidateContent)
                InvalidateContentVersion();
        }

        // Contains
        public bool Contains(RunModifierKind modifierKind)
        {
            return modifiersDict.ContainsKey(modifierKind);
        }

        // Count
        public int Count => modifiersDict.Count;

        // Modifiers
        public ReadOnlyCollection<RunModifier> Modifiers { get; }

        // ContentVersion
        public int ContentVersion { get; private set; }

        // Remove
        public void Remove(RunModifierKind modifierKind)
        {
            if (modifiersDict.TryGetValue(modifierKind, out RunModifier? runModifier))
            {
                modifiersDict.Remove(modifierKind);
                modifiers.Remove(runModifier);
                InvalidateContentVersion();
            }
        }
    }
}
