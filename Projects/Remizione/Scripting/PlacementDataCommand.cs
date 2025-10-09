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
            : base(script, source, body, 1, ChanceArg, CompletedRunsArg, DistributionArg, MaximumArg, MaximumPerRunArg, RollsArg, RoomPhaseArg, RoomWidthArg, StageArg)
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
            var rolls = HasArg(RollsArg) ? Parser.ParseInt32RangeArgument(this, RollsArg) : new Int32Range(1);
            var maximum = HasArg(MaximumArg) ? Parser.ParseInt32Argument(this, MaximumArg) : 0;
            var maximumPerRun = HasArg(MaximumPerRunArg) ? Parser.ParseInt32Argument(this, MaximumPerRunArg) : 0;
            var conditions = new List<PlacementCondition>();

            // Chance
            if (HasArg(ChanceArg))
            {
                var chance = Parser.ParseRatioArgument(this, ChanceArg);
                conditions.Add(new ChancePlacementCondition(chance));
            }

            // Completed runs
            if (HasArg(CompletedRunsArg))
            {
                var completedRuns = Parser.ParseInt32RangeArgument(this, CompletedRunsArg);
                conditions.Add(new CompletedRunsPlacementCondition(completedRuns));
            }

            // Room phase
            if (HasArg(RoomPhaseArg))
            {
                var roomPhase = Parser.ParseEnumArgument<RunPhase>(this, RoomPhaseArg);
                conditions.Add(new RunPhasePlacementCondition(roomPhase));
            }

            // Room width
            if (HasArg(RoomWidthArg))
            {
                var width = Parser.ParseInt32RangeArgument(this, RoomWidthArg);
                conditions.Add(new RoomWidthPlacementCondition(width));
            }

            // Stage
            if (HasArg(StageArg))
            {
                var progress = Parser.ParseInt32RangeArgument(this, StageArg);
                conditions.Add(new CompletedRunsPlacementCondition(progress));
            }

            // Placement data
            var placementData = new PlacementData(distributionStrategy, conditions.ToArray(), rolls, maximum, maximumPerRun);
            gameSession.PlacementDataPool.Add(roomKind, thing.StaticName, placementData);
        }
    }
}