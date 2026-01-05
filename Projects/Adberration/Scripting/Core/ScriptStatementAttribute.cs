using System;

namespace Adberration.Scripting
{
    /// <summary>
    /// ScriptStatementAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ScriptStatementAttribute : CodingContextAttribute
    {
        // Constructor
        public ScriptStatementAttribute(CodingContext context = CodingContext.Any)
            : base(context)
        {
        }
    }
}
