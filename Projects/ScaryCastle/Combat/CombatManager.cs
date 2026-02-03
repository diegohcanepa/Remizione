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
        private readonly ImageSprite combatMask;
        private CombatManagerState currentState;
        private readonly UITargetMeter targetMeter;

        #region Constructor

        // Constructor
        public CombatManager(Actor player, Actor enemy)
            : base(player.Game)
        {
            this.Session = player.Session;
            this.Player = player;
            this.Enemy = enemy;
            this.targetMeter = new UITargetMeter(enemy);
            this.currentState = new PlayerTurnState(this);
            this.combatMask = new(Game, Atlases.UI.GetImage("CombatMask"))
            {
                PivotOrigin = RectanglePoint.Center,
            };

            combatMask.Tweens.OpacityTween = FloatTween.Create(TweenStyle.CubicIn, 0, 1, 1000);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            combatMask.Draw(gameTime);
            targetMeter.Draw(gameTime);
            currentState.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            currentState?.Update(gameTime);
            combatMask.Update(gameTime);
            combatMask.Position = Player.Position - Session.Camera.Offset;
            targetMeter.Update(gameTime);
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

        // Player
        public Actor Player { get; }

        // Session
        public GameSession Session { get; }

        // TransitionTo
        public void TransitionTo(CombatManagerState newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState.Enter();
        }
    }
}