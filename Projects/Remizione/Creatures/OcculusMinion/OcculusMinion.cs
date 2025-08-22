using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// OcculusMinion
    /// </summary>
    public sealed class OcculusMinion : Creature
    {
        // Constructor
        public OcculusMinion(GameSession session, string name)
            : base(session, name)
        {
            AnimationSettings.SupressAll();
            FastMoveFactor = 3;
            Guts = 7;
            ShadowSpotSize = 0;

            _ = new OcculusMinionDecideState(AIStateMachine);
            _ = new OcculusMinionPatrolState(AIStateMachine);
            _ = new OcculusMinionChargeState(AIStateMachine);
        }

        #region Protected members

        protected override void OnHurt(GameThing attacker, int damage, Vector2 knockback)
        {
            base.OnHurt(attacker, damage, knockback);
            AIStateMachine.ChangeState(AIStateName.Decide);
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
        }

        // OnStart
        protected override void OnStart()
        {
            AIStateMachine.ChangeState(AIStateName.Decide);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
        }

        #endregion
    }
}
