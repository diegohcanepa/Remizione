using System;

namespace EngendroAdventure.Scripting
{
    /// <summary>
    /// ForceAwaitAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ForceAwaitAttribute : Attribute
    {
    }
}
