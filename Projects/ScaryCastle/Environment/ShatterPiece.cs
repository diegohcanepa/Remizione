using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    public class ShatterPiece : GameObject
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

        public ShatterPiece(AtlasImage image, Vector2 scale)
        {
            _image = new Sprite(image) { PivotOrigin = RectanglePoint.Center, Scale = scale };
        }

        public void Launch(GameThing owner)
        {
            _room = owner.Session.Room;
            _startPos = new Vector2(owner.X, owner.Y);

            float angle = (float)(Random.Shared.NextDouble() * Math.PI * 2);
            // 0.45f es el punto medio: ni muy chato ni muy esparcido en profundidad
            _direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle) * 0.45f);

            // Velocidad balanceada: 30-50
            _speed = Random.Shared.Next(30, 55);

            _isFirstBounce = true;
            _active = true;
            _isLaunched = false;

            // Un delay un poco más variado (hasta 0.15s) para que no salgan en bloque
            _launchDelay = (float)Random.Shared.NextDouble() * 0.15f;

            CalculateNextArc(10, 18);
        }

        private void CalculateNextArc(float minHeight, float maxHeight)
        {
            _elapsed = 0;
            _arcHeight = Random.Shared.Next((int)minHeight, (int)maxHeight);
            _duration = _isFirstBounce ? 0.35f : 0.2f;

            Vector2 tentativeTarget = _startPos + (_direction * _speed * _duration);

            if (_room?.WalkArea is { } walkArea)
            {
                var bounds = walkArea.Polygon.BoundingRectangleF;

                tentativeTarget.X = MathHelper.Clamp(tentativeTarget.X, bounds.Left + 2, bounds.Right - 2);
                tentativeTarget.Y = MathHelper.Clamp(tentativeTarget.Y, bounds.Top + 2, bounds.Bottom - 2);

                if (!walkArea.Contains(tentativeTarget))
                {
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

            Vector2 groundPos = Vector2.Lerp(_startPos, _targetPos, t);
            float height = 4 * _arcHeight * t * (1 - t);

            _image.Position = new Vector2(groundPos.X, groundPos.Y - height);
            _image.Rotation += dt * (_speed / 5f);

            if (t >= 1)
            {
                if (_isFirstBounce)
                {
                    _isFirstBounce = false;
                    _startPos = _targetPos;
                    _speed *= 0.25f; // Un poquito más de inercia para el segundo rebote
                    CalculateNextArc(3, 7);
                }
                else
                {
                    _active = false;
                }
            }
        }

        protected override void OnDraw(GameTime gameTime)
        {
            if (!_active && _elapsed == 0) return;

            var piecePos = _image.Position;
            var pieceColor = _image.Color;

            _image.Position = piecePos;
            _image.Color = pieceColor;
            _image.Draw(gameTime);
        }

        public Vector2 Scale
        {
            get => _image.Scale;
            set => _image.Scale = value;
        }
    }
}