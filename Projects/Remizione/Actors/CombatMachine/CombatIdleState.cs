using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// CombatIdleState
    /// </summary>
    public sealed class CombatIdleState : State<Actor>
    {
        // Update
        public override void Update(GameTime gameTime)
        {
            if (Owner.Session.Player is not Actor target)
                return;

            if (Owner.CombatBehavior?.Archetype is not { } arch)
                return;

            // Is inside AwarenessRange?
            if (Owner.DistanceToTarget(target) > arch.AwarenessRange)
                return;

            // Is in LOS?
            if (Owner.HasLineOfSightTo(target))
            {
                Owner.IsHostile = true;
                Machine.ChangeState<CombatStepState>();
            }
        }
    }
}