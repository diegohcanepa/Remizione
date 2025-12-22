using Engendro;
using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// UnlockedPool
    /// </summary>
    public sealed class UnlockedPool
    {
        #region Private fields

        private readonly List<MetaItem> metaItems = [];
        private readonly Dictionary<string, MetaItem> metaItemsDict = [];
        private readonly List<RoomConfig> roomConfigs = [];
        private readonly Dictionary<string, RoomConfig> roomConfigsDict = [];
        private readonly GameSession session;
        private readonly List<GameThing> things = [];
        private readonly Dictionary<string, GameThing> thingsDict = [];
        private readonly HashSet<string> unlockedNames = [];

        #endregion

        // Constructor
        public UnlockedPool(GameSession session)
        {
            this.session = session;
        }

        // Deserialize
        public void Deserialize(string data)
        {
            InitializeDefaults();

            var names = data.Split(';', StringSplitOptions.RemoveEmptyEntries);

            foreach (var name in names)
            {
                Unlock(name);
            }
        }

        // FindMetaItem
        public MetaItem? FindMetaItem(string name)
        {
            return metaItemsDict.TryGetValue(name, out var metaItem) ? metaItem : null;
        }

        // GetMetaItems
        public List<MetaItem> GetMetaItems()
        {
            var result = new List<MetaItem>();

            foreach (var name in unlockedNames)
            {
                if (MetaItem.Find(name) is MetaItem metaItem)
                    result.Add(metaItem);
            }

            return result;
        }

        // GetMetaItems
        public List<MetaItem> GetMetaItems(ItemCategory category)
        {
            var result = new List<MetaItem>();

            foreach (var name in unlockedNames)
            {
                if (MetaItem.Find(name) is MetaItem metaItem && metaItem.Category == category)
                    result.Add(metaItem);
            }

            return result;
        }

        // GetMetaItems
        public List<MetaItem> GetMetaItems(Realm realm)
        {
            var result = new List<MetaItem>();

            foreach (var name in unlockedNames)
            {
                if (MetaItem.Find(name) is MetaItem metaItem && metaItem.Realm == realm)
                    result.Add(metaItem);
            }

            return result;
        }

        // FindRandomMetaItem
        public MetaItem? FindRandomMetaItem()
        {
            return GetMetaItems().GetRandomItem();
        }

        // FindRandomMetaItem
        public MetaItem? FindRandomMetaItem(ItemCategory category)
        {
            return GetMetaItems(category).GetRandomItem();
        }

        // FindRandomMetaItem
        public MetaItem? FindRandomMetaItem(Realm realm)
        {
            return GetMetaItems(realm).GetRandomItem();
        }

        // InitializeDefaults
        public void InitializeDefaults()
        {
            unlockedNames.Clear();

            // Meta items
            foreach (var metaItem in MetaItem.AllItems)
            {
                if (metaItem.Unlocked)
                    Unlock(metaItem.Name);
            }

            // Rooms
            foreach (var roomConfig in RoomConfig.All)
            {
                if (roomConfig.Unlocked)
                    Unlock(roomConfig.Name);
            }

            // Things (Actor / Props)
            foreach (var thingConfig in ThingConfig.All)
            {
                if (thingConfig.Unlocked)
                    Unlock(thingConfig.Name);
            }
        }

        // IsUnlocked
        public bool IsUnlocked(string name)
        {
            return unlockedNames.Contains(name);
        }

        // Serialize
        public string Serialize()
        {
            return string.Join(";", unlockedNames);
        }

        // Unlock
        public void Unlock(string name)
        {
            if (IsUnlocked(name))
                return;

            var unlock = false;

            // Is a meta item?
            if (MetaItem.Find(name) is MetaItem metaItem)
            {
                metaItems.Add(metaItem);
                metaItemsDict.Add(name, metaItem);
                unlock = true;
            }

            // Actor / Prop
            else if (session.FindEntity<GameThing>(name) is GameThing thing)
            {
                if (thing is Actor or Prop)
                {
                    things.Add(thing);
                    thingsDict.Add(name, thing);
                    unlock = true;
                }
            }

            // Room
            else if (RoomConfig.Find(name) is RoomConfig roomConfig)
            {
                roomConfigs.Add(roomConfig);
                roomConfigsDict.Add(name, roomConfig);
                unlock = true;
            }

            if (unlock)
                unlockedNames.Add(name);
        }
    }
}
