using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Engendro
{
    /// <summary>
    /// ParticleState
    /// </summary>
    public abstract class ParticleState
    {
        private readonly List<AtlasImage> imageList = [];

        #region Constructor

        // Constructor
        protected ParticleState()
        {
            this.Images = new ReadOnlyCollection<AtlasImage>(imageList);
        }

        #endregion

        #region Protected members

        // AddImage
        protected void AddImage(AtlasImage image)
        {
            if (!imageList.Contains(image))
            {
                imageList.Add(image);
            }
        }

        // AddImages
        protected void AddImages(IEnumerable<AtlasImage> list)
        {
            foreach (var item in list)
            {
                AddImage(item);
            }
        }

        // GenerateFloat
        protected static float GenerateFloat(float value, float deviation)
        {
            if (deviation == 0)
            {
                return value;
            }

            var halfDeviation = deviation / 2.0f;
            return Randomizer.Next(value - halfDeviation, value + halfDeviation);
        }

        // GenerateVector2
        public static Vector2 GenerateVector2(Vector2 value, Vector2 deviation)
        {
            if (deviation == Vector2.Zero)
                return value;

            var x = GenerateFloat(value.X, deviation.X);
            var y = GenerateFloat(value.Y, deviation.Y);

            return new Vector2(x, y);
        }

        #endregion

        // Acceleration
        public abstract Vector2 Acceleration { get; }

        // Color
        public virtual Color Color { get; } = Color.White;

        // GenerateLifespan
        public int GenerateLifespan()
        {
            return Randomizer.Next(MinLifespan, MaxLifespan);
        }

        // GenerateOpacity
        public float GenerateOpacity()
        {
            return GenerateFloat(Opacity, OpacityDeviation);
        }

        // GenerateRotation
        public float GenerateRotation()
        {
            return GenerateFloat(Rotation, RotationDeviation);
        }

        // GenerateScale
        public Vector2 GenerateScale()
        {
            return GenerateVector2(Scale, ScaleDeviation);
        }

        // GenerateSpeed
        public Vector2 GenerateSpeed()
        {
            return GenerateVector2(Speed, SpeedDeviation);
        }

        // GetImage
        public AtlasImage? GetImage()
        {
            return Images.GetRandomElement();
        }

        // Gravity
        public abstract Vector2 Gravity { get; }

        // Images
        public ReadOnlyCollection<AtlasImage> Images { get; }

        // MinLifespan
        public abstract int MinLifespan { get; }

        // MaxLifespan
        public abstract int MaxLifespan { get; }

        // Opacity
        public virtual float Opacity => 1;

        // OpacityDeviation
        public virtual float OpacityDeviation => 0;

        // Rotation
        public virtual float Rotation { get; }

        // RotationDeviation
        public virtual float RotationDeviation { get; }

        // RotationSpeed
        public virtual float RotationSpeed { get; }

        // Scale
        public abstract Vector2 Scale { get; }

        // ScaleDeviation
        public virtual Vector2 ScaleDeviation { get; }

        // Speed
        public abstract Vector2 Speed { get; }

        // SpeedDeviation
        public virtual Vector2 SpeedDeviation { get; }
    }
}
