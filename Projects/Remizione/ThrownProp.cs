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

        private const float ThrowSpeed = 0.3f;      // Píxeles por milisegundo (500 px/s)
        private const int MinThrowDuration = 500;   // Duración mínima en ms para tiros muy cortos
        private const int MaxThrowDuration = 1600;  // Duración máxima en ms para tiros al límite

        #endregion

        #region Private fields

        private float depth;
        private readonly float effectiveArcHeight;
        private readonly Actor owner;
        private GameThing? target;
        private readonly FloatTween xTween = new();
        private readonly FloatTween yTween = new();

        #endregion

        #region Constructor

        // Constructor
        public ThrownProp(Actor owner, Prop prop)
            : base(prop.Session, string.Empty)
        {
            this.owner = owner;
            this.Prop = prop;
            this.Atlas = Atlases.Environment;
            this.PivotOrigin = RectanglePoint.Center;
            this.IgnoreWalkArea = true;
            this.effectiveArcHeight = Prop.ThrownDistance switch
            {
                ThrowDistance.Long => 0.8f,  // Tiro directo y tenso (piedras/lanzas)
                ThrowDistance.Short => 1.3f, // Tiro más abombado por el peso (vasijas)
                _ => 1f
            };

            var animation = AddAnimation(AnimationNames.Default);
            animation.AddFrame(prop.GetHeldPropImageName(), 1000);
        }

        #endregion

        #region Private members

        // Break
        public void Break()
        {
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

        // Launch
        private void Launch(GameThing? target, Vector2 targetPosition)
        {
            var startPos = owner.GetHeldPropPosition();
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
            float peakY = Math.Min(Y, targetPosition.Y) - effectiveArcHeight;

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

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            CheckCollision();
        }

        #endregion

        // ClampToEffectiveRange
        // Limita una posición de destino al rango máximo sobre el plano del suelo compensando la perspectiva.
        public static Vector2 ClampToEffectiveRange(Vector2 origin, Vector2 destination, float maxDistance)
        {
            float currentFlatDistance = FlatDistance(origin, destination);

            if (currentFlatDistance <= maxDistance)
                return destination;

            var delta = destination - origin;
            var unscaledDelta = new Vector2(delta.X, delta.Y / YPerspectiveFactor);
            var normalizedDirection = Vector2.Normalize(unscaledDelta);

            var clampedUnscaled = normalizedDirection * maxDistance;
            return new(origin.X + clampedUnscaled.X,
                       origin.Y + (clampedUnscaled.Y * YPerspectiveFactor));
        }

        /// <summary>
        /// Calcula la distancia real sobre el plano del suelo compensando la compresión visual del eje Y.
        /// </summary>
        public static float FlatDistance(Vector2 origin, Vector2 destination)
        {
            float dx = destination.X - origin.X;
            // Escalamos la diferencia vertical para ajustarla a la escala real del suelo
            float dy = (destination.Y - origin.Y) / YPerspectiveFactor;

            return MathF.Sqrt((dx * dx) + (dy * dy));
        }

        // Depth
        public override float Depth => depth;

        // Drop
        public void Drop()
        {
            var startPos = owner.GetHeldPropPosition() ?? owner.Position;
            Launch(null, new Vector2(startPos.X, owner.Y + 1));
        }

        // Prop
        public Prop Prop { get; }

        // Throw
        public void Throw(GameThing target)
        {
            if (Prop.ThrownDistance == ThrowDistance.None)
                return;

            var targetPos = ClampToEffectiveRange(owner.Position, target.Position, Prop.ThrownDistanceInPixels);

            Launch(target, targetPos);
        }

        // YPerspectiveFactor
        public const float YPerspectiveFactor = 0.75f;
    }
}