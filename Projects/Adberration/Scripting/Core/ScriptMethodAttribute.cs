using System;

namespace Adberration.Scripting
{
    /// <summary>
    /// ScriptMethodAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ScriptMethodAttribute : CodingContextAttribute
    {
        // Constructor
        public ScriptMethodAttribute(CodingContext context = CodingContext.Any)
            : base(context)
        {
        }
    }
}
