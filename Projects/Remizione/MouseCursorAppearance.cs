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

        // DetermineIcon
        private MouseCursorIcon? DetermineIcon()
        {
            var player = context.Session.Player;
            var target = context.Target;
            var heldItem = context.HeldItem;

            if (target != null && !target.CanInteract())
                return null;

            if (player?.HeldProp != null && target != null)
                return target.IsPlayer ? MouseCursorIcon.Cross : MouseCursorIcon.Target;

            // Modal speech text active
            if (SpeechText.ModalInstance != null)
                return MouseCursorIcon.Talk;

            // Session is awaiting script
            if (context.Session.IsAwaiting)
            {
                if (context.Session.AwaitingScript?.CurrentStatement is SayCommand)
                    return MouseCursorIcon.Talk;

                if (EngendroGame.Instance.SceneManager.CurrentScene is DialogBlockScene)
                    return MouseCursorIcon.Cross;

                if (context.Session.IsCurrentScene)
                    return MouseCursorIcon.Wait;

                return null;
            }

            if (heldItem != null && (target == null || !Utils.IsGoToVerb(target.Verb)))
                return null;

            if (target != null)
            {
                if (target.Verb == Verb.PickUp && target.ItemReward != null)
                    return MouseCursorIcon.Sack;

                return MapVerbToIcon(target.Verb);
            }

            return MouseCursorIcon.Cross;
        }

        // RefreshIcon
        private void RefreshIcon()
        {
            var player = context.Session.Player;
            var target = context.Target;
            var heldItem = context.HeldItem;

            // 1. Estados críticos: Congelan cualquier actualización visual inmediatamente
            if (player?.IsDead == true || player?.IsFatigued == true || player?.IsLockedByAttacker == true)
            {
                MouseCursor.Icon = MouseCursorIcon.Wait;
                return;
            }

            // 2. Custom Image
            var desiredCustomImage = heldItem?.Definition.Image;
            if (MouseCursor.CustomImage != desiredCustomImage)
            {
                MouseCursor.CustomImage = desiredCustomImage;
            }

            // 3. Highlight Color: Limpia la memoria si no aplica
            if (heldItem != null && (target == null || !Utils.IsGoToVerb(target.Verb)))
            {
                MouseCursor.HightlightColor = target == null ? null : ColorPalette.MouseCursor.Highlight;
            }
            else
            {
                MouseCursor.HightlightColor = null;
            }

            // 4. Determinar y aplicar el Icono
            var targetIcon = DetermineIcon();
            if (targetIcon.HasValue)
            {
                MouseCursor.Icon = targetIcon.Value;
            }
        }

        // MapVerbToIcon (Reemplaza a SyncIcon)
        private static MouseCursorIcon MapVerbToIcon(Verb verb)
        {
            return verb switch
            {
                Verb.Attack => MouseCursorIcon.Target,
                Verb.LookAt => MouseCursorIcon.Eye,
                Verb.GoDown => MouseCursorIcon.Down,
                Verb.GoLeft => MouseCursorIcon.Left,
                Verb.GoRight => MouseCursorIcon.Right,
                Verb.GoUp => MouseCursorIcon.Up,
                Verb.Lift => MouseCursorIcon.Lift,
                Verb.PickUp => MouseCursorIcon.Sack,
                Verb.Inspect => MouseCursorIcon.Inspect,
                Verb.Talk => MouseCursorIcon.Talk,
                Verb.Use => MouseCursorIcon.Hand,
                Verb.None => MouseCursorIcon.Cross,
                _ => MouseCursorIcon.Cross
            };
        }

        // SyncText
        private void SyncText()
        {
            var target = context.Target;
            var heldItem = context.HeldItem;

            MouseCursor.TooltipColor = ColorPalette.MouseCursor.Tooltip;
            MouseCursor.SubTextColor = ColorPalette.MouseCursor.SubText;

            if (target != null && heldItem == null)
            {
                MouseCursor.Tooltip = target.Label;
                MouseCursor.SubText = null;

                if (target.ItemRewardAmount > 1)
                    MouseCursor.Tooltip += $" (x{target.ItemRewardAmount})";
            }
            else if (heldItem != null)
            {
                MouseCursor.Tooltip = heldItem.Definition.Label;
                MouseCursor.SubText = target?.Label;
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

            var target = context.Target;
            var heldItem = context.HeldItem;
            var verb = target?.Verb;

            if (target != lastKnownTarget || verb != lastKnownVerb || heldItem != lastKnownHeldItem)
            {
                SyncText();
                lastKnownTarget = target;
                lastKnownHeldItem = heldItem;
                lastKnownVerb = verb;
            }
        }
    }
}