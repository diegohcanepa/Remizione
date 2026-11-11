using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// StatusContainer
    /// </summary>
    public sealed class StatusContainer
    {
        private readonly Dictionary<StatusType, int> statuses = [];

        // Apply
        public bool Apply(StatusType status)
        {
            if (statuses.ContainsKey(status))
            {
                Version++;
                statuses[status] += 1;
                if (statuses[status] >= Threshold)
                {
                    statuses[status] = 0;
                    return true;
                }
            }

            return false;
        }

        // Clear
        public void Clear()
        {
            statuses.Clear();
        }

        // GetCount
        public int GetCount(StatusType status)
        {
            return statuses.TryGetValue(status, out int value) ? value : 0;
        }

        // Remove
        public bool Remove(StatusType status)
        {
            if (statuses.Remove(status))
            {
                Version++;
                return true;
            }

            return false;
        }

        // Threshold
        public const float Threshold = 4;

        // Version
        public int Version { get; set; }
    }
}
