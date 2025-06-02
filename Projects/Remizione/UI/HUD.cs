using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remizione.UI;
using System;

namespace Remizione
{
    /// <summary>
    /// HUD
    /// </summary>
    public sealed class HUD : GameObject, IInputHandler
    {
        #region Private fields

        private readonly UIScore ashes;
        private readonly HUDButton inventoryButton;
        private readonly UIDerivedStats playerStats;
        private readonly HUDButton prayerButton;
        private readonly ImageSprite savingIcon;
        private readonly TextSprite sentence;
        private readonly GameSession session;
        private readonly TextSprite statusText;

        #endregion

        #region Constructor

        // Constructor
        public HUD(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            this.playerStats = new(session.Game);

            // DestinationMark
            this.DestinationMark = new DestinationMark(session);

            // Log
            this.Log = new(Game);

            // Saving icon
            this.savingIcon = new ImageSprite(Game, Atlases.UI.SavingIcon)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.Area.GetPoint(RectanglePoint.RightTop, -8, 6)
            };

            // Ashes
            this.ashes = new UIScore(session.Game, TextRepository.GetValue("ActorProperty.Ashes.Name"))
            {
                HideZero = true,
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.SafeArea.GetPoint(RectanglePoint.RightTop, -3, 0),
            };

            // Sentence
            this.sentence = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.TerraLight,
                MaximumWidth = (int)(Screen.NativeWidth * .8f),
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.SafeArea.GetPoint(RectanglePoint.Bottom, 0, -4),
                Scale = ScaleInfo.Text.VeryLarge
            };

            // Message text
            this.statusText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Terra,
                PivotOrigin = RectanglePoint.Top,
                Position = Screen.Area.GetPoint(RectanglePoint.Top, 0, 8),
                Scale = ScaleInfo.Text.VeryLarge
            };

            // Prayer button
            this.prayerButton = new HUDButton(Game, "PrayerIcon", Localization.GetValue(InGameMenuOptionName.Prayers))
            {
                Position = Screen.SafeArea.GetPoint(RectanglePoint.LeftBottom, 4, -4),
            };

            // Inventory button
            this.inventoryButton = new HUDButton(Game, "InventoryIcon", Localization.GetValue(InGameMenuOptionName.Inventory))
            {
                Position = prayerButton.BoundingBox.GetPoint(RectanglePoint.RightBottom),
            };
        }

        #endregion

        #region Private members

        // FormatSentenceText
        private static string FormatSentenceText(string name)
        {
            const string dots = "...";

            int totalLength = dots.Length + name.Length + dots.Length;

            Span<char> buffer = stackalloc char[totalLength];

            // Copy prefix
            dots.AsSpan().CopyTo(buffer);
            int offset = dots.Length;

            // Copy name
            name.AsSpan().CopyTo(buffer.Slice(offset));
            offset += name.Length;

            // Copy suffix
            dots.AsSpan().CopyTo(buffer.Slice(offset));

            // Return string from buffer
            return new string(buffer);
        }

        // UpdateSentence
        private void UpdateSentence()
        {
            if (IsMouseOverButton() && session.Player != null)
            {
                if (inventoryButton.IsMouseOver)
                    sentence.Text = session.Player.Inventory.ToString();

                else if (prayerButton.IsMouseOver)
                    sentence.Text = session.Player.Prayers.ToString();

                return;
            }

            if (session.IsCurrentScene && MouseCursor.Instance.State != MouseCursorState.Wait &&
                session.Player?.InteractiveTarget is GameThing target &&
                (!session.TargetMode || target.CanBeTargeted))
            {
                if (target != sentence.Tag)
                {
                    sentence.Tag = target;
                    sentence.Text = FormatSentenceText(target.LocalizedDisplayName);
                }
            }
            else
            {
                sentence.Text = null;
                sentence.Tag = null;
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (session.FullHUD)
                playerStats.Draw(gameTime);

            if (session.IsCurrentScene)
            {
                Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
                statusText.Draw(gameTime);
                sentence.Draw(gameTime);
                Game.SpriteBatch.End();
            }

            if (session.Player != null && session.FullHUD)
            {
                ashes.Draw(gameTime);
                if (session.IsCurrentScene)
                {
                    prayerButton.Draw(gameTime);
                    inventoryButton.Draw(gameTime);
                }
            }

            Log.Draw(gameTime);

            if (savingIcon.Tweens.IsTweening)
            {
                Game.SpriteBatch.Begin(Game.Camera);
                savingIcon.Draw(gameTime);
                Game.SpriteBatch.End();
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            playerStats.Update(gameTime);

            inventoryButton.Update(gameTime);
            prayerButton.Update(gameTime);

            UpdateSentence();
            statusText.Update(gameTime);

            if (session.Player != null)
            {
                ashes.Score = session.Player.Ashes;
                ashes.Update(gameTime);
            }

            DestinationMark.Update(gameTime);
            Log.Update(gameTime);
            savingIcon.Update(gameTime);
        }

        #endregion

        // DestinationMark
        public DestinationMark DestinationMark { get; }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (inventoryButton.TestPressed(0))
            {
                session.ShowItemContainerScene(ItemContainerCategory.Inventory);
                return HandleInputResult.Handled;
            }

            if (prayerButton.TestPressed(0))
            {
                session.ShowItemContainerScene(ItemContainerCategory.Prayers);
                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // IsMouseOverButton
        public bool IsMouseOverButton()
        {
            return inventoryButton.IsMouseOver || prayerButton.IsMouseOver;
        }

        // Log
        public UILog Log { get; }

        // Reset
        public void Reset()
        {
            playerStats.Actor = session.Player;
        }

        // ShowSavingIcon
        public void ShowSavingIcon()
        {
            savingIcon.Tweens.OpacityTween = FloatTween.Create(TweenStyle.QuadraticInOut, 1, .8f, 300, 10);
        }

        // Status
        public string Status
        {
            get => statusText.Text ?? string.Empty;
            set
            {
                if (statusText.Text != value)
                {
                    statusText.Text = value;
                    statusText.Tweens.OpacityTween = FloatTween.Create(TweenStyle.CubicInOut, 0, 1, 500);
                }
            }
        }
    }
}
