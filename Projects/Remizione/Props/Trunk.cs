using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Trunk
    /// </summary>
    public class Trunk : Prop
    {
        private readonly Sprite lootImage;

        // Constructor
        public Trunk(GameSession session, string name)
            : base(session, name)
        {
            ApproachBehavior = ApproachBehavior.ClosestSide;
            Atlas = Atlases.Props;
            AttachedLightPosition = new(6, 9);

            // Loot image
            this.lootImage = new(Atlas.FindImage($"{DeclaredName}Loot"))
            {
                PivotOrigin = RectanglePoint.Bottom,
            };
            this.lootImage.Tweens.OpacityTween = FloatTween.Create(TweenStyle.Linear, .9f, 1, 90, -1);

            SyncLabelKey();
        }

        #region Private members

        // SyncLabelKey
        private void SyncLabelKey()
        {
            if (IsOpen && ItemReward != null)
                LabelKey = ItemReward != null ? $"Item.{ItemReward.Name}.Name" : string.Empty;
            else
                LabelKey = "Prop.Trunk";
        }

        // SyncLight
        private void SyncLight()
        {
            if (ItemReward == null)
            {
                AttachedLight = null;
            }
            else
            {
                var lightKind = ItemReward.IsKeyItem ? LightKind.KeyItemOrb : LightKind.CommonItemOrb;
                this.AttachedLight ??= new("Light", lightKind)
                {
                    PivotOrigin = RectanglePoint.Center,
                };
                this.lootImage.Color = AttachedLight.Color;
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            if (IsOpen && ItemReward != null)
                lootImage.Draw(gameTime);
        }

        // OnItemRewardChanged
        protected override void OnItemRewardChanged()
        {
            base.OnItemRewardChanged();

            if (ItemReward != null)
            {
                this.AttachedLight ??= new("Light")
                {
                    PivotOrigin = RectanglePoint.Center,
                };

                var lightKind = ItemReward.IsKeyItem ? LightKind.KeyItemOrb : LightKind.CommonItemOrb;
                this.AttachedLight.LightKind = lightKind;
                this.lootImage.Color = AttachedLight.Color;
            }

            SyncLabelKey();
        }

        // OnTransform
        protected override void OnTransform(TransformChange change)
        {
            base.OnTransform(change);
            lootImage?.MatchTransform(this.Sprite);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            IgnoreAttachedLight = ItemReward == null || !IsOpen;

            base.OnUpdate(gameTime);

            if (IsOpen && ItemReward != null)
                lootImage.Update(gameTime);

            Verb = IsOpen && ItemReward != null ? Verb.PickUp : base.Verb;
        }

        #endregion

        // CanInteract
        public override bool CanInteract()
        {
            return (!IsOpen || ItemReward != null) && base.CanInteract();
        }

        // IsOpen
        [ScriptProperty]
        public bool IsOpen
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    SyncLabelKey();
                }
            }
        }
    }
}
