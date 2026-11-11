namespace Adberration.Scripting
{
    /// <summary>
    /// StatementArg
    /// </summary>
    public sealed class StatementArg
    {
        // Constructor
        internal StatementArg(ScriptEnvironment script, string value)
        {
            var pair = value.Split(':');

            this.Name = pair[0];

            if (pair.Length > 1)
            {
                if (ScriptEnvironment.IsConstant(pair[1]))
                {
                    if (script.IsConstantDeclared(pair[1]))
                    {
                        pair[1] = script.GetConstantValue(pair[1]);
                    }
                }

                this.Value = pair[1];
            }
        }

        // HasValue
        public bool HasValue => !string.IsNullOrWhiteSpace(Value);

        // Name
        public string Name { get; }

        // Value
        public string? Value { get; }
    }
}
