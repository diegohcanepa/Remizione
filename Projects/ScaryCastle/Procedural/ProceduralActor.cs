using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ProceduralActor
    /// </summary>
    public abstract class ProceduralActor : Actor, IThingDefinition
    {
        #region Constructor

        // Constructor
        protected ProceduralActor(GameSession session, string name)
            : base(session, name)
        {
            Definition = ActorDefinition.Definitions.Get(DeclaredName);
            CombatBehavior = CombatBehavior.Behaviors.Get(DeclaredName);
            Sensor = new(this);

            BrainMachine = new(this, new CombatDecisionState());
            BrainMachine.Start();
        }

        #endregion

        #region IThingDefinition

        ThingDefinition IThingDefinition.Definition => this.Definition;

        #endregion

        #region Protected members

        // OnEnterRoom
        protected override void OnEnterRoom()
        {
            base.OnEnterRoom();
            Reheal();
            Stand();
        }

        // OnStartAttack
        protected virtual void OnStartAttack(GameThing target, CombatIntent intent)
        {
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            Sensor.Update(gameTime);
            BrainMachine.Update(gameTime);
            
            base.OnUpdate(gameTime);
        }

        #endregion

        // Attack
        public void Attack(GameThing target, CombatIntent intent)
        {
            StopMoving();
            FaceTo(target);
            OnStartAttack(target, intent);
        }

        // AttackRange
        public float AttackRange { get; set; }

        // BrainMachine
        public StateMachine<ProceduralActor> BrainMachine { get; }

        // CombatBehavior
        public CombatBehavior CombatBehavior { get; init; }

        // Definition
        public ActorDefinition Definition { get; }

        // GetTarget
        public Actor? GetTarget()
        {
            var result = Session.Player;
            return result == null || result.IsDead ? null : result;
        }

        // Sensor
        public Sensor Sensor { get; }
    }
}