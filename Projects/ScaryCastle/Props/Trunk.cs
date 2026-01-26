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

        // Constructor
        public Trunk(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Environment;
            DeathSound = Sound.Find(SoundNames.WoodDebris);
            DisplayNameKey = "Prop.Trunk";
            HitEffect = HitEffect.Shake;
            OpenSound = Sound.Find(SoundNames.TrunkOpen);

            this.itemImage = new(Game)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Scale = ScaleInfo.UIElement.Tiny
            };
        }

        #region Protected members

        // OnClosureStatusChanged
        protected override void OnClosureStatusChanged(bool isAction)
        {
            if (ClosureState == ClosureState.Open)
            {
                Loot = ItemDefinition.Get("Apple");
                DisplayNameKey = $"Item.{Loot.Name}.Name";
                itemImage.Position = BoundingBox.GetPoint(RectanglePoint.Top, 0, 14);
            }
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
            itemImage.Draw(gameTime);
        }

        #endregion

        // CanInteract
        public override bool CanInteract()
        {
            return (IsClosed || Loot != null) && base.CanInteract();
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
                }
            }
        }
    }
}
