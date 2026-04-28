using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    // Torch
    public sealed class Torch : Prop
    {
        // Constructor
        public Torch(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Props;

            this.AttachedLight = new("Light")
            {
                Color = new(255, 248, 183),
                LightKind = LightKind.Default,
                PivotOrigin = RectanglePoint.Center,
                Position = new(9),
            };

            AttachedLightPosition = new(9);
            LightIntensity = Intensity.High;
            IsAmbientLightSource = true;
        }

        // LightIntensity
        [ScriptProperty]
        public Intensity LightIntensity
        {
            get;
            set
            {
                field = value;

                if (AttachedLight != null)
                {
                    if (field == Intensity.Low)
                        AttachedLight.Scale = new(5);

                    else if (field == Intensity.Medium)
                        AttachedLight.Scale = new(9);

                    if (field == Intensity.High)
                        AttachedLight.Scale = new(15);
                }
            }
        }
    }
}
