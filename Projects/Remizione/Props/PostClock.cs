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
            HitTestSource = TestPolygon.Hotspot;
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
            // TODO: Trigger countdown
        }

        #endregion
    }
}
