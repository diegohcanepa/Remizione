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
    public sealed class UIStatuses : SessionGameObject<GameSession>
    {
        private readonly List<StatusIcon> activeIcons = [];
        private readonly Dictionary<StatusType, StatusIcon> icons = [];
        private int lastKnownVersion = -1;

        #region Constructor

        // Constructor
        public UIStatuses(GameSession session)
            : base(session)
        {
            foreach (var statusType in Enum.GetValues<StatusType>())
            {
                icons.Add(statusType, new(statusType));
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

        // Refresh
        public void Refresh()
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
            var pos = new Vector2(19, 16);

            for (var i = 0; i < activeIcons.Count; i++)
            {
                var icon = activeIcons[i];
                icon.Position = pos;
                pos.X += icon.BoundingBox.Width + spacing;
            }
        }

        /// <summary>
        /// StatusIcon
        /// </summary>
        private sealed class StatusIcon : GameObject
        {
            private readonly FlatMeter meter;
            private readonly FloatTween rotationTween = new();
            private readonly Vector2Tween scaleTween = new();
            private Status? status;
            private readonly Sprite sprite;

            // Constructor
            public StatusIcon(StatusType statusType)
            {
                this.StatusType = statusType;
                var def = StatusDefinition.Data.Get(statusType.ToString());

                this.sprite = new()
                {
                    PivotOrigin = RectanglePoint.Center,
                    RenderImage = def.Image
                };

                this.meter = new(def.MeterBackColor, def.MeterForeColor, Color.Transparent, new(8, 3), .75f)
                {
                    MaximumValue = Status.MaxValue
                };
            }

            #region Private members

            // Shake
            private void Shake()
            {
                if (!sprite.Tweens.IsTweening)
                {
                    rotationTween.Start(TweenStyle.QuadraticInOut, 0, 5, 50, 6);
                    sprite.Tweens.RotationTween = rotationTween;

                    scaleTween.Start(TweenStyle.QuadraticInOut, Vector2.One, Vector2.One * 1.15f, 150, 2);
                    sprite.Tweens.ScaleTween = scaleTween;
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

                if (meter.Value != status.Value)
                {
                    meter.Value = status.Value;
                    Shake();
                }
                else if (meter.Value >= 8)
                {
                    Shake();
                }

                sprite.Update(gameTime);
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
