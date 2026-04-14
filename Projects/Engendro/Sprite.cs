using Engendro.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engendro
{
    /// <summary>
    /// Sprite
    /// </summary>
    public class Sprite : GameObject, ITransform
    {
        #region Private fields

        private RectangleF boundingBox;
        private Vector2 position;
        private Vector2 scale = Vector2.One;

        #endregion

        #region Constructor

        // Constructor
        public Sprite(AtlasImage? image = null)
        {
            this.RenderImage = image;
            this.Pivot = new SpritePivot(this);
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
            return Position;
        }

        // GetSpeedFactor
        protected virtual float GetSpeedFactor()
        {
            return 1;
        }

        // IsBoundingBoxDirty
        protected bool IsBoundingBoxDirty { get; set; } = true;

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            // Do we have an image? If not then there is nothing to draw...
            if (RenderImage == null || Width == 0 || Height == 0)
                return;

            var pos = Position;
            pos.Y -= Altitude;

            // Has a source rectangle been set?
            if (RenderImage.TextureArea.IsEmpty)
            {
                // No, so draw the entire texture
                Game.SpriteBatch.Draw(RenderImage.Atlas.Texture, pos, null, Color * Opacity * OpacityFactor, Rotation, Pivot.Position, Scale * ScaleFactor, Effects, 0);
            }
            else
            {
                // Yes, so just draw the specified SourceRect
                Game.SpriteBatch.Draw(RenderImage.Atlas.Texture, pos, RenderImage.TextureArea, Color * Opacity * OpacityFactor, Rotation, Pivot.Position, Scale * ScaleFactor, Effects, 0);
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
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds * TimeScale;

            // Position
            Position += Velocity * dt * GetSpeedFactor();

            // Rotation
            Rotation += RotationSpeed * dt;

            // Tweens
            Tweens.Update(gameTime);

            base.OnUpdate(gameTime);
        }

        #endregion

        // Altitude
        public float Altitude
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    if (field < 0)
                        field = 0;

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
                        var origin = this.Pivot.Position;

                        boundingBox = new RectangleF(X + (-origin.X * ScaleX),
                                                     Y + (-origin.Y * ScaleY) - Altitude,
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
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    IsFlippedHorizontally = HasFlag(field, SpriteEffects.FlipHorizontally);
                    IsFlippedVertically = HasFlag(field, SpriteEffects.FlipVertically);
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
                Effects = SpriteEffects.FlipVertically;
            else
                Effects = SpriteEffects.None;
        }

        // FlipUp
        public void FlipUp()
        {
            if (IsFlippedHorizontally)
                Effects = SpriteEffects.FlipHorizontally;
            else
                Effects = SpriteEffects.None;
        }

        // FlipVertically
        public void FlipVertically()
        {
            if (IsFlippedVertically)
                FlipUp();
            else
                FlipDown();
        }

        // Height
        public virtual int Height
        {
            get
            {
                if (RenderImage == null)
                    return 0;
                else if (RenderImage.TextureArea.IsEmpty)
                    return RenderImage.Atlas.Texture.Height;
                else
                    return RenderImage.TextureArea.Height;
            }
        }

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
            get;
            set
            {
                if (value != field)
                    field = MathHelper.Clamp(value, 0, 1);
            }
        } = 1;

        // OpacityFactor
        public float OpacityFactor
        {
            get;
            set
            {
                if (value != field)
                    field = MathHelper.Clamp(value, 0, 1);
            }
        } = 1;

        // Pivot
        public SpritePivot Pivot { get; }

        // PivotOrigin
        public RectanglePoint PivotOrigin
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
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

        // RenderImage
        public AtlasImage? RenderImage
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    IsBoundingBoxDirty = true;
                }
            }
        }

        // Rotation
        public float Rotation
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    OnTransform(TransformChange.Rotation);
                }
            }
        }

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

        // ScaleFactor
        public Vector2 ScaleFactor { get; set; } = Vector2.One;

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

        // Width
        public virtual int Width
        {
            get
            {
                if (RenderImage == null)
                    return 0;

                else if (RenderImage.TextureArea.IsEmpty)
                    return RenderImage.Atlas.Texture.Width;

                else
                    return RenderImage.TextureArea.Width;
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

        /// <summary>
        /// SpritePivot
        /// </summary>
        public sealed class SpritePivot
        {
            private Vector2 position;
            private readonly Sprite sprite;

            // Constructor
            internal SpritePivot(Sprite sprite)
            {
                this.sprite = sprite;
            }

            // AtBottom
            public bool AtBottom => sprite.PivotOrigin is RectanglePoint.LeftBottom or
                                    RectanglePoint.RightBottom or
                                    RectanglePoint.Bottom;

            // AtLeft
            public bool AtLeft => sprite.PivotOrigin is RectanglePoint.LeftBottom or
                                  RectanglePoint.LeftTop or
                                  RectanglePoint.Left;

            // AtMiddle
            public bool AtMiddle => sprite.PivotOrigin == RectanglePoint.Center;

            // AtMiddleX
            public bool AtMiddleX => sprite.PivotOrigin is RectanglePoint.Bottom or
                                     RectanglePoint.Top or
                                     RectanglePoint.Center;

            // AtMiddleY
            public bool AtMiddleY => sprite.PivotOrigin is RectanglePoint.Left or
                                     RectanglePoint.Right or
                                     RectanglePoint.Center;

            // AtRight
            public bool AtRight => sprite.PivotOrigin is RectanglePoint.RightBottom or
                                   RectanglePoint.RightTop or
                                   RectanglePoint.Right;

            // AtTop
            public bool AtTop => sprite.PivotOrigin is RectanglePoint.LeftTop or
                                 RectanglePoint.RightTop or
                                 RectanglePoint.Top;

            // Position
            public Vector2 Position
            {
                get
                {
                    switch (sprite.PivotOrigin)
                    {
                        // Bottom
                        case RectanglePoint.Bottom:
                            position.X = sprite.Width / 2;
                            position.Y = sprite.Height;
                            break;

                        // LeftBottom
                        case RectanglePoint.LeftBottom:
                            position.X = 0;
                            position.Y = sprite.Height;
                            break;

                        // RightBottom
                        case RectanglePoint.RightBottom:
                            position.X = sprite.Width;
                            position.Y = sprite.Height;
                            break;

                        // Left
                        case RectanglePoint.Left:
                            position.X = 0;
                            position.Y = sprite.Height / 2;
                            break;

                        // Middle
                        case RectanglePoint.Center:
                            position.X = sprite.Width / 2;
                            position.Y = sprite.Height / 2;
                            break;

                        // Right
                        case RectanglePoint.Right:
                            position.X = sprite.Width;
                            position.Y = sprite.Height / 2;
                            break;

                        // Top
                        case RectanglePoint.Top:
                            position.X = sprite.Width / 2;
                            position.Y = 0;
                            break;

                        // LeftTop
                        case RectanglePoint.LeftTop:
                            position.X = 0;
                            position.Y = 0;
                            break;

                        // RightTop
                        case RectanglePoint.RightTop:
                            position.X = sprite.Width;
                            position.Y = 0;
                            break;

                        default:
                            break;
                    }

                    return position;
                }
            }
        }
    }
}
