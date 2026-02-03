using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// EnemyTurnState
    /// </summary>
    public sealed class EnemyTurnState : CombatManagerState
    {
        private bool effectsApplied;
        private bool attackLaunched;
        private readonly FloatTween xTween = new();
        private readonly FloatTween yTween = new();

        // Constructor
        public EnemyTurnState(CombatManager manager)
            : base(manager)
        {
        }

        #region Protected members

        // OnEnter
        protected override void OnEnter()
        {
            effectsApplied = false;
            attackLaunched = false;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            // Launch attack
            if (!attackLaunched && TimeInState > .5f)
            {
                xTween.Start(TweenStyle.Linear, Manager.Enemy.X, Manager.Player.X, 200, 2);
                //yTween.Start(TweenStyle.Linear, Manager.Enemy.Y, Manager.Enemy.Y - 5, 100, 2);
                Manager.Enemy.Tweens.XTween = xTween;
                //Manager.Enemy.Tweens.YTween = yTween;
                attackLaunched = true;
            }

            // Apply effects
            if (!effectsApplied && xTween.BounceCount > 0)
            {
                if (Manager.Enemy.Definition != null)
                    EffectDescriptor.Apply(Manager.Enemy.Definition.Effects, Manager.Enemy, Manager.Player, AttackType.Contact);

                Manager.Session.HUD.CombatFeedback.Show("¡Golpe de retina!", 2500);

                effectsApplied = true;
            }

            if (!xTween.IsRunning && effectsApplied)
                Manager.TransitionTo(new PlayerTurnState(Manager));
        }

        #endregion
    }
}
