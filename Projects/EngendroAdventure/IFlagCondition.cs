using EngendroAdventure.Scripting;

namespace EngendroAdventure
{
    /// <summary>
    /// IFlagCondition
    /// </summary>
    public interface IFlagCondition
    {
        // Condition
        FlagCondition? Condition { get; }
    }
}
