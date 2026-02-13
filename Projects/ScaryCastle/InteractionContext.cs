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
    public sealed class InteractionContext
    {
        // Constructor
        public InteractionContext(GameSession session)
        {
            this.Session = session;
        }

        #region Private members

        // CanScanTarget
        private bool CanScanTarget()
        {
            // Modal speech bubble active
            if (SpeechBubble.ModalInstance != null)
                return false;

            // Player is recovering will
            if (Session.Player?.IsTired == true)
                return false;

            // Session is awaiting script
            if (Session.IsAwaiting)
                return false;

            // Inventory is active
            if (!Session.IsCurrentScene)
                return false;

            return true;
        }

        // InvalidateScript
        private void InvalidateScript()
        {
            if (Target == null || Session.Player == null)
            {
                CursorOverride = null;
                Script = null;
                return;
            }

            CursorOverride = Target.GetMouseCursorState();
            if (CursorOverride != null)
            {
                Script = null;
                return;
            }

            if (HeldItem == null)
            {
                Script = null;
                return;
            }

            // Try to find an overload
            Script = Session.ScriptLibrary.FindOverload(Target.DeclaredName, HeldItem.Name);

            // Try to find a routine that represents the item outcome
            if (Script == null)
            {
                if (Session.ScriptLibrary.FindRoutine($"{HeldItem.Name}Outcome") is Script script)
                {
                    if ((HeldItem.Definition.SelfTarget && Target == Session.Player) ||
                        (!HeldItem.Definition.SelfTarget && Target != Session.Player))
                    {
                        Script = script;
                    }
                }
            }
        }

        // RefreshCore
        private void RefreshCore()
        {
            if (HeldItem?.Count <= 0)
                HeldItem = null;
            
            if (!CanScanTarget())
            {
                if (Target != null)
                {
                    Target = null;
                    InvalidateScript();
                }
                return;
            }

            // Scan target
            var newTarget = ScanForTarget();
            if (newTarget != Target)
            {
                Target = newTarget;
                InvalidateScript();
            }
        }

        // ScanForTarget
        private GameThing? ScanForTarget()
        {
            if (Session.Room == null)
                return null;

            var mousePos = InputManager.DefaultPlayer.Mouse.WorldPosition(Session.Camera);

            for (int i = Session.Room.CulledThings.Count - 1; i >= 0; i--)
            {
                // Player exclusion when holding no item
                if (Session.Room.CulledThings[i] == Session.Player && HeldItem == null)
                    continue;

                if (Session.Room.CulledThings[i] is GameThing target && target.CanInteract() && target.RuntimeHotspot.Contains(mousePos))
                    return target;
            }

            return null;
        }

        #endregion

        // CursorOverride
        public MouseCursorState? CursorOverride { get; private set; }

        // HeldItem
        public Item? HeldItem { get; set; }

        // IsValidInteraction
        public bool IsValidInteraction { get; private set; }

        // Refresh
        public void Refresh()
        {
            RefreshCore();

            IsValidInteraction = Target != null;

            if (IsValidInteraction && HeldItem != null)
            {
                IsValidInteraction = Script != null;
                if (HeldItem.Definition.InventoryCategory == InventoryCategory.Sacred && Target is not Actor)
                    IsValidInteraction = false;
            }

            MouseCursorAppearance.Refresh(this);
        }

        // Reset
        public void Reset()
        {
            HeldItem = null;
            Target = null;
            Script = null;
            MouseCursorAppearance.Refresh(this);
        }

        // Script
        public Script? Script { get; private set; }

         // Session
        public GameSession Session { get; }

        // Target
        public GameThing? Target { get; private set; }
    }
}