using EngendroAdventure.Scripting;
using Remizione.Items;

namespace Remizione.Scripting
{
    // DamageBonusEffectCommand
    // Arguments: {ItemName} {Bonus:Float} #percentage
    internal sealed class DamageBonusEffectCommand : NonAwaitableCommand
    {
        // Constructor
        internal DamageBonusEffectCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2, CostArg, PercentageArg)
        {
            var name = MetaItemCommand.ParseItemName(this, 0);
            var bonus = Parser.ParseFloat(this, 1);
            var cost = Parser.ParseInt32Argument(this, CostArg);
            var percentage = HasArg(PercentageArg);

            if (MetaItem.Find(name) is MetaItem metaItem)
                metaItem.AddUpgradeEffect(new DamageBonusEffect(bonus, percentage), cost);
        }
    }
}
