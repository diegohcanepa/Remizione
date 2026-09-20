using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// ItemOrb
    /// </summary>
    public sealed class ItemOrb : Pickable
    {
        private int expirationTimer = Random.Shared.Next(10000, 20000);
        private readonly FloatTween fadeTween = new();

        #region Constructor

        // Constructor
        public ItemOrb(GameSession session, string name)
            : base(session, name)
        {
            ApproachBehavior = ApproachBehavior.Over;
            Atlas = Atlases.Environment;
            Hotspot = new Polygon("0,0;7,0;7,7;0,7");
            RenderLayer = RenderLayer.Default;

            this.AttachedLight = new("Light", LightKind.ItemOrb)
            {
                PivotOrigin = RectanglePoint.Center,
            };

            this.Color = AttachedLight.Color;

            AttachedLightPosition = new(3, 4);
        }

        #endregion

        #region Protected members

        // OnItemRewardChanged
        protected override void OnItemRewardChanged()
        {
            base.OnItemRewardChanged();

            if (ItemReward == null)
            {
                AttachedLight?.Unlit(true);
                DisplayNameKey = string.Empty;
                return;
            }

            AttachedLight ??= new("Light")
            {
                PivotOrigin = RectanglePoint.Center,
            };

            var lightKind = ItemReward.IsKeyItem ? LightKind.KeyItemOrb : LightKind.ItemOrb;
            this.AttachedLight.LightKind = lightKind;
            this.Color = AttachedLight.Color;

            DisplayNameKey = ItemReward != null ? $"Item.{ItemReward.Name}.Name" : string.Empty;
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();

            fadeTween.Start(TweenStyle.CubicIn, 0, 1, 1000);
            Tweens.OpacityTween = FloatTween.Create(TweenStyle.Linear, .5f, .6f, 90, -1);
            Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicIn, .1f, .75f, 400);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (fadeTween.IsRunning)
            {
                fadeTween.Update(gameTime);
                OpacityFactor = fadeTween.CurrentValue;
            }

            if (Session.OutcomeTarget != this)
            {
                if (expirationTimer >= 0 && ItemReward != null && !ItemReward.IsKeyItem)
                {
                    expirationTimer -= gameTime.ElapsedGameTime.Milliseconds;
                    if (expirationTimer < 0)
                    {
                        AttachedLight?.Unlit();
                        Tweens.OpacityTween = FloatTween.Create(TweenStyle.Linear, Opacity, 0, 1000, Unparent);
                    }
                }
            }
        }

        #endregion

        // AllowCleanup
        public bool AllowCleanup => ItemReward == null || !ItemReward.IsKeyItem;

        // Drop
        public static ItemOrb? Drop(ItemDefinition itemDefinition, int amount, GameRoom room, Vector2 position)
        {
            var orb = room.Session.CreateThingClone<ItemOrb>(nameof(ItemOrb));

            orb.ItemReward = itemDefinition;
            orb.ItemRewardAmount = amount;
            orb.Position = position;
            room.Children.Add(orb);

            return orb;
        }
    }
}
