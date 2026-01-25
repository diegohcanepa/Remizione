using System.Globalization;

namespace Adberration.Scripting
{
    // CloneCommand
    // Arguments: {CloneName} [#at:Vector2] [#parent:Entity] [#persistent] [#range:Int32Range]
    [ScriptStatement(CodingContext.Instantiation)]
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
        private void AssertEntityClassCreation(string declaredName)
        {
            // Check if declaration script exists
            var declarationScript = Session.ScriptLibrary.FindDeclaration(declaredName) ?? throw new ScriptException(this, $"The entity class '{declaredName}' is not declared.");

            // Check if entity is cloneable
            if (!declarationScript.Cloneable)
                throw new ScriptException($"The entity '{declaredName}' cannot create clones. Use the '{ScriptSyntax.CloneableKeyword}' keyword.");

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
            var declaredName = ScriptSyntax.GetDeclaredName(name);
            var instanceName = name == declaredName ? string.Empty : name;
            var thing = Session.ScriptEnvironment.CreateThingClone(declaredName, instanceName, HasArg(PersistentArg));

            // Parent (assign parent at last place to ensure correct values before the controller starts)
            var flag = Body.Args.FindArg(ParentArg);
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
            AssertEntityClassCreation(ScriptSyntax.GetDeclaredName(Body.Clauses[0]));

            if (HasArg(RangeArg))
            {
                var range = Parser.ParseInt32RangeArgument(this, RangeArg);
                for (var i = range.Minimum; i <= range.Maximum; i++)
                {
                    CreateCloneInstance(Body.Clauses[0] + i.ToString(CultureInfo.InvariantCulture));
                }
            }
            else
                CreateCloneInstance(Body.Clauses[0]);
        }

        #endregion
    }
}
