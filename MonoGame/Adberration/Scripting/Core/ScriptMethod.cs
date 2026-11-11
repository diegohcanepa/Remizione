using System.Reflection;

namespace Adberration.Scripting
{
    /// <summary>
    /// ScriptMethod
    /// </summary>
    public sealed class ScriptMethod : ScriptMember
    {
        private readonly MethodInfo methodInfo;

        // Constructor
        internal ScriptMethod(Session session, string name, MethodInfo methodInfo, CodingContext context)
            : base(session, name, context)
        {
            this.methodInfo = methodInfo;
        }

        // Invoke
        public void Invoke(object instance)
        {
            methodInfo.Invoke(instance, null);
        }
    }
}
