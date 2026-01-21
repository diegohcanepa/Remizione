using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace Adberration.Scripting
{
    /// <summary>
    /// ScriptLibrary
    /// </summary>
    public sealed class ScriptLibrary
    {
        #region Private fields

        private readonly Dictionary<string, Script> declarations = [];
        private readonly Dictionary<string, Script> scripts = [];

        #endregion

        #region Constructor

        // Constructor
        internal ScriptLibrary(Session session, string path)
        {
            CodeContract.NotEmpty(path, nameof(path));

            this.Session = session;
            this.Path = path;
        }

        #endregion

        #region Private members

        // LoadCore
        private void LoadCore()
        {
            List<string> lines = [];

            using (var stream = TitleContainer.OpenStream(Path))
            {
                string text;

                // No encryption
                if (!XOREncryptor.IsEncryptedXml(stream))
                {
                    using StreamReader r = new(stream);
                    text = r.ReadToEnd();
                }
                else
                {
                    text = XOREncryptor.AsString(stream, XOREncryptor.EncryptionKey);
                }

                // Load lines
                using (StringReader reader = new(text))
                {
                    // Get version
                    Version = reader.ReadLine();

                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();

                        // Remove blank spaces or comments
                        var skip = line.Length == 0 || line.StartsWith(ScriptSyntax.LineComment, StringComparison.OrdinalIgnoreCase);
                        if (!skip)
                        {
                            lines.Add(line);
                        }
                    }
                }

                // Read all lines
                while (lines.Count > 0)
                {
                    Script script = new(Session, lines);

                    if (!string.IsNullOrWhiteSpace(script.EntityName) && script.HasCapability(ScriptCapability.EntityDeclaration))
                    {
                        declarations.Add(script.EntityName, script);
                    }

                    scripts.Add(script.Signature, script);
                }
            }

            // Step 1: Run initialization scripts
            RunInitializationScripts(false);

            // Step 2: Create declared entities (rooms)
            foreach (var script in scripts.Values)
            {
                if (script.HasCapability(ScriptCapability.EntityDeclaration) && script.ScriptType == ScriptType.Room)
                {
                    var createdEntity = script.CreateEntity();
                    createdEntity.Persistent = script.Persistent;
                }
            }

            // Step 3: Create declared entities (things)
            foreach (var script in scripts.Values)
            {
                if (script.HasCapability(ScriptCapability.EntityDeclaration) && script.ScriptType != ScriptType.Room)
                {
                    var createdEntity = script.CreateEntity();
                    createdEntity.Persistent = script.Persistent;
                }
            }

            // Step 4: Run post-initialization scripts
            RunInitializationScripts(true);
        }

        // RunInitializationScripts
        private void RunInitializationScripts(bool post)
        {
            var scriptType = post ? ScriptType.Initialization : ScriptType.Declaration;

            List<Script> scriptsList = [];

            foreach (var script in scripts.Values)
            {
                if (script.ScriptType == scriptType)
                {
                    scriptsList.Add(script);
                }
            }

            for (var i = 0; i < scriptsList.Count; i++)
            {
                var script = scriptsList[i];

                script.Compile();
                Session.ScriptProcessor.RunScript(script);

                if (script.HasCapability(ScriptCapability.Discard))
                {
                    scripts.Remove(script.Signature);
                }
            }
        }

        #endregion

        #region Internal members

        // CompilationPhase
        internal CompilationPhase CompilationPhase { get; private set; }

        // Compile
        internal void Compile()
        {
            List<Script> discardList = [];

            // Compile declarations
            CompilationPhase = CompilationPhase.Declarations;
            foreach (var script in AllScripts)
            {
                if (script.HasCapability(ScriptCapability.EntityDeclaration))
                {
                    script.Compile();
                    Session.ScriptProcessor.RunScript(script);

                    if (script.HasCapability(ScriptCapability.Discard))
                    {
                        discardList.Add(script);
                    }
                }
            }

            // Compile cloning scripts
            CompilationPhase = CompilationPhase.Cloning;
            foreach (var script in AllScripts)
            {
                if (script.ScriptType == ScriptType.Cloning)
                {
                    script.Compile();
                    Session.ScriptProcessor.RunScript(script);

                    if (script.HasCapability(ScriptCapability.Discard))
                    {
                        discardList.Add(script);
                    }
                }
            }

            // Remove useless scripts
            for (var i = 0; i < discardList.Count; i++)
            {
                scripts.Remove(discardList[i].Signature);
            }

            // Compile routines
            CompilationPhase = CompilationPhase.Routines;
            foreach (var script in AllScripts)
            {
                if (script.ScriptType is ScriptType.Routine or ScriptType.NewSession)
                {
                    script.Compile();
                }
            }

            // Compile all other scripts (outcomes)
            CompilationPhase = CompilationPhase.Outcomes;
            foreach (var script in AllScripts)
            {
                if (script.HasCapability(ScriptCapability.EntityDeclaration))
                {
                    continue;
                }

                script.Compile();
                TotalRuntimeStatements += script.StatementCount;
            }
        }

        // FindDeclaration
        internal Script? FindDeclaration(string entityStaticName)
        {
            return declarations.TryGetValue(entityStaticName, out var result) ? result : null;
        }

        // FindScript
        internal Script? FindScript(string name)
        {
            return scripts.TryGetValue(name, out var result) ? result : null;
        }

        // FindScript
        internal Script? FindScript(ScriptType scriptType, string name)
        {
            CodeContract.NotEmpty(name, nameof(name));
            var scriptName = Script.EncodeScriptName(scriptType, name);
            return FindScript(scriptName);
        }

        // Load
        internal void Load()
        {
            LoadCore();
            Compile();
        }

        #endregion

        // AllScripts
        public IEnumerable<Script> AllScripts => scripts.Values;

        // FindOutcome
        public Script? FindOutcome(string name)
        {
            CodeContract.NotEmpty(name, nameof(name));
            return FindScript(ScriptType.Outcome, name);
        }

        // FindRoutine
        public Script? FindRoutine(string name)
        {
            return FindScript(ScriptType.Routine, name);
        }

        // IsDeclared
        public bool IsDeclared(string entityStaticName)
        {
            CodeContract.NotEmpty(entityStaticName, nameof(entityStaticName));
            return declarations.ContainsKey(entityStaticName);
        }

        // Path
        public string Path { get; }

        // Session
        public Session Session { get; }

        // TotalRuntimeStatements
        public int TotalRuntimeStatements { get; private set; }

        // Version
        public string? Version { get; private set; }
    }
}
