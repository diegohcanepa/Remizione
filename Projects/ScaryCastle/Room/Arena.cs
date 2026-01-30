using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// Arena
    /// </summary>
    public sealed class Arena : GameRoom
    {
        #region Private fields

        private BattleState currentState;
        private Actor enemy = null!;
        private Card? enemyCard;
        private readonly Vector2 enemyCardSlotPosition = new(122, 6);
        private readonly UIArenaHealthDisplay enemyHealthMeter;
        private Card? hoveredCard;
        private Actor player = null!;
        private readonly Vector2 playerCardSlotPosition = new(118, 6);
        private readonly UIArenaHealthDisplay playerHealthMeter;
        private FacingDirection? previousEnemyDirection;
        private Vector2? previousEnemyPosition;
        private FacingDirection? previousPlayerDirection;
        private Vector2? previousPlayerPosition;
        private float stateDelayTimer;

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

        // CreateCardSlot
        private ImageSprite CreateCardSlot(float x, float y, RectanglePoint pivotOrigin = RectanglePoint.LeftTop)
        {
            return new ImageSprite(Game, Atlases.UI.CardSlot)
            {
                Opacity = 0,
                PivotOrigin = pivotOrigin,
                Position = new(x, y)
            };
        }

        // EndPlayerTurn
        private void EndPlayerTurn()
        {
            // 1. Descartar mano visualmente
            Session.Deck.DiscardHand();

            // 2. Cambiar a turno enemigo con PAUSA DRAMÁTICA (1.5s)
            // El jugador ve la carta flotando y espera el golpe.
            SetState(BattleState.EnemyTurn, 1.5f);
        }

        // GetHoveredCard
        private Card? GetHoveredCard()
        {
            // Revisamos las cartas de la mano (de atrás hacia adelante por el Z-order)
            for (var i = Session.Deck.DrawnCards.Count - 1; i >= 0; i--)
            {
                if (Session.Deck.DrawnCards[i] is Card card && card.IsMouseOver())
                    return card;
            }
            return null;
        }

        // HandleMouseInput
        private bool HandleMouseInput()
        {
            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
            {
                if (hoveredCard != null)
                {
                    MouseCursor.AnimateClick();
                    hoveredCard.Scale = 1;
                    hoveredCard.MoveTo(playerCardSlotPosition - new Vector2(hoveredCard.BoundingBox.Width, 0), 0, false);
                    Sound.Play(SoundNames.CardFlap);
                    SetState(BattleState.PlayPlayerCard, 0);
                    return true;
                }
            }
            return false;
        }

        // HoverCard
        private void HoverCard(Card? card)
        {
            // Resetear todos
            foreach (var c in Session.Deck.DrawnCards)
            {
                if (c != null)
                    c.IsHovered = false;
            }
            // Activar el actual
            if (card != null)
                card.IsHovered = true;
        }

        // PlayEnemyCard
        private void PlayEnemyCard()
        {
            // El Brain elige una carta y nos devuelve la instancia visual
            enemyCard = Session.Enemy?.Brain?.PickCard(this);

            if (enemyCard != null)
            {
                enemyCard.IsFaceVisible = false;
                enemyCard.Position = new Vector2(290, 80);
                enemyCard.MoveTo(enemyCardSlotPosition, 0, true);
            }
        }

        // PrepareOpponents
        private void PrepareOpponents()
        {
            previousEnemyDirection = enemy.Direction;
            previousEnemyPosition = enemy.Position;
            previousPlayerDirection = player.Direction;
            previousPlayerPosition = player.Position;

            Children.Add(enemy);
            enemy.Position = EnemyPosition;
            enemy.Direction = FacingDirection.Left;
            enemyHealthMeter.Prepare(enemy);

            Children.Add(player);
            player.Position = PlayerPosition;
            player.Direction = FacingDirection.Right;
            playerHealthMeter.Prepare(player);
        }

        // ResolveEnemyAction
        private void ResolveEnemyAction()
        {
            if (enemyCard != null)
            {
                enemyCard.Definition.Apply(enemy, player, false);
                enemyCard = null;
            }

            if (player.HP <= 0)
            {
                SetState(BattleState.Lose, 2.0f);
            }
            else if (enemy.HP <= 0)
            {
                SetState(BattleState.Win, 2.0f);
            }
            else
            {
                //?StartPlayerTurn();
            }
        }

        // RestoreOpponents
        private void RestoreOpponents()
        {
            if (!enemy.IsDead)
            {
                Session.PreviousRoom?.Children.Add(enemy);
                if (previousEnemyDirection.HasValue) enemy.Direction = previousEnemyDirection.Value;
                if (previousEnemyPosition.HasValue) enemy.Position = previousEnemyPosition.Value;
            }

            if (!player.IsDead)
            {
                Session.PreviousRoom?.Children.Add(player);
                if (previousPlayerDirection.HasValue) player.Direction = previousPlayerDirection.Value;
                if (previousPlayerPosition.HasValue) player.Position = previousPlayerPosition.Value;
            }
        }

        // SetState
        private void SetState(BattleState newState, float delayInSeconds)
        {
            currentState = newState;
            stateDelayTimer = delayInSeconds;
        }

        // StartBattle
        private void StartBattle()
        {
            Session.Deck.Shuffle();
            SetState(BattleState.PlayEnemyCard, 1);
        }

        // UpdateInputAndCursor
        private void UpdateInputAndCursor()
        {
            if (Session.Deck.IsBusy || currentState != BattleState.WaitPlayerInput)
            {
                MouseCursor.State = MouseCursorState.Wait;
                return;
            }

            MouseCursor.State = MouseCursorState.Hand;

            var c = GetHoveredCard();
            if (c == null)
            {
                if (hoveredCard != null) HoverCard(null); // Limpiar hover anterior
                hoveredCard = null;
            }
            else if (c != hoveredCard)
            {
                hoveredCard = c;
                HoverCard(c);
                Sound.Play(SoundNames.UISelectC);
            }
        }

        // UpdateStateMachine
        private void UpdateStateMachine()
        {
            switch (currentState)
            {
                // PlayEnemyCard
                case BattleState.PlayEnemyCard:
                    PlayEnemyCard();
                    SetState(BattleState.DrawHand, 1);
                    break;

                // DrawHand
                case BattleState.DrawHand:
                    Session.Deck.DrawHand();
                    SetState(BattleState.WaitPlayerInput, 3);
                    break;

                // PlayPlayerCard
                case BattleState.PlayPlayerCard:
                    if (hoveredCard?.IsMoving == false)
                        SetState(BattleState.PlayerTurn, 2f);
                    break;

                case BattleState.EnemyTurn:
                    ResolveEnemyAction();
                    break;

                case BattleState.Win:
                    // Lógica de victoria (volver al mapa, loot, etc)
                    // Session.GoBackToPreviousRoom();
                    break;

                case BattleState.Lose:
                    // Lógica de Game Over
                    break;
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
            enemyCard?.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            // Solo permitimos input si es el turno del jugador y no hay animaciones bloqueantes
            if (currentState == BattleState.WaitPlayerInput && stateDelayTimer <= 0 && !Session.Deck.IsBusy)
            {
                if (HandleMouseInput())
                    return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();

            enemy = Session.Enemy ?? throw new InvalidOperationException("No enemy found for Arena.");
            player = Session.Player ?? throw new InvalidOperationException("No player found for Arena.");

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

            if (stateDelayTimer > 0)
                stateDelayTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            Session.Deck.Update(gameTime);
            enemyCard?.Update(gameTime);

            UpdateInputAndCursor();

            enemyHealthMeter.Update(gameTime);
            playerHealthMeter.Update(gameTime);

            if (stateDelayTimer <= 0)
                UpdateStateMachine();
        }

        #endregion

        [ScriptProperty]
        public Vector2 EnemyPosition { get; set; }

        [ScriptProperty]
        public Vector2 PlayerPosition { get; set; }
    }
}