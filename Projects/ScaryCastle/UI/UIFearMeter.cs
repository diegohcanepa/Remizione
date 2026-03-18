using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// UIFearMeter
    /// </summary>
    public sealed class UIFearMeter : GameObject
    {
        private readonly Sprite[] icons;
        private readonly GameSession session;
        private readonly FloatTween tween = new();

        #region Constructor

        // Constructor
        public UIFearMeter(GameSession session)
        {
            this.session = session;

            this.icons = new Sprite[10];

            for (var i = 0; i < icons.Length; i++)
            {
                this.icons[i] = new()
                {
                    PivotOrigin = RectanglePoint.Center,
                };
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!RunManager.HasContent)
                return;

            for (var i = 0; i < RunManager.MaximumFear; i++)
            {
                if (icons[i].IsEmpty)
                    break;

                icons[i].Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (!RunManager.HasContent)
                return;

            for (var i = 0; i < RunManager.MaximumFear; i++)
            {
                if (icons[i].IsEmpty)
                    break;

                icons[i].Update(gameTime);
            }
        }

        #endregion

        // Refresh
        public void Refresh(bool blink)
        {
            var pos = Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, 6, -8);

            for (var i = 0; i < icons.Length; i++)
            {
                icons[i].Position = pos;
                icons[i].RenderImage = Atlases.UI.FearEmpty;
                pos.X += icons[i].BoundingBox.Width + 1;
            }

            for (var i = 0; i < session.Fear; i++)
            {
                icons[i].RenderImage = Atlases.UI.FearFull;

                if (blink && i == session.Fear - 1)
                {
                    tween.Start(TweenStyle.Linear, 1, .5f, 400, 10);
                    icons[i].Tweens.OpacityTween = tween;
                }
            }
        }
    }
}
