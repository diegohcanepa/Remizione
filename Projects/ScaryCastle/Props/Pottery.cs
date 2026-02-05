using Engendro.Audio;

namespace ScaryCastle
{
    /// <summary>
    /// Pottery
    /// </summary>
    public class Pottery : BreakableProp
    {
        // Constructor
        public Pottery(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Environment;
            DeathSound = Sound.Find(SoundNames.PotteryBreak);
            DepthOffset = -2;
            DisplayNameKey = "Prop.Pottery";
            HitTestPolygon = TestPolygon.Hotspot;
            HurtSound = Sound.Find(SoundNames.ImpactA);
        }

        // GetMouseCursorState
        public override MouseCursorState? GetMouseCursorState()
        {
            return MouseCursorState.Hit;
        }

        // OnDie
        protected override void OnDie()
        {
            base.OnDie();
            ShowImpactWord(ImpactWordName.CrackYellow);
        }
    }
}
