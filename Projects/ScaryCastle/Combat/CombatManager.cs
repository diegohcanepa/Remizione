using Adberration;
using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// CombatManager
    /// </summary>
    public sealed class CombatManager : GameObject, IInputHandler
    {
        #region Private fields

        private CombatManagerState currentState = null!;
        private readonly UIArenaHealthMeter enemyMeter;
        private readonly UIArenaHealthMeter playerMeter;
        private readonly FacingDirection originalEnemyDirection;
        private Vector2 originalEnemyPosition;
        private readonly FacingDirection originalPlayerDirection;
        private Vector2 originalPlayerPosition;

        #endregion

        #region Constructor

        // Constructor
        public CombatManager(Actor player, Actor enemy)
            : base(player.Game)
        {
            this.Session = player.Session;
            this.Player = player;
            this.Enemy = enemy;

            originalEnemyDirection = enemy.Direction;
            originalEnemyPosition = enemy.Position;
            originalPlayerDirection = enemy.Direction;
            originalPlayerPosition = enemy.Position;

            if (Session.Room is Arena arena)
            {
                var playerAtLeft = Player.X < Enemy.X;
                arena.Children.Add(Enemy);
                arena.Children.Add(Player);

                var leftPos = 80;
                var rightPos = 160;
                var y = 92;

                if (playerAtLeft)
                {
                    Player.Position = new(leftPos, y);
                    Enemy.Position = new(rightPos, y);
                    Player.Direction = FacingDirection.Right;
                    Enemy.Direction = FacingDirection.Left;
                }
                else
                {
                    Enemy.Position = new(leftPos, y);
                    Player.Position = new(rightPos, y);
                    Player.Direction = FacingDirection.Left;
                    Enemy.Direction = FacingDirection.Right;
                }
            }

            RepositionActor(Enemy);
            RepositionActor(Player);

            playerMeter = new(Player);
            enemyMeter = new(Enemy);

            TransitionTo(new PlayerTurnState(this));
        }

        #endregion

        #region Private members

        // RepositionActor
        private static void RepositionActor(Actor actor)
        {
            float offset;
            if (actor.Direction == FacingDirection.Left)
                offset = actor.RuntimeHotspot.BoundingRectangleF.Left - actor.X;
            else
                offset = actor.RuntimeHotspot.BoundingRectangleF.Right - actor.X;

            actor.X += offset;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Session.Camera);
            currentState.Draw(gameTime);

            if (SpeechBubble.ModalInstance == null || SpeechBubble.ModalInstance.Actor != Enemy)
                enemyMeter.Draw(gameTime);

            if (SpeechBubble.ModalInstance == null || SpeechBubble.ModalInstance.Actor != Player)
                playerMeter.Draw(gameTime);


            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            currentState?.Update(gameTime);
            enemyMeter.Update(gameTime);
            playerMeter.Update(gameTime);
        }

        #endregion

        // CanSuspend
        public bool CanSuspend { get; set; } = true;

        // Enemy
        public Actor Enemy { get; }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (currentState == null)
                return HandleInputResult.Unhandled;
            else
                return currentState.HandleInput(gameTime);
        }

        // IsWaitingPlayerInput
        public bool IsWaitingPlayerInput => currentState is PlayerTurnState;

        // Player
        public Actor Player { get; }

        // Session
        public GameSession Session { get; }

        // Terminate
        public void Terminate()
        {
            if (Session.PreviousRoom is GameRoom room)
            {
                room.Children.Add(Player);
                room.Children.Add(Enemy);

                Player.Position = originalPlayerPosition;
                Player.Direction = originalPlayerDirection;
                Enemy.Position = originalEnemyPosition;
                Enemy.Direction = originalEnemyDirection;
            }
        }

        // TransitionTo
        public void TransitionTo(CombatManagerState newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState.Enter();
        }
    }
}