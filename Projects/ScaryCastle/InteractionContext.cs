using Adberration.Scripting;
using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using ScaryCastle.Scripting;

namespace ScaryCastle
{
    /// <summary>
    /// InteractionContext
    /// </summary>
    public sealed class InteractionContext
    {
        private readonly GameSession session;
        private readonly string useVerb = Localization.GetValue(Verb.Use);
        private readonly string withPreposition = TextRepository.GetValue("Misc.WithPreposition");

        // Constructor
        public InteractionContext(GameSession session)
        {
            this.session = session;
        }

        #region Private members

        // InvalidateText
        private void InvalidateText()
        {
            // No target
            if (Target == null)
            {
                Text = null;
                return;
            }

            // Get sentence
            var sentence = Target.GetInteractPrompt() ?? Target.LocalizedDisplayName;

            // Compose text
            if (HeldItem == null)
            {
                Text = sentence;
            }
            else
            {
                Text = $"{useVerb} {HeldItem.Definition.LocalizedDisplayName} {withPreposition} {sentence}";
            }
        }

        // ScanForTarget
        private GameThing? ScanForTarget()
        {
            if (session.Room == null)
                return null;

            var mousePos = InputManager.DefaultPlayer.Mouse.WorldPosition(session.Camera);

            for (int i = session.Room.CulledThings.Count - 1; i >= 0; i--)
            {
                // Player exclusion when holding no item
                if (session.Room.CulledThings[i] == session.Player && HeldItem == null)
                    continue;

                if (session.Room.CulledThings[i] is GameThing target && target.CanInteract() && target.RuntimeHotspot.Contains(mousePos))
                    return target;
            }

            return null;
        }

        #endregion

        // HeldItem
        public Item? HeldItem { get; set; }

        // Target
        public GameThing? Target
        {
            get;
            private set
            {
                if (value != field)
                {
                    field = value;

                    UseWithScript = null;

                    if (field != null && HeldItem != null)
                        UseWithScript = field.Session.ScriptLibrary.FindOverload(field.DeclaredName, HeldItem.Name);

                    InvalidateText();
                }
            }
        }

        // Text
        public string? Text { get; private set; }

        // Update
        public void Update(GameTime gameTime)
        {
            if (HeldItem?.Count <= 0)
                HeldItem = null;

            // No room, no session. 
            if (session.Room == null)
            {
                MouseCursor.State = MouseCursorState.Arrow;
                HeldItem = null;
                Target = null;
                UseWithScript = null;
                return;
            }

            // Session is awaiting
            if (session.IsAwaiting)
            {
                if (session.Player?.HasSpeechBubble == true)
                {
                    MouseCursor.State = MouseCursorState.Arrow;
                }
                else if (session.AwaitingScript?.CurrentStatement is AwaitInputCommand)
                {
                    MouseCursor.State = MouseCursorState.Hand;
                }
                else
                {
                    MouseCursor.State = MouseCursorState.Wait;
                }

                return;
            }

            if (session.HUD.Inventory.IsVisible)
            {
                MouseCursor.State = MouseCursorState.Hand;
                return;
            }

            if (SpeechBubble.ModalInstance == null)
                Target = ScanForTarget();

            if (HeldItem != null)
            {
                MouseCursor.State = MouseCursorState.Item;
                MouseCursor.CustomImage = HeldItem.Definition.Image;
                if (Target == null)
                    MouseCursor.OutlineColor = MouseCursorOutline.None;
                else
                    MouseCursor.OutlineColor = UseWithScript == null ? MouseCursorOutline.Red : MouseCursorOutline.Green;
            }
            else
            {
                MouseCursor.CustomImage = null;
                MouseCursor.State = Target?.GetMouseCursorState() ?? MouseCursorState.Cross;
                MouseCursor.OutlineColor = MouseCursorOutline.None;
            }

            MouseCursor.Text = Text;
        }

        // UseWithScript
        public Script? UseWithScript { get; private set; }
    }
}
