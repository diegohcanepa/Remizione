namespace Adberration.Scripting
{
    // CloneCommand
    // Arguments: {CloneName} [#at:Vector2] [#parent:Entity] [#persistent] [#range:Int32Range]
    internal sealed class CloneCommand : NonAwaitableCommand
    {
        // Constructor
        internal CloneCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, AtArg, ParentArg, PersistentArg, RangeArg)
        {
            //            if (!body.Clauses[0].Contains(ScriptSyntax.CloneSuffix))
            //              throw new ScriptException(this, "Invalid clone name.");

            Parser.ParseInt32RangeArgument(this, RangeArg);

            ExecuteCore();
        }

        #region Private members

        // AssertEntityClassCreation
        private void AssertEntityClassCreation(string staticName)
        {
            // Check if declaration script exists
            var declarationScript = Session.ScriptLibrary.GetDeclaration(staticName) ?? throw new ScriptException(this, $"The entity class '{staticName}' is not declared.");

            // Check if entity is cloneable
            if (!declarationScript.Cloneable)
                throw new ScriptException($"The entity '{staticName}' cannot create clones. Use the '{ScriptSyntax.CloneableKeyword}' keyword.");

            // Ensures that a cloned entity is created in a clone declaration script
            if (Script.ScriptType != ScriptType.Cloning)
            {
                var message = $"Clones can only be declared in [{ScriptType.Cloning}] scripts.";
                throw new ScriptException(this, message);
            }
        }

        // CreateCloneInstance
        private void CreateCloneInstance(string name)
        {
            var staticName = ScriptSyntax.GetStaticName(name);
            var instanceName = name == staticName ? string.Empty : name;
            var thing = Session.ScriptEnvironment.CreateRuntimeThingClone(staticName, instanceName, HasArg(PersistentArg));

            // Parent (assign parent at last place to ensure correct values before the controller starts)
            var flag = Body.Args.GetArg(ParentArg);
            if (flag?.Value != null)
            {
                if (AssertEntity<Entity>(flag.Value) is Entity parent)
                    parent.Children.Add(thing);
            }

            if (HasArg(AtArg))
                thing.Position = Parser.ParseVector2Argument(this, AtArg);
        }

        // ExecuteCore
        private void ExecuteCore()
        {
            AssertEntityClassCreation(ScriptSyntax.GetStaticName(Body.Clauses[0]));

            if (HasArg(RangeArg))
            {
                var range = Parser.ParseInt32RangeArgument(this, RangeArg);
                for (var i = range.Minimum; i <= range.Maximum; i++)
                {
                    CreateCloneInstance(Body.Clauses[0] + i.ToString());
                }
            }
            else
                CreateCloneInstance(Body.Clauses[0]);
        }

        #endregion
    }
}
