using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ActorActionState
    /// </summary>
    public abstract class ActorActionState : ActorAnimatedState
    {
        private bool actionDone;

        // Constructor
        protected ActorActionState(string animationName)
            : base(animationName, false)
        {
        }

        #region Protected members

        // OnExecuteAction
        protected virtual void OnExecuteAction()
        {
        }

        #endregion

        // Enter
        public override void Enter()
        {
            base.Enter();
            actionDone = false;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!actionDone && Owner.AnimationPlayer.Frame?.IsEvent == true)
            {
                actionDone = true;
                OnExecuteAction();
            }

            if (!Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<ActorStandState>();
        }
    }
}