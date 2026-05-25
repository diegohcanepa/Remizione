using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// Invocation
    /// </summary>
    public abstract class Invocation : GameThing
    {
        private bool done;

        // Constructor
        protected Invocation(IGameAction action, GameThing target)
            : base(target.Session, string.Empty)
        {
            this.Action = action;
            this.Target = target;
            this.RenderLayer = RenderLayer.OverBackground;
            this.Atlas = Atlases.Environment;
            this.Scale = ScaleInfo.UIElement.Small;
        }

        #region Protected members

        // OnExecute
        protected virtual void OnExecute()
        {
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (!done && Room != null)
            {
                done = true;
                RenderLayer = RenderLayer.Default;
                Position = Target.Position;
                OnExecute();
                GameActionProcessor.Apply(Action, this, Target, EffectContext.Attack);
                return;
            }

            base.OnUpdate(gameTime);

            if (!AnimationPlayer.IsPlaying)
                Unparent();
        }

        #endregion

        // Action
        public IGameAction Action { get; }

        // Target
        public GameThing Target { get; }
    }
}
