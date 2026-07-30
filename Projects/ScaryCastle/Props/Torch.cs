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
                Color = new(210, 132, 100),
                PivotOrigin = RectanglePoint.Center,
                LightKind = LightKind.Fire,
                Passes = 2,
                Position = new(9),
                Scale = new(12, 18)
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
    }
}
