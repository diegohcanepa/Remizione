using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    // Torch
    public sealed class Torch : Prop
    {
        // Constructor
        public Torch(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Props;

            this.AttachedLight = new Light("Light")
            {
                Color = new(255, 248, 173),
                LightKind = LightKind.Default,
                PivotOrigin = RectanglePoint.Center,
                Position = new(9),
                Scale = new(12)
            };

            AttachedLightPosition = new(9);
        }

        #region Protected members

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
        }

        #endregion
    }
}
