namespace Remizione
{
    /// <summary>
    /// ItemEffect
    /// </summary>
    public abstract class ItemEffect
    {
        // Constructor
        protected ItemEffect(ItemEffectTiming effectTiming)
        {
            this.EffectTiming = effectTiming;
            this.TextRepositoryKey = "ItemEffects." + GetType().Name;
        }

        // TextRepositoryKey
        protected string TextRepositoryKey { get; }

        // OnApply
        protected virtual void OnApply(GameThing target)
        {
        }

        // Apply
        public void Apply(GameThing target)
        {
            if (CanApply(target))
                OnApply(target);
        }

        // CanApply
        public virtual bool CanApply(GameThing target) => true;

        // EffectTiming
        public ItemEffectTiming EffectTiming { get; }

        // GetLocalizedDescription
        public virtual string GetLocalizedDescription() => string.Empty;
    }
}
