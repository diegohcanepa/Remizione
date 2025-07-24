using Engendro;
using EngendroAdventure;
using EngendroAdventure.Scripting;
using System.Collections.Generic;

namespace Remizione.Scripting
{
    // PlacementDataCommand
    // Arguments: {Room} {Phase:PlacementPhase} [#chance:Ratio] [#distribution:DistributionStrategy] [#instances:Int32Range] [#progress:Int32Range]
    internal sealed class PlacementDataCommand : NonAwaitableCommand
    {
        // Constructor
        internal PlacementDataCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2, ChanceArg, DistributionArg, InstancesArg, ProgressArg)
        {
            var thing = AssertEntityNotNull<GameThing>(script.EntityName);

            if (thing.EntityKind != EntityKind.Static)
                return;

            var room = AssertEntityNotNull<ProceduralRoom>(0);
            var phase = Parser.ParseEnum<PlacementPhase>(this, 1);
            var distributionStrategy = Parser.ParseEnumArgument(this, DistributionArg, PlacementDistributionStrategy.Random);
            var maxInstances = HasArg(InstancesArg) ? Parser.ParseInt32RangeArgument(this, InstancesArg) : new Int32Range(1);
            var conditions = new List<PlacementCondition>();

            // Chance
            if (HasArg(ChanceArg))
            {
                var chance = Parser.ParseRatioArgument(this, ChanceArg);
                conditions.Add(new ChancePlacementCondition(chance));
            }

            // Progress
            if (HasArg(ProgressArg))
            {
                var progress = Parser.ParseInt32RangeArgument(this, ProgressArg);
                conditions.Add(new SessionLevelPlacementCondition(progress));
            }

            // Placement data
            var placementData = new PlacementData(phase, distributionStrategy, conditions.ToArray(), maxInstances);
            room.AddPlacementData(thing.StaticName, placementData);
        }
    }
}
