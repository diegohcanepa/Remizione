using Adberration;
using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;
using System.Globalization;

namespace ScaryCastle
{
    /// <summary>
    /// VendingMachine
    /// </summary>
    public sealed class VendingMachine : Prop, IBuyable
    {
        private readonly ImageSprite glass;
        private readonly ImageSprite icon;
        private readonly ImageSprite led;
        private readonly TextSprite priceText;

        // Constructor
        public VendingMachine(GameSession session, string name)
            : base(session, name)
        {
            this.HighlightInteraction = false;

            // Icon
            icon = new(Game)
            {
                PivotOrigin = RectanglePoint.Center,
                Scale = new(.4f)
            };

            // Glass
            glass = new(Game, Atlases.Environment.FindImage($"{nameof(VendingMachine)}Glass"))
            {
                Opacity = .15f
            };

            // Led
            led = new(Game, Atlases.Environment.FindImage($"{nameof(VendingMachine)}Led"))
            {
            };

            // PriceText
            this.priceText = new(Game, Fonts.Common)
            {
                Color = Color.Black * .5f,
                PivotOrigin = RectanglePoint.Center,
                Scale = ScaleInfo.Text.Large
            };
        }

        #region IBuyable explicit members

        // Price
        int IBuyable.Price => MetaItem == null ? 0 : MetaItem.Price;

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
            
            icon.Draw(gameTime);
            glass.Draw(gameTime);
            led.Draw(gameTime);
            priceText.Draw(gameTime);
        }

        // OnParentChanged
        protected override void OnParentChanged(Entity? previousParent)
        {
            base.OnParentChanged(previousParent);

            if (Parent is ProceduralRoom procRoom && Config != null)
                MetaItem = Loot.GetForVending(Session, procRoom.Config, Config.PreferredLootRealm, Config.PreferredLootCategory);
        }

        // OnTransform
        protected override void OnTransform(TransformChange change)
        {
            base.OnTransform(change);

            if (change == TransformChange.Position)
            {
                icon.Position = BoundingBox.GetPoint(RectanglePoint.LeftTop, 10.5f, 12.5f);
                glass.Position = BoundingBox.GetPoint(RectanglePoint.LeftTop, 7, 9);
                led.Position = BoundingBox.GetPoint(RectanglePoint.LeftTop, 16, 4);
                priceText.Position = BoundingBox.GetPoint(RectanglePoint.Top, 0, 4.5f);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            icon.Update(gameTime);
        }

        #endregion

        // EnoughTickets
        [ScriptProperty]
        public bool EnoughTickets => MetaItem != null && Session.Tickets >= MetaItem.Price;

        // GetInteractPrompt
        public override string? GetInteractPrompt()
        {
            if (MetaItem != null)
                return MetaItem.LocalizedDisplayName;
            else
                return null;
        }

        // MetaItem
        public MetaItem? MetaItem
        {
            get;
            set
            {
                field = value;
                icon.Image = field?.Image;
                led.Color = field == null ? Color.Red : Color.Green;
                priceText.Text = field?.Price.ToString(CultureInfo.InvariantCulture);
            }
        }

        // Use
        [ScriptMethod]
        public bool Use()
        {
            if (MetaItem == null || !EnoughTickets)
                return false;

            AllowInteraction = false;
            PlaySound(SoundNames.VendingMachine);
            Bounce();
            icon.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.Linear, icon.Scale, Vector2.Zero, 150);
            Session.Inventory.Add(MetaItem);
            Session.HUD.Log.Show(LogVerb.Bought, MetaItem);
            Session.HUD.SackSlot.AnimateItem(MetaItem, icon.Position);
            Session.Tickets -= MetaItem.Price;

            return true;
        }
    }
}
