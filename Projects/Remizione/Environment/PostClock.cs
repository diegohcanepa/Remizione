using Engendro.Audio;

namespace Remizione
{
    /// <summary>
    /// PostClock
    /// </summary>
    public class PostClock : BreakableProp
    {
        // Constructor
        public PostClock(GameSession session, string name)
            : base(session, name)
        {
            DeathSound = Sound.Find(SoundNames.MetalPropBreak);
            DepthOffset = -8;
            HitTestSource = HitTestSource.Hotspot;
            HurtShake = new(1.5f, 0);
            HurtSound = Sound.Find(SoundNames.ImpactA);
            LootTableName = nameof(Pottery);
            MaxHealth = 6;
            PreventKnockback = true;
            ShakeOnHit = true;
        }

        #region Protected members

        // OnDie
        protected override void OnDie()
        {
            base.OnDie();
            Session.ObjectPools.FloatingTexts.Get()?.Show(BoundingBox.Center, "¡Tiempo Extra!", ColorPalette.Text.Orange);
            Session.RemainingTime += 10000;
        }

        #endregion
    }
}
