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

            // Actor
            persistentType = Track(typeof(Actor));
            persistentType.Track(nameof(Actor.Effects));
            persistentType.Track(nameof(Actor.HP));
            persistentType.Track(nameof(Actor.Position));

            // Bonfire
            persistentType = Track(typeof(Bonfire));
            persistentType.Track(nameof(Bonfire.IsLit));

            // Fleshines
            persistentType = Track(typeof(Fleshiness));
            persistentType.Track(nameof(Fleshiness.Position));

            // Trunk
            persistentType = Track(typeof(Trunk));
            persistentType.Track(nameof(Trunk.IsOpen));
            persistentType.Track(nameof(Trunk.ItemRewardName));
        }
    }
}
