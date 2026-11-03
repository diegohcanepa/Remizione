using Adberration.Scripting;

namespace Remizione.Scripting
{
    // CraftItemCommand
    internal sealed class CraftItemCommand : NonAwaitableCommand
    {
        // Constructor
        internal CraftItemCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession session || session.Player == null)
                return;

            if (session.TryCraft() is ItemDefinition itemDefinition)
            {
                if (session.PlayerData.Inventory.Find(ItemNames.Material) is Item item)
                {
                    item.Remove();
                    var craftedItem = session.PlayerData.Inventory.Add(itemDefinition);
                    session.Player.ShowFlyOff(Localization.GetValue(MessageKind.ItemCrafted), Colors.Palette.Green);
                    session.HUD.BottomMessage.ShowItem(craftedItem, Colors.Palette.Green);
                }
            }
        }
    }
}
