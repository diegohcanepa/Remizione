using Microsoft.Xna.Framework;
using System;

namespace Engendro
{
    /// <summary>
    /// Camera
    /// </summary>
    public class Camera : IDisposable
    {
        #region Private fields

        // Estado interno
        private Vector2 _position;
        private float _zoom = 1f;
        private float _rotation;

        // Variables de cache y cálculo
        private Matrix camTranslationMatrix = Matrix.Identity;
        private Vector3 camTranslationVector = Vector3.Zero;
        private bool disposedValue;
        private bool isMatrixDirty = true;
        private bool isInitializing;

        // Límites (Calculados dinámicamente)
        private float leftBarrier, rightBarrier, topBarrier, bottomBarrier;

        private const float maxZoom = 999;
        private const float minZoom = .1f;

        // Tweens
        private readonly Vector2Tween moveTween = new();
        private readonly FloatTween rotationTween = new();
        private readonly FloatTween shakeHorzTween = new();
        private readonly FloatTween shakeVertTween = new();
        private readonly FloatTween zoomTween = new();

        // Cinemática (Control Maestro)
        private readonly FloatTween flightTween = new();
        private Vector2 flightStartPos;
        private Vector2 flightEndPos;
        private float flightStartInvZoom;
        private float flightEndInvZoom;

        // Matrices auxiliares
        private Matrix resTranslationMatrix = Matrix.Identity;
        private Vector3 resTranslationVector = Vector3.Zero;
        private Matrix rotationTranslationMatrix = Matrix.Identity;
        private Matrix scaleMatrix = Matrix.Identity;
        private Vector3 scaleVector = Vector3.One;
        private Matrix transformationMatrix;

        // Dimensiones
        private int viewportHeight;
        private int viewportWidth;
        private readonly FloatRange zoomRange = new(minZoom, maxZoom);

        #endregion

        #region Constructors

        public Camera(string name)
            : this(name, 0, 0)
        {
        }

        public Camera(string name, int sceneWidth, int sceneHeight)
        {
            this.Game = EngendroGame.Instance;
            this.Name = name;

            UpdateViewportDimensions();

#if WINDOWS
            Game.Window.ClientSizeChanged += Window_ClientSizeChanged;
#endif

            Setup(sceneWidth, sceneHeight);
        }

        #endregion

        #region Private members

        private void ApproachCore(Vector2 targetPosition, GameTime gameTime)
        {
            if (IsTargetFocused)
                return;

            float zoomFactor = Math.Max(_zoom, 1f);
            float timeFactor = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float t = SmoothSpeed / zoomFactor * timeFactor;
            t = MathHelper.Clamp(t, 0f, 1f);

            Vector2 newPos = Vector2.Lerp(_position, targetPosition, t);
            SetPositionCore(newPos);
        }

        private void SetPositionCore(Vector2 value)
        {
            if (value != _position)
            {
                _position = value;
                EnforceBounds();
                isMatrixDirty = true;
            }
        }

        private void SetRotationCore(float value)
        {
            if (Math.Abs(value - _rotation) > 0.00001f)
            {
                _rotation = value;
                isMatrixDirty = true;
            }
        }

        private void SetZoomCore(float value)
        {
            float clamped = zoomRange.Clamp(value);
            if (Math.Abs(clamped - _zoom) > 0.00001f)
            {
                _zoom = clamped;
                EnforceBounds();
                isMatrixDirty = true;
            }
        }

        private void UpdateViewportDimensions()
        {
            this.viewportHeight = Game.ViewportAdapter.VirtualHeight;
            this.viewportWidth = Game.ViewportAdapter.VirtualWidth;
        }

#if WINDOWS
        private void Window_ClientSizeChanged(object? sender, EventArgs e)
        {
            UpdateViewportDimensions();
            EnforceBounds();
            isMatrixDirty = true;
        }
#endif

        /// <summary>
        /// Calcula una posición segura dentro de los límites del mundo basada en un nivel de zoom específico.
        /// Vital para pre-calcular destinos en movimientos cinemáticos.
        /// </summary>
        private Vector2 GetClampedPosition(Vector2 targetPos, float targetZoom)
        {
            float viewW = viewportWidth / targetZoom;
            float viewH = viewportHeight / targetZoom;

            float lB = viewW * 0.5f;
            float rB = Math.Max(SceneWidth - (viewW * 0.5f), lB);
            float tB = viewH * 0.5f;
            float bB = Math.Max(SceneHeight - (viewH * 0.5f), tB);

            bool canScrollH = ScrollLock != ScrollLock.Horizontal && ScrollLock != ScrollLock.All && (SceneWidth * targetZoom) > viewportWidth;
            bool canScrollV = ScrollLock != ScrollLock.Vertical && ScrollLock != ScrollLock.All && (SceneHeight * targetZoom) > viewportHeight;

            float destX = targetPos.X;
            float destY = targetPos.Y;

            if (canScrollH)
                destX = MathHelper.Clamp(destX, lB, rB);
            else
                destX = SceneWidth / 2f;

            if (canScrollV)
                destY = MathHelper.Clamp(destY, tB, bB);
            else
                destY = SceneHeight / 2f;

            return new Vector2(destX, destY);
        }

        private void EnforceBounds()
        {
            // 1. Calculamos cuánto mundo es visible
            float viewW = viewportWidth / _zoom;
            float viewH = viewportHeight / _zoom;

            // 2. Actualizamos las barreras cacheadas para consulta pública
            this.leftBarrier = viewW * 0.5f;
            this.rightBarrier = Math.Max(SceneWidth - (viewW * 0.5f), leftBarrier);
            this.topBarrier = viewH * 0.5f;
            this.bottomBarrier = Math.Max(SceneHeight - (viewH * 0.5f), topBarrier);

            // 3. Flags de scroll
            this.CanScrollHorizontally = ScrollLock != ScrollLock.Horizontal && ScrollLock != ScrollLock.All && (SceneWidth * _zoom) > viewportWidth;
            this.CanScrollVertically = ScrollLock != ScrollLock.Vertical && ScrollLock != ScrollLock.All && (SceneHeight * _zoom) > viewportHeight;

            if (isInitializing) return;

            // 4. Si la cámara NO está bajo control de un Tween absoluto, validamos su posición actual.
            // Si está volando, ignoramos el clamp para permitir que cruce curvas libremente hacia un destino legal.
            if (!IsMoving && !IsFlying)
            {
                Vector2 clampedPos = GetClampedPosition(_position, _zoom);
                if (_position != clampedPos)
                {
                    _position = clampedPos;
                    isMatrixDirty = true;
                }
            }

            // 5. Cajas de Visibilidad (Esto SIEMPRE debe actualizarse, estemos volando o no)
            float halfW = viewW / 2f;
            float halfH = viewH / 2f;

            VisibleBox = new RectangleF(_position.X - halfW, _position.Y - halfH, viewW, viewH);
            Offset = new Vector2(VisibleBox.X, VisibleBox.Y);

            CullingBox = RectangleF.Inflate(VisibleBox,
                VisibleBox.Width * (CullingBoxScale.X - 1),
                VisibleBox.Height * (CullingBoxScale.Y - 1));
        }

        #endregion

        #region Protected members

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
#if WINDOWS
                    Game.Window.ClientSizeChanged -= Window_ClientSizeChanged;
#endif
                }
                disposedValue = true;
            }
        }

        #endregion

        public float ApproachTolerance { get; set; } = 1.5f;

        public bool AtBottom => _position.Y >= bottomBarrier - 0.1f;
        public bool AtLeft => _position.X <= leftBarrier + 0.1f;
        public bool AtRight => _position.X >= rightBarrier - 0.1f;
        public bool AtTop => _position.Y <= topBarrier + 0.1f;

        public bool CanScrollHorizontally { get; private set; }
        public bool CanScrollVertically { get; private set; }

        public RectangleF CullingBox { get; private set; }
        public Vector2 CullingBoxScale { get; set; } = Vector2.One;

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void FocusCenter()
        {
            Target = null;
            StopMoving();
            StopFlying();
            SetPositionCore(new Vector2(SceneWidth / 2f, SceneHeight / 2f));
        }

        public void FocusTarget()
        {
            if (Target != null)
            {
                StopMoving();
                StopFlying();
                SetPositionCore(Target.Position);
            }
        }

        public void Follow(ITransform target, bool focus = false)
        {
            Target = target;
            if (focus) FocusTarget();
        }

        public EngendroGame Game { get; }

        public Matrix GetViewMatrix(Vector2 parallaxFactor)
        {
            if (parallaxFactor == Vector2.Zero)
                return Game.ViewportAdapter.TransformationMatrix;

            float extraX = _position.X * (parallaxFactor.X - 1f);
            float extraY = _position.Y * (parallaxFactor.Y - 1f);
            float shakeX = shakeHorzTween.IsRunning ? shakeHorzTween.CurrentValue : 0f;
            float shakeY = shakeVertTween.IsRunning ? shakeVertTween.CurrentValue : 0f;

            camTranslationVector.X = -(_position.X + extraX) - shakeX;
            camTranslationVector.Y = -(_position.Y + extraY) - shakeY;
            Matrix.CreateTranslation(ref camTranslationVector, out camTranslationMatrix);

            return camTranslationMatrix *
                   rotationTranslationMatrix *
                   scaleMatrix *
                   resTranslationMatrix * Game.ViewportAdapter.TransformationMatrix;
        }

        public Matrix GetTransformationMatrix()
        {
            if (isMatrixDirty)
            {
                float shakeX = shakeHorzTween.IsRunning ? shakeHorzTween.CurrentValue : 0f;
                float shakeY = shakeVertTween.IsRunning ? shakeVertTween.CurrentValue : 0f;

                camTranslationVector.X = -_position.X - shakeX;
                camTranslationVector.Y = -_position.Y - shakeY;
                Matrix.CreateTranslation(ref camTranslationVector, out camTranslationMatrix);

                scaleVector.X = _zoom;
                scaleVector.Y = _zoom;
                Matrix.CreateScale(ref scaleVector, out scaleMatrix);

                resTranslationVector.X = viewportWidth * .5f;
                resTranslationVector.Y = viewportHeight * .5f;
                Matrix.CreateTranslation(ref resTranslationVector, out resTranslationMatrix);

                Matrix.CreateRotationZ(_rotation, out rotationTranslationMatrix);

                transformationMatrix = camTranslationMatrix *
                                       rotationTranslationMatrix *
                                       scaleMatrix *
                                       resTranslationMatrix *
                                       Game.ViewportAdapter.TransformationMatrix;

                isMatrixDirty = false;
            }

            return transformationMatrix;
        }

        public bool IsMoving => moveTween.IsRunning;
        public bool IsFlying => flightTween.IsRunning;
        public bool IsRotating => rotationTween.IsRunning;

        public bool IsTargetFocused => Target != null && Vector2.Distance(_position, Target.Position) < ApproachTolerance;

        /// <summary>
        /// Combina movimiento y zoom en una sola curva de interpolación.
        /// Resuelve matemáticamente la trayectoria recta (Efecto Ken Burns).
        /// </summary>
        public void FlyTo(TweenStyle tweenStyle, Vector2 destination, float targetZoom, int duration)
        {
            Target = null;
            StopMoving();
            StopZooming();
            StopFlying();

            // Pre-calculamos el destino legal basado en el zoom FINAL, no en el actual.
            float clampedTargetZoom = zoomRange.Clamp(targetZoom);
            flightEndPos = GetClampedPosition(destination, clampedTargetZoom);
            flightStartPos = _position;

            // Interpolación focal: Animamos 1/Zoom para trayectorias rectilíneas puras
            flightStartInvZoom = 1f / _zoom;
            flightEndInvZoom = 1f / clampedTargetZoom;

            // Usamos un FloatTween de 0f a 1f como nuestro reloj maestro de Easing
            flightTween.Start(tweenStyle, 0f, 1f, duration);
        }

        public void MoveTo(TweenStyle tweenStyle, Vector2 destination, int duration)
        {
            Target = null;
            StopFlying();
            // Evita estamparse pre-calculando el destino seguro con el zoom actual
            Vector2 safeDestination = GetClampedPosition(destination, _zoom);
            moveTween.Start(tweenStyle, Position, safeDestination, duration);
        }

        public string Name { get; }
        public Vector2 Offset { get; private set; }

        public Vector2 Position
        {
            get => _position;
            set
            {
                StopMoving();
                StopFlying();
                Target = null;
                SetPositionCore(value);
            }
        }

        public void Reset()
        {
            StopShaking();
            StopMoving();
            StopFlying();
            StopFollowing();
            StopZooming();
            StopRotating();

            _zoom = 1;
            _rotation = 0;
            isMatrixDirty = true;
            FocusCenter();
        }

        public void Rotate(TweenStyle tweenStyle, float rotationValue, int duration, int bounceCount = 0)
        {
            if (_rotation == rotationValue) return;
            rotationTween.Start(tweenStyle, _rotation, rotationValue, duration, bounceCount);
        }

        public float Rotation
        {
            get => _rotation;
            set
            {
                StopRotating();
                SetRotationCore(value);
            }
        }

        public int SceneHeight { get; private set; }
        public int SceneWidth { get; private set; }
        public ScrollLock ScrollLock { get; set; }

        public void Setup(int sceneWidth, int sceneHeight, ScrollLock scrollLock = ScrollLock.None, float zoom = 1)
        {
            isInitializing = true;
            Reset();

            this.SceneWidth = sceneWidth;
            this.SceneHeight = sceneHeight;
            this.ScrollLock = scrollLock;

            SetZoomCore(zoom);

            UpdateViewportDimensions();
            isInitializing = false;

            EnforceBounds();
            SetPositionCore(new Vector2(sceneWidth / 2f, sceneHeight / 2f));
        }

        public void Shake(TweenStyle tweenStyle, Vector2 intensity, int duration, int bounces)
        {
            ShakeHorizontally(tweenStyle, intensity.X, duration, bounces);
            ShakeVertically(tweenStyle, intensity.Y, duration, bounces);
        }

        public void ShakeHorizontally(TweenStyle tweenStyle, float intensity, int duration, int bounces)
        {
            shakeHorzTween.Start(tweenStyle, -intensity, intensity, duration, bounces);
        }

        public CameraShakeState ShakeState
        {
            get
            {
                if (shakeHorzTween.IsRunning && shakeVertTween.IsRunning) return CameraShakeState.XY;
                else if (shakeHorzTween.IsRunning) return CameraShakeState.X;
                else if (shakeVertTween.IsRunning) return CameraShakeState.Y;
                else return CameraShakeState.None;
            }
        }

        public void ShakeVertically(TweenStyle tweenStyle, float intensity, int duration, int bounces)
        {
            shakeVertTween.Start(tweenStyle, -intensity, intensity, duration, bounces);
        }

        public float SmoothSpeed { get; set; } = .01f;

        public void StopFollowing()
        {
            Target = null;
        }

        public void StopMoving()
        {
            moveTween.Stop();
        }

        public void StopFlying()
        {
            flightTween.Stop();
        }

        public void StopRotating()
        {
            rotationTween.Stop();
        }

        public void StopShaking()
        {
            shakeHorzTween.Stop();
            shakeVertTween.Stop();
        }
        public void StopZooming()
        {
            zoomTween.Stop();
        }

        public ITransform? Target { get; private set; }

        public override string ToString()
        {
            return Name;
        }

        public void Update(GameTime gameTime)
        {
            // Shake visual puramente
            if (ShakeState != CameraShakeState.None)
            {
                if (shakeHorzTween.IsRunning) shakeHorzTween.Update(gameTime);
                if (shakeVertTween.IsRunning) shakeVertTween.Update(gameTime);
                isMatrixDirty = true;
            }

            if (rotationTween.IsRunning)
            {
                rotationTween.Update(gameTime);
                SetRotationCore(rotationTween.CurrentValue);
            }

            // Exclusividad Cinematica: Si volamos, matamos el update individual
            if (IsFlying)
            {
                flightTween.Update(gameTime);
                float t = flightTween.CurrentValue;

                // Interpolación Focal (Inverse Zoom Lerp)
                float currentInvZoom = MathHelper.Lerp(flightStartInvZoom, flightEndInvZoom, t);
                _zoom = 1f / currentInvZoom;

                // Posición Lineal Pura
                _position = Vector2.Lerp(flightStartPos, flightEndPos, t);

                isMatrixDirty = true;
                EnforceBounds(); // Actualiza volúmenes de visibilidad sin clampear (gracias al IsFlying == true)
            }
            else
            {
                // Fallback a los tweens independientes si no hay FlyTo activo
                if (zoomTween.IsRunning)
                {
                    zoomTween.Update(gameTime);
                    SetZoomCore(zoomTween.CurrentValue);
                }

                if (IsMoving)
                {
                    moveTween.Update(gameTime);
                    SetPositionCore(moveTween.CurrentValue);
                }
                else if (Target != null)
                {
                    if (CanScrollHorizontally || CanScrollVertically)
                        ApproachCore(Target.Position, gameTime);
                }
                else
                {
                    EnforceBounds(); // Caso estático o de resize de ventana
                }
            }
        }

        public RectangleF VisibleBox { get; private set; }

        public float Zoom
        {
            get => _zoom;
            set
            {
                StopZooming();
                StopFlying();
                SetZoomCore(value);
            }
        }

        public ZoomState ZoomState
        {
            get
            {
                if (zoomTween.IsRunning)
                    return zoomTween.EndValue > zoomTween.StartValue ? ZoomState.In : ZoomState.Out;
                if (flightTween.IsRunning) // Soporte para el estado mientras vuela
                    return flightEndInvZoom < flightStartInvZoom ? ZoomState.In : ZoomState.Out;

                return ZoomState.None;
            }
        }

        public void ZoomTo(TweenStyle tweenStyle, float zoomValue, int duration, int bounceCount = 0)
        {
            if (Math.Abs(_zoom - zoomValue) < 0.0001f) return;
            StopFlying();
            zoomTween.Start(tweenStyle, _zoom, zoomValue, duration, bounceCount);
        }
    }
}