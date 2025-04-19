using Engendro.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engendro
{
    /// <summary>
    /// Sprite
    /// </summary>
    public abstract class Sprite : GameObject, ITransform
    {
        #region Private fields

        private float altitude;
        private RectangleF boundingBox;
        private SpriteEffects effects;
        private AtlasImage? internalImage;
        private float opacity = 1;
        private float opacityFactor = 1;
        private Vector2 origin;
        private RectanglePoint pivotOrigin;
        private Vector2 position;
        private float rotation;
        private Vector2 scale = Vector2.One;

        #endregion

        #region Constructor

        // Constructor
        protected Sprite(EngendroGame game)
            : base(game)
        {
            this.Tweens = new TweenManager(this);
        }

        #endregion

        #region Private members

        // HasFlag
        // Avoid usage of Enum.HasFlag() which causes boxing/unboxing
        private static bool HasFlag(SpriteEffects flags, SpriteEffects flagToCheck)
        {
            return (flags & flagToCheck) == flagToCheck;
        }

        #endregion

        #region Protected members

        // GetAbsolutePosition
        protected Vector2 GetAbsolutePosition()
        {
            return VisualParent == null ? Position : Position + VisualParent.GetAbsolutePosition();
        }

        // GetSpeedFactor
        protected virtual float GetSpeedFactor() => 1;

        // InternalImage
        protected AtlasImage? InternalImage
        {
            get => internalImage;
            set
            {
                if (value != internalImage)
                {
                    internalImage = value;
                    IsBoundingBoxDirty = true;
                }
            }
        }

        // IsBoundingBoxDirty
        protected bool IsBoundingBoxDirty { get; set; } = true;

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            // Do we have an image? If not then there is nothing to draw...
            if (InternalImage == null || Width == 0 || Height == 0)
                return;

            var pos = Position;
            pos.Y -= altitude;
            if (VisualParent != null)
                pos = GetAbsolutePosition();

            // Has a source rectangle been set?
            if (InternalImage.TextureArea.IsEmpty)
            {
                // No, so draw the entire texture
                Game.SpriteBatch.Draw(InternalImage.Atlas.Texture, pos, null, Color * opacity * OpacityFactor, Rotation, Origin, Scale, Effects, 0);
            }
            else
            {
                // Yes, so just draw the specified SourceRect
                Game.SpriteBatch.Draw(InternalImage.Atlas.Texture, pos, InternalImage.TextureArea, Color * opacity * OpacityFactor, Rotation, Origin, Scale, Effects, 0);
            }
        }

        // OnTransform
        protected virtual void OnTransform(TransformChange transformChange)
        {
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            // Applies current time scale value
            var totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds * TimeScale;

            // Position
            Velocity *= Inertia;
            Velocity += Acceleration * totalSeconds;

            Position += Velocity * totalSeconds * GetSpeedFactor();

            // Rotation
            Rotation *= RotationInertia;
            Rotation += RotationSpeed * totalSeconds;

            // Tweens
            Tweens.Update(gameTime);

            base.OnUpdate(gameTime);
        }

        #endregion

        // Acceleration
        public Vector2 Acceleration { get; set; }

        // Altitude
        public float Altitude
        {
            get => altitude;
            set
            {
                if (value != altitude)
                {
                    altitude = value;
                    if (altitude < 0)
                        altitude = 0;

                    IsBoundingBoxDirty = true;

                    OnTransform(TransformChange.Altitude);
                }
            }
        }

        // BoundingBox
        public RectangleF BoundingBox
        {
            get
            {
                if (IsBoundingBoxDirty)
                {
                    if (Width == 0 || Height == 0)
                    {
                        boundingBox = new RectangleF(X, Y, 0, 0);
                    }
                    else
                    {
                        var origin = this.Origin;

                        boundingBox = new RectangleF(X + (-origin.X * ScaleX) + (VisualParent == null ? 0 : VisualParent.X),
                                                     Y + (-origin.Y * ScaleY) + (VisualParent == null ? 0 : VisualParent.Y) - Altitude,
                                                     Width * ScaleX,
                                                     Height * ScaleY);
                    }

                    IsBoundingBoxDirty = false;
                }

                return boundingBox;
            }
        }

        // Color
        public Color Color { get; set; } = DefaultColor;

        // DefaultColor
        public static Color DefaultColor { get; } = Color.White;

        // Degrees
        public float Degrees
        {
            get => Geometry.NormalizeAngle(MathHelper.ToDegrees(Rotation));
            set => Rotation = MathHelper.ToRadians(Geometry.NormalizeAngle(value));
        }

        // DistanceTo
        public float DistanceTo(Vector2 position)
        {
            return Vector2.Distance(this.Position, position);
        }

        // DistanceTo
        public float DistanceTo(Sprite target)
        {
            return Vector2.Distance(Position, target.Position);
        }

        // Effects
        public SpriteEffects Effects
        {
            get => effects;
            set
            {
                if (value != effects)
                {
                    this.effects = value;
                    IsFlippedHorizontally = HasFlag(effects, SpriteEffects.FlipHorizontally);
                    IsFlippedVertically = HasFlag(effects, SpriteEffects.FlipVertically);
                    OnTransform(TransformChange.Effects);
                }
            }
        }

        // FlipDown
        public void FlipDown()
        {
            if (IsFlippedHorizontally)
            {
                Effects = SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally;
            }
            else
            {
                Effects = SpriteEffects.FlipVertically;
            }
        }

        // FlipHorizontally
        public void FlipHorizontally()
        {
            if (IsFlippedHorizontally)
            {
                FlipRight();
            }
            else
            {
                FlipLeft();
            }
        }

        // FlipLeft
        public void FlipLeft()
        {
            if (IsFlippedVertically)
            {
                Effects = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
            }
            else
            {
                Effects = SpriteEffects.FlipHorizontally;
            }
        }

        // FlipRight
        public void FlipRight()
        {
            if (IsFlippedVertically)
            {
                Effects = SpriteEffects.FlipVertically;
            }
            else
            {
                Effects = SpriteEffects.None;
            }
        }

        // FlipUp
        public void FlipUp()
        {
            if (IsFlippedHorizontally)
            {
                Effects = SpriteEffects.FlipHorizontally;
            }
            else
            {
                Effects = SpriteEffects.None;
            }
        }

        // FlipVertically
        public void FlipVertically()
        {
            if (IsFlippedVertically)
            {
                FlipUp();
            }
            else
            {
                FlipDown();
            }
        }

        // Height
        public virtual int Height
        {
            get
            {
                if (InternalImage == null)
                {
                    return 0;
                }
                else if (InternalImage.TextureArea.IsEmpty)
                {
                    return InternalImage.Atlas.Texture.Height;
                }
                else
                {
                    return InternalImage.TextureArea.Height;
                }
            }
        }

        // Inertia
        public float Inertia { get; set; } = 1;

        // IsEmpty
        public virtual bool IsEmpty => Width == 0 || Height == 0;

        // IsFlippedHorizontally
        public bool IsFlippedHorizontally { get; private set; }

        // IsFlippedVertically
        public bool IsFlippedVertically { get; private set; }

        // MatchTransform
        public void MatchTransform(Sprite source)
        {
            Effects = source.Effects;
            PivotOrigin = source.PivotOrigin;
            Position = source.Position;
            Rotation = source.Rotation;
            Scale = source.Scale;
        }

        // Opacity
        public float Opacity
        {
            get => opacity;
            set
            {
                if (value != opacity)
                    opacity = MathHelper.Clamp(value, 0, 1);
            }
        }

        // OpacityFactor
        public float OpacityFactor
        {
            get => opacityFactor;
            set
            {
                if (value != opacityFactor)
                    opacityFactor = MathHelper.Clamp(value, 0, 1);
            }
        }

        // Origin
        public Vector2 Origin
        {
            get
            {
                switch (PivotOrigin)
                {
                    // Bottom
                    case RectanglePoint.Bottom:
                        origin.X = Width / 2;
                        origin.Y = Height;
                        break;

                    // LeftBottom
                    case RectanglePoint.LeftBottom:
                        origin.X = 0;
                        origin.Y = Height;
                        break;

                    // RightBottom
                    case RectanglePoint.RightBottom:
                        origin.X = Width;
                        origin.Y = Height;
                        break;

                    // Left
                    case RectanglePoint.Left:
                        origin.X = 0;
                        origin.Y = Height / 2;
                        break;

                    // Middle
                    case RectanglePoint.Middle:
                        origin.X = Width / 2;
                        origin.Y = Height / 2;
                        break;

                    // Right
                    case RectanglePoint.Right:
                        origin.X = Width;
                        origin.Y = Height / 2;
                        break;

                    // Top
                    case RectanglePoint.Top:
                        origin.X = Width / 2;
                        origin.Y = 0;
                        break;

                    // LeftTop
                    case RectanglePoint.LeftTop:
                        origin.X = 0;
                        origin.Y = 0;
                        break;

                    // RightTop
                    case RectanglePoint.RightTop:
                        origin.X = Width;
                        origin.Y = 0;
                        break;
                }

                return origin;
            }
        }

        // PivotOrigin
        public RectanglePoint PivotOrigin
        {
            get => pivotOrigin;
            set
            {
                if (value != pivotOrigin)
                {
                    pivotOrigin = value;
                    IsBoundingBoxDirty = true;
                    OnTransform(TransformChange.PivotOrigin);
                }
            }
        }

        // Position
        public Vector2 Position
        {
            get => position;
            set
            {
                if (value != Position)
                {
                    position.X = value.X;
                    position.Y = value.Y;
                    IsBoundingBoxDirty = true;
                    OnTransform(TransformChange.Position);
                }
            }
        }

        // Rotation
        public float Rotation
        {
            get => rotation;
            set
            {
                if (rotation != value)
                {
                    rotation = value;
                    OnTransform(TransformChange.Rotation);
                }
            }
        }

        // RotationInertia
        public float RotationInertia { get; set; } = 1;

        // RotationSpeed
        public float RotationSpeed { get; set; }

        // Scale
        public Vector2 Scale
        {
            get => scale;
            set
            {
                if (value != Scale)
                {
                    scale.X = value.X;
                    scale.Y = value.Y;
                    IsBoundingBoxDirty = true;
                    OnTransform(TransformChange.Scale);
                }
            }
        }

        // ScaleX
        public float ScaleX
        {
            get => scale.X;
            set
            {
                if (value != scale.X)
                {
                    scale.X = MathHelper.Clamp(value, 0, value);
                    IsBoundingBoxDirty = true;
                    OnTransform(TransformChange.Scale);
                }
            }
        }

        // ScaleY
        public float ScaleY
        {
            get => scale.Y;
            set
            {
                if (value != scale.Y)
                {
                    scale.Y = MathHelper.Clamp(value, 0, value);
                    IsBoundingBoxDirty = true;
                    OnTransform(TransformChange.Scale);
                }
            }
        }

        // SoundEmitter
        public ISoundEmitter? SoundEmitter { get; set; }

        // TimeScale
        public float TimeScale { get; set; } = 1;

        // Tweens
        public TweenManager Tweens { get; }

        // Velocity
        public Vector2 Velocity { get; set; }

        // VisualParent
        public Sprite? VisualParent { get; set; }

        // Width
        public virtual int Width
        {
            get
            {
                if (InternalImage == null)
                    return 0;

                else if (InternalImage.TextureArea.IsEmpty)
                    return InternalImage.Atlas.Texture.Width;

                else
                    return InternalImage.TextureArea.Width;
            }
        }

        // X
        public float X
        {
            get => position.X;
            set
            {
                if (position.X != value)
                {
                    position.X = value;
                    IsBoundingBoxDirty = true;
                    OnTransform(TransformChange.Position);
                }
            }
        }

        // Y
        public float Y
        {
            get => position.Y;
            set
            {
                if (position.Y != value)
                {
                    position.Y = value;
                    IsBoundingBoxDirty = true;
                    OnTransform(TransformChange.Position);
                }
            }
        }
    }
}
