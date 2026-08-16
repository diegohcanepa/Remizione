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
        private void Apply()
        {
            EffectDescriptor.Apply(Definition.EffectDescriptors, manager.Owner, manager.Owner, EffectContext.ApplyStatus);
            Value = 0;
        }

        // Definition
        public StatusDefinition Definition { get; }

        // MaxValue
        public const int MaxValue = 10;

        // StatusType
        public StatusType StatusType { get; }

        // Value
        public int Value
        {
            get;
            set
            {
                if (value != field)
                {
                    field = int.Clamp(value, 0, MaxValue);
                    if (field == MaxValue)
                        Apply();
                    manager.ContentVersion++;
                }
            }
        }
    }
}
