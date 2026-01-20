using Engendro;
using ScaryCastle.Procedural;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// CardDefinition
    /// </summary>
    public sealed class CardDefinition : Definition
    {
        private static readonly Dictionary<string, CardDefinition> definitions = [];

        #region Constructor

        // Constructor
        public CardDefinition(JsonElement element)
            : base(element)
        {
        }

        #endregion

        // ToString
        public override string ToString()
        {
            return Name;
        }
    }
}
