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
        private readonly List<Card> discardPile = [];
        private readonly List<Card?> drawnCards = [];
        private readonly List<Card> drawPile = [];
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
            drawnCards.Clear();
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
            drawnCards.Remove(card);
            Invalidate();
        }

        #endregion

        // Capacity
        public int Capacity { get; set; } = 15;

        // ContentVersion
        public int ContentVersion { get; private set; }

        // DrawCards
        public void DrawCards(int amount)
        {
            for (var i = 0; i < drawPile.Count; i++)
            {
                if (drawPile.Count == 0)
                {
                    if (discardPile.Count == 0)
                        break;

                    Shuffle();
                }

                var card = drawPile[0];
                drawPile.RemoveAt(0);
                drawnCards.Add(card);
            }

            Invalidate();
        }

        // Discard
        public bool Discard(int index)
        {
            if (drawnCards[index] is not Card card)
                return false;

            if (!discardPile.Contains(card))
            {
                if (drawnCards.Remove(card))
                {
                    discardPile.Add(card);
                    card.MoveTo(nextDiscardPosition, 100, true);
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
                    card.IsFaceVisible = false;
                }
            }

            drawnCards.Clear();
            Invalidate();
        }

        // DiscardPile
        public ReadOnlyCollection<Card> DiscardPile { get; }

        // Draw
        public void Draw(GameTime gameTime)
        {
            // Draw pile (reverse)
            for (var i = DrawPile.Count - 1; i >= 0 ; i--)
            {
                cardShadow.Position = new Vector2(DrawPile[i].Position.X - .25f, DrawPile[i].Position.Y);
                cardShadow.Draw(gameTime);
                DrawPile[i].Draw(gameTime);
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
            if (drawnCards.Count > 0)
                DiscardHand();

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

                // B. Robar la primera carta
                Card card = drawPile[0];
                drawPile.RemoveAt(0);

                // C. Agregar a la mano (drawnCards)
                drawnCards.Add(card);

                card.MoveTo(slotPositions[HandSize-1-i], 500 * i + 1, true);
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

        // HandSize
        public const int HandSize = 3;

        // Invalidate
        public void Invalidate()
        {
            unchecked { ContentVersion++; }
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
            if (drawnCards.Remove(card))
            {
                drawnCards.Remove(card);
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

            drawnCards.Clear();
            discardPile.Clear();
            drawPile.Clear();
            drawPile.AddRange(this);
            drawPile.Shuffle();

            var offset = 0f;
            for (var i = drawPile.Count - 1; i >= 0; i--)
            {
                drawPile[i].IsFaceVisible = false;
                drawPile[i].Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, 5 + offset, -drawPile[i].BoundingBox.Height);
                offset += cardOffset;
            }

            nextDiscardPosition = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, -5, -drawPile[0].BoundingBox.Height);

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
        }
    }
}
