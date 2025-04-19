using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EngendroAdventure.Scripting
{
    /// <summary>
    /// StatementArgCollection
    /// </summary>
    public sealed partial class StatementArgCollection : ReadOnlyCollection<StatementArg>
    {
        // Constructor
        internal StatementArgCollection(IList<StatementArg> list)
            : base(list)
        {
        }

        // Contains
        public bool Contains(string name)
        {
            return GetArg(name) != null;
        }

        // GetArg
        public StatementArg? GetArg(string name)
        {
            for (var i = 0; i < Count; i++)
            {
                if (this[i].Name == name)
                {
                    return this[i];
                }
            }

            return null;
        }

        // Index
        public StatementArg this[string name] => GetArg(name) ?? throw new ArgumentException("Name not found in the collection.", nameof(name));
    }
}
