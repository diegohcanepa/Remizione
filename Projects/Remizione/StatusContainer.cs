using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ScaryCastle
{
    /// <summary>
    /// StatusContainer
    /// </summary>
    public sealed class StatusContainer
    {
        private readonly Dictionary<StatusType, Status> statusByType = [];
        private readonly List<Status> statusList = [];

        // Constructor
        public StatusContainer(Actor owner)
        {
            this.Owner = owner;
            this.Statuses = new(statusList);
        }

        // Apply
        public Status Apply(StatusType statusType, int amount)
        {
            if (!statusByType.TryGetValue(statusType, out Status? status))
            {
                status = new Status(this, statusType);
                statusByType.Add(statusType, status);
                statusList.Add(status);
            }

            status.Apply(amount);

            if (status.Value == 0)
                Discard(statusType);

            return status;
        }

        // Clear
        public void Clear()
        {
            statusByType.Clear();
            statusList.Clear();
        }

        // ContentVersion
        public int ContentVersion { get; set; }

        // Discard
        public void Discard(StatusType statusType)
        {
            if (statusByType.TryGetValue(statusType, out Status? status))
            {
                statusByType.Remove(statusType);
                statusList.Remove(status);
                ContentVersion++;
            }
        }

        // Find
        public Status? Find(StatusType statusType)
        {
            return statusByType.TryGetValue(statusType, out Status? status) ? status : null;
        }

        // Owner
        public Actor Owner { get; }

        // Statuses
        public ReadOnlyCollection<Status> Statuses { get; }
    }
}
