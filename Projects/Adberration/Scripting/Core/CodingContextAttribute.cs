using System;

namespace Adberration.Scripting
{
    /// <summary>
    /// CodingContextAttribute
    /// </summary>
    public abstract class CodingContextAttribute : Attribute
    {
        // Constructor
        protected CodingContextAttribute(CodingContext context = CodingContext.Any)
        {
            this.Context = context;
        }

        // Context
        public CodingContext Context { get; }
    }
}
