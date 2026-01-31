using Adberration;
using Adberration.Scripting;
using Engendro;
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
        private LargeHand godHand = null!;

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
            if (Session.Enemy == null)
                throw new InvalidOperationException("No enemy found.");
            else
                EnemyInfo = new(this, Session.Enemy, EnemyPosition, FacingDirection.Left, new(122, 6));

            if (Session.Player == null)
                throw new InvalidOperationException("No player found.");
            else
                PlayerInfo = new(this, Session.Player, PlayerPosition, FacingDirection.Right, new(118, 6));

            Session.Deck.Shuffle();

            devilHand = new(Game, LargHandStyle.Devil, EnemyInfo, PlayerInfo);
            godHand = new(Game, LargHandStyle.God, PlayerInfo, EnemyInfo);

            TransitionTo(new EnemyTurnState(this));
        }

        // EndBattle
        private void EndBattle()
        {
            EnemyInfo.BringBackToPreviousRoom();
            PlayerInfo.BringBackToPreviousRoom();
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            Game.SpriteBatch.Begin(Game.Camera);

            Session.Deck.Draw(gameTime);
            EnemyInfo.Draw(gameTime);
            PlayerInfo.Draw(gameTime);
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

            EnemyInfo.Update(gameTime);
            PlayerInfo.Update(gameTime);

            currentState?.Update(gameTime);

            if (SpeechBubble.ModalInstance != null)
                MouseCursor.State = MouseCursorState.Arrow;
            else
                MouseCursor.State = currentState is PlayerTurnState ? MouseCursorState.Hand : MouseCursorState.Wait;
        }

        #endregion

        // EnemyInfo
        public FighterInfo EnemyInfo { get; private set; } = null!;

        // EnemyDiceRoll
        public int EnemyDiceRoll { get; set; }

        // EnemyPosition
        [ScriptProperty]
        public Vector2 EnemyPosition { get; set; }

        // HitEnemy
        public void HitEnemy()
        {
            godHand.Hit();
        }

        // HitPlayer
        public void HitPlayer()
        {
            devilHand.Hit();
        }

        // PlayerInfo
        public FighterInfo PlayerInfo { get; private set; } = null!;

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