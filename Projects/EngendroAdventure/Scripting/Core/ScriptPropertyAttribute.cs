using System;

namespace EngendroAdventure.Scripting
{
    /// <summary>
    /// ScriptPropertyAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ScriptPropertyAttribute : Attribute
    {
        // Constructor
        public ScriptPropertyAttribute(CodingContext context = CodingContext.Any)
        {
            this.Context = context;
        }

        // Context
        public CodingContext Context { get; }
    }
}
