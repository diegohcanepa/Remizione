using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// CombatFatigueState
    /// </summary>
    public sealed class CombatFatigueState : CombatState
    {
        private readonly FloatTween tween = new();

        // Constructor
        public CombatFatigueState(CombatStateMachine stateMachine)
            : base(stateMachine)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            Actor.Fatigue();
            int faithGain = (int)(Actor.MaxFaith * .5f);
            tween.Start(TweenStyle.Linear, Actor.Faith, faithGain, 1000);
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (tween.IsRunning)
            {
                tween.Update(gameTime);
                Actor.Faith = (int)tween.CurrentValue;
            }
        }
    }
}
