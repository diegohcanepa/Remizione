using Engendro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ScaryCastle.Menus
{
    /// <summary>
    /// Option
    /// </summary>
    public abstract class Option<T> : UIComponent, IOption where T : struct
    {
        #region Private members

        private readonly ImageSprite[] arrows;
        private const float disableColorFactor = .3f;
        private readonly List<string> displayNames = [];
        private readonly ImageSprite icon;
        private readonly FloatTween shake = new();
        private readonly TextSprite[] textSprites;
        private T value;
        private readonly List<T> values = [];

        #endregion

        #region Constructor

        // Constructor
        protected Option(ScaryCastleGame game, string label)
            : this(game, label, true)
        {
        }

        // Constructor
        protected Option(ScaryCastleGame game, string label, bool isEnabled)
            : base(game)
        {
            this.Game = game;

            this.textSprites = new TextSprite[2];

            this.IsEnabled = isEnabled;

            // Label
            this.textSprites[0] = new TextSprite(Game, Fonts.Common)
            {
                Color = ColorPalette.MenuOptionLabel * (isEnabled ? 1 : disableColorFactor),
                PivotOrigin = RectanglePoint.Right,
                Scale = ScaleInfo.MenuOption,
                Text = label
            };

            // Value
            this.textSprites[1] = new TextSprite(Game, Fonts.Common)
            {
                Color = ColorPalette.TextWhite * (isEnabled ? 1 : disableColorFactor),
                PivotOrigin = RectanglePoint.Center,
                Scale = ScaleInfo.MenuOption,
            };

            arrows = new ImageSprite[2];

            arrows[0] = new ImageSprite(game, Atlases.Menu.MenuItemArrowLeft)
            {
                PivotOrigin = RectanglePoint.Right,
                X = 190,
                Scale = new Vector2(.3f)
            };

            if (!isEnabled)
            {
                arrows[0].Color *= disableColorFactor;
            }

            arrows[1] = new ImageSprite(game, Atlases.Menu.MenuItemArrowRight)
            {
                PivotOrigin = RectanglePoint.Left,
                X = 290,
                Scale = new Vector2(.3f)
            };

            if (!isEnabled)
            {
                arrows[1].Color *= disableColorFactor;
            }

            value = default;

            Values = new ReadOnlyCollection<T>(values);
            IsEnabled = isEnabled;

            // Icon
            this.icon = new ImageSprite(Game)
            {
                Color = ColorPalette.MenuOptionLabel,
                PivotOrigin = RectanglePoint.Right,
                Scale = new Vector2(.25f)
            };

            if (!isEnabled)
            {
                icon.Color *= disableColorFactor;
            }
        }

        #endregion

        #region Private members

        // GetDisplayName
        private string GetDisplayName(T value)
        {
            var index = values.IndexOf(value);
            if (index >= 0)
            {
                return displayNames[index];
            }
            else
            {
                return string.Empty;
            }
        }

        // PerformDisableNotification
        private void PerformDisableNotification()
        {
            if (shake.IsRunning)
            {
                return;
            }

            //SoundManager.Play(SoundNames.InputError.ToString());
            shake.Start(TweenStyle.Linear, 0, 1, 40, 4);
        }

        #endregion

        #region Protected members

        // AddValue
        protected void AddValue(string displayName, T value)
        {
            displayNames.Add(displayName);
            values.Add(value);
        }

        // Initialized
        protected bool Initialized { get; private set; }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            textSprites[0].Draw(gameTime);

            if (shake.IsRunning)
            {
                textSprites[1].X += shake.CurrentValue;
            }

            textSprites[1].Draw(gameTime);

            if (shake.IsRunning)
            {
                textSprites[1].X -= shake.CurrentValue;
            }

            if (HasValues)
            {
                arrows[0].Draw(gameTime);
                arrows[1].Draw(gameTime);
            }

            if (!icon.IsEmpty)
            {
                icon.Draw(gameTime);
            }
        }

        // OnInvalidate
        protected override void OnInvalidate()
        {
            base.OnInvalidate();
            arrows[0].Y = textSprites[1].BoundingBox.Center.Y;
            arrows[1].Y = arrows[0].Y;
            textSprites[0].Position = arrows[0].BoundingBox.GetPoint(RectanglePoint.Left, -4, 0);
            LabelBoundingBox = RectangleF.Union(arrows[0].BoundingBox, arrows[1].BoundingBox);
        }

        // OnPerformClick
        protected virtual void OnPerformClick()
        {
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            textSprites[0].Update(gameTime);
            textSprites[1].Update(gameTime);

            if (shake.IsRunning)
            {
                shake.Update(gameTime);
            }

            icon.Position = textSprites[0].BoundingBox.GetPoint(RectanglePoint.Left, -3, 0);
        }

        // OnValueChanged
        protected virtual void OnValueChanged(T newValue)
        {
        }

        // Values
        protected ReadOnlyCollection<T> Values { get; }

        #endregion

        // Description
        public virtual string Description => string.Empty;

        // Game
        public new ScaryCastleGame Game { get; }

        // HasValues
        public bool HasValues => values.Count > 0;

        // IconImage
        public AtlasImage? IconImage
        {
            get => icon.Image;
            set => icon.Image = value;
        }

        // IsEnabled
        public bool IsEnabled { get; }

        // LabelBoundingBox
        public virtual RectangleF LabelBoundingBox { get; private set; }

        // LeftArrowBoundingBox
        public RectangleF LeftArrowBoundingBox => arrows[0].BoundingBox;

        // NextValue
        public bool NextValue()
        {
            if (!IsEnabled)
            {
                PerformDisableNotification();
                return false;
            }

            if (values.Count >= 2)
            {
                var index = values.IndexOf(Value);
                index++;
                if (index == values.Count)
                {
                    index = 0;
                }

                //SoundManager.Play(SoundNames.MenuOptionValue.ToString());
                Value = values[index];
                return true;
            }

            return false;
        }

        // PerformClick
        public void PerformClick()
        {
            OnPerformClick();
        }

        // Position
        public Vector2 Position
        {
            get => textSprites[1].Position;
            set
            {
                textSprites[1].Position = value;
                Invalidate();
            }
        }

        // PreviousValue
        public bool PreviousValue()
        {
            if (!IsEnabled)
            {
                PerformDisableNotification();
                return false;
            }

            if (values.Count >= 2)
            {
                var index = values.IndexOf(Value);
                index--;
                if (index < 0)
                {
                    index = values.Count - 1;
                }

                //SoundManager.Play(SoundNames.MenuOptionValue.ToString());
                Value = values[index];
                return true;
            }

            return false;
        }

        // RightArrowBoundingBox
        public RectangleF RightArrowBoundingBox => arrows[1].BoundingBox;

        // Value
        public T Value
        {
            get => value;
            set
            {
                this.value = value;
                textSprites[1].Text = GetDisplayName(value);

                if (Initialized)
                {
                    OnValueChanged(value);
                }
                else
                {
                    Initialized = true;
                }
            }
        }
    }
}
