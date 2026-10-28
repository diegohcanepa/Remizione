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
        private const float MaxArcHeight = 24f;     // Altura máxima absoluta del pico del arco (en píxeles)

        #endregion

        #region Private fields

        private float depth;
        private bool isDescending;
        private readonly Actor owner;
        private readonly FloatTween xTween = new();
        private readonly FloatTween yTween = new();

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

            var animation = AddAnimation(AnimationNames.Default);
            animation.AddFrame(prop.GetHeldPropImageName(), 1000);
        }

        #endregion

        #region Private members

        // Break
        public void Break()
        {
            isDescending = false;
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
            if (Prop.Definition == null || Room == null)
                return;

            for (int i = 0; i < Room.Children.Count; i++)
            {
                var child = Room.Children[i] as GameThing;

                if (child == null || child == owner || child == this)
                    continue;

                if (child.CanBeHit() && !child.IsDead)
                {
                    if (child.RuntimeHotspot.BoundingRectangleF.Intersects(BoundingBox))
                    {
                        EffectDescriptor.Apply(Prop.Definition.EffectDescriptors, owner, child, EffectContext.Contact);
                        Break();
                        break;
                    }
                }
            }
        }

        // Launch
        private void Launch(Vector2 targetPosition)
        {
            var startPos = owner.GetHeldPropPosition();
            if (owner.Room == null || startPos == null)
                return;

            this.RenderLayer = RenderLayer.Default;
            this.isDescending = false;

            depth = owner.Depth + .01f;
            this.Position = startPos.Value;

            float distance = Vector2.Distance(startPos.Value, targetPosition);
            int throwDuration = Math.Clamp((int)(distance / ThrowSpeed), MinThrowDuration, MaxThrowDuration);

            float arcHeight = MathHelper.Clamp(distance * 0.15f, 6f, MaxArcHeight);

            // 1. Movimiento en X: Directo y lineal hasta el destino
            xTween.Start(TweenStyle.Linear, X, targetPosition.X, throwDuration);

            // 2. Movimiento en Y: Dividido en 2 fases
            int halfDuration = throwDuration / 2;
            float peakY = Math.Min(Y, targetPosition.Y) - arcHeight;

            // Fase 1: Subida (desaceleración) - Sin detección de impacto
            yTween.Start(TweenStyle.QuadraticOut, Y, peakY, halfDuration,
                () =>
                {
                    // Fase 2: Caída (aceleración) - Se activa la ventana de impacto
                    isDescending = true;
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

            if (isDescending)
                CheckCollision();
        }

        #endregion

        public override float Depth => depth;

        // Drop
        public void Drop()
        {
            var startPos = owner.GetHeldPropPosition() ?? owner.Position;
            Launch(new Vector2(startPos.X, owner.Y + 1));
        }

        public Prop Prop { get; }

        // Throw
        public void Throw(Vector2 targetPosition)
        {
            var clampedPos = Projectile.ClampToEffectiveRange(owner.Position, targetPosition, GameSettings.ThrownDistanceShortRange);

            Launch(clampedPos);
        }

        public const float YPerspectiveFactor = 0.75f;
    }
}