namespace Engendro
{
    /// <summary>
    /// UIComponent
    /// </summary>
    public abstract class UIComponent : GameObject
    {
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
