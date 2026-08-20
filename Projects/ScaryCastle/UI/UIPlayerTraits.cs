using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// UIPlayerTraits
    /// </summary>
    public sealed class UIPlayerTraits : SessionGameObject<GameSession>
    {
        private int activeCount;
        private readonly List<Sprite> icons = [];
        private int lastKnownVersion = -1;

        #region Constructor

        // Constructor
        public UIPlayerTraits(GameSession session)
            : base(session)
        {
            for (var i = 0; i < 20; i++)
            {
                var sprite = new Sprite() { PivotOrigin = RectanglePoint.Center, Scale = ScaleInfo.UIElement.Medium };
                icons.Add(sprite);
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (Session.Player == null)
                return;

            for (var i = 0; i < activeCount; i++)
            {
                icons[i].Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Session.CurrentRun == null)
                return;

            if (lastKnownVersion != Session.CurrentRun.Traits.Version)
            {
                lastKnownVersion = Session.CurrentRun.Traits.Version;
                Refresh();
            }
        }

        #endregion

        // Refresh
        public void Refresh()
        {
            activeCount = 0;

            if (Session.CurrentRun == null)
                return;

            for (var i = 0; i < icons.Count; i++)
            {
                icons[i].RenderImage = null;
            }

            activeCount = Session.CurrentRun.Traits.Count;
            for (var i = 0; i < activeCount; i++)
            {
                icons[i].RenderImage = Session.CurrentRun.Traits[i].Image;
            }

            float spacing = 0;
            var pos = new Vector2(10, 116);

            for (var i = 0; i < activeCount; i++)
            {
                var icon = icons[i];
                icon.Position = pos;
                pos.X += icon.BoundingBox.Width + spacing;
            }
        }
    }
}
