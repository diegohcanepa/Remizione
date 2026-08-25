using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ScaryCastle
{
    /// <summary>
    /// RunModifierManager
    /// </summary>
    public sealed class RunModifierManager
    {
        #region Private fields

        private readonly List<RunModifier> activeModifiers = [];
        private readonly Dictionary<string, RunModifier> activeModifiersDict = [];
        private readonly Dictionary<string, RunModifier> allModifiersDict = [];

        #endregion

        #region Constructor

        // Constructor
        public RunModifierManager(GameSession session)
        {
            this.Session = session;

            ActiveModifiers = activeModifiers.AsReadOnly();

            foreach (var definition in GameData.RunModifiers.All)
            {
                var modifier = new RunModifier(this, definition);
                allModifiersDict.Add(modifier.Name, modifier);
            }
        }

        #endregion

        #region Private members

        // InvalidateContentVersion
        private void InvalidateContentVersion()
        {
            unchecked { ContentVersion++; }
        }

        #endregion

        // Activate
        public void Activate(string modifierName)
        {
            if (!IsActive(modifierName))
            {
                if (Find(modifierName) is { } modifier)
                {
                    activeModifiers.Add(modifier);
                    activeModifiersDict.Add(modifierName, modifier);
                    modifier.ResetTimer();
                    InvalidateContentVersion();
                }
            }
        }

        // ActiveCount
        public int ActiveCount => activeModifiersDict.Count;

        // ActiveModifiers
        public ReadOnlyCollection<RunModifier> ActiveModifiers { get; }

        // All
        public IEnumerable<RunModifier> All => allModifiersDict.Values;

        // Clear
        public void Clear()
        {
            activeModifiers.Clear();
            activeModifiersDict.Clear();
            InvalidateContentVersion();
        }

        // Clear
        public void Clear(RunModifierScope scope)
        {
            var invalidateContent = false;

            for (var i = activeModifiers.Count - 1; i >= 0; i--)
            {
                if (activeModifiers[i].Definition.Scope == scope)
                {
                    activeModifiersDict.Remove(activeModifiers[i].Name);
                    activeModifiers.Remove(activeModifiers[i]);
                    invalidateContent = true;
                }
            }

            if (invalidateContent)
                InvalidateContentVersion();
        }

        // ContentVersion
        public int ContentVersion { get; private set; }

        // Deactivate
        public void Deactivate(string modifierName)
        {
            if (activeModifiersDict.TryGetValue(modifierName, out RunModifier? runModifier))
            {
                activeModifiersDict.Remove(modifierName);
                activeModifiers.Remove(runModifier);
                InvalidateContentVersion();
            }
        }

        // Find
        public RunModifier? Find(string modifierName)
        {
            if (allModifiersDict.TryGetValue(modifierName, out RunModifier? modifier))
                return modifier;
            else
                return null;
        }

        // FindActive
        public RunModifier? FindActive(string modifierName)
        {
            if (activeModifiersDict.TryGetValue(modifierName, out RunModifier? modifier))
                return modifier;
            else
                return null;
        }

        // IsActive
        public bool IsActive(string modifierName)
        {
            return FindActive(modifierName) != null;
        }

        // Session
        public GameSession Session { get; }

        // Update
        public void Update(GameTime gameTime)
        {
            for (var i = 0; i < activeModifiers.Count; i++)
            {
                activeModifiers[i].Update(gameTime);
            }
        }
    }
}
