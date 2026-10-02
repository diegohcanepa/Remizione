using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// ThrownProp
    /// </summary>
    public sealed class ThrownProp : GameThing
    {
        #region Constants

        private const float ShortDistancePx = 50;
        private const float LongDistancePx = 150f;
        private const float ThrowSpeed = 0.5f;      // Píxeles por milisegundo (500 px/s)
        private const int MinThrowDuration = 500;   // Duración mínima en ms para tiros muy cortos
        private const int MaxThrowDuration = 1600;   // Duración máxima en ms para tiros al límite

        #endregion

        #region Private fields

        private float depth;
        private readonly Actor owner;
        private GameThing? target;
        private readonly FloatTween xTween = new();
        private readonly FloatTween yTween = new();

        #endregion

        #region Properties

        public Prop Prop { get; }

        public override float Depth => depth;

        /// <summary>
        /// Categoría de peso/distancia del objeto lanzado. Por defecto es None (no lanzable).
        /// </summary>
        public ThrowDistance ThrownDistance { get; set; }

        /// <summary>
        /// Retorna el rango en píxeles según la clase de distancia configurada.
        /// Si es None, el rango efectivo es 0.
        /// </summary>
        public float EffectiveMaxDistance => ThrownDistance switch
        {
            ThrowDistance.Long => LongDistancePx,
            ThrowDistance.Short => ShortDistancePx,
            _ => 0f
        };

        #endregion

        #region Constructor

        public ThrownProp(Actor owner, Prop prop)
            : base(prop.Session, string.Empty)
        {
            this.owner = owner;
            this.Prop = prop;
            this.Atlas = Atlases.Environment;
            this.PivotOrigin = RectanglePoint.Center;
            this.IgnoreWalkArea = true;
            this.ThrownDistance = prop.ThrownDistance;

            var animation = AddAnimation(AnimationNames.Default);
            animation.AddFrame(prop.GetCarriedPropImageName(), 1000);
        }

        #endregion

        #region Private members

        // CheckCollision
        private void CheckCollision()
        {
            if (Prop.Definition == null || target == null)
                return;

            if (target.CanBeHit() && !target.IsDead)
            {
                if (target.RuntimeHotspot.BoundingRectangleF.Intersects(BoundingBox))
                {
                    EffectDescriptor.Apply(Prop.Definition.EffectDescriptors, owner, target, EffectContext.Contact);
                    Break();
                }
            }
        }

        private Vector2 ClampToEffectiveRange(Vector2 origin, Vector2 destination)
        {
            float maxDistance = EffectiveMaxDistance;
            float currentFlatDistance = Utils.FlatDistance(origin, destination);

            if (currentFlatDistance <= maxDistance)
                return destination;

            // Calculamos el vector ajustado
            Vector2 delta = destination - origin;

            // Aplicamos la escala al eje Y para normalizar en espacio de suelo
            Vector2 unscaledDelta = new Vector2(delta.X, delta.Y / Utils.YPerspectiveFactor);
            Vector2 normalizedDirection = Vector2.Normalize(unscaledDelta);

            // Escalamos por la distancia máxima y devolvemos al espacio de pantalla
            Vector2 clampedUnscaled = normalizedDirection * maxDistance;
            return new Vector2(
                origin.X + clampedUnscaled.X,
                origin.Y + (clampedUnscaled.Y * Utils.YPerspectiveFactor)
            );
        }

        private float EffectiveArcHeight => ThrownDistance switch
        {
            ThrowDistance.Long => 0.8f,  // Tiro directo y tenso (piedras/lanzas)
            ThrowDistance.Short => 1.5f, // Tiro más abombado por el peso (vasijas)
            _ => 1f
        };

        private void Launch(GameThing? target, Vector2 targetPosition)
        {
            var startPos = owner.GetCarriedPropPosition();
            if (owner.Room == null || startPos == null)
                return;

            this.target = target;
            this.RenderLayer = RenderLayer.Default;

            depth = owner.Depth + .01f;
            this.Position = startPos.Value;

            // Calculamos la duración proporcional a la distancia real de vuelo
            float distance = Vector2.Distance(startPos.Value, targetPosition);
            int throwDuration = Math.Clamp((int)(distance / ThrowSpeed), MinThrowDuration, MaxThrowDuration);

            // 1. Movimiento en X: Directo y lineal hasta el destino
            xTween.Start(TweenStyle.Linear, X, targetPosition.X, throwDuration);

            // 2. Movimiento en Y: Dividido en 2 fases para crear la parábola del arco
            int halfDuration = throwDuration / 2;

            // Calculamos el pico del arco (punto medio entre el origen y el destino, subiendo 'throwArcHeight')
            float peakY = Math.Min(Y, targetPosition.Y) - EffectiveArcHeight;

            // Fase 1: Subida con desaceleración
            yTween.Start(TweenStyle.QuadraticOut, Y, peakY, halfDuration,
                () =>
                {
                    // Fase 2: Caída con aceleración
                    yTween.Start(TweenStyle.QuadraticIn, Y, targetPosition.Y, halfDuration, Break);
                }
            );

            Tweens.XTween = xTween;
            Tweens.YTween = yTween;

            owner.Room.Children.Add(this);
        }

        #endregion

        #region Protected members

        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            // Únicamente verificamos colisión durante el vuelo.
            // La posición se actualiza automáticamente por el engine mediante XTween y YTween.
            CheckCollision();
        }

        #endregion

        #region Public methods

        public void Break()
        {
            // Cancelamos los tweens por si colisionó antes de tiempo con un enemigo
            Tweens.Reset();

            if (Prop.DeathSound != null)
                PlaySound(Prop.DeathSound);

            RenderLayer = RenderLayer.Background;
            DepthOffset = 0;
            Prop.Position = this.Position;

            if (Room != null)
                Prop.SpawnRemains(Room);

            Unparent();

            Session.Camera.Shake(TweenStyle.Linear, Vector2.One, 40, 6);
        }

        public void Drop()
        {
            // Drop sin target: cae un poco más abajo de los pies del owner
            var startPos = owner.GetCarriedPropPosition() ?? owner.Position;
            Launch(null, new Vector2(startPos.X, owner.Y + 1));
        }

        /// <summary>
        /// Valida si un objetivo está en rango de tiro antes de instanciar el ThrownProp.
        /// Útil para la lógica de pintado del cursor en la UI.
        /// </summary>
        public static bool IsInThrowRange(Actor thrower, Prop prop, GameThing target)
        {
            if (thrower == null || prop == null || target == null)
                return false;

            var distanceClass = prop.ThrownDistance;
            if (distanceClass == ThrowDistance.None)
                return false;

            float maxDistance = distanceClass switch
            {
                ThrowDistance.Long => LongDistancePx,
                ThrowDistance.Short => ShortDistancePx,
                _ => 0f
            };

            // Usamos la distancia plana corregida por perspectiva
            return Utils.FlatDistance(thrower.Position, target.Position) <= maxDistance;
        }

        public void Throw(GameThing target)
        {
            // Si el objeto no se puede lanzar, la acción se ignora o se degrada a Drop
            if (ThrownDistance == ThrowDistance.None)
                return;

            var targetPos = ClampToEffectiveRange(owner.Position, target.Position);
            
            Launch(target, targetPos);
        }

        #endregion
    }
}