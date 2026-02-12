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
        #region Private fields

        private readonly string headbuttVerb = Localization.GetValue(Verb.Headbutt);
        private readonly GameSession session = session;
        private readonly string useVerb = Localization.GetValue(Verb.Use);
        private readonly string withPreposition = TextRepository.GetValue("Misc.WithPreposition");

        #endregion

        #region Private members

        // InvalidateMouseText
        private void InvalidateMouseText()
        {
            // No target
            if (Target == null)
            {
                MouseCursor.ClearText();
                return;
            }

            // Get sentence
            var sentence = Target.LocalizedDisplayName;

            // Compose text
            if (HeldItem == null)
            {
                MouseCursor.Text = session.HeadbuttMode ? $"{headbuttVerb} {sentence}" : sentence;
                if (Target.MaxHP > 0 && Target is ProceduralActor)
                {
                    MouseCursor.TextExtra = $" [{Target.HP}/{Target.MaxHP}]";

                    var hpRatio = Target.HP / Target.MaxHP;
                    if (hpRatio > .7f)
                        MouseCursor.TextExtraColor = ColorPalette.Text.Yellow;
                    else if (hpRatio > .4f)
                        MouseCursor.TextExtraColor = ColorPalette.Text.Orange;
                    else
                        MouseCursor.TextExtraColor = ColorPalette.Text.Red;
                }
            }
            else
            {
                MouseCursor.Text = $"{useVerb} {HeldItem.Definition.LocalizedDisplayName} {withPreposition} {sentence}";
                MouseCursor.TextExtra = null;
            }
        }

        // InvalidateUseWithScript
        private void InvalidateUseWithScript()
        {
            if (Target == null || HeldItem == null || session.Player == null)
            {
                UseWithScript = null;
                return;
            }

            // Try to find an overload
            UseWithScript = session.ScriptLibrary.FindOverload(Target.DeclaredName, HeldItem.Name);

            // Try to find a routine that represents the item outcome
            if (UseWithScript == null)
            {
                if (session.ScriptLibrary.FindRoutine($"{HeldItem.Name}Outcome") is Script script)
                {
                    if ((HeldItem.Definition.SelfTarget && Target == session.Player) ||
                        (!HeldItem.Definition.SelfTarget && Target != session.Player))
                    {
                        UseWithScript = script;
                    }
                }
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

        // PerformInteraction
        public void PerformInteraction()
        {
            static void Fail()
            {
                Sound.Play(SoundNames.Error);
                MouseCursor.Shake();
            }

            if (session.Player == null)
                return;

            MouseCursor.PerformClick();

            if (MouseCursor.State == MouseCursorState.Prohibition)
            {
                Fail();
                return;
            }

            if (Target == null)
            {
                var destination = InputManager.DefaultPlayer.Mouse.WorldPosition(session.Camera);
                session.Player.MoveTo(destination);
                return;
            }

            if (HeldItem != null)
            {
                if (MouseCursor.HightlightState == MouseCursorHightlightState.Red)
                {
                    Fail();
                    return;
                }
                else if (UseWithScript?.ScriptType == ScriptType.Routine)
                {
                    HeldItem = null;
                    session.Player.StopMoving();
                    session.Player.FaceTo(Target);
                    session.BeginOutcome(UseWithScript, Target);
                    return;
                }
            }

            session.Player.ApproachAndInteract(Target, HeldItem, session.HeadbuttMode ? ApproachBehavior.ClosestSide : null);
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
                    field = value;
                    InvalidateUseWithScript();
                    InvalidateMouseText();
                    MouseCursor.HightlightState = Target == null ? MouseCursorHightlightState.None : MouseCursorHightlightState.Green;
                    if (Target != null && HeldItem != null)
                    {
                        if (UseWithScript == null)
                        {
                            MouseCursor.HightlightState = MouseCursorHightlightState.Red;
                        }
                        else
                        {
                            if (HeldItem.Definition.InventoryCategory == InventoryCategory.Sacred && Target is not Actor)
                                MouseCursor.HightlightState = MouseCursorHightlightState.Red;
                            else
                                MouseCursor.HightlightState = MouseCursorHightlightState.Green;
                        }
                    }
                }
            }
        }

        // Update
        public void Update()
        {
            if (HeldItem?.Count <= 0)
                HeldItem = null;

            // No room 
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

            if (session.Player?.IsTired == true)
            {
                MouseCursor.State = MouseCursorState.Wait;
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

            if (Target != null && Target.IsMoving)
            {
                MouseCursor.State = MouseCursorState.Prohibition;
                MouseCursor.CustomImage = null;
                Target = null;
            }
            else if (HeldItem != null)
            {
                MouseCursor.CustomImage = HeldItem.Definition.Image;
            }
            else
            {
                MouseCursor.CustomImage = null;

                if (session.HeadbuttMode)
                    MouseCursor.State = MouseCursorState.Hit;
                else
                    MouseCursor.State = Target?.GetMouseCursorState() ?? MouseCursorState.Cross;
            }

            InvalidateMouseText();
        }

        // UseWithScript
        public Script? UseWithScript { get; private set; }
    }
}
