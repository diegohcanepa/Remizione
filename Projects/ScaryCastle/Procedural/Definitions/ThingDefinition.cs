using Engendro;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// ThingDefinition
    /// </summary>
    public abstract class ThingDefinition : EntityDefinition
    {
        #region Private fields

        private readonly List<EffectDescriptor> effects = [];

        #endregion

        #region Constructor

        // Constructor
        protected ThingDefinition(JsonElement element)
            : base(element)
        {
            // DropChanceMultiplier
            DropChanceMultiplier = element.GetFloat("dropChanceMultiplier", 1f);

            // DropMode
            DropMode = element.GetEnum("dropMode", LootDropMode.Standard);

            // DropTrigger
            DropTrigger = element.GetEnum("dropTrigger", LootDropTrigger.OnDeath);

            // MaxPerRoom
            MaxPerRoom = element.GetInt32("maxPerRoom", -1);
            if (MaxPerRoom < 0)
                MaxPerRoom = -1;

            // MinProgress
            MinProgress = element.GetFloat("minProgress", 0);
            if (MinProgress < 0)
                MinProgress = 0;

            RequiresDeadEnd = element.GetBool("requiresDeadEnd", false);

            if (element.TryGetProperty("effects", out JsonElement effectsArray))
            {
                foreach (var effectJson in effectsArray.EnumerateArray())
                {
                    var effect = new EffectDescriptor(effectJson);
                    effects.Add(effect);
                }
            }

            Effects = effects.AsReadOnly();
        }

        #endregion

        // AssertScriptDeclaration
        public void AssertScriptDeclaration(GameSession session)
        {
            if (session.FindDeclaredThing(Name) == null)
                RaiseValidationError(this, "No script declaration.");
        }

        // DropMode
        public LootDropMode DropMode { get; }

        // DropTrigger
        public LootDropTrigger DropTrigger { get; }

        // DropChanceMultiplier
        public float DropChanceMultiplier { get; }

        // Effects
        public ReadOnlyCollection<EffectDescriptor> Effects { get; }

        // MaxPerRoom
        public int MaxPerRoom { get; }

        // MinProgress
        public Ratio MinProgress { get; }

        // PassesMaxPerRoomConstraint
        public bool PassesMaxPerRoomConstraint(int instanceCount)
        {
            return MaxPerRoom == -1 || instanceCount < MaxPerRoom;
        }

        // RequiresDeadEnd
        public bool RequiresDeadEnd { get; }
    }
}