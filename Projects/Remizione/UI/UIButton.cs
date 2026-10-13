using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione
{
    /// <summary>
    /// UIButton
    /// </summary>
    public sealed class UIButton : GameObject, IBoundingBox
    {
        private readonly Color disabledColor = Color.Gray;
        private readonly TextSprite label;
        private readonly Sprite[] strips;

        #region Constructor

        // Constructor
        public UIButton()
        {
            this.label = new TextSprite(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                Scale = ScaleInfo.Text.Huge
            };

            strips = new Sprite[3];

            strips[0] = new(Atlases.UI.UIButtonContainerEdge) { PivotOrigin = RectanglePoint.RightTop };
            strips[1] = new(Atlases.UI.UIButtonContainerPattern) { PivotOrigin = RectanglePoint.Center };
            strips[2] = new(Atlases.UI.UIButtonContainerEdge)
            {
                PivotOrigin = RectanglePoint.LeftTop,
                Effects = SpriteEffects.FlipHorizontally
            };
        }

        #endregion

        #region Private members

        // Layout
        private void Layout()
        {
            if (label.IsEmpty)
                BoundingBox = RectangleF.Empty;

            strips[1].ScaleX = label.BoundingBox.Width / 2;
            strips[1].Position = label.BoundingBox.GetPoint(RectanglePoint.Center, 0, -.75f);

            strips[0].Position = strips[1].BoundingBox.GetPoint(RectanglePoint.LeftTop);
            strips[2].Position = strips[1].BoundingBox.GetPoint(RectanglePoint.RightTop);

            BoundingBox = RectangleF.Union(strips[0].BoundingBox, strips[1].BoundingBox, strips[2].BoundingBox);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!label.IsEmpty)
            {
                strips[0].Draw(gameTime);
                strips[1].Draw(gameTime);
                strips[2].Draw(gameTime);
                label.Draw(gameTime);
            }
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

            if (IsMouseOver && IsEnabled)
                label.Color = HoverColor;
            else
                label.Color = IsEnabled ? TextColor : disabledColor;
        }

        #endregion

        // AllowSound
        public bool AllowSound { get; set; } = true;

        // BoundingBox
        public RectangleF BoundingBox { get; private set; }

        // Font
        public Font Font
        {
            get => label.Font;
            set => label.Font = value;
        }

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
            set
            {
                if (value != label.PivotOrigin)
                {
                    label.PivotOrigin = value;
                    Layout();
                }
            }
        }

        // Position
        public Vector2 Position
        {
            get => label.Position;
            set
            {
                if (value != label.Position)
                {
                    label.Position = value;
                    Layout();
                }
            }
        }

        // Scale
        public Vector2 Scale
        {
            get => label.Scale;
            set
            {
                if (value != label.Scale)
                {
                    label.Scale = value;
                    Layout();
                }
            }
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
                    Sound.Play(SoundNames.Interact);
            }

            return result;
        }

        // Text
        public string? Text
        {
            get => label.Text;
            set
            {
                if (value != label.Text)
                {
                    label.Text = value;
                    Layout();
                }
            }
        }

        // TextColor
        public Color TextColor { get; set; } = ColorPalette.Text.Terra;

        // X
        public float X
        {
            get => label.X;
            set
            {
                if (value != label.X)
                {
                    label.X = value;
                    Layout();
                }
            }
        }

        // Y
        public float Y
        {
            get => label.Y;
            set
            {
                if (value != label.Y)
                {
                    label.Y = value;
                    Layout();
                }
            }
        }
    }
}
