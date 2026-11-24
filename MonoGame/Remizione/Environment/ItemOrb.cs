using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ItemOrb
    /// </summary>
    public sealed class ItemOrb : Pickable
    {
        private readonly FloatTween fadeTween = new();

        #region Constructor

        // Constructor
        public ItemOrb(GameSession session, string name)
            : base(session, name)
        {
            ApproachBehavior = ApproachBehavior.Over;
            Atlas = Atlases.Environment;
            Hotspot = new Polygon("11,0;11,9;0,9;0,0");
            RenderLayer = RenderLayer.Default;
            AttachedLightPosition = new(5, 4);
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
                LabelKey = string.Empty;
                return;
            }

            AttachedLight ??= new("Light")
            {
                PivotOrigin = RectanglePoint.Center,
                Scale = new(1, .5f)
            };

            this.AttachedLight.LightKind = ItemReward.OrbLightKind;
            this.Color = AttachedLight.Color;

            LabelKey = ItemReward != null ? $"Item.{ItemReward.Name}.Name" : string.Empty;
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();

            fadeTween.Start(TweenStyle.CubicIn, 0, 1, 1000);
            Tweens.OpacityTween = FloatTween.Create(TweenStyle.Linear, .9f, 1, 90, -1);
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

            if (ExpirationTimer > 0 && ItemReward != null)
            {
                ExpirationTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (ExpirationTimer < 0)
                    fadeTween.Start(TweenStyle.CubicIn, OpacityFactor, 0, 2500, Unparent);
            }
        }

        #endregion

        // AllowCleanup
        public bool AllowCleanup => ItemReward == null || ExpirationTimer == 0;

        // Drop
        public static ItemOrb? Drop(string item, int amount, GameRoom room, Vector2 position)
        {
            return GameData.Items.Find(item) is { } def ? Drop(def, amount, room, position) : null;
        }

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

        // ExpirationTimer
        public float ExpirationTimer { get; set; } = GameData.WorldSettings.ItemOrbExpirationInterval;
    }
}
