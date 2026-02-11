using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ActorActionState
    /// </summary>
    public abstract class ActorActionState : ActorState
    {
        private bool actionDone;

        // Constructor
        protected ActorActionState()
            : base()
        {
        }

        #region Protected members

        // OnExecuteAction
        protected virtual void OnExecuteAction()
        {
        }

        #endregion

        // AnimationName
        public string AnimationName { get; set; } = string.Empty;

        // Enter
        public override void Enter()
        {
            base.Enter();
            actionDone = false;
            Owner.AnimationPlayer.Play(AnimationName, false);
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