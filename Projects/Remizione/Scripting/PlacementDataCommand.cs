using Adberration;
using Adberration.Scripting;
using Engendro;
using System.Collections.Generic;

namespace Remizione.Scripting
{
    // PlacementDataCommand
    // Arguments: {RoomKind} [#chance:Ratio] [#distribution:DistributionStrategy] [#instances:Int32Range] [#stage:Int32Range]
    internal sealed class PlacementDataCommand : NonAwaitableCommand
    {
        // Constructor
        internal PlacementDataCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, ChanceArg, DistributionArg, RoomPositionArg, StageArg, TriesArg)
        {
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession gameSession)
                return;

            var thing = AssertEntityNotNull<GameThing>(Script.EntityName);

            if (thing.InstanceKind != InstanceKind.Static)
                return;

            if (thing.PlacementPhase == PlacementPhase.None)
                throw new ScriptException(this, "PlacementPhase is not defined.");

            var roomKind = Parser.ParseEnum<RoomKind>(this, 0);
            var distributionStrategy = Parser.ParseEnumArgument(this, DistributionArg, PlacementDistributionStrategy.Random);
            var tries = HasArg(TriesArg) ? Parser.ParseInt32RangeArgument(this, TriesArg) : new Int32Range(1);
            var conditions = new List<PlacementCondition>();

            // Chance
            if (HasArg(ChanceArg))
            {
                var chance = Parser.ParseRatioArgument(this, ChanceArg);
                conditions.Add(new ChancePlacementCondition(chance));
            }

            // Room position
            if (HasArg(RoomPositionArg))
            {
                var roomPosition = Parser.ParseEnumArgument<RoomPosition>(this, RoomPositionArg);
                conditions.Add(new RoomPositionPlacementCondition(roomPosition));
            }

            // Stage
            if (HasArg(StageArg))
            {
                var progress = Parser.ParseInt32RangeArgument(this, StageArg);
                conditions.Add(new RunProgressPlacementCondition(progress));
            }

            // Placement data
            var placementData = new PlacementData(distributionStrategy, conditions.ToArray(), tries);
            gameSession.PlacementDataPool.Add(roomKind, thing.StaticName, placementData);
        }
    }
}