using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// PlayerStats
    /// </summary>
    public sealed class PlayerStats
    {
        private readonly List<Stat> stats = [];

        // Constructor
        public PlayerStats()
        {
            AmbientLight = AddStat(1);
            Luck = AddStat(1);
        }

        #region Private members

        // AddStat
        private Stat AddStat(float baseValue)
        {
            var result = new Stat(baseValue);
            stats.Add(result);
            return result;
        }

        #endregion

        // AmbientLight
        public Stat AmbientLight { get; }

        // Luck
        public Stat Luck { get; }

        // RemoveAllModifiers
        public void RemoveAllModifiers(object source)
        {
            AmbientLight.RemoveModifiers(source);
            Luck.RemoveModifiers(source);
        }

        // Reset
        public void Reset()
        {
            foreach (var stat in stats)
            {
                stat.Clear();
            }
        }
    }
}
