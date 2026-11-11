namespace Remizione
{
    /// <summary>
    /// Invocation
    /// </summary>
    public abstract class Invocation : GameThing
    {
        // Constructor
        protected Invocation(IAction action, GameThing target)
            : base(target.Session, string.Empty)
        {
            this.Action = action;
            this.Target = target;
            this.RenderLayer = RenderLayer.OverBackground;
            this.Atlas = Atlases.Environment;
            this.Scale = ScaleInfo.UIElement.Small;
        }

        // Action
        public IAction Action { get; }

        // Target
        public GameThing Target { get; }
    }
}
