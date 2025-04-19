using System.Globalization;

namespace EngendroAdventure.Scripting
{
    // SetPropertyCommand
    // Arguments: => {PropertyExpression} = {Value}
    internal sealed class SetPropertyCommand : NonAwaitableCommand
    {
        #region Constructor

        // Constructor
        internal SetPropertyCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 3)
        {
            if (Parser.ParsePropertyExpression(this, body.Clauses[0], true) is PropertyExpression propertyExpression)
            {
                var property = propertyExpression.Property;
                if (!property.PropertyInfo.CanWrite)
                {
                    throw new ScriptException(this, $"'{property.Name}' is read-only.");
                }

                AssertKeyword(1, ScriptSyntax.AssignmentOp, ScriptSyntax.AdditionAssignmentOp, ScriptSyntax.SubtractionAssignmentOp);

                // Fake set, just to check if property value is valid
                property.SetFakeValue(this, body.Clauses[2]);
            }
        }

        #endregion

        // OnExecute
        protected override void OnExecute()
        {
            var expression = Parser.ParsePropertyExpression(this, Body.Clauses[0], false);

            if (expression.Instance == null)
            {
                throw new ScriptException("Instance is null.");
            }

            var op = Body.Clauses[1];
            var value = Body.Clauses[2];
            var targetProperty = expression.Property;
            var targetInstance = expression.Instance;

            if (op != ScriptSyntax.AssignmentOp)
            {
                if (targetProperty.PropertyType == typeof(int))
                {
                    if (targetProperty.GetValue(targetInstance) is int intValue)
                    {
                        if (op == ScriptSyntax.SubtractionAssignmentOp)
                        {
                            intValue -= Parser.ParseInt32(this, 2);
                        }
                        else
                        {
                            intValue += Parser.ParseInt32(this, 2);
                        }

                        value = intValue.ToString(CultureInfo.InvariantCulture);
                    }
                }

                else if (targetProperty.PropertyType == typeof(float))
                {
                    if (targetProperty.GetValue(targetInstance) is float floatValue)
                    {
                        if (op == ScriptSyntax.SubtractionAssignmentOp)
                        {
                            floatValue -= Parser.ParseFloat(this, 2);
                        }
                        else
                        {
                            floatValue += Parser.ParseFloat(this, 2);
                        }

                        value = floatValue.ToString(CultureInfo.InvariantCulture);
                    }
                }
            }

            targetProperty.SetValue(this, targetInstance, value);
        }
    }
}
