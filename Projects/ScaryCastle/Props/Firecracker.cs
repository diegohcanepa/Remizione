using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// Firecracker
    /// </summary>
    public sealed class Firecracker : Prop
    {
        private int cooldown = 1500;

        // Constructor
        public Firecracker(GameSession session, Item item, Vector2 spawnPosition)
            : base(session, string.Empty)
        {
            Atlas = Atlases.Props;
            DepthOffset = -1;
            Position = spawnPosition;

            var animation = AddAnimation("Default");
            animation.AddFrameSequence("Firecracker", 80, 1, 3);
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (cooldown > 0)
                base.OnDraw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (cooldown > 0)
            {
                cooldown -= gameTime.ElapsedGameTime.Milliseconds;
                if (cooldown <= 0)
                {
                    if (Room != null && Session.PlayerInventory.Find("Firecracker") is Item item)
                    {
                        item.Use(this, null, EffectContext.Attack);
                        Session.Camera.Shake(TweenStyle.Linear, new Vector2(1.5f), 66, 4);
                    }
                }
            }
        }

        #endregion
    }
}
