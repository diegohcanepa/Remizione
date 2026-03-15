using Adberration.Scripting;
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
            if (Session.Player == attacker)
                IsAngry = true;

            base.OnTakeDamage(attacker, amount, damageType);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (contactCooldown > 0)
                contactCooldown -= gameTime.ElapsedGameTime.Milliseconds;

            if (contactCooldown <= 0 && Session.Player != null)
            {
                if (RuntimeCollider.Contains(Session.Player.Position))
                {
                    EffectDescriptor.Apply(this, Session.Player);
                    contactCooldown = 500;
                    return;
                }
            }
        }

        #endregion

        // CombatBehavior
        public CombatBehavior CombatBehavior { get; init; }

        // CounterAttack
        [ScriptProperty]
        public bool CounterAttack { get; set; }

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
    }
}