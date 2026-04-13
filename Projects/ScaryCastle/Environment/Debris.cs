using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// Debris
    /// </summary>
    public class Debris : GameObject, IPoolable
    {
        private readonly Sprite _image;
        private GameRoom? _room;

        private Vector2 _startPos;
        private Vector2 _targetPos;
        private float _arcHeight;
        private float _duration;
        private float _elapsed;

        private bool _isFirstBounce;
        private bool _active;
        private bool _isLaunched;
        private float _launchDelay;
        private Vector2 _direction;
        private float _speed;
        private float _rotationSpeed; // Nueva: para evitar rotación uniforme

        // Constructor
        public Debris()
        {
            _image = new Sprite()
            {
                PivotOrigin = RectanglePoint.Center,
            };
        }

        public void Launch(GameThing owner)
        {
            _room = owner.Session.Room;
            _startPos = new Vector2(owner.X, owner.Y);

            // 1. Variación de ángulo y deformación de perspectiva (Y)
            float angle = (float)(Random.Shared.NextDouble() * Math.PI * 2);
            float flattenFactor = 0.35f + (float)Random.Shared.NextDouble() * 0.25f;
            _direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle) * flattenFactor);

            // 2. Velocidad con rango más amplio
            _speed = Random.Shared.Next(35, 65);

            // 3. Rotación única (algunas giran hacia atrás, otras rápido, otras lento)
            _rotationSpeed = (float)(Random.Shared.NextDouble() * 12 - 6);

            _isFirstBounce = true;
            _active = true;
            _isLaunched = false;

            // 4. Delay de salida más generoso para romper el "bloque" inicial
            _launchDelay = (float)Random.Shared.NextDouble() * 0.2f;

            CalculateNextArc(12, 22);
        }

        private void CalculateNextArc(float minHeight, float maxHeight)
        {
            _elapsed = 0;
            _arcHeight = Random.Shared.Next((int)minHeight, (int)maxHeight);

            // 5. Duración aleatoria: esto es lo que evita que todas aterricen a la vez
            float baseDuration = _isFirstBounce ? 0.3f : 0.15f;
            _duration = baseDuration + (float)Random.Shared.NextDouble() * 0.25f;

            Vector2 tentativeTarget = _startPos + (_direction * _speed * _duration);

            if (_room?.WalkArea is { } walkArea)
            {
                var bounds = walkArea.Polygon.BoundingRectangleF;

                tentativeTarget.X = MathHelper.Clamp(tentativeTarget.X, bounds.Left + 2, bounds.Right - 2);
                tentativeTarget.Y = MathHelper.Clamp(tentativeTarget.Y, bounds.Top + 2, bounds.Bottom - 2);

                if (!walkArea.Contains(tentativeTarget))
                {
                    // Lógica de rebote simple contra bordes del WalkArea
                    if (tentativeTarget.Y < bounds.Top + 15)
                    {
                        _direction.Y = Math.Abs(_direction.Y);
                        _direction.X = -_direction.X;
                    }
                    else if (tentativeTarget.Y > bounds.Bottom - 10)
                    {
                        _direction.Y = -Math.Abs(_direction.Y);
                    }
                    else
                    {
                        _direction.X = -_direction.X;
                    }

                    int safety = 0;
                    while (!walkArea.Contains(tentativeTarget) && safety < 10)
                    {
                        tentativeTarget = Vector2.Lerp(tentativeTarget, _startPos, 0.5f);
                        safety++;
                    }
                }
            }

            _targetPos = tentativeTarget;
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            if (!_active) return;
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!_isLaunched)
            {
                _launchDelay -= dt;
                if (_launchDelay <= 0) _isLaunched = true;
                return;
            }

            _elapsed += dt;
            float t = MathHelper.Clamp(_elapsed / _duration, 0, 1);

            // Interpolación de posición en "suelo"
            Vector2 groundPos = Vector2.Lerp(_startPos, _targetPos, t);

            // Parábola de altura
            float height = 4 * _arcHeight * t * (1 - t);

            _image.Position = new Vector2(groundPos.X, groundPos.Y - height);

            // Usamos la velocidad de rotación calculada en Launch
            _image.Rotation += dt * _rotationSpeed;

            if (t >= 1)
            {
                if (_isFirstBounce)
                {
                    _isFirstBounce = false;
                    _startPos = _targetPos;

                    // 6. Fricción aleatoria para que no todas se deslicen igual al final
                    float friction = 0.15f + (float)Random.Shared.NextDouble() * 0.25f;
                    _speed *= friction;

                    CalculateNextArc(4, 9);
                }
                else
                {
                    _active = false;
                }
            }
        }

        protected override void OnDraw(GameTime gameTime)
        {
            // Solo dibujamos si está activa o si acaba de terminar (para evitar parpadeo)
            if (!_active && _elapsed == 0) return;
            _image.Draw(gameTime);
        }

        // Image
        public AtlasImage? Image
        {
            get => _image.RenderImage;
            set => _image.RenderImage = value;
        }

        // Reset
        public void Reset()
        {
            Image = null;
            Scale = Vector2.One;
        }

        public Vector2 Scale
        {
            get => _image.Scale;
            set => _image.Scale = value;
        }
    }
}