using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EngendroAdventure.Scripting
{
    /// <summary>
    /// FlagCondition
    /// </summary>
    public sealed class FlagCondition
    {
        // Constructor
        internal FlagCondition(IList<FlagExpression> expressions)
        {
            this.Expressions = new ReadOnlyCollection<FlagExpression>(expressions);
        }

        // Evaluate
        public bool Evaluate()
        {
            for (var i = 0; i < Expressions.Count; i++)
            {
                if (Expressions[i].Negate)
                {
                    if (!Expressions[i].FlagValue == false)
                    {
                        return false;
                    }
                }
                else
                {
                    if (Expressions[i].FlagValue == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        // Expressions
        public ReadOnlyCollection<FlagExpression> Expressions { get; }
    }
}
