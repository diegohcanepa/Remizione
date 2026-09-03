using Adberration;
using Adberration.Persistence;
using System.Globalization;

namespace ScaryCastle
{
    /// <summary>
    /// ScaryCastlePersistenceModel
    /// </summary>
    internal sealed class ScaryCastlePersistenceModel : PersistenceModel
    {
        // Constructor
        internal ScaryCastlePersistenceModel()
            : base(GameSettings.Build.ToString(CultureInfo.InvariantCulture))
        {
            PersistentType persistentType;

            persistentType = MapType(typeof(Actor));
            persistentType.Map(nameof(Actor.Effects));
            persistentType.Map(nameof(Actor.HP));
            persistentType.Map(nameof(Actor.Position));
        }
    }
}
