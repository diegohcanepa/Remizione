namespace Adberration.Scripting
{
    // NewCommand
    // Arguments: {DynamicName} [#at:Vector2] [#parent:Entity] [#persistent] [#range:Int32Range]
    internal sealed class NewCommand : NonAwaitableCommand
    {
        // Constructor
        internal NewCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, AtArg, ParentArg, PersistentArg, RangeArg)
        {
            //            if (!body.Clauses[0].Contains(ScriptSyntax.DynamicSuffix))
            //              throw new ScriptException(this, "Invalid dynamic name.");

            Parser.ParseInt32RangeArgument(this, RangeArg);

            ExecuteCore();
        }

        #region Private members

        // AssertEntityClassCreation
        private void AssertEntityClassCreation(string staticName)
        {
            // Check if declaration script exists
            var declarationScript = Session.ScriptLibrary.GetDeclaration(staticName) ?? throw new ScriptException(this, $"The entity class '{staticName}' is not declared.");

            // Check if entity is instantiable
            if (!declarationScript.Instantiable)
                throw new ScriptException($"The entity '{staticName}' cannot create dynamic instances. Use the '{ScriptSyntax.InstantiableKeyword}' keyword.");

            // Ensures that a dynamic entity is created in a dynamic declaration script
            if (Script.ScriptType != ScriptType.Instantiation)
            {
                var message = $"Dynamic entites can only be declared in [DynamicDeclaration] scripts.";
                throw new ScriptException(this, message);
            }
        }

        // CreateInstance
        private void CreateInstance(string name)
        {
            var staticName = ScriptSyntax.GetStaticName(name);
            var instanceName = name == staticName ? string.Empty : name;
            var thing = Session.ScriptEnvironment.CreateDynamicThing(staticName, instanceName, HasArg(PersistentArg));

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
                    CreateInstance(Body.Clauses[0] + i.ToString());
                }
            }
            else
                CreateInstance(Body.Clauses[0]);
        }

        #endregion
    }
}
