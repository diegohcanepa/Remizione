using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// ProceduralActor
    /// </summary>
    public abstract class ProceduralActor : Actor, IThingDefinition
    {
        #region Private fields

        private bool isAttacking;
        private readonly FloatTween nervousTween = new();

        #endregion

        #region Constructor

        // Constructor
        protected ProceduralActor(GameSession session, string name)
            : base(session, name)
        {
            Definition = ActorDefinition.Definitions.Get(DeclaredName);
            CombatBehavior = CombatBehavior.Behaviors.Find(DeclaredName);
            nervousTween.Start(TweenStyle.Linear, 0, .4f, 40, -1);
        }

        #endregion

        #region IThingDefinition

        ThingDefinition IThingDefinition.Definition => this.Definition;

        #endregion

        #region Private members

        // CheckCombatLogic
        private void CheckCombatLogic()
        {
            if (Session.Player == null)
                return;

            // Intentar Atacar si el timer venció
            if (AttackCooldown <= 0)
            {
                if (IsFacingTowards(Session.Player))
                {
                    var dist = Vector2.Distance(Position, Session.Player.Position);

                    if (dist <= AttackRange)
                    {
                        if (!RequiresLineOfSight || InLineOfSight(Session.Player.Position))
                        {
                            StartAttack();
                        }
                    }
                }
            }
        }

        // EndAttack
        protected void EndAttack()
        {
            isAttacking = false;
            AttackCooldown = AttackRate;
        }

        // InLineOfSight
        protected virtual bool InLineOfSight(Vector2 targetPosition)
        {
            if (Room?.WalkArea != null)
                return Room.WalkArea.InLineOfSight(Position, targetPosition);
            else
                return true;
        }

        // IsInsideVisibleBox
        private bool IsInsideVisibleBox(Vector2 pos)
        {
            var view = Session.Camera.VisibleBox;
            view.Inflate(-20, -20);
            return view.Contains(pos);
        }

        // StartAttack
        private void StartAttack()
        {
            isAttacking = true;
            StopMoving();
            Session.Player?.StopMoving();
            BeginAttackExecution();
        }

        // UpdateMovementBehavior
        private void UpdateMovementBehavior(GameTime gameTime)
        {
            if (!AllowMovementBehavior || MoveRate.IsEmpty || IsMoving)
                return;

            if (MoveCooldown > 0)
            {
                MoveCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                if (MoveCooldown <= 0)
                {
                    BeginMovementBehavior();
                    MoveCooldown = MoveRate.GetRandomValue(Random.Shared);
                }
            }
            else
                MoveCooldown = MoveRate.GetRandomValue(Random.Shared);
        }

        #endregion

        #region Protected members

        // AttackCooldown
        protected int AttackCooldown { get; set; }

        // AttackRange
        protected float AttackRange { get; set; } = 8;

        // AttackRate
        protected int AttackRate { get; set; } = 2000;

        // BeginAttackExecution
        protected virtual void BeginAttackExecution()
        {
        }

        // BeginMovementBehavior
        protected virtual void BeginMovementBehavior()
        {
            MoveRandomly();
        }

        // CheckForAggroTrigger
        protected virtual void CheckForAggroTrigger()
        {
            /*
            if (Session.Player == null)
                return;

            float dist = Vector2.Distance(Position, Session.Player.Position);

            // Si entra en rango visual (ej. 250px) y está en pantalla
            if (dist < 250 && IsInsideVisibleBox(Position))
            {
                if (!RequiresLineOfSight || InLineOfSight(Session.Player.Position))
                {
                    IsNervous = true;
                }
            }
            */
        }

        // DeAggroDistance
        protected float DeAggroDistance { get; set; } = 500;

        // ManageRandomMovement
        protected void ManageRandomMovement(GameTime gameTime)
        {
            if (!AllowMovementBehavior || IsMoving)
                return;

            if (MoveCooldown > 0)
            {
                MoveCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                if (MoveCooldown <= 0)
                {
                    MoveRandomly();
                    MoveCooldown = Session.Random.Next(3000, 6000);
                }
            }
            else
                MoveCooldown = 500;
        }

        // MoveCooldown
        protected int MoveCooldown { get; set; }

        // MoveRate
        protected Int32Range MoveRate { get; set; } = new(5000);

        // OnAggro
        protected virtual void OnAggro() { } // Opcional: Sonido "¡Te vi!"

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (IsNervous)
            {
                SupressOnTransformNotification++;
                X += nervousTween.CurrentValue;
            }

            base.OnDraw(gameTime);

            if (IsNervous)
            {
                X -= nervousTween.CurrentValue;
                SupressOnTransformNotification--;
            }
        }

        // OnTakeDamage
        protected override void OnTakeDamage(GameThing attacker, int amount, DamageType damageType, Vector2 knockback)
        {
            base.OnTakeDamage(attacker, amount, damageType, knockback);
            IsNervous = true;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            nervousTween.Update(gameTime);

            // 1. Timer de Ataque (Solo si está nervioso y libre)
            if (IsNervous && !isAttacking)
            {
                if (AttackCooldown > 0)
                    AttackCooldown -= gameTime.ElapsedGameTime.Milliseconds;
            }

            // 2. Máquina de Estados Simplificada
            if (!isAttacking)
            {
                // A. Movimiento (Virtual: Patrulla o Persecución)
                UpdateMovementBehavior(gameTime);

                if (!IsNervous)
                {
                    // B. Si está tranquilo -> Chequear si debe enojarse
                    CheckForAggroTrigger();
                }
                else
                {
                    // C. Si está nervioso -> Lógica de Combate
                    CheckCombatLogic();

                    // Opcional: Calmarse si el jugador se aleja mucho
                    if (Session.Player != null && Vector2.Distance(Position, Session.Player.Position) > DeAggroDistance)
                    {
                        IsNervous = false;
                    }
                }
            }
            else
            {
                // D. Ejecución del ataque (esperando animación/proyectil)
                UpdateAttackExecution(gameTime);
            }
        }

        // RequiresLineOfSight
        protected bool RequiresLineOfSight { get; set; } = true;

        // UpdateAttackExecution
        protected abstract void UpdateAttackExecution(GameTime gameTime);

        #endregion

        // AllowMovementBehavior
        public bool AllowMovementBehavior { get; set; }

        // Definition
        public ActorDefinition Definition { get; }

        // IsNervous
        [ScriptProperty]
        public bool IsNervous
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    if (field)
                    {
                        AttackCooldown = AttackRate / 2;
                        OnAggro();
                    }
                }
            }
        }
    }
}