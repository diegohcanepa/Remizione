using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace EngendroAdventure.Scripting
{
    /// <summary>
    /// ScriptStatement
    /// </summary>
    public sealed class ScriptStatement : ScriptMember
    {
        private readonly ConstructorInfo constructorInfo;

        // Constructor
        internal ScriptStatement(Session session, string name, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type statementType, CodingContext context)
            : base(session, name, context)
        {
            this.StatementType = statementType;

            if (!typeof(Statement).IsAssignableFrom(statementType))
            {
                throw new ArgumentException("The supplied type is not a statement.", nameof(statementType));
            }

            this.constructorInfo = statementType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)[0];
        }

        // CreateInstance
        public Statement? CreateInstance(Script script, string source, StatementBody body)
        {
            return constructorInfo.Invoke(new object[] { script, source, body }) as Statement;
        }

        // StatementType
        public Type StatementType { get; }
    }
}
