using Engendro;
using System.Text.Json;

namespace Remizione
{
    /// <summary>
    /// CombatArchetypeParams
    /// </summary>
    public readonly struct CombatArchetypeParams
    {
        // Constructor
        public CombatArchetypeParams(JsonElement element)
        {
            AwarenessRange = element.GetFloat("awarenessRange", 50f);
            CooldownDuration = element.GetFloat("cooldownDuration", 1.5f);
            ExposedPauseDuration = element.GetFloat("exposedPauseDuration", 2f);
            FallbackMovement = element.GetEnum("fallbackMovement", FallbackMovementKind.None);
            LoseSightRange = element.GetFloat("loseSightRange", 100f);
            MaxStepPerTurn = element.GetFloat("maxStepPerTurn", 10f);
        }

        // AwarenessRange
        public float AwarenessRange { get; }

        // CooldownDuration
        public float CooldownDuration { get; }

        // ExposedPauseDuration
        public float ExposedPauseDuration { get; }

        // FallbackMovement
        public FallbackMovementKind FallbackMovement { get; }

        // LoseSightRange
        public float LoseSightRange { get; }

        // MaxStepPerTurn
        public float MaxStepPerTurn { get; }
    }
}
