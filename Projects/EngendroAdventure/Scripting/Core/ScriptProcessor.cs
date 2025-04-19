using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace EngendroAdventure.Scripting
{
    /// <summary>
    /// ScriptProcessor
    /// </summary>
    public sealed class ScriptProcessor
    {
        private readonly GameTime emptyGameTime = new();
        private readonly List<Script> scripts = [];

        #region Constructor

        // Constructor
        internal ScriptProcessor(Session session)
        {
            this.Session = session;
        }

        #endregion

        #region Private members

        // UpdateScripts
        private void UpdateScripts(GameTime gameTime)
        {
            if (scripts.Count > 0)
            {
                for (var i = scripts.Count - 1; scripts.Count > 0 && i >= 0; i--)
                {
                    var script = scripts[i];

                    if (!script.IsDiscarded)
                    {
                        script.Update(gameTime);
                    }

                    // If script is done
                    if (script.IsCompleted || script.IsDiscarded)
                    {
                        scripts.Remove(script);
                    }

                    if (Session.IsDisposed)
                    {
                        return;
                    }
                }
            }
        }

        #endregion

        #region Internal members

        // Update
        internal void Update(GameTime gameTime)
        {
            UpdateScripts(gameTime);
        }

        #endregion

        // ExecuteCommand
        public void ExecuteCommand(string sourceLine)
        {
            if (Script.CreateStatement(Session.ScriptEnvironment, new Script.RuntimeScript(Session, sourceLine), sourceLine) is Command command)
            {
                command.Execute();
            }
            else
            {
                throw new ScriptException($"Unrecognized command '{sourceLine}'.");
            }
        }

        // IsExecutingScript
        public bool IsExecutingScript(Script script)
        {
            return scripts.Count > 0 && scripts.Contains(script) && !script.IsDiscarded;
        }

        // PauseScript
        public void PauseScript(Script script)
        {
            if (scripts.Contains(script))
            {
                script.IsPaused = true;
            }
        }

        // ResumeScript
        public void ResumeScript(Script script)
        {
            if (scripts.Contains(script))
            {
                script.IsPaused = false;
            }
        }

        // RunScript
        public void RunScript(Script script)
        {
            if (IsExecutingScript(script))
            {
                return;
            }

            script.PrepareForExecution();
            GameTime gameTime = new();
            while (!script.IsCompleted)
            {
                script.Update(gameTime);
            }
        }

        // Session
        public Session Session { get; }

        // StartScript
        public void StartScript(Script script)
        {
            if (!script.IsCompiled)
            {
                throw new InvalidOperationException("Script not compiled.");
            }

            // Check if the same script is not already running
            var isExecuting = IsExecutingScript(script);
            if (isExecuting && !script.IsDiscarded)
            {
                return;
            }

            script.PrepareForExecution();

            if (!isExecuting)
            {
                if (!scripts.Contains(script))
                {
                    scripts.Add(script);
                }

                script.Update(emptyGameTime);
            }
        }

        // StopScript
        public void StopScript(Script script)
        {
            if (IsExecutingScript(script))
            {
                script.Discard();
            }
        }
    }
}
