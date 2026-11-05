using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// UnlockManager
    /// </summary>
    internal sealed class UnlockManager
    {
        private readonly PlayerData playerData;
        private readonly HashSet<string> unlockedDefinitions = [];

        // Constructor
        internal UnlockManager(PlayerData playerData)
        {
            this.playerData = playerData;
        }

        #region Serialization

        // Deserialize
        internal void Deserialize(string data)
        {
            unlockedDefinitions.Clear();

            if (string.IsNullOrWhiteSpace(data))
                return;

            var names = data.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var name in names)
            {
                if (Definition.IsDefined(name))
                    unlockedDefinitions.Add(name);
            }
        }

        // Serialize
        internal string Serialize()
        {
            return string.Join(",", unlockedDefinitions);
        }

        #endregion

        #region Private members

        // CanUnlock
        private bool CanUnlock(Definition def)
        {
            return def.UnlockMode switch
            {
                UnlockMode.None => true,
                UnlockMode.Manual => false,
                UnlockMode.BonfiresLit => playerData.Bonfires >= def.RequiredBonfires,
                UnlockMode.Room => !string.IsNullOrEmpty(def.RequiredRoom) && IsUnlocked(def.RequiredRoom),
                _ => false
            };
        }

        // Refresh
        private void Refresh(Definition definition)
        {
            if (!IsUnlocked(definition.Name) && CanUnlock(definition))
                Unlock(definition.Name);
        }

        #endregion

        // IsUnlocked
        internal bool IsUnlocked(string definitionName)
        {
            return unlockedDefinitions.Contains(definitionName);
        }

        // Refresh
        internal void Refresh()
        {
            // Items
            for (int i = 0; i < GameData.Items.Count; i++)
            {
                Refresh(GameData.Items[i]);
            }

            // Props
            for (int i = 0; i < GameData.Props.Count; i++)
            {
                Refresh(GameData.Props[i]);
            }

            // Actors
            for (int i = 0; i < GameData.Actors.Count; i++)
            {
                Refresh(GameData.Actors[i]);
            }
        }

        // Unlock
        internal void Unlock(string name)
        {
            unlockedDefinitions.Add(name);
        }
    }
}
