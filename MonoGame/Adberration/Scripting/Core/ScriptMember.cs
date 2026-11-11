using Engendro;

namespace Adberration.Scripting
{
    /// <summary>
    /// ScriptMember
    /// </summary>
    public abstract class ScriptMember : INamedObject
    {
        // Constructor
        protected ScriptMember(Session session, string name, CodingContext context)
        {
            this.Session = session;
            this.Name = name;
            this.Context = context;
        }

        // Context
        public CodingContext Context { get; }

        // Name
        public string Name { get; }

        // Session
        public Session Session { get; }

        // ToString
        public override string ToString()
        {
            return Name;
        }
    }
}
