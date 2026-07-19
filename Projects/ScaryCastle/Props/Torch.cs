using Adberration.Scripting;
using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// Torch
    /// </summary>
    public sealed class Torch : Prop
    {
        // Constructor
        public Torch(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Props;
            ApproachBehavior = ApproachBehavior.None;
            DisplayNameKey = "Prop.Torch";
            Verb = Verb.Examine;

            this.AttachedLight = new("Light")
            {
                Ambient = true,
                Color = new(190, 132, 50),
                PivotOrigin = RectanglePoint.Center,
                LightKind = LightKind.Fire,
                Passes = 2,
                Position = new(9),
                Scale = new(10, 16)
            };

            AttachedLightPosition = new(9);
        }

        // InvalidateAnimation
        private void InvalidateAnimation()
        {
            AnimationPlayer.Play(IgnoreAttachedLight ? "Off" : "On");
        }

        // OnIgnoreAttachedLightChanged
        protected override void OnIgnoreAttachedLightChanged()
        {
            base.OnIgnoreAttachedLightChanged();
            InvalidateAnimation();
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            InvalidateAnimation();
        }

        // ExtraLight
        [ScriptProperty]
        public bool ExtraLight
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    if (AttachedLight != null)
                        AttachedLight.Scale = field ? new(12, 16) : new(10, 16);
                }
            }
        }
    }
}
