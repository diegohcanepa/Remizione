using Adberration;
using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;
using ScaryCastle.Battle;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// Arena
    /// </summary>
    public sealed class Arena : GameRoom
    {
        private BattleState currentState = BattleState.Intro;
        private Actor enemy = null!;
        private Intent? pendingEnemyIntent;
        private Actor player = null!;
        private FacingDirection? previousEnemyDirection;
        private Vector2? previousEnemyPosition;
        private FacingDirection? previousPlayerDirection;
        private Vector2? previousPlayerPosition;
        private float stateTimer = 0f;

        // Constructor
        public Arena(GameSession session, string name)
            : base(session, name)
        {
        }

        #region Private members

        // ApplyIntent
        private void ApplyIntent(Intent intent, Actor source, Actor target)
        {
            switch (intent.IntentType)
            {
                case IntentType.Attack:
                    target.HP -= intent.Value;
                    break;
            }
        }

        // EndPlayerTurn
        private void EndPlayerTurn()
        {
            if (currentState != BattleState.PlayerTurn) return;

            // Descartar la mano actual
            Session.Deck.DiscardHand();

            // Pasamos a turno enemigo (con un pequeño delay para drama)
            currentState = BattleState.EnemyTurn;
            stateTimer = 1.0f; // 1 segundo de espera
        }

        // PrepareOpponents
        private void PrepareOpponents()
        {
            // Enemy
            previousEnemyDirection = enemy.Direction;
            previousEnemyPosition = enemy.Position;
            Children.Add(enemy);
            enemy.Position = EnemyPosition;
            enemy.Direction = FacingDirection.Left;

            // Player
            previousPlayerDirection = player.Direction;
            previousPlayerPosition = player.Position;
            Children.Add(player);
            player.Position = PlayerPosition;
            player.Direction = FacingDirection.Right;
        }

        // ResolveEnemyAction
        private void ResolveEnemyAction()
        {
            // Ejecutar la intención que pensó al inicio del turno
            if (pendingEnemyIntent != null)
            {
                ApplyIntent(pendingEnemyIntent, enemy, player);
                pendingEnemyIntent = null;
            }

            // Chequear condiciones de victoria/derrota
            if (player.HP <= 0)
            {
                currentState = BattleState.Lose;
            }
            else if (enemy.HP <= 0)
            {
                currentState = BattleState.Win;
            }
            else
            {
                // Si nadie murió, vuelve a jugar el player
                StartPlayerTurn();
            }
        }

        // ResolveTurn
        private void ResolveTurn()
        {
            if (pendingEnemyIntent != null)
                ApplyIntent(pendingEnemyIntent, enemy, player);

            pendingEnemyIntent = null;
        }

        // RestoreOpponents
        private void RestoreOpponents()
        {
            // Restore enemy
            if (!enemy.IsDead)
            {
                Session.PreviousRoom?.Children.Add(enemy);

                if (previousEnemyDirection != null)
                    enemy.Direction = previousEnemyDirection.Value;

                if (previousEnemyPosition != null)
                    enemy.Position = previousEnemyPosition.Value;
            }

            // Restore player
            if (!player.IsDead)
            {
                Session.PreviousRoom?.Children.Add(player);

                if (previousPlayerDirection != null)
                    player.Direction = previousPlayerDirection.Value;

                if (previousPlayerPosition != null)
                    player.Position = previousPlayerPosition.Value;
            }
        }

        // StartBattle
        private void StartBattle()
        {
            Session.Deck.Shuffle();
            StartPlayerTurn();
        }

        // StartEnemyTurn
        private void StartEnemyTurn()
        {
            pendingEnemyIntent = Session.Enemy?.Brain?.DecideIntent();
        }

        // StartPlayerTurn
        private void StartPlayerTurn()
        {
            pendingEnemyIntent = enemy.Brain?.DecideIntent();

            Session.Deck.DrawHand();

            currentState = BattleState.PlayerTurn;
        }

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

            enemy = Session.Enemy ?? throw new InvalidOperationException();
            player = Session.Player ?? throw new InvalidOperationException();
            PrepareOpponents();

            StartBattle();
        }

        // OnUnload
        protected override void OnUnload()
        {
            base.OnUnload();

            RestoreOpponents();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            Session.Deck.Update(gameTime);

            // Máquina de estados
            switch (currentState)
            {
                case BattleState.EnemyTurn:
                    stateTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (stateTimer <= 0)
                    {
                        ResolveEnemyAction();
                    }
                    break;

                    // Aquí podrías agregar lógica para animaciones de victoria/derrota
            }
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