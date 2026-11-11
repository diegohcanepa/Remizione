using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// Projectile
    /// </summary>
    public sealed class Projectile : GameThing
    {
        #region Constants

        private const float EffectiveArcHeight = 0.7f;
        private const int MinThrowDuration = 300;   // Duración mínima en ms para tiros muy cortos
        private const int MaxThrowDuration = 1600;  // Duración máxima en ms para tiros al límite
        private const float ThrowSpeed = 1.2f;      // Píxeles por milisegundo (500 px/s)

        #endregion

        #region Private fields

        private float depth;
        private GameThing? target;
        private Actor thrower = null!;
        private readonly FloatTween xTween = new();
        private const float yPerspectiveFactor = .75f;
        private readonly FloatTween yTween = new();

        #endregion

        #region Constructor

        // Constructor
        public Projectile(GameSession session)
            : base(session, string.Empty)
        {
            this.Atlas = Atlases.Environment;
            this.IgnoreWalkArea = true;
        }

        #endregion

        #region Private members

        // CheckCollision
        private void CheckCollision()
        {
            if (target == null)
                return;

            if (target.CanBeHit() && !target.IsDead)
            {
                if (!target.RuntimeHotspot.BoundingRectangleF.Intersects(BoundingBox))
                    return;

                if (Action.ImpactEffectType == ImpactEffectType.None)
                {
                    if (target.RuntimeHotspot.BoundingRectangleF.Intersects(BoundingBox))
                        EffectDescriptor.Apply(Action.EffectDescriptors, thrower, target, EffectContext.Contact);
                }
                else if (Action.ImpactEffectType == ImpactEffectType.Lightning)
                {
                    var lightning = new LightningInvocation(Action, target) { Position = target.Position };
                    thrower?.Room?.Children.Add(lightning);
                }

                Hit();
            }
        }

        // FlatDistance
        // Calcula la distancia real sobre el plano del suelo compensando la compresión visual del eje Y.
        private static float FlatDistance(Vector2 origin, Vector2 destination)
        {
            float dx = destination.X - origin.X;
            // Escalamos la diferencia vertical para ajustarla a la escala real del suelo
            float dy = (destination.Y - origin.Y) / yPerspectiveFactor;

            return MathF.Sqrt((dx * dx) + (dy * dy));
        }

        // Hit
        public void Hit()
        {
            Unparent();
        }

        // Launch
        private void Launch(Vector2 spawnPosition, GameThing? target, Vector2 targetPosition)
        {
            if (thrower.Room == null)
                return;

            this.target = target;
            this.RenderLayer = RenderLayer.Default;

            depth = thrower.Depth + .01f;
            this.Position = spawnPosition;

            // Calculamos la duración proporcional a la distancia real de vuelo
            float distance = Vector2.Distance(spawnPosition, targetPosition);
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
                    yTween.Start(TweenStyle.QuadraticIn, Y, targetPosition.Y, halfDuration, Hit);
                }
            );

            Tweens.XTween = xTween;
            Tweens.YTween = yTween;

            thrower?.Room?.Children.Add(this);
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

        // IAction
        public IAction Action { get; private set; } = null!;

        // ClampToEffectiveRange
        // Limita una posición de destino al rango máximo sobre el plano del suelo compensando la perspectiva.
        public static Vector2 ClampToEffectiveRange(Vector2 origin, Vector2 destination, float maxDistance)
        {
            float currentFlatDistance = FlatDistance(origin, destination);

            if (currentFlatDistance <= maxDistance)
                return destination;

            var delta = destination - origin;
            var unscaledDelta = new Vector2(delta.X, delta.Y / yPerspectiveFactor);
            var normalizedDirection = Vector2.Normalize(unscaledDelta);

            var clampedUnscaled = normalizedDirection * maxDistance;
            return new(origin.X + clampedUnscaled.X,
                       origin.Y + (clampedUnscaled.Y * yPerspectiveFactor));
        }

        // Depth
        public override float Depth => depth;

        // Throw
        public void Throw(Actor thrower, Vector2 spawnPosition, IAction action, GameThing target)
        {
            this.thrower = thrower;
            this.Action = action;
            this.PivotOrigin = RectanglePoint.Center;
            var animation = AddAnimation(AnimationNames.Default);
            animation.AddFrame(action.ProjectileImageName, 1000);
            var targetPos = ClampToEffectiveRange(thrower.Position, target.Position, GameSettings.ThrownDistanceLongRange);
            Launch(spawnPosition, target, targetPos);
        }
    }
}