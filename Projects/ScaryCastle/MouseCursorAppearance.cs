using Engendro;
using ScaryCastle.Scripting;

namespace ScaryCastle
{
    /// <summary>
    /// MouseCursorAppearance
    /// </summary>
    internal static class MouseCursorAppearance
    {
        private static GameThing? lastKnownTarget;

        #region Private members

        // RefreshIcon
        private static void RefreshIcon(InteractionContext context)
        {
            if (context.Target == context.Session.Player && context.HeldItem == null)
                return;

            // Modal speech text active
            if (SpeechText.ModalInstance != null)
            {
                MouseCursor.Icon = MouseCursorIcon.Talk;
                return;
            }

            /*
            if (context.Session.Player != null && !context.Session.Player.CanHandleInput)
            {
                MouseCursor.Icon = MouseCursorIcon.Wait;
                return;
            }
            */

            // Session is awaiting script
            if (context.Session.IsAwaiting)
            {
                if (context.Session.AwaitingScript?.CurrentStatement is SayCommand)
                {
                    MouseCursor.Icon = MouseCursorIcon.Talk;
                    return;
                }

                if (EngendroGame.Instance.SceneManager.CurrentScene is DialogBlockScene)
                {
                    MouseCursor.Icon = MouseCursorIcon.Cross;
                    return;
                }

                if (context.Session.ActiveNPC == null)
                {
                    if (context.Session.IsCurrentScene)
                        MouseCursor.Icon = MouseCursorIcon.Wait;
                }
                else if (context.Session.ActiveNPC.CombatDecisionType is CombatDecisionType.Attack or CombatDecisionType.Charge or CombatDecisionType.Curse)
                {
                    MouseCursor.Icon = MouseCursorIcon.Skull;
                }

                return;
            }

            if (context.HeldItem != null && (context.Target == null || !context.Target.IsGoToVerb))
            {
                MouseCursor.CustomImage = context.HeldItem.Definition.Image;
                MouseCursor.HightlightColor = context.Target == null ? null : ColorPalette.MouseCursor.Highlight;
            }
            else
            {
                if (context.Target != null)
                {
                    SyncIcon(context.Target.Verb);
                }
                else
                {
                    MouseCursor.Icon = MouseCursorIcon.Cross;
                }
            }
        }

        // SyncIcon
        private static void SyncIcon(Verb verb)
        {
            switch (verb)
            {
                // Attack
                case Verb.Attack:
                    MouseCursor.Icon = MouseCursorIcon.Attack;
                    break;

                // Examine
                case Verb.Examine:
                    MouseCursor.Icon = MouseCursorIcon.Eye;
                    break;

                // GoDown
                case Verb.GoDown:
                    MouseCursor.Icon = MouseCursorIcon.Down;
                    break;

                // GoLeft
                case Verb.GoLeft:
                    MouseCursor.Icon = MouseCursorIcon.Left;
                    break;

                // GoRight
                case Verb.GoRight:
                    MouseCursor.Icon = MouseCursorIcon.Right;
                    break;

                // GoUp
                case Verb.GoUp:
                    MouseCursor.Icon = MouseCursorIcon.Up;
                    break;

                // Lift
                case Verb.Lift:
                    MouseCursor.Icon = MouseCursorIcon.Lift;
                    break;

                // Pickup
                case Verb.PickUp:
                    MouseCursor.Icon = MouseCursorIcon.PickUp;
                    break;

                // Talk
                case Verb.Talk:
                    MouseCursor.Icon = MouseCursorIcon.Talk;
                    break;

                // Use
                case Verb.Use:
                    MouseCursor.Icon = MouseCursorIcon.Hand;
                    break;

                default:
                    break;
            }
        }

        // SyncText
        private static void SyncText(GameThing target)
        {
            const string surpriseLabel = "[?]";

            MouseCursor.Tooltip = target.DisplayName;

            MouseCursor.SubTextColor = ColorPalette.MouseCursor.SubText;

            if (target.ItemReward != null)
            {
                MouseCursor.SubText = target.ItemReward.DisplayName;
            }
            else if (target.CoinReward > 0)
            {
                MouseCursor.SubText = GameData.Items.Get(ItemNames.Coin).DisplayName;
            }
            else if (target.Definition?.DropTrigger == LootDropTrigger.OnImpact)
            {
                MouseCursor.SubText = surpriseLabel;
            }
            else
            {
                MouseCursor.SubText = null;
            }
        }

        #endregion

        // Refresh
        internal static void Refresh(InteractionContext context)
        {
            RefreshIcon(context);

            if (context.Target != null && context.Target != lastKnownTarget)
            {
                SyncText(context.Target);
                lastKnownTarget = context.Target;
            }
        }
    }
}