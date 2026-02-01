using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// Card
    /// </summary>
    public sealed class Card : GameObject
    {
        #region Private fields

        private readonly ImageSprite actionIcon;
        private readonly Countdown actionIconEffectCountdown = new() { DefaultDuration = Random.Shared.Next(1500, 2500) };
        private readonly ImageSprite cardContainer;
        private readonly ImageSprite cardContainerShadow;
        private readonly ImageSprite categoryIcon;
        private readonly ImageSprite energyCostNumber;
        private readonly FloatTween floatTween = new();
        private readonly Vector2Tween heartTween = new();
        private readonly Vector2Tween positionTween = new();
        private readonly FloatTween scaleTween = new();

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

            // Category icon
            this.categoryIcon = new(Game, Definition.CategoryImage)
            {
                PivotOrigin = RectanglePoint.Center
            };

            // Energy cost number
            this.energyCostNumber = new(Game, Definition.EnergyCostImage)
            {
                PivotOrigin = RectanglePoint.RightBottom
            };

            if (Definition.Action is CardAction.Damage or CardAction.Heal)
                actionIconEffectCountdown.Start();

            Refresh();
        }

        #endregion

        #region Private members

        // MoveToCompleted
        public void MoveToCompleted(bool flip)
        {
            if (flip)
                Flip();
            else
                Sound.Play(SoundNames.CardFlap);
        }

        // MoveTo
        public void MoveTo(Vector2 destination, int duration, int delay, bool flip, bool scale = false)
        {
            positionTween.StartDelay = delay;
            positionTween.Start(TweenStyle.CubicIn, Position, destination, duration, () => MoveToCompleted(flip));

            if (scale)
            {
                scaleTween.Start(TweenStyle.CubicIn, 0, 1, duration);
                Scale = 0;
            }
        }

        // Refresh
        private void Refresh()
        {
            var scale = new Vector2(Scale);
            cardContainer.Image = IsFaceUp ? Definition.FrontImage : Atlases.UI.CardBack;
            cardContainerShadow.Image = cardContainer.Image;
            cardContainer.Scale = scale;
            cardContainerShadow.MatchTransform(cardContainer);
            cardContainerShadow.Position += Vector2.One;

            if (!IsFaceUp)
                return;

            actionIcon.Tweens.Reset();
            actionIcon.Scale = scale;
            categoryIcon.Scale = scale;
            energyCostNumber.Scale = scale;

            categoryIcon.Position = cardContainer.BoundingBox.GetPoint(RectanglePoint.Top, 0, 2 * scale.Y);
            actionIcon.Position = cardContainer.BoundingBox.GetPoint(RectanglePoint.Center, -.5f * scale.X, -.4f * scale.Y);
            energyCostNumber.Position = cardContainer.BoundingBox.GetPoint(RectanglePoint.RightBottom, -1 * scale.X, -1 * scale.Y);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (Float)
                Position += new Vector2(0, floatTween.CurrentValue);

            cardContainerShadow.Draw(gameTime);
            cardContainer.Draw(gameTime);

            if (IsFaceUp)
            {
                categoryIcon.Draw(gameTime);
                actionIcon.Draw(gameTime);
                energyCostNumber.Draw(gameTime);
            }

            if (Float)
                Position -= new Vector2(0, floatTween.CurrentValue);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            floatTween.Update(gameTime);
            actionIcon.Update(gameTime);
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

            if (IsMoving)
            {
                positionTween.Update(gameTime);
                Position = positionTween.CurrentValue;

                if (scaleTween.IsRunning)
                {
                    scaleTween.Update(gameTime);
                    Scale = scaleTween.CurrentValue;
                }
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
            IsFaceUp = !IsFaceUp;
            Sound.Play(SoundNames.CardFlap);
        }

        // Float
        public bool Float
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    if (field)
                    {
                        if (!floatTween.IsRunning)
                            floatTween.Start(TweenStyle.CubicInOut, 0, .75f, 400, -1);
                    }
                    else
                    {
                        floatTween.Stop();
                    }
                }
            }
        }

        // IsFaceUp
        public bool IsFaceUp
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

        // IsHovered
        public bool IsHovered
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    if (value)
                        Scale = 1.05f;
                    else
                        Scale = 1;
                }
            }
        }

        // IsMouseOver
        public bool IsMouseOver()
        {
            return BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.WorldPosition(Game.Camera));
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

        // ToString
        public override string ToString()
        {
            return Name;
        }
    }
}
