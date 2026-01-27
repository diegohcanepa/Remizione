using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScaryCastle.UI
{
    /// <summary>
    /// UIDeck
    /// </summary>
    public sealed class UIDeck : GameObject
    {
        private readonly ImageSprite cardBack;
        private readonly ImageSprite cardShadow;
        private readonly Deck deck;

        // Constructor
        public UIDeck(Deck deck)
            : base(deck.Session.Game)
        {
            this.deck = deck;

            this.cardBack = new(Game, Atlases.UI.CardBack)
            {
            };

            this.cardShadow = new(Game, Atlases.UI.CardBack)
            {
                Color = Color.Black * ColorPalette.ShadowOpacity
            };
        }

        #region Private members

        // RenderDiscardPile
        private void RenderDiscardPile(GameTime gameTime)
        {
            if (deck.DiscardPile.Count == 0)
                return;

            cardBack.Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, -5, -cardBack.BoundingBox.Height);
            cardShadow.Position = cardBack.Position;
            cardShadow.X -= .5f;

            for (var i = 0; i < deck.DrawPile.Count; i++)
            {
                cardBack.Draw(gameTime);
                cardShadow.Draw(gameTime);
                cardBack.X += 1.25f;
                cardShadow.X += 1.25f;
            }
        }

        // RenderDrawPile
        private void RenderDrawPile(GameTime gameTime)
        {
            if (deck.DrawPile.Count == 0)
                return;

            cardBack.Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, 5, - cardBack.BoundingBox.Height);
            cardShadow.Position = cardBack.Position;
            cardShadow.X -= .5f;
         
            for (var i = 0; i < deck.DrawPile.Count; i++)
            {
                cardBack.Draw(gameTime);
                cardShadow.Draw(gameTime);
                cardBack.X += 1.25f;
                cardShadow.X += 1.25f;
            }
        }

        // RenderRevealedCards
        private void RenderRevealedCards(GameTime gameTime)
        {
            if (deck.DrawnCards.Count == 0)
                return;
        }

        #endregion

            #region Protected members

            // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            RenderDrawPile(gameTime);
            RenderRevealedCards(gameTime);
            RenderDiscardPile(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
        }

        #endregion
    }
}
