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
        #region private fields

        private readonly List<StatusMeter> activeMeters = [];
        private readonly Dictionary<StatusType, StatusMeter> allMeters = [];
        private int lastKnownVersion = -1;

        #endregion

        #region Constructor

        // Constructor
        public UIStatuses(GameSession session)
            : base(session)
        {
            // Cache all status meters
            foreach (var statusType in Enum.GetValues<StatusType>())
            {
                allMeters.Add(statusType, new(this, statusType));
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (Actor == null)
                return;

            for (var i = 0; i < activeMeters.Count; i++)
            {
                activeMeters[i].Draw(gameTime);
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

            for (var i = 0; i < activeMeters.Count; i++)
            {
                activeMeters[i].Update(gameTime);
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
            for (int i = 0; i < activeMeters.Count; i++)
            {
                activeMeters[i].Status = null;
            }

            activeMeters.Clear();

            if (Actor == null)
                return;

            foreach (var status in Actor.StatusManager.Statuses)
            {
                var meter = allMeters[status.StatusType];
                meter.Status = status;
                activeMeters.Add(meter);
            }

            float spacing = 1;
            var pos = new Vector2(19, 16);

            for (var i = 0; i < activeMeters.Count; i++)
            {
                var icon = activeMeters[i];
                icon.Position = pos;
                pos.X += icon.BoundingBox.Width + spacing;
            }
        }

        /// <summary>
        /// StatusMeter
        /// </summary>
        private sealed class StatusMeter : GameObject
        {
            private readonly FlatMeter meter;
            private readonly UIStatuses owner;
            private readonly FloatTween rotationTween = new();
            private readonly Vector2Tween scaleTween = new();
            private readonly Sprite sprite;

            // Constructor
            public StatusMeter(UIStatuses owner, StatusType statusType)
            {
                this.owner = owner;
                this.StatusType = statusType;
                var def = GameData.Statuses.Get(statusType.ToString());

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

            #region Protected members

            // OnDraw
            protected override void OnDraw(GameTime gameTime)
            {
                if (Status == null)
                    return;

                sprite.Draw(gameTime);
                meter.Draw(gameTime);
            }

            // OnUpdate
            protected override void OnUpdate(GameTime gameTime)
            {
                if (Status == null)
                    return;

                if (meter.Value != Status.Value)
                {
                    meter.Value = Status.Value;
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

            // Status
            public Status? Status { get; set; }

            // StatusType
            public StatusType StatusType { get; }
        }
    }
}
