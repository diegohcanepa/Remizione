namespace ScaryCastle
{
    /// <summary>
    /// ProceduralProp
    /// </summary>
    public abstract class ProceduralProp : Prop, IProceduralThing
    {
        // Constructor
        protected ProceduralProp(GameSession session, string name)
            : base(session, name)
        {
            Definition = ThingDefinition.Get(DeclaredName);
        }

        // Definition
        public ThingDefinition Definition { get; }
    }
}
