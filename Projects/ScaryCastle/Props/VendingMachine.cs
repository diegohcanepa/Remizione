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
            glass = new(Game, Atlases.Environment.GetImage($"{nameof(VendingMachine)}Glass"))
            {
                Opacity = .15f
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
                icon.Position = BoundingBox.GetPoint(RectanglePoint.LeftTop, 10, 12.5f);
                glass.Position = BoundingBox.GetPoint(RectanglePoint.LeftTop, 6, 8);
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
        public bool EnoughTickets
        {
            get
            {
                return MetaItem == null ? false : Session.Tickets >= MetaItem.Price;
            }
        }

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
            Session.Inventory.Add(MetaItem, 1);
            Session.HUD.Log.Show(LogVerb.Bought, MetaItem);
            Session.HUD.SackSlot.AnimateItem(MetaItem, icon.Position);
            Session.Tickets -= MetaItem.Price;

            return true;
        }
    }
}
