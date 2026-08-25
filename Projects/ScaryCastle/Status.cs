namespace ScaryCastle
{
    /// <summary>
    /// Status
    /// </summary>
    public sealed class Status
    {
        // Constructor
        public Status(StatusContainer container, StatusType statusType)
        {
            this.Container = container;
            this.StatusType = statusType;
            this.Definition = GameData.Statuses.Get(statusType.ToString());
        }

        // Apply
        public void Apply(int amount)
        {
            if (amount <= 0)
                return;

            Value += amount;

            if (Value >= MaxValue)
            {
                EffectDescriptor.Apply(Definition.EffectDescriptors, Container.Owner, Container.Owner, EffectContext.ApplyStatus);
                Value = 0;
            }

            Container.ContentVersion++;
        }

        // Container
        public StatusContainer Container { get; }

        // Definition
        public StatusDefinition Definition { get; }

        // MaxValue
        public const int MaxValue = 10;

        // StatusType
        public StatusType StatusType { get; }

        // Value
        public int Value { get; private set; }
    }
}
