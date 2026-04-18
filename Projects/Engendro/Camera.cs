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

        // Constructor
        public Camera(string name)
            : this(name, 0, 0)
        {
        }

        // Constructor
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

        // ApproachCore
        private void ApproachCore(Vector2 targetPosition, GameTime gameTime)
        {
            if (IsTargetFocused)
                return;

            float zoomFactor = Math.Max(_zoom, 1f);
            float timeFactor = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float t = SmoothSpeed / zoomFactor * timeFactor;
            t = MathHelper.Clamp(t, 0f, 1f);

            Vector2 newPos = Vector2.Lerp(_position, targetPosition, t);

            // CRÍTICO: Usar el método interno. 
            // Si usáramos "Position = ...", el setter público pondría Target = null,
            // rompiendo la persecución en el primer frame.
            SetPositionCore(newPos);
        }

        // SetPositionCore
        private void SetPositionCore(Vector2 value)
        {
            // Chequeo de redundancia para evitar ciclos de CPU innecesarios
            if (value != _position)
            {
                _position = value;

                // Aplicamos las reglas del mundo (Clamp, ScrollLock, etc)
                // Esto puede modificar _position nuevamente si se sale de los límites.
                EnforceBounds();

                isMatrixDirty = true;
            }
        }

        // SetRotationCore
        private void SetRotationCore(float value)
        {
            if (Math.Abs(value - _rotation) > 0.00001f)
            {
                _rotation = value;
                isMatrixDirty = true;
            }
        }

        // SetZoomCore
        private void SetZoomCore(float value)
        {
            float clamped = zoomRange.Clamp(value);

            // Tolerancia epsilon para evitar float drift
            if (Math.Abs(clamped - _zoom) > 0.00001f)
            {
                _zoom = clamped;

                // Al cambiar el zoom, cambia el tamaño del mundo visible.
                // Es OBLIGATORIO recalcular los límites inmediatamente.
                EnforceBounds();

                isMatrixDirty = true;
            }
        }

        // UpdateViewportDimensions
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

        // EnforceBounds
        private void EnforceBounds()
        {
            // 1. Calculamos cuánto mundo es visible
            float viewW = viewportWidth / _zoom;
            float viewH = viewportHeight / _zoom;

            // 2. Actualizamos las barreras
            this.leftBarrier = viewW * 0.5f;
            this.rightBarrier = Math.Max(SceneWidth - (viewW * 0.5f), leftBarrier);
            this.topBarrier = viewH * 0.5f;
            this.bottomBarrier = Math.Max(SceneHeight - (viewH * 0.5f), topBarrier);

            // 3. Flags de scroll
            this.CanScrollHorizontally = ScrollLock != ScrollLock.Horizontal && ScrollLock != ScrollLock.All && (SceneWidth * _zoom) > viewportWidth;
            this.CanScrollVertically = ScrollLock != ScrollLock.Vertical && ScrollLock != ScrollLock.All && (SceneHeight * _zoom) > viewportHeight;

            if (isInitializing) return;

            // 4. Clamp sobre la posición actual
            float x = _position.X;
            float y = _position.Y;

            if (CanScrollHorizontally)
                x = MathHelper.Clamp(x, leftBarrier, rightBarrier);
            else
                x = SceneWidth / 2f;

            if (CanScrollVertically)
                y = MathHelper.Clamp(y, topBarrier, bottomBarrier);
            else
                y = SceneHeight / 2f;

            // Actualizamos _position directamente (ya estamos en un contexto privado/seguro)
            if (_position.X != x || _position.Y != y)
            {
                _position.X = x;
                _position.Y = y;
                isMatrixDirty = true;
            }

            // 5. Actualizamos Cajas de Visibilidad
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

        // Dispose
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

        // ApproachTolerance
        public float ApproachTolerance { get; set; } = 1.5f;

        // AtBottom
        public bool AtBottom => _position.Y >= bottomBarrier - 0.1f;

        // AtLeft 
        public bool AtLeft => _position.X <= leftBarrier + 0.1f;

        // AtRight
        public bool AtRight => _position.X >= rightBarrier - 0.1f;

        // AtTop
        public bool AtTop => _position.Y <= topBarrier + 0.1f;

        // CanScrollHorizontally 
        public bool CanScrollHorizontally { get; private set; }

        // CanScrollVertically
        public bool CanScrollVertically { get; private set; }

        // CullingBox
        public RectangleF CullingBox { get; private set; }

        // CullingBoxScale 
        public Vector2 CullingBoxScale { get; set; } = Vector2.One;

        // Dispose
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        // FocusCenter
        public void FocusCenter()
        {
            Target = null; // Dejamos de seguir
            StopMoving();
            SetPositionCore(new Vector2(SceneWidth / 2f, SceneHeight / 2f));
        }

        // FocusTarget
        public void FocusTarget()
        {
            if (Target != null)
            {
                StopMoving();
                // NO ponemos Target = null aquí, porque explícitamente queremos enfocarlo.
                // Usamos el setter interno para evitar la anulación que hace el setter público.
                SetPositionCore(Target.Position);
            }
        }

        // Follow
        public void Follow(ITransform target)
        {
            Follow(target, false);
        }

        // Follow
        public void Follow(ITransform target, bool focus)
        {
            Target = target;
            if (focus) FocusTarget();
        }

        // Game
        public EngendroGame Game { get; }

        // GetViewMatrix
        public Matrix GetViewMatrix(Vector2 parallaxFactor)
        {
            // 1. Calculamos el desfase relativo (Parallax)
            float extraX = _position.X * (parallaxFactor.X - 1f);
            float extraY = _position.Y * (parallaxFactor.Y - 1f);

            // 2. IMPORTANTE: Capturamos el Shake actual
            float shakeX = shakeHorzTween.IsRunning ? shakeHorzTween.CurrentValue : 0f;
            float shakeY = shakeVertTween.IsRunning ? shakeVertTween.CurrentValue : 0f;

            // 3. Aplicamos el desplazamiento completo:
            // Posición invertida + Desfase Parallax + Shake
            // Nota: El shake se suma a la posición de la cámara, por lo que en la vista es negativo
            camTranslationVector.X = -_position.X - extraX - shakeX;
            camTranslationVector.Y = -_position.Y - extraY - shakeY;
            camTranslationVector.Z = 0;

            Matrix.CreateTranslation(ref camTranslationVector, out camTranslationMatrix);

            // 4. Multiplicación final manteniendo la jerarquía
            // El Shake ahora será procesado antes del Zoom y el Centrado, 
            // lo que garantiza que todo el frame tiemble coordinadamente.
            return camTranslationMatrix *
                   rotationTranslationMatrix *
                   scaleMatrix *
                   resTranslationMatrix * Game.ViewportAdapter.TransformationMatrix;
        }

        // GetTransformationMatrix
        public Matrix GetTransformationMatrix()
        {
            if (isMatrixDirty)
            {
                // Separamos visualmente el Shake
                float shakeX = shakeHorzTween.IsRunning ? shakeHorzTween.CurrentValue : 0f;
                float shakeY = shakeVertTween.IsRunning ? shakeVertTween.CurrentValue : 0f;

                // 1. Invertir posición (Mundo -> Vista).
                // CORRECCIÓN MATEMÁTICA: El shake es un offset a la posición de la cámara.
                // Si la cámara se mueve a la derecha (+X), el mundo se mueve a la izquierda (-X).
                // Por tanto, es -(Position + Shake) => -Position - Shake.
                var posX = -_position.X - shakeX;
                var posY = -_position.Y - shakeY;

                camTranslationVector.X = posX;
                camTranslationVector.Y = posY;
                Matrix.CreateTranslation(ref camTranslationVector, out camTranslationMatrix);

                // 2. Escalar (Zoom)
                scaleVector.X = _zoom;
                scaleVector.Y = _zoom;
                Matrix.CreateScale(ref scaleVector, out scaleMatrix);

                // 3. Centrar origen en la pantalla
                resTranslationVector.X = viewportWidth * .5f;
                resTranslationVector.Y = viewportHeight * .5f;
                Matrix.CreateTranslation(ref resTranslationVector, out resTranslationMatrix);

                // 4. Rotar
                Matrix.CreateRotationZ(_rotation, out rotationTranslationMatrix);

                // Multiplicación final
                transformationMatrix = camTranslationMatrix *
                                       rotationTranslationMatrix *
                                       scaleMatrix *
                                       resTranslationMatrix *
                                       Game.ViewportAdapter.TransformationMatrix;

                isMatrixDirty = false;
            }

            return transformationMatrix;
        }

        // IsMoving
        public bool IsMoving => moveTween.IsRunning;

        // IsRotating
        public bool IsRotating => rotationTween.IsRunning;

        // IsTargetFocused
        public bool IsTargetFocused => Target != null && Vector2.Distance(_position, Target.Position) < ApproachTolerance;

        // MoveTo
        public void MoveTo(TweenStyle tweenStyle, Vector2 destination, int duration)
        {
            Target = null; // MoveTo es incompatible con seguir un target
            moveTween.Start(tweenStyle, Position, destination, duration);
        }

        // Name
        public string Name { get; }

        // Offset
        public Vector2 Offset { get; private set; }

        // Position
        public Vector2 Position
        {
            get => _position;
            set
            {
                StopMoving();
                Target = null; // Intervención manual rompe el seguimiento automático
                SetPositionCore(value);
            }
        }

        // Reset
        public void Reset()
        {
            StopShaking();
            StopMoving();
            StopFollowing();
            StopZooming();
            StopRotating();

            _zoom = 1;
            _rotation = 0;
            isMatrixDirty = true;
            FocusCenter();
        }

        // Rotate
        public void Rotate(TweenStyle tweenStyle, float rotationValue, int duration, int bounceCount = 0)
        {
            if (_rotation == rotationValue) return;
            rotationTween.Start(tweenStyle, _rotation, rotationValue, duration, bounceCount);
        }

        // Rotation
        public float Rotation
        {
            get => _rotation;
            set
            {
                StopRotating();
                SetRotationCore(value);
            }
        }

        // SceneHeight
        public int SceneHeight { get; private set; }

        // SceneWidth
        public int SceneWidth { get; private set; }

        // ScrollLock
        public ScrollLock ScrollLock { get; set; }

        // Setup
        public void Setup(int sceneWidth, int sceneHeight, ScrollLock scrollLock = ScrollLock.None, float zoom = 1)
        {
            isInitializing = true;
            Reset();

            this.SceneWidth = sceneWidth;
            this.SceneHeight = sceneHeight;
            this.ScrollLock = scrollLock;

            SetZoomCore(zoom); // Usa interno para evitar conflictos

            UpdateViewportDimensions();
            isInitializing = false;

            EnforceBounds();
            // Inicializar en el centro usando interno para no disparar eventos
            SetPositionCore(new Vector2(sceneWidth / 2f, sceneHeight / 2f));
        }

        // Shake
        public void Shake(TweenStyle tweenStyle, Vector2 intensity, int duration, int bounces)
        {
            ShakeHorizontally(tweenStyle, intensity.X, duration, bounces);
            ShakeVertically(tweenStyle, intensity.Y, duration, bounces);
        }

        // ShakeHorizontally
        public void ShakeHorizontally(TweenStyle tweenStyle, float intensity, int duration, int bounces)
        {
            shakeHorzTween.Start(tweenStyle, -intensity, intensity, duration, bounces);
        }

        // ShakeState
        public CameraShakeState ShakeState
        {
            get
            {
                if (shakeHorzTween.IsRunning && shakeVertTween.IsRunning)
                    return CameraShakeState.XY;

                else if (shakeHorzTween.IsRunning)
                    return CameraShakeState.X;

                else if (shakeVertTween.IsRunning)
                    return CameraShakeState.Y;

                else
                    return CameraShakeState.None;
            }
        }

        // ShakeVertically
        public void ShakeVertically(TweenStyle tweenStyle, float intensity, int duration, int bounces)
        {
            shakeVertTween.Start(tweenStyle, -intensity, intensity, duration, bounces);
        }

        // SmoothSpeed
        public float SmoothSpeed { get; set; } = .01f;

        // StopFollowing
        public void StopFollowing()
        {
            Target = null;
        }

        // StopMoving
        public void StopMoving()
        {
            moveTween.Stop();
        }

        // StopRotating
        public void StopRotating()
        {
            rotationTween.Stop();
        }

        // StopShaking
        public void StopShaking()
        {
            shakeHorzTween.Stop();
            shakeVertTween.Stop();
        }

        // StopZooming
        public void StopZooming()
        {
            zoomTween.Stop();
        }

        // Target
        public ITransform? Target { get; private set; }

        // ToString
        public override string ToString()
        {
            return Name;
        }

        // Update
        public void Update(GameTime gameTime)
        {
            // --- ACTUALIZACIÓN DE TWEENS ---
            // Usamos siempre los setters INTERNAL para evitar que el setter público
            // detecte el cambio como "intervención de usuario" y detenga el tween.

            // Shake (Visual only, dirty flag handled in GetMatrix logic mostly, but good to ensure dirty here)
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
                // Si el mapa es más chico que la pantalla, no tiene sentido seguir al jugador
                if (CanScrollHorizontally || CanScrollVertically)
                    ApproachCore(Target.Position, gameTime);
            }
            else
            {
                // Caso borde: Resize de ventana o cambio de zoom estático que requiere revalidar límites
                EnforceBounds();
            }
        }

        // VisibleBox
        public RectangleF VisibleBox { get; private set; }

        // Zoom
        public float Zoom
        {
            get => _zoom;
            set
            {
                StopZooming();
                SetZoomCore(value);
            }
        }

        // ZoomState
        public ZoomState ZoomState
        {
            get
            {
                if (zoomTween.IsRunning)
                    return zoomTween.EndValue > zoomTween.StartValue ? ZoomState.In : ZoomState.Out;
                return ZoomState.None;
            }
        }

        // ZoomTo
        public void ZoomTo(TweenStyle tweenStyle, float zoomValue, int duration, int bounceCount = 0)
        {
            if (Math.Abs(_zoom - zoomValue) < 0.0001f) return;
            zoomTween.Start(tweenStyle, _zoom, zoomValue, duration, bounceCount);
        }
    }
}