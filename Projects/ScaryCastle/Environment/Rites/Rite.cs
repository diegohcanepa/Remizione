using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// Rite
    /// </summary>
    public abstract class Rite : GameThing
    {
        private bool done;

        // Constructor
        protected Rite(GameThing target, Item item)
            : base(target.Session, string.Empty)
        {
            this.Item = item;
            this.Position = target.Position;
            this.RenderLayer = RenderLayer.OverBackground;
            this.Atlas = Atlases.Environment;
            this.Scale = ScaleInfo.UIElement.Small;
        }

        #region Protected members

        // OnCast
        protected virtual void OnCast()
        {
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (!done && Room != null)
            {
                done = true;
                RenderLayer = RenderLayer.Default;
                OnCast();
                Item.Use(this, Session.OutcomeTarget, EffectContext.Attack);
                return;
            }

            base.OnUpdate(gameTime);

            if (!AnimationPlayer.IsPlaying)
                Unparent();
        }

        #endregion

        // Item
        public Item Item { get; }
    }
}
