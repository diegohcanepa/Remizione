namespace ScaryCastle
{
    /// <summary>
    /// ProceduralProp
    /// </summary>
    public abstract class ProceduralProp : Prop, IThingDefinition
    {
        // Constructor
        protected ProceduralProp(GameSession session, string name)
            : base(session, name)
        {
            Definition = PropDefinition.Definitions.Get(DeclaredName);
        }

        #region IProceduralThing explicit implementation

        // Definition
        ThingDefinition IThingDefinition.Definition => this.Definition;

        #endregion

        // Definition
        public PropDefinition Definition { get; }
    }
}
