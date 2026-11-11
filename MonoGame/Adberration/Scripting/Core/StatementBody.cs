using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Adberration.Scripting
{
    /// <summary>
    /// StatementBody
    /// </summary>
    public sealed class StatementBody
    {
        #region Constructor

        // Constructor
        internal StatementBody(ScriptEnvironment script, string[] tokens)
        {
            this.Owner = tokens[0];

            List<string> clauseList = [];
            List<StatementArg> flagList = [];

            // Iterate tokens. The zero-index is skipped because it is the statement name.
            for (var i = 1; i < tokens.Length; i++)
            {
                var token = tokens[i];

                if (token == ScriptSyntax.AwaitKeyword)
                {
                    Await = true;
                    continue;
                }

                if (token.StartsWith(ScriptSyntax.ArgumentPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    flagList.Add(new StatementArg(script, token));
                }
                else
                {
                    if (ScriptEnvironment.IsConstant(token))
                    {
                        var values = token.Split(',');

                        for (var j = 0; j < values.Length; j++)
                        {
                            if (script.IsConstantDeclared(values[j]))
                            {
                                values[j] = script.GetConstantValue(values[j]);
                            }
                        }

                        token = string.Join(",", values);
                    }

                    clauseList.Add(token);
                }
            }

            this.Clauses = new ReadOnlyCollection<string>(clauseList);
            this.Args = new StatementArgCollection(flagList);
        }

        #endregion

        // Args
        public StatementArgCollection Args { get; }

        // Await
        public bool Await { get; }

        // Clauses
        public ReadOnlyCollection<string> Clauses { get; }

        // Owner
        public string Owner { get; }
    }
}
