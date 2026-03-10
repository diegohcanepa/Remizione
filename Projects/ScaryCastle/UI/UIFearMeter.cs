using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// UIFearMeter
    /// </summary>
    public sealed class UIFearMeter : GameObject
    {
        #region Private fields

        private int fullSkulls;
        private bool hasHalfSkull;
        private int lastKnownMaxValue;
        private int lastKnownValue;
        private readonly GameSession session;
        private readonly Sprite[] skulls;
        private int totalSkulls;

        #endregion

        #region Constructor

        // Constructor
        public UIFearMeter(GameSession session, Vector2 margin)
            : base(session.Game)
        {
            this.session = session;
            this.skulls = new Sprite[10];
            var pos = Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, margin);

            for (var i = 0; i < skulls.Length; i++)
            {
                skulls[i] = new(Game, Atlases.UI.SkullFull)
                {
                    Position = pos
                };

                pos.X += skulls[i].BoundingBox.Width + .5f;
            }

            Refresh();
        }

        #endregion

        #region Private members

        // Refresh
        private void Refresh()
        {
            fullSkulls = session.FearManager.CurrentFear / 2;
            hasHalfSkull = session.FearManager.CurrentFear % 2 == 1;
            totalSkulls = session.FearManager.MaxFear / 2;

            for (int i = 0; i < totalSkulls; i++)
            {
                if (i < fullSkulls)
                    skulls[i].RenderImage = Atlases.UI.SkullFull;

                else if (i == fullSkulls && hasHalfSkull)
                    skulls[i].RenderImage = Atlases.UI.SkullHalf;

                else
                    skulls[i].RenderImage = Atlases.UI.SkullEmpty;
            }

            lastKnownValue = session.FearManager.CurrentFear;
            lastKnownMaxValue = session.FearManager.MaxFear;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            for (var i = 0; i < totalSkulls; i++)
            {
                skulls[i].Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (lastKnownValue != session.FearManager.CurrentFear || lastKnownMaxValue != session.FearManager.MaxFear)
            {
                if (lastKnownValue < session.FearManager.CurrentFear)
                    Sound.Play(SoundNames.Fear);

                Refresh();
            }
        }

        #endregion
    }
}
