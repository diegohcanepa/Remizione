using EngendroAdventure.Scripting;

namespace Remizione.Scripting
{
    // PlacementConditionCommand
    // Arguments: {Phase:PlacementPhase} [#block-tag:WorldBlockTag] [#chance:Ratio] [#cycles:Int32Range] [#instances:Int32Range] [#player-level:Int32Range] [#world-size:Integer]
    internal sealed class PlacementConditionCommand : NonAwaitableCommand
    {
        // Constructor
        internal PlacementConditionCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, BlockTagArg, ChanceArg, CyclesArg, InstancesArg, PlayerLevelArg, WorldSizeArg)
        {
            var thing = AssertEntityNotNull<GameThing>(Script.EntityName);
            thing.PlacementPhase = Parser.ParseEnum<PlacementPhase>(this, 0);

            // Chance
            if (HasArg(ChanceArg))
            {
                var chance = Parser.ParseRatioArgument(this, ChanceArg);
                thing.AddPlacementCondition(new ChancePlacementCondition(chance));
            }

            // Instances
            if (HasArg(InstancesArg))
                thing.InstancesPerBlock = Parser.ParseInt32RangeArgument(this, InstancesArg);

            // Player level
            if (HasArg(PlayerLevelArg))
            {
                var playerLevel = Parser.ParseInt32RangeArgument(this, PlayerLevelArg);
                thing.AddPlacementCondition(new PlayerLevelPlacementCondition(playerLevel));
            }
        }
    }
}
