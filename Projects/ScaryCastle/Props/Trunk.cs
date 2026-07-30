using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// Trunk
    /// </summary>
    public class Trunk : Openable, ILootContainer<ItemDefinition>
    {
        private readonly Sprite lootImage;

        // Constructor
        public Trunk(GameSession session, string name)
            : base(session, name)
        {
            ApproachBehavior = ApproachBehavior.ClosestSide;
            Atlas = Atlases.Props;
            DeathSound = Sound.Find(SoundNames.WoodDebris);
            DisplayNameKey = "Prop.Trunk";
            LockedSound = Sound.Find(SoundNames.TrunkLocked);
            OpenSound = Sound.Find(SoundNames.TrunkOpen);
            OverheadOrigin = new(6, 2);
            UnlockSound = Sound.Find(SoundNames.LockOpen);

            this.lootImage = new(Atlas.FindImage($"{DeclaredName}LootBag"))
            {
                PivotOrigin = RectanglePoint.Bottom,
            };
        }

        #region Protected members

        // OnClosureStatusChanged
        protected override void OnClosureStatusChanged(bool actionInProgress)
        {
            if (IsOpen)
            {
                if (Session.LootGenerator.RollForLoot(this) is ItemDefinition loot)
                {
                    this.Loot = loot;
                    DisplayNameKey = $"Item.{loot.Name}.Name";
                }

                if (actionInProgress)
                    Bounce();
            }
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            if (IsOpen && Loot != null)
                lootImage.Draw(gameTime);
        }

        // OnTransform
        protected override void OnTransform(TransformChange change)
        {
            base.OnTransform(change);
            lootImage?.MatchTransform(this.Sprite);
        }

        #endregion

        // CanInteract
        public override bool CanInteract()
        {
            return (!IsOpen || Loot != null) && base.CanInteract();
        }

        // HasLoot
        [ScriptProperty]
        public bool HasLoot => IsOpen && Loot != null;

        // Loot
        public ItemDefinition? Loot { get; set; }
    }
}
