using Adberration;
using Adberration.Persistence;

namespace ScaryCastle
{
    /// <summary>
    /// ScaryCastlePersistenceModel
    /// </summary>
    internal sealed class ScaryCastlePersistenceModel : PersistenceModel
    {
        // Constructor
        internal ScaryCastlePersistenceModel()
            : base(GameSettings.Build.ToString())
        {
            PersistentType persistentType;

            persistentType = MapType(typeof(Actor));
            persistentType.Map(nameof(Actor.Effects));
            persistentType.Map(nameof(Actor.HP));
            persistentType.Map(nameof(Actor.Position));
        }
    }
}
