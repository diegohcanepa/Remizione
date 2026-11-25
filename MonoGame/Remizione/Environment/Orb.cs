using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Orb
    /// </summary>
    public sealed class Orb : Pickable
    {
        private readonly FloatTween fadeTween = new();

        #region Constructor

        // Constructor
        public Orb(GameSession session, string name)
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
                Transient = true;
                return;
            }

            AttachedLight ??= new("Light")
            {
                PivotOrigin = RectanglePoint.Center,
                Scale = new(1, .5f)
            };

            this.AttachedLight.LightKind = ItemReward.OrbLightKind;
            this.Color = AttachedLight.Color;
            this.ExpirationTimer = ItemReward.ItemKind == ItemKind.Key ? 0 : GameData.WorldSettings.OrbExpirationInterval;
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
                if (ExpirationTimer <= 0)
                {
                    fadeTween.Start(TweenStyle.CubicIn, OpacityFactor, 0, 2500, Unparent);
                    Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicIn, Scale, Vector2.Zero, 3500);
                }
            }
        }

        #endregion

        // Drop
        public static Orb? Drop(string itemName, GameRoom room, Vector2 position, int amount = 1)
        {
            return GameData.Items.Find(itemName) is { } def ? Drop(def, room, position, amount) : null;
        }

        // Drop
        public static Orb? Drop(ItemDefinition itemDefinition, GameRoom room, Vector2 position, int amount = 1)
        {
            var orb = room.Session.CreateThingClone<Orb>(nameof(Orb));

            orb.ItemReward = itemDefinition;
            orb.ItemRewardAmount = amount;
            orb.Position = position;
            room.Children.Add(orb);

            return orb;
        }

        // ExpirationTimer
        public float ExpirationTimer
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Transient = field > 0;
                }
            }
        }

        // Transient
        public bool Transient { get; private set; }
    }
}
