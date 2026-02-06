namespace ScaryCastle
{
    /// <summary>
    /// ProceduralActor
    /// </summary>
    public abstract class ProceduralActor : Actor, IProceduralThing
    {
        // Constructor
        protected ProceduralActor(GameSession session, string name)
            : base(session, name)
        {
            Definition = ThingDefinition.Get(DeclaredName);
        }

        // Definition
        public ThingDefinition Definition { get; }
    }
}
