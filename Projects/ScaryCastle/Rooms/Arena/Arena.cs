using Adberration;
using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;
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
        private readonly Vector2 enemyCardSlotPosition = new(122, 6);
        private readonly UIArenaHealthMeter enemyHealthMeter;
        private readonly UIArenaHealthMeter playerHealthMeter;
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
            this.enemyHealthMeter = new(Game);
            this.playerHealthMeter = new(Game);
        }

        #endregion

        #region Private members

        // BeginBattle
        private void BeginBattle()
        {
            Enemy = Session.Enemy ?? throw new InvalidOperationException("No enemy found for Arena.");
            Player = Session.Player ?? throw new InvalidOperationException("No player found for Arena.");

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
            enemyHealthMeter.Prepare(Enemy);

            // Add player
            Children.Add(Player);
            Player.Position = PlayerPosition;
            Player.Direction = FacingDirection.Right;
            playerHealthMeter.Prepare(Player);

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
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            Game.SpriteBatch.Begin(Game.Camera);

            enemyHealthMeter.Draw(gameTime);
            playerHealthMeter.Draw(gameTime);

            Session.Deck.Draw(gameTime);
            EnemyCard?.Draw(gameTime);

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

            Session.Deck.Update(gameTime);
            EnemyCard?.Update(gameTime);
            enemyHealthMeter.Update(gameTime);
            playerHealthMeter.Update(gameTime);

            currentState?.Update(gameTime);

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

        // HoveredCard
        public Card? HoveredCard { get; set; }

        // Player
        public Actor Player { get; private set; } = null!;

        // PlayerCardSlotPosition
        public Vector2 PlayerCardSlotPosition { get; } = new(118, 6);

        // PlayEnemyCard
        public void PlayEnemyCard()
        {
            if (Enemy.Brain.PickCard(this) is Card card)
            {
                this.EnemyCard = card;

                EnemyCard.IsFaceVisible = false;
                EnemyCard.Position = new Vector2(290, 80);
                EnemyCard.MoveTo(enemyCardSlotPosition, 0, true);
                EnemyCard.Float = true;
            }
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