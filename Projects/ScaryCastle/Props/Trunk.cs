using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// Trunk
    /// </summary>
    public class Trunk : Openable, ILootConatiner<ItemDefinition>
    {
        private readonly ImageSprite itemImage;
        private readonly ImageSprite itemImageShadow;

        // Constructor
        public Trunk(GameSession session, string name)
            : base(session, name)
        {
            ApproachBehavior = ApproachBehavior.ClosestSide;
            Atlas = Atlases.Environment;
            DeathSound = Sound.Find(SoundNames.WoodDebris);
            DisplayNameKey = "Prop.Trunk";
            LockedSound = Sound.Find(SoundNames.TrunkLocked);
            OpenSound = Sound.Find(SoundNames.TrunkOpen);
            OverheadOrigin = new(6, 2);
            UnlockSound = Sound.Find(SoundNames.LockOpen);

            this.itemImage = new(Game)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Scale = ScaleInfo.UIElement.Tiny
            };

            this.itemImageShadow = new(Game)
            {
                Color = Color.Black,
                Opacity = ColorPalette.ShadowOpacity,
                PivotOrigin = RectanglePoint.Bottom,
                Scale = ScaleInfo.UIElement.Tiny
            };
        }

        #region Protected members

        // OnClosureStatusChanged
        protected override void OnClosureStatusChanged(bool actionInProgress)
        {
            if (IsOpen)
            {
                if (Session.LootGenerator.Get(this) is ItemDefinition loot)
                {
                    Loot = loot;
                    DisplayNameKey = $"Item.{Loot.Name}.Name";
                    itemImage.Position = BoundingBox.GetPoint(RectanglePoint.Top, 0, 14);
                    itemImageShadow.Position = itemImage.Position;
                    itemImageShadow.Y += 1;
                }

                if (actionInProgress)
                    Bounce();
            }
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
            itemImageShadow.Draw(gameTime);
            itemImage.Draw(gameTime);
        }

        #endregion

        // CanInteract
        public override bool CanInteract()
        {
            return (!IsOpen || Loot != null) && base.CanInteract();
        }

        // Loot
        public ItemDefinition? Loot
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    itemImage.Image = field?.Image;
                    itemImageShadow.Image = itemImage.Image;
                }
            }
        }
    }
}
