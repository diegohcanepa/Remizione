using System;

namespace Adberration.Scripting
{
    /// <summary>
    /// ScriptMethodAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ScriptMethodAttribute : Attribute
    {
        // Constructor
        public ScriptMethodAttribute(CodingContext context = CodingContext.Any)
        {
            this.Context = context;
        }

        // Context
        public CodingContext Context { get; }
    }
}
