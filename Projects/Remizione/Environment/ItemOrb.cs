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
            DepthOffset = -2;
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
        }

        #endregion

        // Drop
        public static ItemOrb? Drop(ItemDefinition itemDefinition, GameRoom room, Vector2 position)
        {
            var loot = room.Session.CreateThingClone<ItemOrb>(nameof(ItemOrb));
            loot.ItemReward = itemDefinition;
            loot.Position = position;
            room.Children.Add(loot);

            return loot;
        }
    }
}
