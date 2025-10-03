using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Nosy Hemorrhoid
    /// </summary>
    public sealed class NosyHemorrhoid : Creature
    {
        // Constructor
        public NosyHemorrhoid(GameSession session, string name)
            : base(session, name)
        {
            AnimationSettings.SupressAll();
            FastMoveFactor = 3;
            Guts = 7;
            ShadowSpotSize = 0;

            _ = new NosyHemorrhoidDecideState(AIStateMachine);
            _ = new NosyHemorrhoidPatrolState(AIStateMachine);
            _ = new NosyHemorrhoidChargeState(AIStateMachine);
        }

        #region Protected members

        // OnFindEnemy
        protected override GameThing? OnFindEnemy() => LastKnownAttacker;

        // OnHurt
        protected override void OnHurt(GameThing attacker, int damage, DamageType damageType, Vector2 knockback)
        {
            base.OnHurt(attacker, damage, damageType, knockback);
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
