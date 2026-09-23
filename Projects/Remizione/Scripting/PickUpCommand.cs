using Adberration.Scripting;
using Microsoft.Xna.Framework;
using System;

namespace Remizione.Scripting
{
    // PickUp
    internal sealed class PickUpCommand : NonAwaitableCommand
    {
        // Constructor
        internal PickUpCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            AssertEntity<GameThing>(0);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession session)
                return;

            if (AssertEntity<GameThing>(0) is not GameThing target)
                return;

            if (target.ItemReward == null || target.ItemRewardAmount <= 0)
                return;

            // 1. Calculamos el espacio disponible en el inventario para este ítem
            var availableSpace = GameSettings.MaxItemAmount;
            if (session.PlayerData.Inventory.Find(target.ItemReward.Name) is Item carriedItem)
            {
                availableSpace = Math.Max(0, GameSettings.MaxItemAmount - carriedItem.Amount);
            }

            // 2. Tomamos el mínimo entre lo disponible en el target y el espacio en inventario
            var collectAmount = Math.Min(target.ItemRewardAmount, availableSpace);

            // Si el inventario está lleno para este ítem, no hacemos nada
            if (collectAmount <= 0)
                return;

            // 3. Reproducimos sonido solo si realmente vamos a juntar algo
            target.ItemReward.PickupSound?.Play();

            // 4. Descontamos la cantidad recolectada del target
            target.ItemRewardAmount -= collectAmount;

            // 5. Agregamos al inventario y disparamos la animación del HUD
            if (session.PlayerData.Inventory.Add(target.ItemReward, collectAmount) is Item item)
            {
                var startPos = ((target.Position - Session.Camera.Position) * Session.Camera.Zoom) + (new Vector2(240, 135) * 0.5f);
                session.HUD.InventoryMeter.AnimateAddItem(item, startPos);
            }

            if (session.Player != null)
                EffectDescriptor.Apply(target.ItemReward.EffectDescriptors, session.Player, null, EffectContext.Collect);

            // 6. Si se vació por completo el target, limpiamos la recompensa y lo removemos
            if (target.ItemRewardAmount == 0)
            {
                target.ItemReward = null;
                target.Unparent();
            }
        }
    }
}