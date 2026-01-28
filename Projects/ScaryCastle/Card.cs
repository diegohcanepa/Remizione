using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// Card
    /// </summary>
    public sealed class Card : GameObject
    {
        #region Private fields

        private readonly ImageSprite actionIcon;
        private readonly Countdown actionIconEffectCountdown = new() { DefaultDuration = 1500 };
        private readonly ImageSprite bonusValueIcon;
        private readonly ImageSprite categoryIcon;
        private readonly ImageSprite cardContainer;
        private readonly ImageSprite cardContainerShadow;
        private readonly ImageSprite diceIcon;
        private readonly Countdown diceIconEffectCountdown = new() { DefaultDuration = 3500 };
        private readonly ImageSprite diceThresholdNumber;
        private readonly Vector2Tween diceTween = new();
        private readonly Vector2Tween heartTween = new();
        private readonly Vector2Tween positionTween = new();

        #endregion

        #region Constructors

        // Constructor
        public Card(EngendroGame game, string cardName)
            : this(game, CardDefinition.Get(cardName))
        {

        }

        // Constructor
        public Card(EngendroGame game, CardDefinition definition)
            : base(game)
        {
            this.Definition = definition;

            // Card container
            this.cardContainer = new(Game)
            {
            };

            // Card container shadow
            this.cardContainerShadow = new(Game)
            {
                Color = Color.Black,
                Opacity = ColorPalette.ShadowOpacity
            };

            // Action icon
            this.actionIcon = new(Game, Definition.ActionImage)
            {
                PivotOrigin = RectanglePoint.Center
            };

            // Bonus value number image
            this.bonusValueIcon = new(Game, Definition.BonusValueImage)
            {
                PivotOrigin = RectanglePoint.RightBottom,
            };

            // Category icon
            this.categoryIcon = new(Game, Definition.CategoryImage)
            {
                PivotOrigin = RectanglePoint.Center
            };

            // Dice icon
            this.diceIcon = new(Game, Atlases.UI.GetImage("CardDice"))
            {
                PivotOrigin = RectanglePoint.Bottom,
            };

            // Dice threshold number image
            this.diceThresholdNumber = new(Game, Definition.DiceThresholdImage)
            {
                PivotOrigin = RectanglePoint.LeftBottom,
            };

            if (Definition.Action is CardAction.Damage or CardAction.Heal)
                actionIconEffectCountdown.Start();

            if (Definition.HasBonus)
                diceIconEffectCountdown.Start();

            Refresh();
        }

        #endregion

        #region Private members

        // MoveTo
        public void MoveTo(Vector2 destination, int delay, bool flip)
        {
            positionTween.StartDelay = delay;
            positionTween.Start(TweenStyle.CubicIn, Position, destination, 500, flip ? Flip : null);
        }

        // Refresh
        private void Refresh()
        {
            cardContainer.Image = IsFaceVisible ? Definition.FrontImage : Definition.BackImage;
            cardContainerShadow.Image = cardContainer.Image;

            if (!IsFaceVisible)
                return;

            actionIcon.Tweens.Reset();
            diceIcon.Tweens.Reset();

            var scale = new Vector2(Scale);

            cardContainer.Scale = scale;
            actionIcon.Scale = scale;
            categoryIcon.Scale = scale;
            diceIcon.Scale = scale * .55f;
            diceThresholdNumber.Scale = scale * .65f;
            bonusValueIcon.Scale = scale * .65f;

            cardContainerShadow.MatchTransform(cardContainer);
            cardContainerShadow.Position = cardContainer.Position + Vector2.One;

            actionIcon.Position = cardContainer.BoundingBox.GetPoint(RectanglePoint.Center, -.5f * actionIcon.ScaleX, -1f * actionIcon.ScaleY);
            categoryIcon.Position = cardContainer.BoundingBox.GetPoint(RectanglePoint.Top, 0, 1 * categoryIcon.ScaleY);
            diceIcon.Position = cardContainer.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, -4f * diceIcon.ScaleY);
            diceThresholdNumber.Position = cardContainer.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 3.5f * diceThresholdNumber.ScaleX, -3.5f * diceThresholdNumber.ScaleY);
            bonusValueIcon.Position = cardContainer.BoundingBox.GetPoint(RectanglePoint.RightBottom, -3.5f * bonusValueIcon.ScaleX, -3.5f * bonusValueIcon.ScaleY);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            cardContainerShadow.Draw(gameTime);
            cardContainer.Draw(gameTime);

            if (IsFaceVisible)
            {
                categoryIcon.Draw(gameTime);
                actionIcon.Draw(gameTime);

                if (Definition.HasBonus)
                {
                    diceIcon.Draw(gameTime);
                    diceThresholdNumber.Draw(gameTime);
                    bonusValueIcon.Draw(gameTime);
                }
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            actionIcon.Update(gameTime);
            diceIcon.Update(gameTime);
            cardContainer.Update(gameTime);

            if (actionIconEffectCountdown.IsRunning)
            {
                actionIconEffectCountdown.Update(gameTime);
                if (!actionIconEffectCountdown.IsRunning)
                {
                    if (Definition.Action is CardAction.Damage or CardAction.Heal)
                    {
                        heartTween.Start(TweenStyle.CubicInOut, Vector2.One, Vector2.One * 1.1f, 100, 4);
                        actionIcon.Tweens.ScaleTween = heartTween;
                    }

                    actionIconEffectCountdown.Restart();
                }
            }

            if (diceIconEffectCountdown.IsRunning)
            {
                diceIconEffectCountdown.Update(gameTime);
                if (!diceIconEffectCountdown.IsRunning)
                {
                    diceTween.Start(TweenStyle.Linear, diceIcon.Position, diceIcon.Position - new Vector2(.25f), 30, 4);
                    diceIcon.Tweens.PositionTween = diceTween;
                    diceIconEffectCountdown.Restart();
                }
            }

            if (IsMoving)
            {
                positionTween.Update(gameTime);
                Position = positionTween.CurrentValue;
            }
        }

        #endregion

        // BoundingBox
        public RectangleF BoundingBox => cardContainer.BoundingBox;

        // Definition
        public CardDefinition Definition { get; }

        // Flip
        public void Flip()
        {
            IsFaceVisible = !IsFaceVisible;
            Sound.Play(SoundNames.CardFlap);
        }

        // IsFaceVisible
        public bool IsFaceVisible
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Refresh();
                }
            }
        }

        // IsMoving
        public bool IsMoving => positionTween.IsRunning;

        // Name
        public string Name => Definition.Name;

        // Position
        public Vector2 Position
        {
            get => cardContainer.Position;
            set
            {
                if (value != cardContainer.Position)
                {
                    cardContainer.Position = value;
                    Refresh();
                }
            }
        }

        // Scale
        public float Scale
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Refresh();
                }
            }
        } = 1;
    }
}
