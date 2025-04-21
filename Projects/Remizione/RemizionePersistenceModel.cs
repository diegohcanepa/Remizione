using EngendroAdventure;
using EngendroAdventure.Persistence;

namespace Remizione
{
    /// <summary>
    /// RemizionePersistenceModel
    /// </summary>
    internal sealed class RemizionePersistenceModel : PersistenceModel
    {
        // Constructor
        internal RemizionePersistenceModel()
            : base(GameSettings.Build.ToString())
        {
            PersistentType persistentType;

            persistentType = MapType(typeof(Actor));
            persistentType.Map(nameof(Actor.Effects));
            persistentType.Map(nameof(Actor.HP));
            persistentType.Map(nameof(Actor.Position));
            persistentType.Map(nameof(Actor.Stamina));
        }
    }
}
