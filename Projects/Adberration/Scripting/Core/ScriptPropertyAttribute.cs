using System;

namespace Adberration.Scripting
{
    /// <summary>
    /// ScriptPropertyAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ScriptPropertyAttribute : CodingContextAttribute
    {
        // Constructor
        public ScriptPropertyAttribute(CodingContext context = CodingContext.Any)
            : base(context)
        {
        }
    }
}
