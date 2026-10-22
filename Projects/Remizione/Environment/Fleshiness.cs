using Engendro;

namespace Remizione
{
    /// <summary>
    /// Fleshiness
    /// </summary>
    public sealed class Fleshiness : Prop
    {
        // Constructor
        public Fleshiness(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Environment;
            LabelKey = "Prop.Fleshiness";
            Hotspot = new Polygon("8,1;22,4;22,7;0,7;0,1");
            Opacity = .5f;
            RenderLayer = RenderLayer.Background;
        }

        // CanInteract
        public override bool CanInteract()
        {
            return true;
        }
    }
}
