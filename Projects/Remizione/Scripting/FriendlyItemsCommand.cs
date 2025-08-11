using Adberration;
using Adberration.Scripting;
using System.Collections.Generic;

namespace Remizione.Scripting
{
    // FriendlyItemsCommand
    // Arguments: {MetaItem[,MetaItem]}
    internal sealed class FriendlyItemsCommand : NonAwaitableCommand
    {
        // Constructor
        internal FriendlyItemsCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            var thing = AssertEntityNotNull<GameThing>(Script.EntityName);
            if (thing.InstanceKind != InstanceKind.Static)
                return;

            var names = Parser.ParseNames(this, 0);
            var metaItems = new List<MetaItem>();
            
            for (var i = 0; i < names.Length; i++)
            {
                if (MetaItem.Find(names[i]) is MetaItem metaItem)
                    metaItems.Add(metaItem);
                else
                    throw new ScriptException(this, $"Meta item '{names[i]}' does not exist.");
            }

            thing.Session.RegisterFriendlyItems(thing.StaticName, metaItems.ToArray());
        }
    }
}
