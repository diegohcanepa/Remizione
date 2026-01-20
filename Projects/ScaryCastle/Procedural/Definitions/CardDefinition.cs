using Engendro;
using ScaryCastle.Procedural;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// CardDefinition
    /// </summary>
    public sealed class CardDefinition : Definition
    {
        private static readonly Dictionary<string, CardDefinition> data = [];
        private static readonly List<CardDefinition> dataList = [];

        #region Constructor

        // Constructor
        public CardDefinition(JsonElement element)
            : base(element)
        {
            data.Add(Name, this);
            dataList.Add(this);
        }

        #endregion

        #region Static members

        // All
        public static ReadOnlyCollection<CardDefinition> All { get; } = new(dataList);

        // Find
        public static CardDefinition? Find(string name)
        {
            return data.TryGetValue(name, out var result) ? result : null;
        }

        // Get
        public static CardDefinition Get(string name)
        {
            return Find(name) ?? throw new InvalidOperationException($"{nameof(CardDefinition)} '{name}' not found.");
        }

        // Load
        public static void Load(string fileName)
        {
            if (data.Count > 0)
                throw new InvalidOperationException("Data already loaded.");

            Utils.LoadJsonData(fileName, element => new CardDefinition(element));
        }

        #endregion

        // ToString
        public override string ToString()
        {
            return Name;
        }
    }
}
