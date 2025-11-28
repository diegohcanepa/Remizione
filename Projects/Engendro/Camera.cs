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

        // Constructor
        public Camera(EngendroGame game, string name)
            : this(game, name, 0, 0)
        {
        }

        // Constructor
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

        // Approach
        private void Approach(Vector2 targetPosition)
        {
            if (IsTargetFocused)
                return;

            // Calculates the difference between the current camera position and the target
            var difference = targetPosition - Position;

            // Apply interpolation to get closer to the target
            Position += difference * SmoothSpeed;

            // Limits the camera position within the edges of the scene
            ClampToSceneBounds();
        }

        // ClampToSceneBounds
        private void ClampToSceneBounds()
        {
            // Calculate the visible width and height based on zoom
            var visibleWidth = viewportWidth / Zoom;
            var visibleHeight = viewportHeight / Zoom;

            Rectangle sceneBounds = new(0, 0, SceneWidth, SceneHeight);

            // Calculate the minimum and maximum limits for the camera position
            var minX = sceneBounds.Left + visibleWidth / 2;
            var maxX = sceneBounds.Right - visibleWidth / 2;
            var minY = sceneBounds.Top + visibleHeight / 2;
            var maxY = sceneBounds.Bottom - visibleHeight / 2;

            // Applies constraint to camera position, snapping it to the center of the view
            Position = new Vector2(MathHelper.Clamp(Position.X, minX, maxX), MathHelper.Clamp(Position.Y, minY, maxY));
        }

#if WINDOWS
        // Window_ClientSizeChanged
        private void Window_ClientSizeChanged(object? sender, EventArgs e)
        {
            isMatrixDirty = true;
        }
#endif

        // InvalidateLimits
        private void InvalidateLimits()
        {
            var vw = viewportWidth / ZoomCore;
            var vh = viewportHeight / ZoomCore;

            this.leftBarrier = vw * .5f;
            this.rightBarrier = SceneWidth - vw * .5f;
            this.bottomBarrier = SceneHeight - vh * .5f;
            this.topBarrier = vh * .5f;

            this.CanScrollHorizontally = ScrollLock != ScrollLock.Horizontal && ScrollLock != ScrollLock.All && SceneWidth * ZoomCore > viewportWidth;
            this.CanScrollVertically = ScrollLock != ScrollLock.Vertical && ScrollLock != ScrollLock.All && SceneHeight * ZoomCore > viewportHeight;
        }

        // ZoomCore
        private float ZoomCore
        {
            get;
            set
            {
                value = zoomRange.Clamp(value);

                if (value != field)
                {
                    field = value;
                    InvalidateLimits();
                    isMatrixDirty = true;
                }
            }
        } = 1;

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
        public bool AtBottom => VisibleBox.Bottom == SceneHeight;

        // AtLeft
        public bool AtLeft => VisibleBox.Left == 0;

        // AtRight
        public bool AtRight => VisibleBox.Right == SceneWidth;

        // AtTop
        public bool AtTop => VisibleBox.Top == 0;

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
            Position = new Vector2((float)SceneWidth / 2, (float)SceneHeight / 2);
        }

        // FocusTarget
        public void FocusTarget()
        {
            if (Target != null)
            {
                StopMoving();

                if (CanScrollHorizontally || CanScrollVertically)
                    Position = Target.Position;
                else
                    FocusCenter();
            }
        }

        // FollowTarget
        public void FollowTarget(ITransform target)
        {
            FollowTarget(target, false);
        }

        // FollowTarget
        public void FollowTarget(ITransform target, bool focus)
        {
            Target = target;
            if (focus)
                FocusTarget();
        }

        // Game
        public EngendroGame Game { get; }

        // GetTransformationMatrix
        public Matrix GetTransformationMatrix()
        {
            if (isMatrixDirty)
            {
                var posX = -position.X;
                var posY = -position.Y;

                if (ShakeState != CameraShakeState.None)
                {
                    if (shakeHorzTween.IsRunning)
                    {
                        posX += shakeHorzTween.CurrentValue;
                    }

                    if (shakeVertTween.IsRunning)
                    {
                        posY += shakeVertTween.CurrentValue;
                    }
                }

                camTranslationVector.X = posX;
                camTranslationVector.Y = posY;

                Matrix.CreateTranslation(ref camTranslationVector, out camTranslationMatrix);

                scaleVector.X = ZoomCore;
                scaleVector.Y = ZoomCore;
                scaleVector.Z = 1;

                Matrix.CreateScale(ref scaleVector, out scaleMatrix);

                resTranslationVector.X = viewportWidth * .5f;
                resTranslationVector.Y = viewportHeight * .5f;
                resTranslationVector.Z = 0;

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

        // IsTargetFocused
        public bool IsTargetFocused => Target != null && Vector2.Distance(Position, Target.Position) < ApproachTolerance;

        // IsMoving
        public bool IsMoving => moveTween.IsRunning;

        // IsRotating
        public bool IsRotating => rotationTween.IsRunning;

        // MoveTo
        public void MoveTo(TweenStyle tweenStyle, Vector2 destination, int duration)
        {
            Target = null;

            // Clamp destination to keep tween out effect
            if (destination.X - (VisibleBox.Width / 2) < 0)
                destination.X = VisibleBox.Width / 2;

            if (destination.Y - (VisibleBox.Height / 2) < 0)
                destination.Y = VisibleBox.Height / 2;

            // Clamp destination to keep tween out effect
            if (destination.X + (VisibleBox.Width / 2) > SceneWidth)
                destination.X = SceneWidth - VisibleBox.Width / 2;

            if (destination.Y + (VisibleBox.Height / 2) > SceneHeight)
                destination.Y = SceneHeight - VisibleBox.Height / 2;

            moveTween.Start(tweenStyle, Position, destination, duration);
        }

        // Name
        public string Name { get; }

        // Offset
        public Vector2 Offset { get; private set; }

        // Position
        public Vector2 Position
        {
            get => position;
            set
            {
                if (value == position && !isInitializing)
                    return;

                if (!CanScrollHorizontally)
                    value.X = VisibleBox.Center.X;

                if (!CanScrollVertically)
                    value.Y = VisibleBox.Center.Y;

                position = value;

                position.X = MathHelper.Clamp(position.X, leftBarrier, rightBarrier);
                position.Y = MathHelper.Clamp(position.Y, topBarrier, bottomBarrier);

                VisibleBox = new RectangleF(position.X - (viewportWidth / 2 / ZoomCore), position.Y - viewportHeight / 2 / ZoomCore, viewportWidth / ZoomCore, viewportHeight / ZoomCore);

                CullingBox = RectangleF.Inflate(VisibleBox,
                                                VisibleBox.Width * (CullingBoxScale.X - 1),
                                                VisibleBox.Height * (CullingBoxScale.Y - 1));

                Offset = new Vector2(position.X - (VisibleBox.Width / 2), position.Y - (VisibleBox.Height / 2));

                isMatrixDirty = true;
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
            Position = Vector2.Zero;
            Zoom = 1;
            Rotation = 0;
        }

        // Rotate
        public void Rotate(TweenStyle tweenStyle, float rotationValue, int duration)
        {
            Rotate(tweenStyle, rotationValue, duration, 0);
        }

        // Rotate
        public void Rotate(TweenStyle tweenStyle, float rotationValue, int duration, int bounceCount)
        {
            if (Rotation == rotationValue)
            {
                return;
            }

            rotationTween.Start(tweenStyle, Rotation, rotationValue, duration, bounceCount);
        }

        // Rotation
        public float Rotation
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    isMatrixDirty = true;
                }
            }
        }

        // SceneHeight
        public int SceneHeight { get; private set; }

        // SceneWidth
        public int SceneWidth { get; private set; }

        // ScrollLock
        public ScrollLock ScrollLock { get; set; }

        // Setup
        public void Setup(int sceneWidth, int sceneHeight)
        {
            Setup(sceneWidth, sceneHeight, ScrollLock.None, 1);
        }

        // Setup
        public void Setup(int sceneWidth, int sceneHeight, ScrollLock scrollLock, float zoom)
        {
            isInitializing = true;

            Reset();

            this.SceneWidth = sceneWidth;
            this.SceneHeight = sceneHeight;
            this.Zoom = zoom;
            this.ScrollLock = scrollLock;
            this.isMatrixDirty = true;

            InvalidateLimits();

            var newPos = Position;

            if (!CanScrollHorizontally)
                newPos.X = viewportWidth / 2;

            if (!CanScrollVertically)
                newPos.Y = viewportHeight / 2;

            this.Position = newPos;

            isInitializing = false;
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
        public void StopFollowing() => Target = null;

        // StopMoving
        public void StopMoving() => moveTween.Stop();

        // StopRotating
        public void StopRotating() => rotationTween.Stop();

        // StopShaking
        public void StopShaking()
        {
            shakeHorzTween.Stop();
            shakeVertTween.Stop();
        }

        // StopZooming
        public void StopZooming() => zoomTween.Stop();

        // Target
        public ITransform? Target { get; private set; }

        // ToString
        public override string ToString() => Name;

        // Update
        public void Update(GameTime gameTime)
        {
            // Shake
            if (ShakeState != CameraShakeState.None)
            {
                if (shakeHorzTween.IsRunning)
                    shakeHorzTween.Update(gameTime);

                if (shakeVertTween.IsRunning)
                    shakeVertTween.Update(gameTime);

                isMatrixDirty = true;
            }

            // Rotation
            if (rotationTween.IsRunning)
            {
                rotationTween.Update(gameTime);
                Rotation = rotationTween.CurrentValue;
            }

            // Zoom
            if (zoomTween.IsRunning)
            {
                zoomTween.Update(gameTime);
                ZoomCore = zoomTween.CurrentValue;
            }

            // Movement
            if (IsMoving)
            {
                moveTween.Update(gameTime);
                Position = moveTween.CurrentValue;
            }
            else if (Target != null)
            {
                if (CanScrollHorizontally || CanScrollVertically)
                    Approach(Target.Position);
            }
        }

        // VisibleBox
        public RectangleF VisibleBox { get; private set; }

        // Zoom
        public float Zoom
        {
            get => ZoomCore;
            set
            {
                if (value != ZoomCore)
                {
                    ZoomCore = value;
                    StopZooming();
                }
            }
        }

        // ZoomTo
        public void ZoomTo(TweenStyle tweenStyle, float zoomValue, int duration)
        {
            ZoomTo(tweenStyle, zoomValue, duration, 0);
        }

        // ZoomTo
        public void ZoomTo(TweenStyle tweenStyle, float zoomValue, int duration, int bounceCount)
        {
            if (Zoom == zoomValue)
                return;

            zoomTween.Start(tweenStyle, Zoom, zoomValue, duration, bounceCount);
        }

        // ZoomState
        public ZoomState ZoomState
        {
            get
            {
                if (zoomTween.IsRunning)
                    return zoomTween.EndValue > zoomTween.StartValue ? ZoomState.In : ZoomState.Out;
                else
                    return ZoomState.None;
            }
        }
    }
}
