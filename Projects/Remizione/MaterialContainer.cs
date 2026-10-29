using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// MaterialContainer
    /// </summary>
    public sealed class MaterialContainer
    {
        private readonly Dictionary<Material, int> materials = [];

        #region Private members

        // Refresh
        private void Refresh()
        {
            Total = 0;
            foreach (var value in materials.Values)
            {
                Total += value;
            }
        }

        #endregion

        // Add
        public void Add(Material material, int amount)
        {
            if (materials.ContainsKey(material))
                materials[material] += amount;
            else
                materials[material] = amount;

            if (materials[material] > GameSettings.MinMaterialsToCraft)
                materials[material] = GameSettings.MinMaterialsToCraft;

            Refresh();
            Version++;
        }

        // Clear
        public void Clear()
        {
            materials.Clear();
            Refresh();
        }

        // GetCount
        public int GetCount(Material material)
        {
            return materials.TryGetValue(material, out int value) ? value : 0;
        }

        // Remove
        public void Remove(Material material, int amount)
        {
            if (materials.ContainsKey(material))
            {
                materials[material] -= amount;
                if (materials[material] <= 0)
                    materials.Remove(material);
            }

            Refresh();
            Version++;
        }

        // Total
        public int Total { get; private set; }

        // Version
        public int Version { get; set; }
    }
}
