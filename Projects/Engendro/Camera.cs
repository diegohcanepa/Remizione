using Microsoft.Xna.Framework;
using System;

namespace Engendro
{
    /// <summary>
    /// Camera
    /// </summary>
    public partial class Camera : IDisposable
    {
        #region Private fields

        // Estado interno
        private Vector2 _position;
        private float _zoom = 1f;
        private float _rotation = 0f;

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

        // Tweens (Compatibles con tu sistema)
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

        public Camera(EngendroGame game, string name)
            : this(game, name, 0, 0)
        {
        }

        public Camera(EngendroGame game, string name, int sceneWidth, int sceneHeight)
        {
            this.Game = game;
            this.Name = name;

            UpdateViewportDimensions();

#if WINDOWS
            game.Window.ClientSizeChanged += Window_ClientSizeChanged;
#endif

            Setup(sceneWidth, sceneHeight);
        }

        #endregion

        #region Private members

        private void UpdateViewportDimensions()
        {
            this.viewportHeight = Game.ViewportAdapter.VirtualHeight;
            this.viewportWidth = Game.ViewportAdapter.VirtualWidth;
        }

        private void Approach(Vector2 targetPosition, GameTime gameTime)
        {
            if (IsTargetFocused)
                return;

            // --- LÓGICA RESTAURADA (Idéntica a tu versión original) ---
            float zoomFactor = Math.Max(Zoom, 1f);
            float timeFactor = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Tu cálculo original de velocidad
            float t = SmoothSpeed / zoomFactor * timeFactor;
            t = MathHelper.Clamp(t, 0f, 1f);

            // Interpolamos hacia el objetivo.
            // Al asignar a Position, EnforceBounds se encargará de los límites,
            // pero el movimiento será exactamente como lo tenías.
            Position = Vector2.Lerp(Position, targetPosition, t);
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
        /// Método centralizado que asegura que la cámara no salga del mapa.
        /// Reemplaza la lógica dispersa que tenías en los setters.
        /// </summary>
        private void EnforceBounds()
        {
            // 1. Calculamos cuánto mundo es visible
            float viewW = viewportWidth / Zoom;
            float viewH = viewportHeight / Zoom;

            // 2. Actualizamos las barreras (usadas por propiedades públicas como AtLeft)
            this.leftBarrier = viewW * 0.5f;
            this.rightBarrier = Math.Max(SceneWidth - (viewW * 0.5f), leftBarrier);
            this.topBarrier = viewH * 0.5f;
            this.bottomBarrier = Math.Max(SceneHeight - (viewH * 0.5f), topBarrier);

            // 3. Determinamos flags de scroll
            this.CanScrollHorizontally = ScrollLock != ScrollLock.Horizontal && ScrollLock != ScrollLock.All && (SceneWidth * Zoom) > viewportWidth;
            this.CanScrollVertically = ScrollLock != ScrollLock.Vertical && ScrollLock != ScrollLock.All && (SceneHeight * Zoom) > viewportHeight;

            if (isInitializing) return;

            // 4. Aplicamos Clamp a la posición actual (_position)
            float x = _position.X;
            float y = _position.Y;

            if (CanScrollHorizontally)
                x = MathHelper.Clamp(x, leftBarrier, rightBarrier);
            else
                x = SceneWidth / 2f; // Centrar si el mapa es más chico que la vista

            if (CanScrollVertically)
                y = MathHelper.Clamp(y, topBarrier, bottomBarrier);
            else
                y = SceneHeight / 2f; // Centrar si el mapa es más chico que la vista

            // Asignación directa al campo para evitar recursión infinita
            if (_position.X != x || _position.Y != y)
            {
                _position.X = x;
                _position.Y = y;
                isMatrixDirty = true;
            }

            // 5. Actualizamos VisibleBox, Offset y CullingBox basados en la posición final validada
            float halfW = viewW / 2f;
            float halfH = viewH / 2f;

            VisibleBox = new RectangleF(_position.X - halfW, _position.Y - halfH, viewW, viewH);

            // Offset restaurado: Es la esquina superior izquierda del viewport
            Offset = new Vector2(VisibleBox.X, VisibleBox.Y);

            // CullingBox restaurado
            CullingBox = RectangleF.Inflate(VisibleBox,
                VisibleBox.Width * (CullingBoxScale.X - 1),
                VisibleBox.Height * (CullingBoxScale.Y - 1));
        }

        // Propiedad legacy para compatibilidad interna con tu código original
        private float ZoomCore
        {
            get => Zoom;
            set => Zoom = value;
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

        // --- PUBLIC PROPERTIES & METHODS ---

        public float ApproachTolerance { get; set; } = 1.5f;

        // Propiedades de estado (calculadas contra las barreras actualizadas en EnforceBounds)
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
            // Simplemente apuntamos al centro. EnforceBounds corregirá si es necesario.
            Position = new Vector2(SceneWidth / 2f, SceneHeight / 2f);
        }

        public void FocusTarget()
        {
            if (Target != null)
            {
                StopMoving();
                Position = Target.Position;
            }
        }

        public void Follow(ITransform target) => Follow(target, false);

        public void Follow(ITransform target, bool focus)
        {
            Target = target;
            if (focus) FocusTarget();
        }

        public EngendroGame Game { get; }

        public Matrix GetTransformationMatrix()
        {
            if (isMatrixDirty)
            {
                // Separamos visualmente el Shake de la posición lógica para evitar problemas de offset
                float shakeX = shakeHorzTween.IsRunning ? shakeHorzTween.CurrentValue : 0f;
                float shakeY = shakeVertTween.IsRunning ? shakeVertTween.CurrentValue : 0f;

                // 1. Invertir posición (Mundo -> Vista) + Shake
                var posX = -_position.X + shakeX;
                var posY = -_position.Y + shakeY;

                camTranslationVector.X = posX;
                camTranslationVector.Y = posY;
                Matrix.CreateTranslation(ref camTranslationVector, out camTranslationMatrix);

                // 2. Escalar (Zoom)
                scaleVector.X = Zoom;
                scaleVector.Y = Zoom;
                Matrix.CreateScale(ref scaleVector, out scaleMatrix);

                // 3. Centrar origen en la pantalla
                resTranslationVector.X = viewportWidth * .5f;
                resTranslationVector.Y = viewportHeight * .5f;
                Matrix.CreateTranslation(ref resTranslationVector, out resTranslationMatrix);

                // 4. Rotar
                Matrix.CreateRotationZ(Rotation, out rotationTranslationMatrix);

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

        public bool IsTargetFocused => Target != null && Vector2.Distance(Position, Target.Position) < ApproachTolerance;
        public bool IsMoving => moveTween.IsRunning;
        public bool IsRotating => rotationTween.IsRunning;

        public void MoveTo(TweenStyle tweenStyle, Vector2 destination, int duration)
        {
            Target = null;
            moveTween.Start(tweenStyle, Position, destination, duration);
        }

        public string Name { get; }

        // Offset restaurado
        public Vector2 Offset { get; private set; }

        public Vector2 Position
        {
            get => _position;
            set
            {
                if (value != _position)
                {
                    _position = value;
                    // Forzamos validación inmediata:
                    // Esto recalcula límites, visibleBox y Offset al instante.
                    EnforceBounds();
                    isMatrixDirty = true;
                }
            }
        }

        public void Reset()
        {
            StopShaking();
            StopMoving();
            StopFollowing();
            StopZooming();
            StopRotating();

            Zoom = 1;
            Rotation = 0;
            FocusCenter();
        }

        public void Rotate(TweenStyle tweenStyle, float rotationValue, int duration, int bounceCount = 0)
        {
            if (Rotation == rotationValue) return;
            rotationTween.Start(tweenStyle, Rotation, rotationValue, duration, bounceCount);
        }

        public float Rotation
        {
            get => _rotation;
            set
            {
                if (value != _rotation)
                {
                    _rotation = value;
                    isMatrixDirty = true;
                }
            }
        }

        public int SceneHeight { get; private set; }
        public int SceneWidth { get; private set; }
        public ScrollLock ScrollLock { get; set; }

        public void Setup(int sceneWidth, int sceneHeight, ScrollLock scrollLock = ScrollLock.None, float zoom = 1)
        {
            isInitializing = true;
            Reset(); // Valores base

            this.SceneWidth = sceneWidth;
            this.SceneHeight = sceneHeight;
            this.ScrollLock = scrollLock;

            // Asignamos Zoom directamente al field para evitar recalculos prematuros
            this._zoom = zoomRange.Clamp(zoom);

            // Recalculamos dimensiones
            UpdateViewportDimensions();
            isInitializing = false;

            // Aplicamos límites y posicionamos en el centro
            EnforceBounds();
            this.Position = new Vector2(sceneWidth / 2f, sceneHeight / 2f);
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

        public void StopFollowing() => Target = null;
        public void StopMoving() => moveTween.Stop();
        public void StopRotating() => rotationTween.Stop();
        public void StopShaking()
        {
            shakeHorzTween.Stop();
            shakeVertTween.Stop();
        }
        public void StopZooming() => zoomTween.Stop();

        public ITransform? Target { get; private set; }

        public override string ToString() => Name;

        public void Update(GameTime gameTime)
        {
            // Actualización de Tweens de Shake (solo visual)
            if (ShakeState != CameraShakeState.None)
            {
                if (shakeHorzTween.IsRunning) shakeHorzTween.Update(gameTime);
                if (shakeVertTween.IsRunning) shakeVertTween.Update(gameTime);
                isMatrixDirty = true;
            }

            if (rotationTween.IsRunning)
            {
                rotationTween.Update(gameTime);
                Rotation = rotationTween.CurrentValue;
            }

            if (zoomTween.IsRunning)
            {
                zoomTween.Update(gameTime);
                Zoom = zoomTween.CurrentValue;
            }

            if (IsMoving)
            {
                moveTween.Update(gameTime);
                // El setter llama a EnforceBounds automáticamente
                Position = moveTween.CurrentValue;
            }
            else if (Target != null)
            {
                if (CanScrollHorizontally || CanScrollVertically)
                    Approach(Target.Position, gameTime);
            }
            else
            {
                // CRÍTICO: Aunque no nos movamos, debemos asegurar límites 
                // por si cambió el Zoom o el tamaño de ventana.
                EnforceBounds();
            }
        }

        public RectangleF VisibleBox { get; private set; }

        public float Zoom
        {
            get => _zoom;
            set
            {
                float clamped = zoomRange.Clamp(value);
                // Usamos una tolerancia pequeña para evitar dirty flags innecesarios
                if (Math.Abs(clamped - _zoom) > 0.00001f)
                {
                    _zoom = clamped;
                    StopZooming();
                    isMatrixDirty = true;
                    // Al cambiar el zoom, el área visible cambia, debemos recalcular límites ya.
                    EnforceBounds();
                }
            }
        }

        public void ZoomTo(TweenStyle tweenStyle, float zoomValue, int duration, int bounceCount = 0)
        {
            if (Math.Abs(Zoom - zoomValue) < 0.0001f) return;
            zoomTween.Start(tweenStyle, Zoom, zoomValue, duration, bounceCount);
        }

        public ZoomState ZoomState
        {
            get
            {
                if (zoomTween.IsRunning)
                    return zoomTween.EndValue > zoomTween.StartValue ? ZoomState.In : ZoomState.Out;
                return ZoomState.None;
            }
        }
    }
}