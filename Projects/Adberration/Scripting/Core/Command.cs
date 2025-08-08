using System;
using System.Linq;

namespace Adberration.Scripting
{
    /// <summary>
    /// Command
    /// </summary>
    public abstract class Command : Statement
    {
        #region Constructor

        // Constructor
        protected Command(Script script, StatementType statementType, string source, StatementBody body, int clauseCount, params string[] argList)
            : base(script, statementType, source, body, clauseCount)
        {
            for (var i = 0; i < Body.Args.Count; i++)
            {
                if (!argList.Contains(Body.Args[i].Name))
                {
                    throw new ScriptException(this, $"'{Body.Args[i].Name}' is not a valid argument.");
                }
            }
        }

        #endregion

        #region Protected members

        // OnExecute
        protected virtual void OnExecute()
        {
        }

        // OnExecutionCompleted
        protected virtual void OnExecutionCompleted()
        {
        }

        #endregion

        #region Internal members

        // CanBeginExecution
        internal protected virtual bool CanBeginExecution()
        {
            return true;
        }

        // Done
        internal void Done()
        {
            IsExecuted = false;
            OnExecutionCompleted();
        }

        // Execute
        internal void Execute()
        {
            if (IsExecuted)
            {
                throw new InvalidOperationException("Statement already executed.");
            }

            OnExecute();
            IsExecuted = true;
        }

        #endregion

        // GetAssetFiles
        public virtual string[]? GetAssetFiles()
        {
            return null;
        }

        // IsExecuted
        public bool IsExecuted { get; private set; }
    }
}
