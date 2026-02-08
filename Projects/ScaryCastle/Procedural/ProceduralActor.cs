using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ProceduralActor
    /// </summary>
    public abstract class ProceduralActor : Actor, IProceduralThing
    {
        private int attackCooldown;
        private UIContextualHealthMeter? healthMeter;
        private int movementCooldown;
        private readonly FloatTween nervousTween = new();

        // Constructor
        protected ProceduralActor(GameSession session, string name)
            : base(session, name)
        {
            Definition = ActorDefinition.Definitions.Get(DeclaredName);
            CombatBehavior = CombatBehavior.Behaviors.Find(DeclaredName);
            nervousTween.Start(TweenStyle.Linear, 0, .3f, 40, -1);
        }

        #region IProceduralThing explicit implementation

        // Definition
        ThingDefinition IProceduralThing.Definition => this.Definition;

        #endregion

        private void AttackCore()
        {
            if (CombatBehavior == null || Session.Player == null)
                return;

            if (Brain.Decide(CombatBehavior, HP, MaxHP) is CombatIntentDescriptor intent)
                EffectDescriptor.Apply(intent.EffectDescriptors, this, Session.Player);
        }


        // Attack
        [ScriptMethod]
        public void Attack()
        {
            attackCooldown = 5000;
            var distance = Direction == FacingDirection.Left ? -AttackRange : AttackRange;
            Tweens.XTween = FloatTween.Create(TweenStyle.Linear, X, X + distance, 100, 2, AttackCore);
        }

        // AttackRange
        public int AttackRange { get; set; } = 15;

        // CombatBehavior
        public CombatBehavior? CombatBehavior { get; }

        // Definition
        public ActorDefinition Definition { get; }

        // OnTakeDamage
        protected override void OnTakeDamage(GameThing attacker, int amount, DamageType damageType, Vector2 knockback)
        {
            base.OnTakeDamage(attacker, amount, damageType, knockback);

            if (!IsDead)
            {
                ShowHealthMeter();
                AudioManager.Music.PlayTag("Combat");
                IsNervous = true;
            }
        }

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

            healthMeter?.Draw(gameTime);
        }

        // OnStartMoving
        protected override void OnStartMoving()
        {
            base.OnStartMoving();
            healthMeter?.Hide();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (attackCooldown > 0)
                attackCooldown -= gameTime.ElapsedGameTime.Milliseconds;

            nervousTween?.Update(gameTime);

            healthMeter?.Update(gameTime);

            if (!Session.IsAwaiting)
            {
                if (IsNervous && Session.Player != null)
                {
                    if (attackCooldown <= 0 && DistanceTo(Session.Player) <= AttackRange)
                    {
                        var script = Session.ScriptLibrary.FindOutcome(DeclaredName);
                        if (script != null)
                        {
                            StopMoving();
                            IsAttacking = true;
                            Session.BeginOutcome(script, this);
                            return;
                        }
                    }
                }

                if (AllowRandomMovement && !IsMoving)
                {
                    if (movementCooldown > 0)
                    {
                        movementCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                        if (movementCooldown <= 0)
                        {
                            MoveRandomly();
                            AllowRandomMovement = true;
                        }
                    }
                }
            }
        }

        // AllowRandomMovement
        public bool AllowRandomMovement
        {
            get;
            set
            {
                field = value;
                movementCooldown = 5000;
            }
        }

        // HideHealthMeter
        public void HideHealthMeter()
        {
            healthMeter?.Hide();
        }

        // IsNervous
        [ScriptProperty]
        public bool IsNervous { get; set; }

        // ShowHealthMeter
        public void ShowHealthMeter()
        {
            healthMeter ??= new(this);
            healthMeter.Show();
        }
    }
}
