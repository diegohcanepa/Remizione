using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// EnviousEye
    /// </summary>
    public sealed class EnviousEye : Enemy
    {
        // Constructor
        public EnviousEye(GameSession session, string name)
            : base(session, name)
        {
            AnimationSettings.SupressAll();
            FastMoveFactor = 3;
            Guts = 7;
            ShadowSpotSize = 0;

            _ = new EnviousEyeDecideState(AIStateMachine);
            _ = new EnviousEyePatrolState(AIStateMachine);
            _ = new EnviousEyeChargeState(AIStateMachine);
        }

        #region Protected members

        // OnStart
        protected override void OnStart()
        {
            AIStateMachine.ChangeState(AIStateName.Decide);
        }

        // OnTakeDamage
        protected override void OnTakeDamage(GameThing attacker, int damage, DamageType damageType, Vector2 knockback)
        {
            base.OnTakeDamage(attacker, damage, damageType, knockback);
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
