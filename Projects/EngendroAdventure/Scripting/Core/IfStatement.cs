using System;
using System.Globalization;

namespace EngendroAdventure.Scripting
{
    // IfStatement
    // Syntax: {PropertyExpression}] {== | != | > | < | <= | >=} {bool|number|enum|PropertyExpression}
    internal sealed class IfCoreStatement : ConditionalStatement
    {
        // Constructor
        internal IfCoreStatement(Script script, string source, StatementBody body)
            : base(script, StatementType.If, source, body)
        {
            // Left op (property expression)
            var propertyExpression = Parser.ParsePropertyExpression(this, body.Clauses[0], true);
            var leftProperty = propertyExpression.Property;

            // Comparison operator
            Parser.ParseComparisonOperator(this, body.Clauses[1]);

            // Right op
            var rightOp = body.Clauses[2];

            // Numeric (int or float)
            if (leftProperty.IsNumeric)
            {
                if (leftProperty.PropertyType == typeof(int))
                {
                    Parser.ParseInt32(this, rightOp);
                }
                else
                {
                    Parser.ParseFloat(this, rightOp);
                }
            }

            // Boolean
            else if (leftProperty.IsBoolean)
            {
                Parser.ParseBoolean(this, rightOp);
            }

            // Enum
            else if (leftProperty.IsEnum)
            {
                var enumType = leftProperty.PropertyType;
                if (!Enum.IsDefined(enumType, rightOp))
                {
                    throw new ScriptException(this, $"'{rightOp}' is not a valid enumeration member in '{enumType.FullName}'");
                }
            }

            else if (!leftProperty.IsString)
            {
                throw new ScriptException(this, $"'{rightOp}' is not a valid value.");
            }
        }

        #region Private members

        // EvaluateBooleanOperands
        private static bool EvaluateBooleanOperands(ComparisonOperator op, bool leftOperand, bool rightOperand)
        {
            if (op == ComparisonOperator.Equality)
            {
                return leftOperand == rightOperand;
            }
            else
            {
                return leftOperand != rightOperand;
            }
        }

        // EvaluateEnumOperands
        private static bool EvaluateEnumOperands(ComparisonOperator op, string leftOperand, string rightOperand)
        {
            if (op == ComparisonOperator.Equality)
            {
                return leftOperand == rightOperand;
            }
            else
            {
                return leftOperand != rightOperand;
            }
        }

        // EvaluateCore
        private bool EvaluateCore(ScriptProperty property, object instance, ComparisonOperator op)
        {
            // Numeric
            if (property.IsNumeric)
            {
                var leftOp = Convert.ToSingle(property.GetValue(instance), CultureInfo.InvariantCulture);
                var rightOp = Parser.ParseFloat(this, Body.Clauses[2]);
                return EvaluateNumericOperands(op, leftOp, rightOp);
            }

            // Boolean
            else if (property.IsBoolean)
            {
                if (property.GetValue(instance) is bool leftOp)
                {
                    var rightOp = Parser.ParseBoolean(this, Body.Clauses[2]);
                    return EvaluateBooleanOperands(op, leftOp, rightOp);
                }
            }

            // Enumeration
            else if (property.IsEnum)
            {
                if (property.GetValue(instance)?.ToString() is string leftOp)
                {
                    var rightOp = Body.Clauses[2];
                    return EvaluateEnumOperands(op, leftOp, rightOp);
                }
            }

            // Quoted String
            else if (property.IsString)
            {
                if (property.GetValue(instance) is string leftOp)
                {
                    var rightOp = Parser.ParseQuotedString(this, Body.Clauses[2]);
                    return EvaluateStringOperands(op, leftOp, rightOp);
                }
            }

            throw new ScriptException(this, "Unexpected error found while evaluating operator.");
        }

        // EvaluateNumericOperands
        private static bool EvaluateNumericOperands(ComparisonOperator op, float leftOperand, float rightOperand)
        {
            return op switch
            {
                // Inequality
                ComparisonOperator.Inequality => leftOperand != rightOperand,

                // Greater
                ComparisonOperator.GreaterThan => leftOperand > rightOperand,

                // GreaterThan
                ComparisonOperator.GreaterThanOrEqual => leftOperand >= rightOperand,

                // Less
                ComparisonOperator.LessThan => leftOperand < rightOperand,

                // LessThan
                ComparisonOperator.LessThanOrEqual => leftOperand <= rightOperand,

                // Equality
                _ => leftOperand == rightOperand,
            };
        }

        // EvaluateStringOperands
        private static bool EvaluateStringOperands(ComparisonOperator op, string leftOperand, string rightOperand)
        {
            if (op == ComparisonOperator.Equality)
            {
                return leftOperand == rightOperand;
            }
            else
            {
                return leftOperand != rightOperand;
            }
        }

        #endregion

        // Evaluate
        public override bool Evaluate()
        {
            // Left operand
            var leftExpression = Parser.ParsePropertyExpression(this, Body.Clauses[0], false);
            if (leftExpression.Instance == null)
            {
                return false;
            }

            // Operator
            var op = Parser.ParseComparisonOperator(this, Body.Clauses[1]);
            return EvaluateCore(leftExpression.Property, leftExpression.Instance, op);
        }
    }
}
