using Adberration;
using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// UICycleMeter
    /// </summary>
    public sealed class UICycleMeter : SessionGameObject<GameSession>
    {
        private readonly Sprite icon;

        #region Constructor

        // Constructor
        public UICycleMeter(GameSession session)
            : base(session)
        {
            // Icon
            this.icon = new(Atlases.UI.GetImage("CycleMeter01"))
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightTop, -2, 0)
            };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            icon.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            icon.Update(gameTime);
        }

        #endregion
    }
}