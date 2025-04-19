namespace TextRepositoryEditor
{
    /// <summary>
    /// PropertyGridNodeWrapper
    /// </summary>
    public abstract class PropertyGridNodeWrapper<T> where T : CustomTreeNode
    {
        // Constructor
        public PropertyGridNodeWrapper(T node)
        {
            this.Node = node;
        }

        // Node
        protected T Node { get; }
    }
}
