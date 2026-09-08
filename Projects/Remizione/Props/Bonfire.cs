using Adberration.Scripting;
using Engendro.Audio;

namespace Remizione
{
    /// <summary>
    /// Bonfire
    /// </summary>
    public class Bonfire : Prop
    {
        // Constructor
        public Bonfire(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Props;
        }

        // IsLit
        [ScriptProperty]
        public bool IsLit { get; set; }
    }
}
