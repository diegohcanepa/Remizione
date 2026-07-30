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
        private int breakTimer = -1;
        //private readonly Debris debris;
        private bool isBroken;
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

            //this.debris = new Debris(this);

            this.lootImage = new(Atlas.FindImage($"{DeclaredName}LootBag"))
            {
                PivotOrigin = RectanglePoint.Bottom,
            };
        }

        // Break
        private void Break()
        {
            isBroken = true;

            if (DeathSound != null)
                PlaySound(DeathSound);

            RenderLayer = RenderLayer.Background;
            DepthOffset = 0;
            //debris.Launch();
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
                {
                    Bounce();
                    if (Loot == null)
                        breakTimer = 2000;
                }
            }
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (isBroken)
            {
                //debris.Draw(gameTime);
            }
            else
            {
                base.OnDraw(gameTime);

                if (IsOpen && Loot != null)
                    lootImage.Draw(gameTime);
            }
        }

        // OnTransform
        protected override void OnTransform(TransformChange change)
        {
            base.OnTransform(change);
            lootImage?.MatchTransform(this.Sprite);
        }

        // OnUnload
        protected override void OnUnload()
        {
            base.OnUnload();
            //debris.Release();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (!isBroken)
            {
                if (breakTimer > 0)
                {
                    breakTimer -= gameTime.ElapsedGameTime.Milliseconds;
                    if (breakTimer < 0)
                    {
                        Break();
                        return;
                    }
                }
            }
            else
            {
                //debris.Update(gameTime);
            }
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
        public ItemDefinition? Loot
        {
            get;
            set
            {
                if (value != field)
                {
                    if (field != null && value == null)
                        breakTimer = 2000;

                    field = value;
                }
            }
        }
    }
}
