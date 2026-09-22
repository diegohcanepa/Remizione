using System;
using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ThrownProp
    /// </summary>
    public sealed class ThrownProp : GameThing
    {
        #region Private fields

        private readonly float throwArcHeight = 2; // Altura máxima en px que sube el prop sobre la línea de tiro
        private readonly int throwDuration = 500;     // Duración total del tiro en ms
        private float depth;
        private readonly Actor owner;
        private GameThing? target;
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

        private void Launch(GameThing? target, Vector2 targetPosition)
        {
            var startPos = owner.GetCarriedPropPosition();
            if (owner.Room == null || startPos == null)
                return;

            this.target = target;
            this.RenderLayer = RenderLayer.Default;

            depth = owner.Depth + .01f;
            this.Position = startPos.Value;

            // 1. Movimiento en X: Directo y lineal hasta el destino
            xTween.Start(TweenStyle.Linear, X, targetPosition.X, throwDuration);

            // 2. Movimiento en Y: Dividido en 2 fases para crear la parábola del arco
            int halfDuration = throwDuration / 2;

            // Calculamos el pico del arco (punto medio entre el origen y el destino, subiendo 'throwArcHeight')
            float peakY = Math.Min(Y, targetPosition.Y) - throwArcHeight;

            // Fase 1: Subida con desaceleración (QuadEaseOut simula perder impulso hacia arriba)
            yTween.Start(TweenStyle.QuadraticOut, Y, peakY, halfDuration,
                () =>
                {
                    // Fase 2: Caída con aceleración (QuadEaseIn simula atracción por gravedad)
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

        public override float Depth => depth;

        public void Drop()
        {
            // Drop sin target: cae un poco más abajo de los pies del owner
            var startPos = owner.GetCarriedPropPosition() ?? owner.Position;
            Launch(null, new Vector2(startPos.X, owner.Y + 1));
        }

        public Prop Prop { get; }

        public void Throw(GameThing target)
        {
            Launch(target, target.Position);
        }
    }
}