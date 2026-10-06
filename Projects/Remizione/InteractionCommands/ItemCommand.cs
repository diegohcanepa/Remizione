namespace Remizione.InteractionCommands
{
    /// <summary>
    /// ItemCommand
    /// </summary>
    public sealed class ItemCommand : InteractionCommand
    {
        // CanExecute
        public override bool CanExecute(InteractionData data, Actor player, GameThing target, Verb verb)
        {
            return player.Session.InteractionContext.HeldItem is IAction action && action.ActionKind != ActionKind.ScriptOutcome;
        }

        // Execute
        public override void Execute(InteractionData data, Actor player, GameThing target, Verb verb)
        {
            if (player.Session.InteractionContext.HeldItem is Item heldItem)
            {
                if (heldItem.Definition.ActionKind == ActionKind.Projectile)
                {
                    if (player.Session.Room?.WalkArea is not WalkArea walkArea)
                        return;

                    if (!walkArea.InLineOfSight(player.Position, target.Position, RaycastContext.LineOfSight, out _))
                    {
                        player.Session.HUD.Message.Show(MessageKind.OutOfSight);
                        MouseCursor.Shake();
                        return;
                    }
                }

                player.ExecuteAction(heldItem, target);
            }
        }
    }
}
