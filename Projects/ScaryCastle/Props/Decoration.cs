using Engendro;

namespace ScaryCastle.Props
{
    /// <summary>
    /// Decoration
    /// </summary>
    public sealed class Decoration : Prop
    {
        // Constructor
        public Decoration(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Props;
            CollisionDetection = false;
            AutoPlayAnimation = false;
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            AnimationPlayer.GoTo(FramePosition.Random);
        }
    }
}
