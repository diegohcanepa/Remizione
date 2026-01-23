using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// Card
    /// </summary>
    public sealed class Card : GameObject
    {
        private readonly ImageSprite categoryIcon;
        private readonly ImageSprite cardContainer;

        // Constructor
        public Card(Deck deck, CardDefinition definition)
            : base(deck.Session.Game)
        {
            this.Deck = deck;
            this.Definition = definition;

            // Card container
            this.cardContainer = new(Game)
            {
            };

            // Category icon
            this.categoryIcon = new(Game, Definition.CategoryImage)
            {
                PivotOrigin = RectanglePoint.Center
            };

            Refresh();
        }

        #region Private members

        // Refresh
        private void Refresh()
        {
            cardContainer.Image = IsFaceVisible ? Definition.FrontImage : Definition.BackImage;
            
            if (!IsFaceVisible)
                return;

            categoryIcon.Position = cardContainer.BoundingBox.GetPoint(RectanglePoint.Top, 0, 1);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            cardContainer.Draw(gameTime);
            categoryIcon.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            cardContainer.Update(gameTime);
        }

        #endregion

        // Deck
        public Deck Deck { get; }

        // Definition
        public CardDefinition Definition { get; }

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
    }
}
