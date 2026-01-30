using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScaryCastle.UI;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// Arena: Implementación con Patrón State nativo (sin librería externa).
    /// </summary>
    public sealed class Arena : GameRoom
    {
        #region Private fields

        private ArenaState currentState = null!;
        private LargeHand devilHand = null!;
        private readonly Vector2 enemyCardSlotPosition = new(122, 6);
        private UIArenaHealthMeter enemyHealthMeter = null!;
        private LargeHand godHand = null!;
        private readonly Vector2 playerCardSlotPosition = new(118, 6);
        private UIArenaHealthMeter playerHealthMeter = null!;
        private FacingDirection? previousEnemyDirection;
        private Vector2? previousEnemyPosition;
        private FacingDirection? previousPlayerDirection;
        private Vector2? previousPlayerPosition;

        #endregion

        #region Constructor

        // Constructor
        public Arena(GameSession session, string name)
            : base(session, name)
        {
        }

        #endregion

        #region Private members

        // BeginBattle
        private void BeginBattle()
        {
            Enemy = Session.Enemy ?? throw new InvalidOperationException("No enemy found for Arena.");
            Player = Session.Player ?? throw new InvalidOperationException("No player found for Arena.");

            Enemy.Scale += new Vector2(.2f);
            Player.Scale += new Vector2(.2f);

            Session.Deck.Shuffle();

            // Preserve position and direction in previous room
            previousEnemyDirection = Enemy.Direction;
            previousEnemyPosition = Enemy.Position;
            previousPlayerDirection = Player.Direction;
            previousPlayerPosition = Player.Position;

            // Add enemy
            Children.Add(Enemy);
            Enemy.Position = EnemyPosition;
            Enemy.Direction = FacingDirection.Left;

            // Add player
            Children.Add(Player);
            Player.Position = PlayerPosition;
            Player.Direction = FacingDirection.Right;

            enemyHealthMeter = new(Enemy);
            playerHealthMeter = new(Player);

            devilHand = new(Game, LargHandStyle.Devil, Enemy, Player, playerHealthMeter);
            godHand = new(Game, LargHandStyle.God, Player, Enemy, enemyHealthMeter);

            TransitionTo(new EnemyTurnState(this));
        }

        // EndBattle
        private void EndBattle()
        {
            if (!Enemy.IsDead)
            {
                Session.PreviousRoom?.Children.Add(Enemy);
                if (previousEnemyDirection.HasValue) Enemy.Direction = previousEnemyDirection.Value;
                if (previousEnemyPosition.HasValue) Enemy.Position = previousEnemyPosition.Value;
            }

            if (!Player.IsDead)
            {
                Session.PreviousRoom?.Children.Add(Player);
                if (previousPlayerDirection.HasValue) Player.Direction = previousPlayerDirection.Value;
                if (previousPlayerPosition.HasValue) Player.Position = previousPlayerPosition.Value;
            }

            Enemy.Scale -= new Vector2(.2f);
            Player.Scale -= new Vector2(.2f);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            Game.SpriteBatch.Begin(Game.Camera);

            Session.Deck.Draw(gameTime);
            EnemyCard?.Draw(gameTime);

            if (SpeechBubble.ModalInstance?.Actor != Enemy)
                enemyHealthMeter.Draw(gameTime);

            if (SpeechBubble.ModalInstance?.Actor != Player)
                playerHealthMeter.Draw(gameTime);

            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            devilHand.Draw(gameTime);
            godHand.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            return currentState != null ? currentState.HandleInput(gameTime) : HandleInputResult.Unhandled;
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            BeginBattle();
        }

        // OnUnload
        protected override void OnUnload()
        {
            base.OnUnload();
            EndBattle();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            devilHand.Update(gameTime);
            godHand.Update(gameTime);

            Session.Deck.Update(gameTime);
            EnemyCard?.Update(gameTime);

            enemyHealthMeter.Update(gameTime);
            playerHealthMeter.Update(gameTime);

            currentState?.Update(gameTime);

            if (SpeechBubble.ModalInstance != null)
                MouseCursor.State = MouseCursorState.Arrow;
            else
                MouseCursor.State = currentState is PlayerInputState ? MouseCursorState.Hand : MouseCursorState.Wait;
        }

        #endregion

        // Enemy
        public Actor Enemy { get; private set; } = null!;

        // EnemyCard
        public Card? EnemyCard { get; set; }

        // EnemyPosition
        [ScriptProperty]
        public Vector2 EnemyPosition { get; set; }

        // HitEnemy
        public void HitEnemy()
        {
            if (PlayerCard != null)
                godHand.Hit(PlayerCard);
        }

        // HitPlayer
        public void HitPlayer()
        {
            if (EnemyCard != null)
                devilHand.Hit(EnemyCard);
        }

        // Player
        public Actor Player { get; private set; } = null!;

        // PlayerCard
        public Card? PlayerCard { get; set; }

        // PlayEnemyCard
        public void PlayEnemyCard()
        {
            if (Enemy.Brain.PickCard(this) is Card card)
            {
                this.EnemyCard = card;

                EnemyCard.IsFaceUp = false;
                EnemyCard.Position = Enemy.RuntimeHotspot.BoundingRectangleF.Center;
                EnemyCard.MoveTo(enemyCardSlotPosition, 500, 0, true, true);
                EnemyCard.Float = true;
            }
        }

        // PlayPlayerCard
        public void PlayPlayerCard(Card card)
        {
            this.PlayerCard = card;
            card.MoveTo(playerCardSlotPosition - new Vector2(card.BoundingBox.Width, 0), 400, 0, false);
            card.Float = true;
        }

        // PlayerPosition
        [ScriptProperty]
        public Vector2 PlayerPosition { get; set; }

        // TransitionTo
        public void TransitionTo(ArenaState newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState.Enter();
        }
    }
}