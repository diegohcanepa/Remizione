using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ScaryCastle
{
    /// <summary>
    /// Deck
    /// </summary>
    public sealed class Deck : Collection<Card>
    {
        #region Private fields

        private const float cardOffset = .75f;
        private readonly ImageSprite cardShadow;
        private readonly TextSprite discardPileAmount;
        private readonly ImageSprite discardPileAmountContainer;
        private readonly TextSprite drawPileAmount;
        private readonly ImageSprite drawPileAmountContainer;
        private readonly List<Card> discardPile = [];
        private readonly Card?[] drawnCards = [null, null, null];
        private readonly List<Card> drawPile = [];
        private int lastDiscardPileKnownAmount = -1;
        private int lastDrawPileKnownAmount = -1;
        private Vector2 nextDiscardPosition;
        private readonly Vector2[] slotPositions = new Vector2[HandSize];

        #endregion

        #region Constructor

        // Constructor
        public Deck(GameSession session)
            : base()
        {
            this.Session = session;
            this.DiscardPile = discardPile.AsReadOnly();
            this.DrawPile = drawPile.AsReadOnly();
            this.DrawnCards = drawnCards.AsReadOnly();

            // Card shadow
            this.cardShadow = new ImageSprite(session.Game, Atlases.UI.CardBack)
            {
                Color = Color.Black,
                Opacity = ColorPalette.ShadowOpacity
            };

            // Discard pile Count container
            this.discardPileAmountContainer = new ImageSprite(session.Game, Atlases.UI.GetImage("DeckCountContainer"))
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightTop, -4, 92)
            };

            // Discard pile amount text
            this.discardPileAmount = new TextSprite(session.Game, Fonts.Common)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Center,
                Position = discardPileAmountContainer.BoundingBox.GetPoint(RectanglePoint.Center, 0, .5f),
                Scale = ScaleInfo.Text.Giant
            };

            // Draw pile Count container
            this.drawPileAmountContainer = new ImageSprite(session.Game, Atlases.UI.GetImage("DeckCountContainer"))
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftTop, 4, 92)
            };

            // Draw pile amount text
            this.drawPileAmount = new TextSprite(session.Game, Fonts.Common)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Center,
                Position = drawPileAmountContainer.BoundingBox.GetPoint(RectanglePoint.Center, 0, .5f),
                Scale = ScaleInfo.Text.Giant
            };

            Add(new Card(session.Game, "CardTest3"));
            Add(new Card(session.Game, "CardTest"));
            Add(new Card(session.Game, "CardTest3"));
            Add(new Card(session.Game, "CardTest"));
            Add(new Card(session.Game, "CardTest3"));
            Add(new Card(session.Game, "CardTest2"));
            Add(new Card(session.Game, "CardTest2"));
            Add(new Card(session.Game, "CardTest2"));
            Add(new Card(session.Game, "CardTest"));
            Add(new Card(session.Game, "CardTest"));


            Shuffle();
        }

        #endregion

        #region Private members

        // RecycleDiscardToDraw
        private void RecycleDiscardToDraw()
        {
            drawPile.AddRange(discardPile);
            discardPile.Clear();
            drawPile.Shuffle();
        }

        // SetupSlotPositions
        private void SetupSlotPositions()
        {
            // Pre-calculate drawn card positions
            if (slotPositions[0] == Vector2.Zero)
            {
                float screenWidth = Screen.NativeWidth;
                float slotWidth = this[0].BoundingBox.Width;
                float spacing = 3;
                float rowWidth = (HandSize * slotWidth) + ((HandSize - 1) * spacing);
                float startingX = (screenWidth - rowWidth) / 2;

                for (int i = 0; i < Deck.HandSize; i++)
                {
                    slotPositions[i] = new Vector2(startingX + (i * (slotWidth + spacing)), Screen.Area.Bottom - this[0].BoundingBox.Height - 5);
                }
            }
        }

        #endregion

        #region Protected members

        // ClearItems
        protected override void ClearItems()
        {
            base.ClearItems();
            discardPile.Clear();
            drawPile.Clear();

            for (var i = 0; i < drawnCards.Length; i++)
            {
                drawnCards[i] = null;
            }

            Invalidate();
        }

        // InsertItem
        protected override void InsertItem(int index, Card item)
        {
            if (IsFull)
                throw new InvalidOperationException("Deck is full.");

            base.InsertItem(index, item);

            drawPile.Add(item);

            Invalidate();
        }

        // RemoveItem
        protected override void RemoveItem(int index)
        {
            var card = this[index];
            base.RemoveItem(index);
            discardPile.Remove(card);
            drawPile.Remove(card);

            for (var i = 0; i < drawnCards.Length; i++)
            {
                if (drawnCards[i] == card)
                    drawnCards[i] = null;
            }
            
            Invalidate();
        }

        #endregion

        // Capacity
        public int Capacity { get; set; } = 15;

        // ContentVersion
        public int ContentVersion { get; private set; }

        // Discard
        public bool Discard(int index)
        {
            if (drawnCards[index] is Card card)
                return Discard(card);
            else
                return false;
        }

        // Discard
        public bool Discard(Card card)
        {
            if (!discardPile.Contains(card))
            {
                var index = drawnCards.IndexOf(card);

                if (index >= 0)
                {
                    drawnCards[index] = null;
                    discardPile.Add(card);
                    card.Float = false;
                    card.MoveTo(nextDiscardPosition, 300, 0, true);
                    nextDiscardPosition.X -= cardOffset;

                    Invalidate();
                    return true;
                }
            }

            return false;
        }

        // DiscardHand
        public void DiscardHand()
        {
            foreach (var card in drawnCards)
            {
                if (card != null)
                {
                    discardPile.Add(card);
                    card.IsFaceUp = false;
                }
            }

            for (var i = 0; i < drawnCards.Length; i++)
            {
                drawnCards[i] = null;
            }

            Invalidate();
        }

        // DiscardPile
        public ReadOnlyCollection<Card> DiscardPile { get; }

        // Draw
        public void Draw(GameTime gameTime)
        {
            // Draw pile (reverse)
            for (var i = DrawPile.Count - 1; i >= 0; i--)
            {
                cardShadow.Position = new Vector2(DrawPile[i].Position.X - .25f, DrawPile[i].Position.Y);
                cardShadow.Draw(gameTime);
                DrawPile[i].Draw(gameTime);
            }

            if (drawPile.Count > 0)
            {
                drawPileAmountContainer.Draw(gameTime);
                drawPileAmount.Draw(gameTime);
            }

            if (discardPile.Count > 0)
            {
                discardPileAmountContainer.Draw(gameTime);
                discardPileAmount.Draw(gameTime);
            }

            // Discard pile
            for (var i = 0; i < DiscardPile.Count; i++)
            {
                DiscardPile[i].Draw(gameTime);
            }

            // Drawn cards
            for (var i = DrawnCards.Count - 1; i >= 0; i--)
            {
                DrawnCards[i]?.Draw(gameTime);
            }
        }

        // DrawnCards
        public ReadOnlyCollection<Card?> DrawnCards { get; }

        // DrawHand
        public void DrawHand()
        {
            // Asegurarnos de que la mano anterior esté limpia (por seguridad)
            //if (drawnCards.Length > 0)
              //  DiscardHand();

            for (int i = 0; i < HandSize; i++)
            {
                // A. Si no quedan cartas en el mazo de robo...
                if (drawPile.Count == 0)
                {
                    // Si tampoco hay en el descarte, no hay nada más que robar.
                    if (discardPile.Count == 0)
                        break;

                    // Reciclaje: Mover descarte a robo y barajar
                    RecycleDiscardToDraw();
                }

                if (drawnCards[i] == null)
                {
                    Card card = drawPile[0];
                    drawPile.RemoveAt(0);
                    drawnCards[i] = card;
                    card.MoveTo(slotPositions[HandSize - 1 - i] - new Vector2(0, 2), 400, (500 * i) + 1, true);
                }
            }

            Invalidate();
        }

        // DrawPile
        public ReadOnlyCollection<Card> DrawPile { get; }

        // Find
        public Card? Find(string name)
        {
            for (var i = 0; i < Count; i++)
            {
                if (this[i].Name == name)
                    return this[i];
            }

            return null;
        }

        // Get
        public Card Get(string name)
        {
            return Find(name) ?? throw new InvalidOperationException($"Card '{name}' not found.");
        }

        // GetSlotPosition
        public Vector2 GetSlotPosition(int index)
        {
            return slotPositions[index];
        }

        // HandSize
        public const int HandSize = 3;

        // Invalidate
        public void Invalidate()
        {
            unchecked { ContentVersion++; }
        }

        // IsBusy
        public bool IsBusy
        {
            get
            {
                for (var i = 0; i < drawnCards.Length; i++)
                {
                    if (drawnCards[i] is Card card && card.IsMoving)
                        return true;
                }

                return false;
            }
        }

        // IsEmpty
        public bool IsEmpty => Count == 0;

        // IsFull
        public bool IsFull => Count == Capacity;

        // LoadState
        public void LoadState(string data)
        {
            void AddCards(IEnumerable<string> cardNames, IList<Card> targetList, bool linkCards)
            {
                targetList.Clear();

                foreach (var cardName in cardNames)
                {
                    if (linkCards)
                    {
                        if (Find(cardName) is Card existingCard)
                            targetList.Add(existingCard);
                    }
                    else
                    {
                        targetList.Add(new Card(Session.Game, cardName));
                    }
                }
            }

            Clear();

            if (string.IsNullOrEmpty(data))
                return;

            var cardGroups = data.Split(';');

            if (cardGroups.Length > 0)
            {
                // Deck
                AddCards(cardGroups[0].Split(','), this, false);

                // Draw pile
                if (cardGroups.Length > 1)
                    AddCards(cardGroups[1].Split(','), drawPile, true);

                // Discard pile
                if (cardGroups.Length > 2)
                    AddCards(cardGroups[2].Split(','), discardPile, true);
            }
        }

        // PlayCard
        public void PlayCard(Card card)
        {
            var index = drawnCards.IndexOf(card);
            if (index >= 0)
            {
                drawnCards[index] = null;
                discardPile.Add(card);
                Invalidate();
            }
        }

        // Remove
        public bool Remove(string name)
        {
            return Find(name) is Card card && Remove(card);
        }

        // SaveState
        public string SaveState()
        {
            string Serialize(IEnumerable<Card> cards)
            {
                var result = new List<string>();

                foreach (var card in cards)
                {
                    result.Add(card.Name);
                }

                return string.Join(",", result);
            }

            var result = new List<string>
            {
                Serialize(this),
                Serialize(drawPile),
                Serialize(discardPile)
            };

            return string.Join(";", result);
        }

        // Session
        public GameSession Session { get; }

        // Shuffle
        public void Shuffle()
        {
            if (Count == 0)
                return;

            for (var i = 0; i < drawnCards.Length; i++)
            {
                drawnCards[i] = null;
            }

            discardPile.Clear();
            drawPile.Clear();
            drawPile.AddRange(this);
            drawPile.Shuffle();

            var offset = 0f;
            for (var i = drawPile.Count - 1; i >= 0; i--)
            {
                drawPile[i].IsFaceUp = false;
                drawPile[i].Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, 5 + offset, -drawPile[i].BoundingBox.Height - 3);
                offset += cardOffset;
            }

            nextDiscardPosition = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, -25, -drawPile[0].BoundingBox.Height - 3);

            SetupSlotPositions();

            Invalidate();
        }

        // Update
        public void Update(GameTime gameTime)
        {
            for (var i = 0; i < Count; i++)
            {
                this[i].Update(gameTime);
            }

            if (lastDiscardPileKnownAmount != discardPile.Count)
            {
                lastDiscardPileKnownAmount = discardPile.Count;
                discardPileAmount.Text = lastDiscardPileKnownAmount.ToString();
            }

            if (lastDrawPileKnownAmount != drawPile.Count)
            {
                lastDrawPileKnownAmount = drawPile.Count;
                drawPileAmount.Text = lastDrawPileKnownAmount.ToString();
            }
        }
    }
}
