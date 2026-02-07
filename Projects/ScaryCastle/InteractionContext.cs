using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Engendro.Input;
using ScaryCastle.Scripting;

namespace ScaryCastle
{
    /// <summary>
    /// InteractionContext
    /// </summary>
    public sealed class InteractionContext(GameSession session)
    {
        private readonly GameSession session = session;
        private readonly string headbuttVerb = Localization.GetValue(Verb.Headbutt);
        private readonly string useVerb = Localization.GetValue(Verb.Use);
        private readonly string withPreposition = TextRepository.GetValue("Misc.WithPreposition");

        #region Private members

        // InvalidateText
        private void InvalidateText()
        {
            // No target
            if (Target == null)
            {
                MouseCursor.Text = null;
                return;
            }

            // Get sentence
            var sentence = Target.LocalizedDisplayName;

            // Compose text
            if (HeldItem == null)
            {
                MouseCursor.Text = session.HeadbuttMode ? $"{headbuttVerb} {sentence}" : sentence;
            }
            else
            {
                MouseCursor.Text = $"{useVerb} {HeldItem.Definition.LocalizedDisplayName} {withPreposition} {sentence}";
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
        public Item? HeldItem
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    if (field == null)
                        MouseCursor.CustomImage = null;
                    session.HeadbuttMode = false;
                }
            }
        }

        // Reset
        public void Reset()
        {
            HeldItem = null;
            Target = null;
            UseWithScript = null;
        }

        // Target
        public GameThing? Target
        {
            get;
            private set
            {
                if (value != field)
                {
                    (field as ProceduralActor)?.HideHealthMeter();

                    field = value;

                    UseWithScript = null;

                    if (field != null && HeldItem != null)
                        UseWithScript = field.Session.ScriptLibrary.FindOverload(field.DeclaredName, HeldItem.Name);

                    InvalidateText();

                    (field as ProceduralActor)?.ShowHealthMeter();
                }
            }
        }

        // Update
        public void Update()
        {
            if (HeldItem?.Count <= 0)
                HeldItem = null;

            // No room, no session. 
            if (session.Room == null)
            {
                Reset();
                return;
            }

            // Modal speech bubble active
            if (SpeechBubble.ModalInstance != null)
            {
                MouseCursor.State = MouseCursorState.Arrow;
                MouseCursor.CustomImage = null;
                return;
            }

            // Session is awaiting script
            if (session.IsAwaiting)
            {
                MouseCursor.State = session.AwaitingScript?.CurrentStatement is AwaitInputCommand ? MouseCursorState.Hand : MouseCursorState.Wait;
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
                MouseCursor.CustomImage = HeldItem.Definition.Image;
                MouseCursor.Hightlight = Target != null;
            }
            else
            {
                MouseCursor.CustomImage = null;
                
                if (session.HeadbuttMode)
                    MouseCursor.State = MouseCursorState.Hit;
                else
                    MouseCursor.State = Target?.GetMouseCursorState() ?? MouseCursorState.Cross;

                MouseCursor.Hightlight = false;
            }

            InvalidateText();
        }

        // TryInteract
        public bool TryInteract()
        {
            if (Target == null)
                return false;

            if (session.Player is not Actor player)
                return false;

            if (HeldItem != null && UseWithScript == null)
            {
                Sound.Play(SoundNames.Error);
                MouseCursor.Shake();
            }
            else
            {
                player.ApproachAndInteract(Target, HeldItem);
            }

            return true;
        }

        // UseWithScript
        public Script? UseWithScript { get; private set; }
    }
}
