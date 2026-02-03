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

        private float bottomBarrier;
        private Matrix camTranslationMatrix = Matrix.Identity;
        private Vector3 camTranslationVector = Vector3.Zero;
        private bool disposedValue;
        private bool isMatrixDirty = true;
        private bool isInitializing;
        private float leftBarrier;
        private const float maxZoom = 999;
        private const float minZoom = .1f;
        private readonly Vector2Tween moveTween = new();
        private Vector2 position;
        private Matrix resTranslationMatrix = Matrix.Identity;
        private Vector3 resTranslationVector = Vector3.Zero;
        private float rightBarrier;
        private readonly FloatTween rotationTween = new();
        private Matrix rotationTranslationMatrix = Matrix.Identity;
        private Matrix scaleMatrix = Matrix.Identity;
        private Vector3 scaleVector = Vector3.One;
        private readonly FloatTween shakeHorzTween = new();
        private readonly FloatTween shakeVertTween = new();
        private float topBarrier;
        private Matrix transformationMatrix;
        private readonly int viewportHeight;
        private readonly int viewportWidth;
        private readonly FloatRange zoomRange = new(minZoom, maxZoom);
        private readonly FloatTween zoomTween = new();

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
            this.viewportHeight = game.ViewportAdapter.VirtualHeight;
            this.viewportWidth = game.ViewportAdapter.VirtualWidth;

#if WINDOWS
            game.Window.ClientSizeChanged += Window_ClientSizeChanged;
#endif

            Setup(sceneWidth, sceneHeight);
        }

        #endregion

        #region Private members

        private void Approach(Vector2 targetPosition, GameTime gameTime)
        {
            if (IsTargetFocused)
                return;

            float zoomFactor = Math.Max(Zoom, 1f);
            float timeFactor = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Ajustamos la velocidad según zoom y delta time
            float t = SmoothSpeed / zoomFactor * timeFactor;
            t = MathHelper.Clamp(t, 0f, 1f);

            // Interpolamos hacia el objetivo
            // El setter de Position se encargará de frenar el movimiento si llegamos a un borde
            Position = Vector2.Lerp(Position, targetPosition, t);
        }

#if WINDOWS
        private void Window_ClientSizeChanged(object? sender, EventArgs e)
        {
            isMatrixDirty = true;
        }
#endif

        private void InvalidateLimits()
        {
            // Calculamos cuánto mundo es visible
            var vw = viewportWidth / ZoomCore;
            var vh = viewportHeight / ZoomCore;

            // Definimos los límites donde la cámara puede poner su CENTRO
            this.leftBarrier = vw * .5f;
            this.rightBarrier = SceneWidth - (vw * .5f);
            this.bottomBarrier = SceneHeight - (vh * .5f);
            this.topBarrier = vh * .5f;

            // Determinamos si es necesario hacer scroll
            this.CanScrollHorizontally = ScrollLock != ScrollLock.Horizontal && ScrollLock != ScrollLock.All && (SceneWidth * ZoomCore) > viewportWidth;
            this.CanScrollVertically = ScrollLock != ScrollLock.Vertical && ScrollLock != ScrollLock.All && (SceneHeight * ZoomCore) > viewportHeight;
        }

        private float ZoomCore
        {
            get;
            set
            {
                value = zoomRange.Clamp(value);

                // Solo recalculamos si hubo cambio real
                if (Math.Abs(value - field) > 0.00001f)
                {
                    field = value;
                    InvalidateLimits();
                    isMatrixDirty = true;

                    // Al cambiar el zoom, validamos la posición actual
                    // (Esto re-dispara el setter de Position con las nuevas barreras)
                    Position = Position;
                }
            }
        } = 1;

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

        // Properties

        public float ApproachTolerance { get; set; } = 1.5f;

        // Tolerancia float para evitar flickering en comparaciones exactas
        public bool AtBottom => VisibleBox.Bottom >= SceneHeight - 0.1f;
        public bool AtLeft => VisibleBox.Left <= 0.1f;
        public bool AtRight => VisibleBox.Right >= SceneWidth - 0.1f;
        public bool AtTop => VisibleBox.Top <= 0.1f;

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
            // Apuntamos al centro de la escena. El setter decidirá si lo permite.
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

        public void FollowTarget(ITransform target) => FollowTarget(target, false);

        public void FollowTarget(ITransform target, bool focus)
        {
            Target = target;
            if (focus) FocusTarget();
        }

        public EngendroGame Game { get; }

        public Matrix GetTransformationMatrix()
        {
            if (isMatrixDirty)
            {
                var posX = -position.X;
                var posY = -position.Y;

                if (ShakeState != CameraShakeState.None)
                {
                    if (shakeHorzTween.IsRunning) posX += shakeHorzTween.CurrentValue;
                    if (shakeVertTween.IsRunning) posY += shakeVertTween.CurrentValue;
                }

                camTranslationVector.X = posX;
                camTranslationVector.Y = posY;
                Matrix.CreateTranslation(ref camTranslationVector, out camTranslationMatrix);

                scaleVector.X = ZoomCore;
                scaleVector.Y = ZoomCore;
                Matrix.CreateScale(ref scaleVector, out scaleMatrix);

                resTranslationVector.X = viewportWidth * .5f;
                resTranslationVector.Y = viewportHeight * .5f;
                Matrix.CreateTranslation(ref resTranslationVector, out resTranslationMatrix);

                Matrix.CreateRotationZ(Rotation, out rotationTranslationMatrix);

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

            // Clamp preventivo para que el tween apunte a un lugar válido,
            // aunque el Setter corregiría de todas formas frame a frame.
            float destX = CanScrollHorizontally ? MathHelper.Clamp(destination.X, leftBarrier, rightBarrier) : SceneWidth / 2f;
            float destY = CanScrollVertically ? MathHelper.Clamp(destination.Y, topBarrier, bottomBarrier) : SceneHeight / 2f;

            moveTween.Start(tweenStyle, Position, new Vector2(destX, destY), duration);
        }

        public string Name { get; }
        public Vector2 Offset { get; private set; }

        /// <summary>
        /// Posición central de la cámara.
        /// El setter contiene la lógica crítica de límites.
        /// </summary>
        public Vector2 Position
        {
            get => position;
            set
            {
                if (value == position && !isInitializing)
                    return;

                float finalX = value.X;
                float finalY = value.Y;

                // --- LÓGICA CRÍTICA ---
                // Si hay scroll, respetamos los límites (Clamp).
                // Si NO hay scroll, forzamos el centro de la escena.
                // Esto evita el error de "barreras invertidas" cuando el Zoom aleja mucho.

                if (CanScrollHorizontally)
                    finalX = MathHelper.Clamp(finalX, leftBarrier, rightBarrier);
                else
                    finalX = SceneWidth / 2f;

                if (CanScrollVertically)
                    finalY = MathHelper.Clamp(finalY, topBarrier, bottomBarrier);
                else
                    finalY = SceneHeight / 2f;

                position = new Vector2(finalX, finalY);

                // Recalculamos el área visible basada en la posición final real
                float halfVisibleW = (viewportWidth / ZoomCore) * 0.5f;
                float halfVisibleH = (viewportHeight / ZoomCore) * 0.5f;

                VisibleBox = new RectangleF(
                    position.X - halfVisibleW,
                    position.Y - halfVisibleH,
                    viewportWidth / ZoomCore,
                    viewportHeight / ZoomCore
                );

                CullingBox = RectangleF.Inflate(VisibleBox,
                    VisibleBox.Width * (CullingBoxScale.X - 1),
                    VisibleBox.Height * (CullingBoxScale.Y - 1));

                Offset = new Vector2(
                    position.X - halfVisibleW,
                    position.Y - halfVisibleH
                );

                isMatrixDirty = true;
            }
        }

        public void Reset()
        {
            StopShaking();
            StopMoving();
            StopFollowing();
            StopZooming();
            StopRotating();

            // Reiniciamos valores base
            Zoom = 1;
            Rotation = 0;

            // Centramos (ahora con Zoom 1 es seguro)
            FocusCenter();
        }

        public void Rotate(TweenStyle tweenStyle, float rotationValue, int duration, int bounceCount = 0)
        {
            if (Rotation == rotationValue) return;
            rotationTween.Start(tweenStyle, Rotation, rotationValue, duration, bounceCount);
        }

        public float Rotation
        {
            get => field;
            set
            {
                if (value != field)
                {
                    field = value;
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

            // 1. PRIMERO Reset: Limpia tweens y pone valores default (Zoom=1).
            Reset();

            // 2. AHORA configuramos las propiedades nuevas.
            this.SceneWidth = sceneWidth;
            this.SceneHeight = sceneHeight;
            this.ScrollLock = scrollLock;

            // 3. Aplicamos el Zoom deseado (sobrescribe el 1 del Reset).
            this.ZoomCore = zoom;

            // 4. Calculamos barreras y scroll flags con el zoom final.
            InvalidateLimits();

            // 5. Posicionamos en el centro.
            // El Setter usará CanScrollHorizontally/Vertically (calculados arriba) 
            // para decidir si usa Clamp o fuerza el centro.
            this.Position = new Vector2(sceneWidth / 2f, sceneHeight / 2f);

            isInitializing = false;
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
                ZoomCore = zoomTween.CurrentValue;
            }

            if (IsMoving)
            {
                moveTween.Update(gameTime);
                Position = moveTween.CurrentValue;
            }
            else if (Target != null)
            {
                if (CanScrollHorizontally || CanScrollVertically)
                    Approach(Target.Position, gameTime);
            }
        }

        public RectangleF VisibleBox { get; private set; }

        public float Zoom
        {
            get => ZoomCore;
            set
            {
                if (Math.Abs(value - ZoomCore) > 0.00001f)
                {
                    ZoomCore = value;
                    StopZooming();
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