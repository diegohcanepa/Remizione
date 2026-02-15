using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// BrainInvestigateState
    /// </summary>
    public class BrainInvestigateState : BrainState
    {
        // Constructor
        public BrainInvestigateState()
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();

            // Lógica para moverse hacia TargetLocation...
            // Context.MoveTo(TargetLocation);
        }

        // TargetLocation
        public Vector2 TargetLocation { get; set; }
    }
}
