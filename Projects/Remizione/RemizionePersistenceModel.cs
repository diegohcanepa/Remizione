using Adberration;
using Adberration.Persistence;
using System.Globalization;

namespace Remizione
{
    /// <summary>
    /// RemizionePersistenceModel
    /// </summary>
    internal sealed class RemizionePersistenceModel : PersistenceModel
    {
        // Constructor
        internal RemizionePersistenceModel()
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
