using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// LurkerArchetype
    /// </summary>
    public class LurkerArchetype : CombatArchetype
    {
        // Se alerta desde lejos porque es asustadiza, pero no necesariamente para atacar
        public override float AwarenessRange => 140f;

        // Se cansa o duda poco después de actuar, movimiento inquieto
        public override float CooldownDuration => 0.8f;

        // Si la acorralás y le pegás, salta desesperada a morderte (reacción de rata arrinconada)
        public override float CounterAttackChance => 0;

        // Pausas cortas en Exposed: es hiperactiva, no se queda congelada mirando
        public override float ExposedPauseDuration => 0.6f;

        // Su comportamiento por defecto si estás a tiro de peligro es huir / zafar
        public override FallbackMovementKind FallbackMovement => FallbackMovementKind.Lurk;

        // Abandona la persecución rápido si te alejás un poco
        public override float LoseSightRange => 150f;

        // Pasos cortos y rpidos (pique estilo roedor)
        public override float MaxStepPerTurn => 15f;
    }
}