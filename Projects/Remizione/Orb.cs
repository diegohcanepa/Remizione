using Engendro;

namespace Remizione
{
    /// <summary>
    /// Orb
    /// </summary>
    public class Orb : Pickup
    {
        // Constructor
        public Orb(GameSession session, string name)
            : base(session, name)
        {
            DefaultImageName = "Orb";
            Highlight = false;
            Tweens.OpacityTween = FloatTween.Create(TweenStyle.Linear, .4f, .6f, 70, -1);
        }
    }
}
