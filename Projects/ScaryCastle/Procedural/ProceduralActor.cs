using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Globalization;
using System.Linq;

namespace ScaryCastle
{
    /// <summary>
    /// ProceduralActor
    /// </summary>
    public abstract class ProceduralActor : Actor, IThingDefinition
    {
        #region Private fields

        private readonly FloatTween angryTween = FloatTween.Create(TweenStyle.Linear, 0, .5f, 40, -1);
        private readonly ImageSprite heartIcon;
        private readonly TextSprite hpText;
        private bool isAttacking;

        #endregion

        #region Constructor

        // Constructor
        protected ProceduralActor(GameSession session, string name)
            : base(session, name)
        {
            heartIcon = new(Game, Atlases.UI.HeartFull)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Scale = ScaleInfo.UIElement.Medium
            };

            hpText = new(Game, Fonts.CommonOutline)
            {
                PivotOrigin = RectanglePoint.Left,
                Scale = ScaleInfo.Text.ExtraLarge
            };

            Definition = ActorDefinition.Definitions.Get(DeclaredName);
            CombatBehavior = CombatBehavior.Behaviors.Find(DeclaredName);
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

        // StartAttack
        private void StartAttack()
        {
            AttackCooldown = AttackRate;
            if (OnStartAttack())
            {
                isAttacking = true;
                Session.Player?.StopMoving();
            }
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
                    OnBeginMovementBehavior();
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
                    IsAngry = true;
                }
            }
            */
        }

        // IsInsideVisibleBox
        protected bool IsInsideVisibleBox(Vector2 pos)
        {
            var view = Session.Camera.VisibleBox;
            view.Inflate(-20, -20);
            return view.Contains(pos);
        }

        // DeAggroDistance
        protected float DeAggroDistance { get; set; } = 500;

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

        // MoveCooldown
        protected int MoveCooldown { get; set; }

        // MoveRate
        protected Int32Range MoveRate { get; set; } = new(5000);

        // OnEnterRoom
        protected override void OnEnterRoom()
        {
            base.OnEnterRoom();
            Reheal();
            AttackCooldown = Random.Shared.Next(AttackRate / 4, AttackRate + 1);
            IsAttacking = false;
            IsAngry = true;
            Stand();
        }

        // OnBeginMovementBehavior
        protected virtual void OnBeginMovementBehavior()
        {
            MoveRandomly();
        }

        // OnDie
        protected override void OnDie()
        {
            base.OnDie();
            Session.RemoveAngryActor(this);
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            var shake = AttackRate > 0 && AttackCooldown <= AttackRate / 4 && !IsMoving;

            if (shake)
            {
                SupressOnTransformNotification++;
                X += angryTween.CurrentValue;
            }

            base.OnDraw(gameTime);

            if (shake)
            {
                X -= angryTween.CurrentValue;
                SupressOnTransformNotification--;
            }

            if (!Session.IsCurrentScene)
            {
                heartIcon.Draw(gameTime);
                hpText.Draw(gameTime);
            }
        }

        // OnGetAngry
        protected virtual void OnGetAngry()
        {
        }

        // OnStartAttack
        protected virtual bool OnStartAttack()
        {
            return false;
        }

        // OnTakeDamage
        protected override void OnTakeDamage(GameThing attacker, int amount, DamageType damageType, Vector2 knockback)
        {
            base.OnTakeDamage(attacker, amount, damageType, knockback);

            if (!Session.AngryMode && Room != null)
            {
                foreach (var actor in Room.Children.OfType<ProceduralActor>())
                {
                    actor.IsAngry = true;
                }
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Session.Player != null && !Session.Player.IsDead)
            {
                if (Session.AwaitingScript != null && !Session.AwaitingScript.Interruptible)
                    return;
            }

            base.OnUpdate(gameTime);

            angryTween.Update(gameTime);

            // 1. Timer de Ataque (Solo si está nervioso y libre)
            if (IsAngry && !isAttacking)
            {
                if (AttackCooldown > 0)
                    AttackCooldown -= gameTime.ElapsedGameTime.Milliseconds;
            }

            // 2. Máquina de Estados Simplificada
            if (!isAttacking)
            {
                // A. Movimiento (Virtual: Patrulla o Persecución)
                UpdateMovementBehavior(gameTime);

                if (!IsAngry)
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
                        IsAngry = false;
                    }
                }
            }
            else
            {
                // D. Ejecución del ataque (esperando animación/proyectil)
                OnUpdateAttackExecution(gameTime);
            }

            if (MaxHP > 0)
            {
                hpText.Text = HP.ToString(CultureInfo.InvariantCulture);
                heartIcon.Position = GetOverheadPosition();
                heartIcon.X -= heartIcon.BoundingBox.Width / 2;
                hpText.Position = heartIcon.BoundingBox.GetPoint(RectanglePoint.Right, .5f, .5f);
            }
        }

        // OnUpdateAttackExecution
        protected abstract void OnUpdateAttackExecution(GameTime gameTime);

        // RequiresLineOfSight
        protected bool RequiresLineOfSight { get; set; } = true;

        #endregion

        // AllowMovementBehavior
        public bool AllowMovementBehavior { get; set; }

        // Definition
        public ActorDefinition Definition { get; }

        // IsAngry
        [ScriptProperty]
        public bool IsAngry
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    if (field)
                    {
                        Session.AddAngryActor(this);
                        AttackCooldown = AttackRate / 2;
                        OnGetAngry();
                    }
                    else
                    {
                        Session.RemoveAngryActor(this);
                    }
                }
            }
        }
    }
}