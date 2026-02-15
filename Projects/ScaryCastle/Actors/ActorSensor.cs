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

        // Constructor
        public Sensor(ProceduralActor owner)
        {
            this.owner = owner;
            this.scanTimer = (float)Random.Shared.NextDouble() * SCAN_INTERVAL;
        }

        #endregion

        #region Private members

        // CheckLineOfSight
        private bool CheckLineOfSight(Vector2 start, Vector2 end)
        {
            return (owner.Room?.WalkArea) == null || owner.Room.WalkArea.InLineOfSight(start, end);
        }

        // GetFacingDirection
        private Vector2 GetFacingDirection()
        {
            return owner.Direction == FacingDirection.Right ? Vector2.UnitX : -Vector2.UnitX;
        }

        // IsInViewCone
        private bool IsInViewCone(Vector2 myPos, Vector2 targetPos)
        {
            // Si el ángulo es 360, ve todo
            if (ViewAngle >= 360f)
                return true;

            Vector2 dirToTarget = Vector2.Normalize(targetPos - myPos);

            // Asumo que tu Actor tiene una dirección hacia donde mira.
            // Si usas SpriteEffects.FlipHorizontally, tendrás que derivar esto.
            // Ejemplo: Vector2 facing = _owner.FacingRight ? Vector2.UnitX : -Vector2.UnitX;
            Vector2 facingDir = GetFacingDirection();

            // Producto Punto:
            // 1.0  = Enfrente exacto
            // 0.0  = 90 grados (costado)
            // -1.0 = 180 grados (atrás)
            float dot = Vector2.Dot(facingDir, dirToTarget);

            // Convertimos el ángulo de visión a un valor de umbral para el Dot.
            // Dividimos por 2 porque ViewAngle es el cono total (izquierda + derecha).
            float threshold = MathF.Cos(MathHelper.ToRadians(ViewAngle / 2f));

            return dot >= threshold;
        }

        // PerformScan
        private void PerformScan()
        {
            // No target or target is dead
            var target = owner.GetTarget();
            if (target == null)
            {
                CanSeeTarget = false;
                return;
            }

            Vector2 myPos = owner.Position;
            Vector2 targetPos = target.Position;

            // Usamos Distancia al Cuadrado para evitar la raíz cuadrada (más rápido)
            float distSq = Vector2.DistanceSquared(myPos, targetPos);

            // --- FASE 1: OÍDO (El chequeo más barato y tramposo) ---
            // Si el jugador hace ruido (no está sneaking) y está cerca, actualizamos la memoria.
            // Asumo que el Player tiene una propiedad IsSneaking.
            bool isSneaking = false; // target.IsSneaking; 

            float hearingRangeSq = HearingRange * HearingRange;
            if (distSq < hearingRangeSq && !isSneaking)
            {
                // Lo escuchamos -> Actualizamos memoria, pero NO CanSeeTarget
                UpdateMemory(targetPos);
            }

            // --- FASE 2: VISIÓN (Cascada de filtros) ---

            // A. Filtro de Distancia Máxima
            float sightRangeSq = SightRange * SightRange;
            if (distSq > sightRangeSq)
            {
                CanSeeTarget = false;
                return; // Está muy lejos, ni intentamos calcular ángulos
            }

            // B. Filtro de Ángulo (Producto Punto)
            if (!IsInViewCone(myPos, targetPos))
            {
                CanSeeTarget = false;
                return; // Está en mi rango, pero a mis espaldas
            }

            // C. Filtro de Raycast (Física) - El más costoso, solo llegamos aquí si pasó lo anterior
            // Asumo que tu motor tiene un método para chequear colisión de línea
            if (CheckLineOfSight(myPos, targetPos))
            {
                CanSeeTarget = true;
                UpdateMemory(targetPos);
            }
            else
            {
                CanSeeTarget = false;
                // Nota: Si lo veía y se escondió detrás de una pared, 
                // CanSeeTarget se vuelve false, pero LastKnownTargetPos se mantiene (Memoria).
            }
        }

        // UpdateMemory
        private void UpdateMemory(Vector2 pos)
        {
            LastKnownTargetPos = pos;
            memoryTimer = 0; // Reseteamos el contador de olvido
        }

        #endregion

        // Forget
        public void Forget()
        {
            LastKnownTargetPos = null;
            CanSeeTarget = false;
            memoryTimer = 0;
        }

        // CanSeeTarget (¿Tiene contacto visual directo en este frame exacto?)
        public bool CanSeeTarget { get; private set; }

        // HearingRange (Qué tan lejos escucha - atraviesa paredes)
        public float HearingRange { get; set; } = 150;

        // IsAlerted (Helper rápido para la StateMachine: ¿Está en modo combate/búsqueda?)
        public bool IsAlerted => LastKnownTargetPos.HasValue;

        // LastKnownTargetPos (¿Sabe dónde está el jugador? (Ya sea por verlo ahora o recordarlo)
        public Vector2? LastKnownTargetPos { get; private set; }

        // MemoryDuration (Cuánto tiempo tarda en "olvidar" una posición conocida si no ve al target)
        public float MemoryDuration { get; set; } = 5000; // 5 segundos

        // SightRange (Qué tan lejos ve en pixels)
        public float SightRange { get; set; } = 400;

        // Update
        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // 1. Gestión de Memoria (Olvido)
            if (LastKnownTargetPos.HasValue && !CanSeeTarget)
            {
                memoryTimer += dt;
                if (memoryTimer >= MemoryDuration)
                    Forget();
            }

            // 2. Throttling del Escaneo (Optimización de CPU)
            scanTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (scanTimer <= 0)
            {
                // Resetear timer con un pequeño jitter (+/- 10%) para evitar picos de CPU
                scanTimer = SCAN_INTERVAL + ((float)Random.Shared.NextDouble() * .05f);
                PerformScan();
            }
        }

        // ViewAngle (Ángulo de visión en grados (ej. 90° = 45° a cada lado, 360° = Ojos en la espalda)
        public float ViewAngle { get; set; } = 110;
    }
}