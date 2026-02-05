using Engendro;
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

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            //this.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicInOut, Vector2.One, new(1, 1.05f), 600, -1);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
        }

        #endregion
    }
}
