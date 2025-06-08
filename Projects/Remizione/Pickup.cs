using Engendro.Audio;
using EngendroAdventure.Scripting;

namespace Remizione
{
    /// <summary>
    /// Pickup
    /// </summary>
    public class Pickup : GameThing
    {
        // Constructor
        public Pickup(GameSession session, string name)
            : base(session, name)
        {
            this.Atlas = Atlases.Environment;
            this.DisplayName = $"Item.{StaticName}.Name";
            this.IgnoreWalkArea = false;
        }

        // PickUpSound
        [ScriptProperty(CodingContext.EntityDeclaration)]
        public Sound? PickUpSound { get; set; }
    }
}
