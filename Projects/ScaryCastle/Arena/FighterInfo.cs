using Adberration;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScaryCastle
{
    /// <summary>
    /// FighterInfo
    /// </summary>
    public sealed class FighterInfo
    {
        private bool cardFromBrain;

        // Constructor
        public FighterInfo(Arena arena, Actor actor, Vector2 position, FacingDirection direction, Vector2 slotPosition)
        {
            this.Actor = actor;
            this.Arena = arena;
            this.OriginalDirection = actor.Direction;
            this.OriginalPosition = actor.Position;
            this.Actor.Scale += new Vector2(.2f);
            this.SlotPosition = slotPosition;

            // Add enemy
            arena.Children.Add(Actor);
            Actor.Position = position;
            Actor.Direction = direction;

            this.HealthMeter = new(actor);
        }

        // Actor
        public Actor Actor { get; }

        // Arena
        public Arena Arena { get; }

        // BringBackToPreviousRoom
        public void BringBackToPreviousRoom()
        {
            Actor.Scale -= new Vector2(.2f);
            Actor.Session.PreviousRoom?.Children.Add(Actor);
            Actor.Position = OriginalPosition;
            Actor.Direction = OriginalDirection;
        }

        // Card
        public Card? Card { get; private set; }

        // DiceRollResult
        public int DiceRollResult { get; set; }

        // Draw
        public void Draw(GameTime gameTime)
        {
            if (SpeechBubble.ModalInstance?.Actor != Actor)
                HealthMeter.Draw(gameTime);

            if (cardFromBrain)
                Card?.Draw(gameTime);
        }

        // HealthMeter
        public UIArenaHealthMeter HealthMeter { get; }

        // OriginalDirection
        public FacingDirection OriginalDirection { get; }

        // OriginalPosition
        public Vector2 OriginalPosition { get; }

        // PlayCard
        public Card? PlayCard()
        {
            this.Card = Actor.Brain.PickCard(Arena);
            
            if (Card != null)
            {
                Card.IsFaceUp = false;
                Card.Position = Actor.RuntimeHotspot.BoundingRectangleF.Center;
                Card.MoveTo(SlotPosition, 500, 0, true, true);
                Card.Float = true;
                cardFromBrain = true;
            }

            return Card;
        }

        // PlayCard
        public void PlayCard(Card card)
        {
            this.Card = card;
            this.Card.MoveTo(SlotPosition - new Vector2(card.BoundingBox.Width, 0), 400, 0, false);
            card.Float = true;
        }

        // ResetCard
        public void ResetCard()
        {
            this.Card = null;
            cardFromBrain = false;
        }

        // SlotPosition
        public Vector2 SlotPosition { get; }

        // Update
        public void Update(GameTime gameTime)
        {
            HealthMeter.Update(gameTime);

            if (cardFromBrain)
                Card?.Update(gameTime);
        }
    }
}
