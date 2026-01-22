using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// Coin
    /// </summary>
    public sealed class Coin : GameObject
    {
        private bool dropped = false;

        // Usamos FloatTween para controlar el progreso (0.0 a 1.0) de la curva
        private readonly FloatTween curveTween = new();
        private readonly FloatTween opacityTween = new();

        private readonly GameSession session;
        private readonly ImageSprite sprite;

        // Puntos para la curva de Bézier
        private Vector2 p0; // Origen (Pantalla)
        private Vector2 p1; // Punto de control (Curvatura)
        private Vector2 p2; // Destino (HUD)

        // Constructor
        public Coin(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            this.sprite = new ImageSprite(session.Game, Atlases.Environment.Coin)
            {
                PivotOrigin = RectanglePoint.Center,
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!curveTween.IsDelayed)
                sprite.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            sprite.Update(gameTime);

            if (dropped)
            {
                // Actualizamos el tween de tiempo
                curveTween.Update(gameTime);

                // Si el tween está corriendo, calculamos la posición en la curva
                if (curveTween.IsRunning)
                {
                    float t = curveTween.CurrentValue;

                    // Cálculo de la Curva de Bézier Cuadrática
                    // Llevamos la moneda de P0 a P2, curvándose hacia P1.
                    Vector2 a = Vector2.Lerp(p0, p1, t);
                    Vector2 b = Vector2.Lerp(p1, p2, t);
                    sprite.Position = Vector2.Lerp(a, b, t);
                }

                // Verificamos si terminó la animación
                if (!curveTween.IsRunning && !curveTween.IsDelayed)
                {
                    dropped = false;
                    session.Coins++;
                    session.ObjectPools.Coins.Return(this);
                }
            }
        }

        #endregion

        // Drop
        public void Drop(GameRoom room, Vector2 origin, int delay)
        {
            // 1. P0: Posición de inicio convertida a Coordenadas de Pantalla (Screen Space)
            // Restamos el Offset de la cámara del nivel y multiplicamos por su Zoom.
            p0 = (origin - session.Camera.Offset) * session.Camera.Zoom;

            // 2. P2: Posición final (El ícono en el HUD ya está en coordenadas de pantalla)
            p2 = session.HUD.CoinMeter.IconBoundingBox.Center;

            // 3. P1: Punto de control para la curva sutil
            // Calculamos la distancia y el punto medio
            float distance = Vector2.Distance(p0, p2);
            Vector2 midPoint = (p0 + p2) / 2;

            // Calculamos vector perpendicular para dar la "panza" a la curva
            Vector2 direction = p2 - p0;
            Vector2 perpendicular = new Vector2(-direction.Y, direction.X);
            if (perpendicular != Vector2.Zero) perpendicular.Normalize();

            // Factor de curvatura: 10% a 20% de la distancia total (Sutil)
            float curvatureFactor = (Random.Shared.NextSingle() * 0.1f) + 0.1f;

            // Aleatoriedad: A veces curva a la izquierda, a veces a la derecha
            if (Random.Shared.Next(2) == 0)
                curvatureFactor *= -1;

            // Definimos el punto de control
            p1 = midPoint + (perpendicular * (distance * curvatureFactor));

            // Configuramos la duración basada en la distancia
            var tweenDuration = (int)(distance * 2.5f);
            if (tweenDuration < 300)
                tweenDuration = 300;

            // Posicionamos el sprite inicialmente
            sprite.Position = p0;

            // Iniciamos el Tween de la curva (0f -> 1f)
            // Usamos SineIn para que empiece lento y acelere hacia el HUD
            curveTween.StartDelay = delay;
            curveTween.Start(TweenStyle.SineIn, 0f, 1f, tweenDuration);

            // Tween de escala (efecto "pop" al aparecer)
            opacityTween.StartDelay = delay;
            opacityTween.Start(TweenStyle.CubicIn, 0, 1, 200);
            sprite.Tweens.OpacityTween = opacityTween;

            dropped = true;
        }
    }
}