using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using ScaryCastle.UI;

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
        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
            
            Game.SpriteBatch.Begin(Game.Camera);
            Session.Deck.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            return HandleInputResult.Handled;
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();

            Session.Deck.Shuffle();
            Session.Deck.DrawHand();

            if (Session.Player is Actor player)
            {
                Children.Add(player);
                player.Position = PlayerPosition;
                player.Direction = FacingDirection.Right;
            }

            if (Session.Enemy is Actor enemy)
            {
                Children.Add(enemy);
                enemy.Position = EnemyPosition;
                enemy.Direction = FacingDirection.Left; 
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            Session.Deck.Update(gameTime);
        }

        #endregion

        // EnemyPosition
        [ScriptProperty]
        public Vector2 EnemyPosition { get; set; }

        // PlayerPosition
        [ScriptProperty]
        public Vector2 PlayerPosition { get; set; }
    }
}