using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// Trap
    /// </summary>
    public abstract class Trap : Prop
    {
        // Constructor
        public Trap(GameSession session, string name)
            : base(session, name)
        {
        }

        #region Protected fields

        // ActivatingDuration
        protected float ActivatingDuration { get; set; } = .2f;

        // ActiveDuration
        protected float ActiveDuration { get; set; } = .5f;

        // CooldownDuration
        protected float CooldownDuration { get; set; } = 2f;

        // IdleDuration
        protected float IdleDuration { get; set; }

        // IsAutomaticReset
        protected bool IsAutomaticReset { get; set; } = true;

        // WarningDuration
        protected float WarningDuration { get; set; } = .5f;

        #endregion

        #region Protected members

        // OnActivate
        protected override void OnActivate()
        {
            base.OnActivate();

            if (TrapState == TrapState.None)
                TransitionTo(TrapState.Idle);
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
        }

        // OnStateEnter
        protected abstract void OnStateEnter(TrapState state);

        // OnStateExit
        protected virtual void OnStateExit(TrapState state)
        {
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            StateTimer += deltaTime;

            // Dispatch de Update por estado
            switch (TrapState)
            {
                // Idle
                case TrapState.Idle:
                    OnUpdateIdle(gameTime);
                    if (IdleDuration == 0 || StateTimer >= IdleDuration)
                        TryTrigger();
                    break;

                // Warning
                case TrapState.Warning:
                    OnUpdateWarning(gameTime);
                    if (StateTimer >= WarningDuration)
                        TransitionTo(TrapState.Activating);
                    break;

                // Activating
                case TrapState.Activating:
                    OnUpdateActivating(gameTime);
                    if (StateTimer >= ActivatingDuration)
                        TransitionTo(TrapState.Active);
                    break;

                // Active
                case TrapState.Active:
                    OnUpdateActive(gameTime);
                    if (StateTimer >= ActiveDuration)
                        TransitionTo(TrapState.Cooldown);
                    break;

                // Cooldown
                case TrapState.Cooldown:
                    OnUpdateCooldown(gameTime);
                    if (StateTimer >= CooldownDuration)
                    {
                        if (IsAutomaticReset)
                            TransitionTo(TrapState.Idle);
                        else
                            TransitionTo(TrapState.Disabled);
                    }
                    break;

                // Disabled
                case TrapState.Disabled:
                    OnUpdateDisabled(gameTime);
                    break;
            }
        }

        // OnUpdateIdle
        protected virtual void OnUpdateIdle(GameTime gameTime)
        {
        }

        // OnUpdateWarning
        protected virtual void OnUpdateWarning(GameTime gameTime)
        {
        }

        // OnUpdateActivating
        protected virtual void OnUpdateActivating(GameTime gameTime)
        {
        }

        // OnUpdateActive
        protected virtual void OnUpdateActive(GameTime gameTime)
        {
        }

        // OnUpdateCooldown
        protected virtual void OnUpdateCooldown(GameTime gameTime)
        {
        }

        // OnUpdateDisabled
        protected virtual void OnUpdateDisabled(GameTime gameTime)
        {
        }

        #endregion

        // DisableTrap
        public virtual void DisableTrap()
        {
            TransitionTo(TrapState.Disabled);
        }

        // StateTimer
        public float StateTimer { get; private set; }

        // TransitionTo
        public void TransitionTo(TrapState newState)
        {
            if (TrapState == newState && StateTimer > 0)
                return;

            TrapState previousState = TrapState;
            OnStateExit(previousState);

            TrapState = newState;
            StateTimer = 0;

            OnStateEnter(TrapState);
        }

        // TryTrigger (Trigger manual para trampas que se activan por proximidad/contacto)
        public virtual bool TryTrigger()
        {
            if (TrapState != TrapState.Idle)
                return false;

            if (WarningDuration > 0)
                TransitionTo(TrapState.Warning);
            else
                TransitionTo(TrapState.Activating);

            return true;
        }

        // TrapState
        public TrapState TrapState { get; private set; }
    }
}