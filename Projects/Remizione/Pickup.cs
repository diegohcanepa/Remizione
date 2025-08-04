using Engendro.Audio;
using EngendroAdventure.Scripting;

namespace Remizione
{
    /// <summary>
    /// Pickup
    /// </summary>
    public class Pickup : Prop
    {
        // Constructor
        public Pickup(GameSession session, string name)
            : base(session, name)
        {
            this.Atlas = Atlases.Environment;
            this.CellMargin = 0;
            this.DisplayName = $"Item.{StaticName}.Name";
            this.IgnoreWalkArea = false;
        }

        // PickUpSound
        [ScriptProperty(CodingContext.EntityDeclaration)]
        public Sound? PickUpSound { get; set; }
    }
}
