namespace Engendro
{
    /// <summary>
    /// UIComponent
    /// </summary>
    public abstract class UIComponent : GameObject
    {
        // Constructor
        protected UIComponent(EngendroGame game)
            : base(game)
        {
        }

        // OnInvalidate
        protected virtual void OnInvalidate()
        {
        }

        // Invalidate
        public void Invalidate()
        {
            OnInvalidate();
        }
    }
}
