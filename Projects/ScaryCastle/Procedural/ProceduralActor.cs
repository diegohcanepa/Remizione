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
        #region Constructor

        // Constructor
        protected ProceduralActor(GameSession session, string name)
            : base(session, name)
        {
            Definition = ActorDefinition.Definitions.Get(DeclaredName);
            CombatBehavior = CombatBehavior.Behaviors.Get(DeclaredName);
            Faction = Faction.Evil;
            Sensor = new(this);

            BrainMachine = new(this, new BrainPatrolState());
            BrainMachine.Start();
        }

        #endregion

        #region IThingDefinition

        ThingDefinition IThingDefinition.Definition => this.Definition;

        #endregion

        #region Private members

        // GetSeparationForce
        private Vector2 GetSeparationForce()
        {
            var separationForce = Vector2.Zero;
            int neighborsCount = 0;

            if (Room != null)
            {
                for (var i = 0; i < Room.Children.Count; i++)
                {
                    // Self
                    if (Room.Children[i] == this)
                        continue;

                    if (Room.Children[i] is not ProceduralActor other)
                        continue;

                    // Distancia a mi compañero
                    float distSq = Vector2.DistanceSquared(this.Position, other.Position);
                    float radiusSq = SeparationRadius * SeparationRadius;

                    // Si está invadiendo mi espacio personal
                    if (distSq < radiusSq && distSq > 0)
                    {
                        // Vector que apunta DESDE el otro HACIA mí (Empujón)
                        Vector2 pushDir = this.Position - other.Position;

                        // Normalizamos para tener solo dirección
                        pushDir.Normalize();

                        // Peso inverso: Cuanto más cerca, más fuerte empuja
                        // (Evita division por cero agregando un pequeño float)
                        float strength = 1.0f - (distSq / radiusSq);

                        separationForce += pushDir * strength;
                        neighborsCount++;
                    }
                }
            }

            if (neighborsCount > 0)
            {
                // Promediamos la fuerza
                separationForce /= neighborsCount;
                // Escalamos por la configuración de fuerza
                separationForce *= SeparationWeight;
            }

            return separationForce;
        }

        #endregion

        #region Protected members

        // OnAdjustMoveDirection
        protected override Vector2 OnAdjustMoveDirection(Vector2 direction)
        {
            var separation = GetSeparationForce();

            var finalDirection = direction + separation;

            // Volvemos a normalizar para que no camine más rápido en diagonal
            if (finalDirection.LengthSquared() > 0)
                finalDirection.Normalize();

            return finalDirection;
        }

        // OnEnterRoom
        protected override void OnEnterRoom()
        {
            base.OnEnterRoom();

            // --- AUTOMATIZACIÓN CRÍTICA ---
            // El radio de separación es el "Espacio Personal".
            // Lo calculamos basado en el ancho del colisionador (BoundingBox).
            // Multiplicamos por 0.8 para permitir un ligero solapamiento (se ve más natural).
            if (BoundingBox.Width > 0)
            {
                SeparationRadius = BoundingBox.Width / 2f * 0.9f;
            }
            else
            {
                SeparationRadius = 40f; // Fallback por si no tiene collider aún
            }

            // Ajuste por Arquetipo (Opcional pero recomendado)
            // Los enemigos tácticos respetan más el espacio personal que los Berserkers.
            switch (CombatBehavior.Archetype)
            {
                case CombatBehaviorArchetype.Tactical:
                    SeparationWeight = 3.5f; // Se separan mucho (formación)
                    break;
                case CombatBehaviorArchetype.Berserk:
                    SeparationWeight = 1.0f; // Se amontonan un poco (horda)
                    break;
            }

            Reheal();
            Stand();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            Sensor.Update(gameTime);
            BrainMachine.Update(gameTime);

            base.OnUpdate(gameTime);

            if (GetTarget() is { } target && RuntimeCollider.Contains(target.Position))
            {
                EffectDescriptor.Apply(this, target);
            }
        }

        // SeparationRadius (Configuración: Qué tan "gordos" son los enemigos (Radio personal))
        protected float SeparationRadius { get; set; }

        // SeparationWeight (Qué tan fuerte se empujan entre sí)
        protected float SeparationWeight { get; set; } = 2;

        #endregion

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

        // IsInAttackRange
        public bool IsInAttackRange(GameThing target)
        {
            if (!IsInAttackLane(target))
                return false;

            // CONDICIÓN X: Debe estar al alcance de mi arma
            float dx = Math.Abs(Position.X - target.X);
            if (dx > AttackRange)
                return false;

            return true;
        }

        // Sensor
        public Sensor Sensor { get; }
    }
}