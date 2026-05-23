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
        protected Invocation(GameThing target, Item item)
            : base(target.Session, string.Empty)
        {
            this.Target = target;
            this.Item = item;
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
                Item.Use(this, Target, EffectContext.Attack);
                return;
            }

            base.OnUpdate(gameTime);

            if (!AnimationPlayer.IsPlaying)
                Unparent();
        }

        #endregion

        // Item
        public Item Item { get; }

        // Target
        public GameThing Target { get; }
    }
}
