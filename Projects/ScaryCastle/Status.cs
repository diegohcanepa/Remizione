namespace ScaryCastle
{
    /// <summary>
    /// Status
    /// </summary>
    public sealed class Status
    {
        private readonly StatusManager manager;

        // Constructor
        public Status(StatusManager manager, StatusType statusType)
        {
            this.manager = manager;
            this.StatusType = statusType;
            this.Definition = StatusDefinition.Data.Get(statusType.ToString());
        }

        // Apply
        public void Apply(int amount)
        {
            if (amount <= 0)
                return;

            Value += amount;

            if (Value >= MaxValue)
            {
                EffectDescriptor.Apply(Definition.EffectDescriptors, manager.Owner, manager.Owner, EffectContext.ApplyStatus);
                Value = 0;
            }

            manager.ContentVersion++;
        }

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
