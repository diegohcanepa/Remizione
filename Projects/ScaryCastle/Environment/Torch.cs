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

            this.AttachedLight = new("Light")
            {
                Color = new(255, 248, 183),
                LightKind = LightKind.Ambient,
                PivotOrigin = RectanglePoint.Center,
                Position = new(9),
                Scale = new(15)
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
