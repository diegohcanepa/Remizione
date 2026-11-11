using Microsoft.Xna.Framework;
using System.Reflection;

namespace Adberration.Scripting
{
    /// <summary>
    /// AwaitableCommand
    /// </summary>
    public abstract class AwaitableCommand : Command
    {
        // Constructor
        protected AwaitableCommand(Script script, string source, StatementBody body, int clauseCount, params string[] supportedFlags)
            : base(script, StatementType.AwaitableCommand, source, body, clauseCount, supportedFlags)
        {
            ImplicitAwait = GetType().GetTypeInfo().GetCustomAttribute<ForceAwaitAttribute>() != null;

            if (!Script.HasCapability(ScriptCapability.Await) && ShouldAwait)
                throw new ScriptException(this, $"The script {Script.Name} is not awaitable.");
        }

        #region Protected members

        // OnUpdate
        protected virtual void OnUpdate(GameTime gameTime)
        {
        }

        #endregion

        #region Internal members

        // Update
        internal void Update(GameTime gameTime)
        {
            OnUpdate(gameTime);
        }

        #endregion

        // ImplicitAwait
        public bool ImplicitAwait { get; }

        // IsAwaiting
        public virtual bool IsAwaiting()
        {
            return false;
        }

        // ShouldAwait
        public bool ShouldAwait => Body.Await || ImplicitAwait;
    }
}
