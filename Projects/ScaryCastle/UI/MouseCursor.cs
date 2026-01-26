using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScaryCastle.Effects;
using ScaryCastle.Scripting;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// MouseCursor
    /// </summary>
    public static class MouseCursor
    {
        #region Private fields

        private static readonly AtlasImage?[] cursorImages;
        private static readonly ImageSprite cursorSprite;
        private static readonly Vector2Tween scaleTween = new();
        private static readonly FloatTween shakeTween = new();
        private static readonly TextSprite textSprite;
        private static readonly string useVerb;
        private static readonly string withPreposition;

        #endregion

        #region Constructor

        // Constructor
        static MouseCursor()
        {
            // Cursor sprite
            cursorSprite = new ImageSprite(EngendroGame.Instance)
            {
                PivotOrigin = RectanglePoint.Center,
                Scale = GetCurrentScale()
            };

            const string prefix = "MouseCursor";
            var names = Enum.GetNames<MouseCursorState>();

            cursorImages = new AtlasImage[names.Length];
            for (var i = 0; i < cursorImages.Length; i++)
            {
                var imageName = $"{prefix}{names[i]}";
                cursorImages[i] = Atlases.UI.FindImage(imageName);
            }

            textSprite = new(EngendroGame.Instance, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Sentence,
                PivotOrigin = RectanglePoint.LeftTop,
                Scale = ScaleInfo.UISentence
            };

            // Cargamos los textos de localización una sola vez
            useVerb = Localization.GetValue(Verb.Use);
            withPreposition = TextRepository.GetValue("Misc.WithPreposition");
        }

        #endregion

        #region Private members

        // ClampTextToScreen
        private static void ClampTextToScreen()
        {
            var offset = Item == null ? 4 : 2;

            if (!textSprite.IsEmpty)
            {
                textSprite.PivotOrigin = RectanglePoint.LeftTop;
                textSprite.Position = cursorSprite.BoundingBox.GetPoint(RectanglePoint.RightBottom, -offset, -offset);

                if (!textSprite.BoundingBox.IsInside(EngendroGame.Instance.Camera.VisibleBox))
                {
                    textSprite.PivotOrigin = RectanglePoint.RightTop;
                    textSprite.Position = cursorSprite.BoundingBox.GetPoint(RectanglePoint.LeftBottom, offset, -offset);
                }

                if (textSprite.BoundingBox.Bottom >= Screen.NativeHeight)
                    textSprite.Y -= 10;
            }
        }

        // GetCurrentScale
        private static Vector2 GetCurrentScale()
        {
            return Item != null ? ScaleInfo.InventoryHeldItem : ScaleInfo.UIElement.Large;
        }

        // InvalidateCursorImage
        private static void InvalidateCursorImage()
        {
            if (State == MouseCursorState.Item)
                cursorSprite.Image = Item?.Definition.Image;
            else
                cursorSprite.Image = cursorImages[(int)State];

            cursorSprite.Scale = GetCurrentScale();
            cursorSprite.PivotOrigin = State == MouseCursorState.Arrow ? RectanglePoint.LeftTop : RectanglePoint.Center;
        }

        // InvalidateText
        private static void InvalidateText()
        {
            // No target
            if (Target == null)
            {
                textSprite.Text = null;
                return;
            }

            // Get sentence
            var sentence = Target.GetInteractPrompt() ?? Target.LocalizedDisplayName;

            // Compose text
            if (Item == null)
            {
                textSprite.Text = sentence;
            }
            else
            {
                textSprite.Text = $"{useVerb} {Item.Definition.LocalizedDisplayName} {withPreposition} {sentence}";
            }
        }

        // ScanForTarget
        private static GameThing? ScanForTarget()
        {
            if (Room == null)
                return null;

            var mousePos = InputManager.DefaultPlayer.Mouse.WorldPosition(Room.Session.Camera);

            for (int i = Room.CulledThings.Count - 1; i >= 0; i--)
            {
                // Player exclusion when holding no item
                if (Room.CulledThings[i] == Room.Session.Player && Item == null)
                    continue;

                if (Room.CulledThings[i] is GameThing target && target.CanInteract() && target.RuntimeHotspot.Contains(mousePos))
                    return target;
            }

            return null;
        }

        #endregion

        // AnimateClick
        public static void AnimateClick()
        {
            scaleTween.Start(TweenStyle.QuadraticIn, GetCurrentScale() * .9f, GetCurrentScale(), 150);
            cursorSprite.Tweens.ScaleTween = scaleTween;
        }

        // Draw
        public static void Draw(GameTime gameTime)
        {
            OutlineEffect? effect = Item != null && Target != null && State == MouseCursorState.Item ? ScaryCastleGame.Effects.Outline : null;

            if (effect != null && cursorSprite.Image?.Atlas != null)
            {
                if (Target != null && Item != null && UseWithScript == null)
                {
                    effect.Color.SetValue(ColorPalette.MouseCursorRedOutline);
                }
                else
                {
                    effect.Color.SetValue(ColorPalette.MouseCursorOutline);
                }

                effect.TextureSize.SetValue(new Vector2(cursorSprite.Image.Atlas.Texture.Width, cursorSprite.Image.Atlas.Texture.Height));
                effect.Thickness.SetValue(1.2f);
            }

            EngendroGame.Instance.SpriteBatch.Begin(EngendroGame.Instance.Camera, SamplerState.PointClamp, effect?.Effect);
            cursorSprite.X += shakeTween.IsRunning ? shakeTween.CurrentValue : 0;
            cursorSprite.Draw(gameTime);
            cursorSprite.X -= shakeTween.IsRunning ? shakeTween.CurrentValue : 0;
            EngendroGame.Instance.SpriteBatch.End();

            EngendroGame.Instance.SpriteBatch.Begin(EngendroGame.Instance.Camera);
            if (State is MouseCursorState.Cross or MouseCursorState.Item)
                textSprite.Draw(gameTime);
            EngendroGame.Instance.SpriteBatch.End();
        }

        // Item
        public static Item? Item { get; set; }

        // PerformClick
        public static void PerformClick()
        {
            AnimateClick();
            Sound.Play(SoundNames.Interact);
        }

        // Room
        public static GameRoom? Room
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Item = null;
                    Target = null;
                    UseWithScript = null;
                }
            }
        }

        // Shake
        public static void Shake()
        {
            shakeTween.Start(TweenStyle.CubicInOut, 0, 1, 50, 4);
        }

        // State
        public static MouseCursorState State
        {
            get;
            private set
            {
                if (value != field)
                {
                    field = value;
                    InvalidateCursorImage();
                }
            }
        }

        // Target
        public static GameThing? Target
        {
            get;
            private set
            {
                if (value != field)
                {
                    field = value;

                    UseWithScript = null;

                    if (field != null && Item != null)
                        UseWithScript = field.Session.ScriptLibrary.FindRoutine($"{field.DeclaredName}-With-{Item.Name}");

                    InvalidateText();
                }
            }
        }

        // Text
        public static string? Text => textSprite.Text;

        // Update
        public static void Update(GameTime gameTime)
        {
            if (Item?.Count == 0)
                Item = null;

            cursorSprite.Position = InputManager.DefaultPlayer.Mouse.VirtualPosition;
            cursorSprite.Update(gameTime);
            shakeTween.Update(gameTime);

            // No room, no session. 
            if (Room == null)
            {
                State = MouseCursorState.Arrow;
                return;
            }

            // Session is awaiting
            if (Room.Session.IsAwaiting)
            {
                if (Room.Session.Player?.HasSpeechBubble == true)
                    State = MouseCursorState.Arrow;

                else if (Room.Session.AwaitingScript?.CurrentStatement is AwaitInputCommand)
                    State = MouseCursorState.Hand;

                else
                    State = MouseCursorState.Wait;

                return;
            }

            if (Room.Session.HUD.Inventory.IsVisible)
            {
                State = MouseCursorState.Hand;
                return;
            }

            if (SpeechBubble.ModalInstance == null)
                Target = ScanForTarget();

            if (Item != null)
                State = MouseCursorState.Item;
            else
                State = Target?.GetMouseCursorState() ?? MouseCursorState.Cross;

            ClampTextToScreen();
        }

        // UseWithScript
        public static Script? UseWithScript { get; private set; }
    }
}