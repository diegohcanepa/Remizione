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

        private const float maxZoom = 999f;
        private const float minZoom = 0.1f;

        private float leftBarrier, rightBarrier, topBarrier, bottomBarrier;
        private bool disposedValue;
        private bool isMatrixDirty = true;

        private Vector2 position;
        private float rotation;
        private float zoom = 1f;
        private readonly FloatRange zoomRange = new(minZoom, maxZoom);

        private readonly FloatTween flightTween = new();
        private readonly FloatTween rotationTween = new();
        private readonly FloatTween shakeHorzTween = new();
        private readonly FloatTween shakeVertTween = new();
        private readonly FloatTween zoomTween = new();

        private Vector2 flightStartPos;
        private Vector2 flightEndPos;
        private float flightStartInvZoom;
        private float flightEndInvZoom;

        private Matrix transformationMatrix;
        private int viewportWidth;
        private int viewportHeight;

        #endregion

        #region Constructors

        public Camera(string name)
            : this(name, 0, 0)
        {
        }

        public Camera(string name, int sceneWidth, int sceneHeight)
        {
            Game = EngendroGame.Instance;
            Name = name;

            UpdateViewportDimensions();

#if WINDOWS
            Game.Window.ClientSizeChanged += Window_ClientSizeChanged;
#endif

            Setup(sceneWidth, sceneHeight);
        }

        #endregion

        #region Private members

        private void ApproachTarget(Vector2 targetPosition, float dt)
        {
            if (Vector2.DistanceSquared(position, targetPosition) <= 0.0001f)
            {
                SetPosition(targetPosition);
                return;
            }

            float rate = SmoothSpeed / Math.Max(zoom, 1f);
            float t = 1f - MathF.Exp(-rate * dt);
            SetPosition(Vector2.Lerp(position, targetPosition, t));
        }

        private void ClearMovementModes()
        {
            Target = null;
            flightTween.Stop();
        }

        private Matrix CreateViewMatrix(Vector2 parallaxFactor)
        {
            float shakeX = shakeHorzTween.IsRunning ? shakeHorzTween.CurrentValue : 0f;
            float shakeY = shakeVertTween.IsRunning ? shakeVertTween.CurrentValue : 0f;

            return Matrix.CreateTranslation(-(position.X * parallaxFactor.X) - shakeX, -(position.Y * parallaxFactor.Y) - shakeY, 0f) *
                   Matrix.CreateRotationZ(rotation) *
                   Matrix.CreateScale(zoom, zoom, 1f) *
                   Matrix.CreateTranslation(viewportWidth * 0.5f, viewportHeight * 0.5f, 0f) *
                   Game.ViewportAdapter.TransformationMatrix;
        }

        private Vector2 GetClampedPosition(Vector2 targetPos, float targetZoom)
        {
            float halfW = (viewportWidth / targetZoom) * 0.5f;
            float halfH = (viewportHeight / targetZoom) * 0.5f;

            bool canScrollH = ScrollLock != ScrollLock.Horizontal && ScrollLock != ScrollLock.All && (SceneWidth * targetZoom) > viewportWidth;
            bool canScrollV = ScrollLock != ScrollLock.Vertical && ScrollLock != ScrollLock.All && (SceneHeight * targetZoom) > viewportHeight;

            float destX = canScrollH ? MathHelper.Clamp(targetPos.X, halfW, Math.Max(SceneWidth - halfW, halfW)) : SceneWidth * 0.5f;
            float destY = canScrollV ? MathHelper.Clamp(targetPos.Y, halfH, Math.Max(SceneHeight - halfH, halfH)) : SceneHeight * 0.5f;

            return new Vector2(destX, destY);
        }

        private void SetPosition(Vector2 value)
        {
            if (value == position) return;
            position = value;
            isMatrixDirty = true;
            UpdateBounds();
        }

        private void SetRotation(float value)
        {
            if (Math.Abs(value - rotation) <= 0.00001f) return;
            rotation = value;
            isMatrixDirty = true;
        }

        private void SetZoom(float value)
        {
            float clamped = zoomRange.Clamp(value);
            if (Math.Abs(clamped - zoom) <= 0.00001f) return;
            zoom = clamped;
            isMatrixDirty = true;
            UpdateBounds();
        }

        private void UpdateBounds()
        {
            float viewW = viewportWidth / zoom;
            float viewH = viewportHeight / zoom;

            leftBarrier = viewW * 0.5f;
            rightBarrier = Math.Max(SceneWidth - leftBarrier, leftBarrier);
            topBarrier = viewH * 0.5f;
            bottomBarrier = Math.Max(SceneHeight - topBarrier, topBarrier);

            CanScrollHorizontally = ScrollLock != ScrollLock.Horizontal && ScrollLock != ScrollLock.All && (SceneWidth * zoom) > viewportWidth;
            CanScrollVertically = ScrollLock != ScrollLock.Vertical && ScrollLock != ScrollLock.All && (SceneHeight * zoom) > viewportHeight;

            if (!IsFlying)
            {
                Vector2 clamped = GetClampedPosition(position, zoom);
                if (position != clamped)
                {
                    position = clamped;
                    isMatrixDirty = true;
                }
            }

            VisibleBox = new RectangleF(position.X - leftBarrier, position.Y - topBarrier, viewW, viewH);
            Offset = new Vector2(VisibleBox.X, VisibleBox.Y);
            CullingBox = RectangleF.Inflate(VisibleBox, VisibleBox.Width * (CullingBoxScale.X - 1f), VisibleBox.Height * (CullingBoxScale.Y - 1f));
        }

        private void UpdateViewportDimensions()
        {
            viewportWidth = Game.ViewportAdapter.VirtualWidth;
            viewportHeight = Game.ViewportAdapter.VirtualHeight;
        }

#if WINDOWS
        private void Window_ClientSizeChanged(object? sender, EventArgs e)
        {
            UpdateViewportDimensions();
            UpdateBounds();
            isMatrixDirty = true;
        }
#endif

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

        #region Public members

        public float ApproachTolerance { get; set; } = 1.5f;
        public bool AtBottom => position.Y >= bottomBarrier - 0.1f;
        public bool AtLeft => position.X <= leftBarrier + 0.1f;
        public bool AtRight => position.X >= rightBarrier - 0.1f;
        public bool AtTop => position.Y <= topBarrier + 0.1f;
        public bool CanScrollHorizontally { get; private set; }
        public bool CanScrollVertically { get; private set; }
        public RectangleF CullingBox { get; private set; }
        public Vector2 CullingBoxScale { get; set; } = Vector2.One;
        public EngendroGame Game { get; }
        public bool IsFlying => flightTween.IsRunning;
        public bool IsMoving => flightTween.IsRunning && flightStartPos != flightEndPos;
        public bool IsRotating => rotationTween.IsRunning;
        public bool IsTargetFocused => Target != null && Vector2.Distance(position, Target.Position) < ApproachTolerance;
        public string Name { get; }
        public Vector2 Offset { get; private set; }
        public int SceneHeight { get; private set; }
        public int SceneWidth { get; private set; }
        public float SmoothSpeed { get; set; } = 0.01f;
        public ITransform? Target { get; private set; }
        public RectangleF VisibleBox { get; private set; }

        public Vector2 Position
        {
            get => position;
            set
            {
                ClearMovementModes();
                SetPosition(value);
            }
        }

        public float Rotation
        {
            get => rotation;
            set
            {
                rotationTween.Stop();
                SetRotation(value);
            }
        }

        public ScrollLock ScrollLock
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                UpdateBounds();
            }
        }

        public CameraShakeState ShakeState
        {
            get
            {
                if (shakeHorzTween.IsRunning && shakeVertTween.IsRunning) return CameraShakeState.XY;
                if (shakeHorzTween.IsRunning) return CameraShakeState.X;
                if (shakeVertTween.IsRunning) return CameraShakeState.Y;
                return CameraShakeState.None;
            }
        }

        public float Zoom
        {
            get => zoom;
            set
            {
                flightTween.Stop();
                zoomTween.Stop();
                SetZoom(value);
            }
        }

        public ZoomState ZoomState
        {
            get
            {
                if (zoomTween.IsRunning)
                    return zoomTween.EndValue > zoomTween.StartValue ? ZoomState.In : ZoomState.Out;
                if (flightTween.IsRunning && Math.Abs(flightEndInvZoom - flightStartInvZoom) >= 0.00001f)
                    return flightEndInvZoom < flightStartInvZoom ? ZoomState.In : ZoomState.Out;
                return ZoomState.None;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void FlyTo(TweenStyle tweenStyle, Vector2 destination, float targetZoom, int duration, int bounceCount = 0)
        {
            ClearMovementModes();
            zoomTween.Stop();

            float clampedZoom = zoomRange.Clamp(targetZoom);
            flightEndPos = GetClampedPosition(destination, clampedZoom);

            if (duration <= 0)
            {
                SetZoom(clampedZoom);
                SetPosition(flightEndPos);
                return;
            }

            flightStartPos = position;
            flightStartInvZoom = 1f / zoom;
            flightEndInvZoom = 1f / clampedZoom;
            flightTween.Start(tweenStyle, 0f, 1f, duration, bounceCount);
        }

        public void FocusCenter()
        {
            ClearMovementModes();
            SetPosition(new Vector2(SceneWidth * 0.5f, SceneHeight * 0.5f));
        }

        public void FocusTarget()
        {
            if (Target == null) return;
            flightTween.Stop();
            SetPosition(Target.Position);
        }

        public void Follow(ITransform target, bool focus = false)
        {
            flightTween.Stop();
            Target = target;
            if (focus) FocusTarget();
        }

        public Matrix GetTransformationMatrix()
        {
            if (isMatrixDirty)
            {
                transformationMatrix = CreateViewMatrix(Vector2.One);
                isMatrixDirty = false;
            }
            return transformationMatrix;
        }

        public Matrix GetViewMatrix(Vector2 parallaxFactor)
        {
            if (parallaxFactor == Vector2.Zero) return Game.ViewportAdapter.TransformationMatrix;
            if (parallaxFactor == Vector2.One) return GetTransformationMatrix();
            return CreateViewMatrix(parallaxFactor);
        }

        public void MoveTo(TweenStyle tweenStyle, Vector2 destination, int duration)
        {
            FlyTo(tweenStyle, destination, zoom, duration);
        }

        public void Reset()
        {
            StopShaking();
            StopFlying();
            StopFollowing();
            StopZooming();
            StopRotating();

            zoom = 1f;
            rotation = 0f;
            position = new Vector2(SceneWidth * 0.5f, SceneHeight * 0.5f);
            isMatrixDirty = true;
            UpdateBounds();
        }

        public void Rotate(TweenStyle tweenStyle, float rotationValue, int duration, int bounceCount = 0)
        {
            if (Math.Abs(rotation - rotationValue) <= 0.00001f) return;
            if (duration <= 0)
            {
                Rotation = rotationValue;
                return;
            }
            rotationTween.Start(tweenStyle, rotation, rotationValue, duration, bounceCount);
        }

        public void Setup(int sceneWidth, int sceneHeight, ScrollLock scrollLock = ScrollLock.None, float zoom = 1f)
        {
            SceneWidth = sceneWidth;
            SceneHeight = sceneHeight;
            ScrollLock = scrollLock;
            UpdateViewportDimensions();
            Reset();
            Zoom = zoom;
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

        public void ShakeVertically(TweenStyle tweenStyle, float intensity, int duration, int bounces)
        {
            shakeVertTween.Start(tweenStyle, -intensity, intensity, duration, bounces);
        }

        public void StopFlying() => flightTween.Stop();
        public void StopFollowing() => Target = null;
        public void StopMoving() => flightTween.Stop();
        public void StopRotating() => rotationTween.Stop();
        public void StopZooming() => zoomTween.Stop();

        public void StopShaking()
        {
            shakeHorzTween.Stop();
            shakeVertTween.Stop();
        }

        public override string ToString() => Name;

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (shakeHorzTween.IsRunning)
            {
                shakeHorzTween.Update(gameTime);
                isMatrixDirty = true;
            }

            if (shakeVertTween.IsRunning)
            {
                shakeVertTween.Update(gameTime);
                isMatrixDirty = true;
            }

            if (rotationTween.IsRunning)
            {
                rotationTween.Update(gameTime);
                SetRotation(rotationTween.CurrentValue);
            }

            if (flightTween.IsRunning)
            {
                flightTween.Update(gameTime);
                float t = flightTween.CurrentValue;
                zoom = 1f / MathHelper.Lerp(flightStartInvZoom, flightEndInvZoom, t);
                position = Vector2.Lerp(flightStartPos, flightEndPos, t);
                isMatrixDirty = true;
                UpdateBounds();
                return;
            }

            if (zoomTween.IsRunning)
            {
                zoomTween.Update(gameTime);
                SetZoom(zoomTween.CurrentValue);
            }

            if (Target != null && (CanScrollHorizontally || CanScrollVertically))
                ApproachTarget(Target.Position, dt);
            else
                UpdateBounds();
        }

        public void ZoomTo(TweenStyle tweenStyle, float zoomValue, int duration, int bounceCount = 0)
        {
            float clampedZoom = zoomRange.Clamp(zoomValue);
            if (Math.Abs(zoom - clampedZoom) <= 0.0001f) return;

            flightTween.Stop();
            if (duration <= 0)
            {
                Zoom = clampedZoom;
                return;
            }
            zoomTween.Start(tweenStyle, zoom, clampedZoom, duration, bounceCount);
        }

        #endregion
    }
}