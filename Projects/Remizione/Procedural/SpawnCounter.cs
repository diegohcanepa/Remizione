using System;
using System.Collections.Generic;
using System.Text;

namespace Remizione.Procedural
{
    /// <summary>
    /// SpawnCounter
    /// </summary>
    public sealed class SpawnCounter
    {
        private readonly Dictionary<string, int> spawnData = [];

        // GetCount
        public int GetCount(string name)
        {
            return spawnData.TryGetValue(name, out var value) ? value : 0;
        }

        // Increment
        public void Increment(string name)
        {
            if (spawnData.TryGetValue(name, out var value))
                spawnData[name] = ++value;
            else
                spawnData[name] = 1;
        }

        // Reset
        public void Reset()
        {
            spawnData.Clear();
        }
    }
}
