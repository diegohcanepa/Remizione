using Engendro;
using Engendro.Input;
using ScaryCastle.Scripting;

namespace ScaryCastle
{
    /// <summary>
    /// MouseCursorAppearance
    /// </summary>
    internal static class MouseCursorAppearance
    {
        #region Private members

        // RefreshCursor
        private static void RefreshCursor(InteractionContext context)
        {
            if (context.HeldItem?.Definition.ActionKind == ActionKind.Projectile)
            {
                if (context.Session.Player != null)
                {
                    var mousePos = InputManager.DefaultPlayer.Mouse.WorldPosition(context.Session.Camera);
                    MouseCursor.FlipCustomImage = mousePos.X < context.Session.Player.X;
                }
            }

            // Inventory
            if (context.Session.Game.SceneManager.CurrentScene is InventoryScene)
            {
                MouseCursor.State = MouseCursorState.Hand;
                return;
            }

            // DialogOption
            if (context.Session.Game.SceneManager.CurrentScene is DialogBlockScene)
            {
                MouseCursor.State = MouseCursorState.Arrow;
                return;
            }

            // Modal speech text active
            if (SpeechText.ModalInstance != null)
            {
                MouseCursor.State = MouseCursorState.Arrow;
                return;
            }

            // Session is awaiting script
            if (context.Session.IsAwaiting)
            {
                if (context.Session.AwaitingScript?.CurrentStatement is AwaitInputCommand)
                    MouseCursor.State = MouseCursorState.Hand;
                else if (context.Session.AwaitingScript?.CurrentStatement is SayCommand)
                    MouseCursor.State = MouseCursorState.Arrow;
                else
                    MouseCursor.State = context.Session.ActiveNPC == null ? MouseCursorState.Wait : MouseCursorState.Skull;

                return;
            }

            if (context.HeldItem != null && (context.Target == null || context.Target.Cursor == MouseCursorState.Cross))
            {
                MouseCursor.CustomImage = context.HeldItem.Definition.Image;
            }
            else
            {
                MouseCursor.State = context.Target != null ? context.Target.Cursor : MouseCursorState.Cross;
            }
        }

        // RefreshText
        private static void RefreshText(InteractionContext context)
        {
            if (MouseCursor.IsArrow)
                return;

            if (context.Target != null)
            {
                if (context.Target.Faction == Faction.Evil && context.Target is Actor)
                {
                    if (context.Target.ItemReward != null)
                    {
                        MouseCursor.SubText = context.Target.ItemReward.DisplayName;
                    }
                    else if (context.Target.CoinReward > 0)
                    {
                        MouseCursor.SubText = context.CoinDisplayName;
                    }
                    else if ((context.Target as IThingDefinition)?.Definition?.DropTrigger == LootDropTrigger.OnImpact)
                    {
                        MouseCursor.SubText = "[?]";
                    }
                }

                // No item 
                MouseCursor.Text = context.Target.DisplaySentence;
            }
        }

        #endregion

        // Refresh
        internal static void Refresh(InteractionContext context)
        {
            MouseCursor.Reset();

            RefreshCursor(context);
            RefreshText(context);

            if (context.Target != null)
            {
                if (context.Session.Player != null && context.HeldItem?.Definition.ActionKind == ActionKind.Projectile)
                {
                    var mousePos = InputManager.DefaultPlayer.Mouse.WorldPosition(context.Session.Camera);
                    MouseCursor.FlipCustomImage = mousePos.X < context.Session.Player.X;
                }
            }
        }
    }
}
