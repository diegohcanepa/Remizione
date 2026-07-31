using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// UIStatuses
    /// </summary>
    public sealed class UIStatuses : GameObject
    {
        private readonly List<StatusIcon> activeIcons = [];
        private readonly Dictionary<StatusType, StatusIcon> icons = [];
        private int lastKnownVersion = -1;

        #region Constructor

        // Constructor
        public UIStatuses()
            : base()
        {
            foreach (var statusType in Enum.GetValues<StatusType>())
            {
                icons.Add(statusType, new(statusType));
            }
        }

        #endregion

        #region Private members

        // Refresh
        private void Refresh()
        {
            activeIcons.Clear();
            if (Actor == null)
                return;

            foreach (var status in Actor.StatusManager.Statuses)
            {
                if (status.Value > 0)
                {
                    var activeIcon = icons[status.StatusType];
                    activeIcons.Add(activeIcon);
                }
            }

            float spacing = 1;
            float x = 16;
            float y = 13;

            for (var i = 0; i < activeIcons.Count; i++)
            {
                var icon = activeIcons[i];
                icon.Position = new(x, y);
                x += icon.BoundingBox.Width + spacing;
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (Actor == null)
                return;

            for (var i = 0; i < activeIcons.Count; i++)
            {
                activeIcons[i].Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Actor == null)
                return;

            if (lastKnownVersion != Actor.StatusManager.ContentVersion)
            {
                Refresh();
                lastKnownVersion = Actor.StatusManager.ContentVersion;
            }

            for (var i = 0; i < activeIcons.Count; i++)
            {
                activeIcons[i].Update(gameTime);
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

                    if (field != null)
                    {
                        foreach (var icon in icons.Values)
                        {
                            icon.Actor = field;
                        }
                    }

                    Refresh();
                }
            }
        }

        /// <summary>
        /// StatusIcon
        /// </summary>
        private sealed class StatusIcon : GameObject
        {
            private readonly FlatMeter meter;
            private Status? status;
            private readonly Sprite sprite;

            // Constructor
            public StatusIcon(StatusType statusType)
            {
                this.StatusType = statusType;
                var def = StatusDefinition.Container.Get(statusType.ToString());

                this.sprite = new()
                {
                    RenderImage = def.Image
                };

                this.meter = new(def.MeterBackColor, def.MeterForeColor, Color.Transparent, new(8, 3), .75f)
                {
                    MaximumValue = Status.MaxValue
                };
            }

            // Actor
            public Actor? Actor
            {
                get;
                set
                {
                    if (value != field)
                    {
                        field = value;
                        status = field?.StatusManager.GetStatus(StatusType);
                    }
                }
            }

            #region Protected members

            // OnDraw
            protected override void OnDraw(GameTime gameTime)
            {
                if (status == null)
                    return;

                sprite.Draw(gameTime);
                meter.Draw(gameTime);
            }

            // OnUpdate
            protected override void OnUpdate(GameTime gameTime)
            {
                if (status == null)
                    return;

                meter.Value = status.Value;
                meter.Update(gameTime);
            }

            #endregion

            // BoundingBox
            public RectangleF BoundingBox => sprite.BoundingBox;

            // Position
            public Vector2 Position
            {
                get => sprite.Position;
                set
                {
                    sprite.Position = value;
                    meter.Position = sprite.BoundingBox.GetPoint(RectanglePoint.Bottom);
                }
            }

            // StatusType
            public StatusType StatusType { get; }
        }
    }
}
