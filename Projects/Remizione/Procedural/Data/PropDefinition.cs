using Engendro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Remizione
{
    /// <summary>
    /// PropDefinition
    /// </summary>
    public sealed class PropDefinition : ThingDefinition
    {
        #region Constructor

        // Constructor
        public PropDefinition(JsonElement element)
            : base(element, true, Faction.Good)
        {
        }

        #endregion
    }
}