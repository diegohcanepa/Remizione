using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// Arena
    /// </summary>
    public sealed class Arena : GameRoom
    {
        // Constructor
        public Arena(GameSession session, string name)
            : base(session, name)
        {
        }

        #region Private members

        // PrepareEnemyTurn
        private void PrepareEnemyTurn()
        {
        }

        // StartBattle
        private void StartBattle()
        {
            // Reset deck
            Session.Deck.Shuffle();

            PrepareEnemyTurn();
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            StartBattle();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
        }

        #endregion

        public Actor? Enemy { get; set; }
    }
}