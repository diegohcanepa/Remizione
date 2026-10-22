using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Remizione
{
    /// <summary>
    /// UIButton
    /// </summary>
    public abstract class UIButton : GameObject, IBoundingBox
    {
        #region Protected members

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Mouse)
                IsMouseOver = BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.VirtualPosition);
            else
                IsMouseOver = false;
        }

        #endregion

        // Action
        public Action? Action { get; set; }

        // BoundingBox
        public abstract RectangleF BoundingBox { get; }

        // HoverSound
        public Sound? HoverSound { get; set; } = Sound.Find(SoundNames.ButtonHover);

        // IsEnabled
        public bool IsEnabled { get; set; }

        // IsMouseOver
        public bool IsMouseOver
        {
            get;
            private set
            {
                if (value != field)
                {
                    field = value;
                    if (field)
                        HoverSound?.Play();
                }
            }
        }

        // PivotOrigin
        public abstract RectanglePoint PivotOrigin { get; set; }

        // Position
        public abstract Vector2 Position { get; set; }

        // PressSound
        public Sound? PressSound { get; set; } = Sound.Find(SoundNames.ButtonPress);

        // Scale
        public abstract Vector2 Scale { get; set; }

        // Tag
        public object? Tag { get; set; }

        // Tip
        public string Tip { get; set; } = string.Empty;

        // TestPressed
        public bool TestPressed()
        {
            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed() && IsMouseOver)
            {
                PressSound?.Play();
                Action?.Invoke();
                return true;
            }

            return false;
        }

        // X
        public abstract float X { get; set; }

        // Y
        public abstract float Y { get; set; }
    }
}
