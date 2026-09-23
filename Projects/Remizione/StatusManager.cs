using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// StatusManager
    /// </summary>
    public sealed class StatusManager
    {
        private const float StatusThreshold = 10;
        private readonly Dictionary<StatusType, float> activeStatuses = [];
        private readonly Dictionary<StatusType, float> baseResistances = [];
        private readonly Dictionary<StatusType, float> temporaryModifiers = [];

        // ApplyStatusDamage
        public bool ApplyStatusDamage(StatusType type, float amount)
        {
            float multiplier = GetStatusMultiplier(type);
            float effectiveAmount = amount * multiplier;

            if (effectiveAmount <= 0.0f)
                return false;

            activeStatuses.TryGetValue(type, out float current);
            current += effectiveAmount;

            if (current >= StatusThreshold)
            {
                activeStatuses[type] = 0.0f;
                return true;
            }

            activeStatuses[type] = current;
            
            return false;
        }

        // ContentVersion
        public int ContentVersion { get; set; }

        // GetStatusMultiplier
        public float GetStatusMultiplier(StatusType type)
        {
            float baseMultiplier = 1;
            
            if (baseResistances.TryGetValue(type, out float customBase))
                baseMultiplier = customBase;

            if (baseMultiplier <= 0)
                return 0;

            float tempModifier = 1;
            if (temporaryModifiers.TryGetValue(type, out float customTemp))
                tempModifier = customTemp;

            return Math.Max(0, baseMultiplier * tempModifier);
        }

        // GetStatusValue
        public float GetStatusValue(StatusType type)
        {
            return activeStatuses.TryGetValue(type, out float current) ? current : 0;
        }

        // SetBaseResistance
        public void SetBaseResistance(StatusType type, float multiplier)
        {
            baseResistances[type] = Math.Max(0, multiplier);
        }

        // SetTemporaryModifier
        public void SetTemporaryModifier(StatusType type, float modifier)
        {
            if (Math.Abs(modifier - 1) < 0.001f)
            {
                temporaryModifiers?.Remove(type);
                return;
            }

            temporaryModifiers[type] = Math.Max(0, modifier);
        }
    }
}
