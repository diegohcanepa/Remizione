using Adberration;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// Sensor
    /// </summary>
    public sealed class Sensor
    {
        #region Private Fields

        private float memoryTimer;
        private readonly ProceduralActor owner;
        private float scanTimer;
        private const float SCAN_INTERVAL = .2f;

        #endregion

        #region Constructor

        public Sensor(ProceduralActor owner)
        {
            this.owner = owner;
            // Jitter inicial para que no todos los NPCs escaneen en el mismo frame
            this.scanTimer = (float)Random.Shared.NextDouble() * SCAN_INTERVAL;
        }

        #endregion

        #region Private members

        private bool CheckLineOfSight(Vector2 start, Vector2 end)
        {
            return (owner.Room?.WalkArea) == null || owner.Room.WalkArea.InLineOfSight(start, end);
        }

        private Vector2 GetFacingDirection()
        {
            return owner.Direction == FacingDirection.Right ? Vector2.UnitX : -Vector2.UnitX;
        }

        private bool IsInViewCone(Vector2 myPos, Vector2 targetPos)
        {
            if (ViewAngle >= 360f) return true;

            Vector2 dirToTarget = Vector2.Normalize(targetPos - myPos);
            Vector2 facingDir = GetFacingDirection();

            float dot = Vector2.Dot(facingDir, dirToTarget);
            float threshold = MathF.Cos(MathHelper.ToRadians(ViewAngle / 2f));

            return dot >= threshold;
        }

        /// <summary>
        /// Nueva lógica de escaneo: Busca el objetivo hostil más cercano y procesa su visibilidad.
        /// </summary>
        private void PerformScan()
        {
            // 1. Validar objetivo actual: Si se murió, lo soltamos.
            if (owner.Target != null && owner.Target.IsDead)
            {
                owner.Target = null;
            }

            // 2. Búsqueda de objetivo: Si no tenemos uno, escaneamos la habitación.
            // Esto evita el hardcoding del Player y permite facciones.
            if (owner.Target == null && owner.Room != null)
            {
                float closestDistSq = float.MaxValue;
                Actor? bestTarget = null;

                for (int i = 0; i < owner.Room.Children.Count; i++)
                {
                    if (owner.Room.Children[i] is Actor other &&
                        !other.IsDead &&
                        other != owner &&
                        owner.IsHostile(other))
                    {
                        float dSq = Vector2.DistanceSquared(owner.Position, other.Position);
                        if (dSq < closestDistSq)
                        {
                            closestDistSq = dSq;
                            bestTarget = other;
                        }
                    }
                }
                owner.Target = bestTarget;
            }

            // Si tras el escaneo no hay nadie hostil cerca, terminamos.
            if (owner.Target == null)
            {
                CanSeeTarget = false;
                return;
            }

            // 3. Procesamiento de Percepción sobre el Target actual
            Vector2 myPos = owner.Position;
            Vector2 targetPos = owner.Target.Position;
            float distSq = Vector2.DistanceSquared(myPos, targetPos);

            // Fase de Visión: Cascada de filtros (Rango -> Ángulo -> Raycast).
            if (distSq > (SightRange * SightRange))
            {
                CanSeeTarget = false;
                return;
            }

            if (!IsInViewCone(myPos, targetPos))
            {
                CanSeeTarget = false;
                return;
            }

            if (CheckLineOfSight(myPos, targetPos))
            {
                CanSeeTarget = true;
                UpdateMemory(targetPos);
            }
            else
            {
                CanSeeTarget = false;
            }
        }

        private void UpdateMemory(Vector2 pos)
        {
            LastKnownTargetPos = pos;
            memoryTimer = 0;
        }

        #endregion

        #region Settings

        public float MemoryDuration { get; set; } = 5000;
        public float SightRange { get; set; } = 250; // Aumentado para IA procedural
        public float ViewAngle { get; set; } = 110;

        #endregion

        public void Forget()
        {
            LastKnownTargetPos = null;
            CanSeeTarget = false;
            memoryTimer = 0;
        }

        public bool CanSeeTarget { get; private set; }
        public bool IsAlerted => LastKnownTargetPos.HasValue;
        public Vector2? LastKnownTargetPos { get; private set; }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (LastKnownTargetPos.HasValue && !CanSeeTarget)
            {
                memoryTimer += dt;
                if (memoryTimer >= MemoryDuration)
                    Forget();
            }

            scanTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (scanTimer <= 0)
            {
                scanTimer = SCAN_INTERVAL + ((float)Random.Shared.NextDouble() * .05f);
                PerformScan();
            }
        }
    }
}