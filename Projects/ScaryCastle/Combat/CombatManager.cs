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
        private CombatManagerState currentState = null!;
        private UIArenaHealthMeter enemyMeter;
        private FacingDirection originalEnemyDirection;
        private Vector2 originalEnemyPosition;
        private FacingDirection originalPlayerDirection;
        private Vector2 originalPlayerPosition;
        private UIArenaHealthMeter playerMeter;

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

                var leftPos = 90;
                var rightPos = 150;
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

            playerMeter = new(Player);
            enemyMeter = new(Enemy);

            TransitionTo(new PlayerTurnState(this));
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            //targetMeter.Draw(gameTime);
            currentState.Draw(gameTime);
            enemyMeter.Draw(gameTime);
            playerMeter.Draw(gameTime);
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