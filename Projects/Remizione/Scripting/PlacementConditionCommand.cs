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

            // Block tag
            if (HasArg(BlockTagArg))
            {
                var tag = Parser.ParseEnumArgument<WorldBlockTag>(this, BlockTagArg);
                thing.AddPlacementCondition(new BlockTagPlacementCondition(tag));
            }

            // Chance
            if (HasArg(ChanceArg))
            {
                var chance = Parser.ParseRatioArgument(this, ChanceArg);
                thing.AddPlacementCondition(new ChancePlacementCondition(chance));
            }

            // Cycles
            if (HasArg(CyclesArg))
            {
                var cycles = Parser.ParseInt32RangeArgument(this, InstancesArg);
                thing.AddPlacementCondition(new WorldCyclesPlacementCondition(cycles));
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

            // World size
            if (HasArg(WorldSizeArg))
            {
                var worldSize = Parser.ParseInt32RangeArgument(this, WorldSizeArg);
                thing.AddPlacementCondition(new WorldSizePlacementCondition(worldSize));
            }
        }
    }
}
