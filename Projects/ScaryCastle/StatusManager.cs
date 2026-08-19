using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ScaryCastle
{
    /// <summary>
    /// StatusManager
    /// </summary>
    public sealed class StatusManager
    {
        private readonly Dictionary<StatusType, Status> statuses = [];
        private readonly List<Status> statusList = [];

        // Constructor
        public StatusManager(Actor owner)
        {
            this.Owner = owner;

            foreach (var statusType in Enum.GetValues<StatusType>())
            {
                var status = new Status(this, statusType);
                statuses.Add(statusType, status);
                statusList.Add(status);
            }

            this.Statuses = new(statusList);
        }

        // Add
        public void Add(StatusType statusType, int amount)
        {
            if (statuses.TryGetValue(statusType, out Status? status))
                status.Value += amount;
        }

        // Clear
        public void Clear()
        {
            for (var i = 0; i < statusList.Count; i++)
            {
                statusList[i].Value = 0;
            }
        }

        // ContentVersion
        public int ContentVersion { get; set; }

        // GetStatus
        public Status GetStatus(StatusType statusType)
        {
            return statuses[statusType];
        }

        // Owner
        public Actor Owner { get; }

        // Statuses
        public ReadOnlyCollection<Status> Statuses { get; }
    }
}
