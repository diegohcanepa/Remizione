using Engendro;
using Remizione.Scripting;

namespace Remizione
{
    /// <summary>
    /// MouseCursorAppearance
    /// </summary>
    internal sealed class MouseCursorAppearance(InteractionContext context)
    {
        private Item? lastKnownHeldItem;
        private GameThing? lastKnownTarget;
        private Verb? lastKnownVerb;

        #region Private members

        // RefreshIcon
        private void RefreshIcon()
        {
            if (context.HeldItem?.Definition.Image != MouseCursor.CustomImage)
                MouseCursor.CustomImage = context.HeldItem?.Definition.Image;

            if (context.Target != null && !context.Target.CanInteract())
                return;

            if (context.Session.Player?.HeldProp != null && context.Target != null)
            {
                SyncIcon(context.Target.IsPlayer ? Verb.None : Verb.Attack);
                return;
            }

            // Modal speech text active
            if (SpeechText.ModalInstance != null)
            {
                MouseCursor.Icon = MouseCursorIcon.Talk;
                return;
            }

            if (context.Session.Player?.IsLockedByAttacker == true)
            {
                MouseCursor.Icon = MouseCursorIcon.Cross;
                return;
            }

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

                if (context.Session.IsCurrentScene)
                    MouseCursor.Icon = MouseCursorIcon.Wait;

                return;
            }

            if (context.HeldItem != null && (context.Target == null || !Utils.IsGoToVerb(context.Target.Verb)))
            {
                MouseCursor.CustomImage = context.HeldItem.Definition.Image;
                MouseCursor.HightlightColor = context.Target == null ? null : ColorPalette.MouseCursor.Highlight;
            }
            else
            {
                if (context.Target != null)
                {
                    if (context.Target.Verb == Verb.PickUp && context.Target.ItemReward != null)
                        MouseCursor.Icon = MouseCursorIcon.Sack;
                    else
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
                    MouseCursor.Icon = MouseCursorIcon.Target;
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
                    MouseCursor.Icon = MouseCursorIcon.Sack;
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
        private void SyncText()
        {
            if (context.Target != null && context.HeldItem == null)
            {
                MouseCursor.Tooltip = context.Target.Label;
                MouseCursor.SubText = null;

                if (context.Target.ItemRewardAmount > 1)
                    MouseCursor.Tooltip += $" (x{context.Target.ItemRewardAmount})";
            }
            else if (context.HeldItem != null)
            {
                MouseCursor.Tooltip = context.HeldItem.Definition.Label;
                MouseCursor.SubText = context.Target?.Label;
            }
            else
            {
                MouseCursor.Tooltip = null;
                MouseCursor.SubText = null;
            }
        }

        #endregion

        // Refresh
        internal void Refresh()
        {
            RefreshIcon();

            if (context.Target != lastKnownTarget || context.Target?.Verb != lastKnownVerb || context.HeldItem != lastKnownHeldItem)
            {
                SyncText();
                lastKnownTarget = context.Target;
                lastKnownHeldItem = context.HeldItem;
                lastKnownVerb = context.Target?.Verb;
            }
        }
    }
}