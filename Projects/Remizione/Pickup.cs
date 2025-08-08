using Engendro.Audio;
using Adberration.Scripting;

namespace Remizione
{
    /// <summary>
    /// Pickup
    /// </summary>
    public class Pickup : Prop
    {
        private string itemName = string.Empty;

        // Constructor
        public Pickup(GameSession session, string name)
            : base(session, name)
        {
            this.Atlas = Atlases.Environment;
            this.CellMargin = 0;
            this.IgnoreWalkArea = false;
            this.ItemName = StaticName;
        }

        // ItemName
        [ScriptProperty]
        public string ItemName
        {
            get => itemName;
            protected set
            {
                if (itemName != value)
                {
                    itemName = value;
                    DisplayName = $"Item.{itemName}.Name";
                }
            }
        }

        // PickUpSound
        [ScriptProperty(CodingContext.EntityDeclaration)]
        public Sound? PickUpSound { get; set; }
    }
}
