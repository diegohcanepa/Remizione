using Engendro.Audio;

namespace ScaryCastle
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
            HitEffect = HitEffect.Shake;
            HitTestPolygon = TestPolygon.Hotspot;
            HurtSound = Sound.Find(SoundNames.ImpactA);
            LootTableName = nameof(Pottery);
            MaxHP = 6;
            PreventKnockback = true;
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
