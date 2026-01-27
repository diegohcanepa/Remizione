using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// EnviousEye
    /// </summary>
    public sealed class EnviousEye : Actor
    {
        // Constructor
        public EnviousEye(GameSession session, string name)
            : base(session, name)
        {
            AnimationSettings.SupressAll();
            FastMoveFactor = 3;
            Guts = 7;
            ShadowSpotSize = 0;
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
