using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// UIStatuses
    /// </summary>
    public sealed class UIStatuses : SessionGameObject<GameSession>
    {
        #region private fields

        private int lastKnownVersion = -1;
        private readonly Sprite[] meters;
        private static readonly StatusType[] statuses = Enum.GetValues<StatusType>();

        #endregion

        #region Constructor

        // Constructor
        public UIStatuses(GameSession session)
            : base(session)
        {
            meters = new Sprite[Enum.GetValues<StatusType>().Length];
            for (var i = 0; i < meters.Length; i++)
            {
                meters[i] = new() { PivotOrigin = RectanglePoint.Top };
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (Actor == null)
                return;

            for (var i = 0; i < meters.Length; i++)
            {
                if (meters[i].RenderImage != null)
                    meters[i].Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Actor?.Statuses == null)
                return;

            if (lastKnownVersion != Actor.Statuses.Version)
            {
                Refresh();
                lastKnownVersion = Actor.Statuses.Version;
            }

            for (var i = 0; i < meters.Length; i++)
            {
                if (meters[i].RenderImage != null)
                    meters[i].Update(gameTime);
            }
        }

        #endregion

        // Actor
        public Actor? Actor
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Refresh();
                }
            }
        }

        // Refresh
        public void Refresh()
        {
            if (Actor?.Statuses == null)
                return;

            var startPos = new Vector2(9, 12);

            for (var i = 0; i < meters.Length; i++)
            {
                var value = Actor.Statuses.GetCount(statuses[i]);

                if (value <= 0)
                {
                    meters[i].RenderImage = null;
                }
                else if (Atlases.UI.FindImage($"Status{statuses[i]}{value}") is { } icon)
                {
                    meters[i].RenderImage = icon;
                    meters[i].Position = startPos;
                    startPos.Y += meters[i].BoundingBox.Height + 1;
                }
            }
        }
    }
}
