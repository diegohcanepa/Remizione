using Engendro;

namespace EngendroAdventure
{
    /// <summary>
    /// EntityComponent
    /// </summary>
    public abstract class EntityComponent : GameObject
    {
        // Constructor
        protected EntityComponent(Entity entity)
            : base(entity.Game)
        {
            this.Entity = entity;
        }

        // Entity
        public Entity Entity { get; }
    }
}