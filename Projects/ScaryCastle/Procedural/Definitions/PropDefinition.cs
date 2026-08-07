using Engendro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// PropDefinition
    /// </summary>
    public sealed class PropDefinition : ThingDefinition
    {
        private readonly List<PlacementType> placements = [];

        #region Constructor

        // Constructor
        public PropDefinition(JsonElement element)
            : base(element)
        {
            Faction = element.GetEnum("faction", Faction.Good);

            // Placements
            if (element.TryGetProperty("placements", out JsonElement placementsElement))
            {
                foreach (var item in placementsElement.EnumerateArray())
                {
                    if (Enum.TryParse<PlacementType>(item.GetString(), out var value))
                    {
                        placements.Add(value);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Cannot parse placement value.");
                    }
                }
            }

            this.Placements = placements.AsReadOnly();

            // RequiresPlaceholder
            RequiresPlaceholder = element.GetBool("requiresPlaceholder", true);

            // InteractionCost
            InteractionCost = element.GetInt32("interactionCost", 0);

            // InteractionCostType
            InteractionCostType = element.GetEnum("interactionCostType", InteractionCostType.None);

            Container.Add(this);
        }

        #endregion

        // ApplyInteractionCost
        public void ApplyInteractionCost(Actor actor)
        {
            if (InteractionCostType == InteractionCostType.Faith)
                actor.Energy -= InteractionCost;
            else if (InteractionCostType == InteractionCostType.Stamina)
                actor.Stamina -= InteractionCost;
        }

        // Container
        public static DataContainer<PropDefinition> Container { get; } = new(element => new PropDefinition(element));

        // InteractionCost
        public int InteractionCost { get; }

        // InteractionCostType
        public InteractionCostType InteractionCostType { get; }

        // Placements
        public ReadOnlyCollection<PlacementType> Placements { get; }

        // RequiresPlaceholder
        public bool RequiresPlaceholder { get; }
    }
}