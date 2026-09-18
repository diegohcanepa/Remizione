using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// UIButton
    /// </summary>
    public sealed class UIButton : GameObject, IBoundingBox
    {
        private readonly TextSprite label;

        // Constructor
        public UIButton()
        {
            this.label = new TextSprite(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                Scale = ScaleInfo.Text.Huge
            };
        }

        #region Private members
        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            label.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            IsMouseOver = false;

            if (IsEnabled)
            {
                if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Mouse)
                    IsMouseOver = BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.VirtualPosition);
            }

            label.Update(gameTime);

            label.Color = IsMouseOver ? HoverColor : TextColor;
        }

        #endregion

        // AllowSound
        public bool AllowSound { get; set; } = true;

        // BoundingBox
        public RectangleF BoundingBox => label.BoundingBox;

        // HoverColor
        public Color HoverColor { get; set; } = ColorPalette.Text.Yellow;

        // IsEnabled
        public bool IsEnabled { get; set; } = true;

        // IsMouseOver
        public bool IsMouseOver { get; private set; }

        // PivotOrigin
        public RectanglePoint PivotOrigin
        {
            get => label.PivotOrigin;
            set => label.PivotOrigin = value;
        }

        // Position
        public Vector2 Position
        {
            get => label.Position;
            set => label.Position = value;
        }

        // Scale
        public Vector2 Scale
        {
            get => label.Scale;
            set => label.Scale = value;
        }

        // Sound
        public Sound? Sound { get; set; }

        // Tag
        public object? Tag { get; set; }

        // TestPressed
        public bool TestPressed()
        {
            if (!IsEnabled)
                return false;

            var result = false;

            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed() && IsMouseOver)
                result = true;

            if (result && AllowSound)
            {
                if (Sound != null)
                    Sound.Play();
                else
                    Sound.Play(SoundNames.UISelectA);
            }

            return result;
        }

        // Text
        public string? Text
        {
            get => label.Text;
            set => label.Text = value;
        }

        // TextColor
        public Color TextColor { get; set; } = ColorPalette.Text.Terra;

        // X
        public float X
        {
            get => label.X;
            set => label.X = value;
        }

        // Y
        public float Y
        {
            get => label.Y;
            set => label.Y = value;
        }
    }
}
