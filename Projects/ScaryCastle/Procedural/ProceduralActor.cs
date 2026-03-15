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
        private float contactCooldown;

        #region Constructor

        // Constructor
        protected ProceduralActor(GameSession session, string name)
            : base(session, name)
        {
            Definition = ActorDefinition.Definitions.Get(DeclaredName);
            CombatBehavior = CombatBehavior.Behaviors.Get(DeclaredName);
            Faction = Faction.Evil;
        }

        #endregion

        #region IThingDefinition

        ThingDefinition IThingDefinition.Definition => this.Definition;

        #endregion

        #region Protected members

        // OnDie
        protected override void OnDie()
        {
            base.OnDie();
            Session.Fear -= FearBonus;
        }

        // OnEnterRoom
        protected override void OnEnterRoom()
        {
            base.OnEnterRoom();
            Reheal();
            Stand();
        }

        // OnTakeDamage
        protected override void OnTakeDamage(GameThing attacker, int amount, DamageType damageType)
        {
            base.OnTakeDamage(attacker, amount, damageType);

            if (attacker is Actor)
                IsAngry = true;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (contactCooldown > 0)
                contactCooldown -= gameTime.ElapsedGameTime.Milliseconds;

            if (AttackTimer > 0)
                AttackTimer -= gameTime.ElapsedGameTime.Milliseconds;

            if (contactCooldown <= 0 && Target != null)
            {
                if (RuntimeCollider.Contains(Target.Position))
                {
                    EffectDescriptor.Apply(this, Target);
                    contactCooldown = 500;
                    return;
                }
            }
        }

        #endregion

        // AttackTimer
        public int AttackTimer { get; set; }

        // Brain
        public BrainConfig Brain { get; } = new();

        // CanInteract
        public override bool CanInteract()
        {
            return !IsAngry && base.CanInteract();
        }

        // CombatBehavior
        public CombatBehavior CombatBehavior { get; init; }

        // CurrentRage
        public int CurrentRage { get; set; }

        // Definition
        public ActorDefinition Definition { get; }

        // FearBonus
        [ScriptProperty]
        public int FearBonus { get; set; } = 1;

        // IsAngry
        [ScriptProperty]
        public bool IsAngry { get; set; }

        // IsHostile
        public virtual bool IsHostile(Actor other)
        {
            return this.Faction == Faction.Evil && other == Session.Player;
        }

        // IsInAttackRange
        public bool IsInAttackRange(GameThing target)
        {
            if (!IsInAttackLane(target))
                return false;

            // CONDICIÓN X: Debe estar al alcance de mi arma
            float dx = Math.Abs(Position.X - target.X);
            
            return dx <= Brain.AttackRange;
        }

        // ResetAttackTimer
        public void ResetAttackTimer()
        {
            AttackTimer = Brain.AttackCooldown;
        }

        // Target
        public Actor? Target { get; set; }
    }
}