using Adberration;
using Adberration.Scripting;
using Engendro;

namespace Remizione
{
    /// <summary>
    /// GroundTorch
    /// </summary>
    public sealed class GroundTorch : Prop
    {
        // Constructor
        public GroundTorch(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Props;
            LabelKey = "Prop.Torch";
            DefaultVerb = Verb.Examine;
            DepthOffset = -3;

            this.AttachedLight = new("Light", LightKind.Fire)
            {
                Passes = 2,
                PivotOrigin = RectanglePoint.Center,
                Position = new(9),
                Scale = new(18)
            };

            AttachedLight.Unlit(true);

            AttachedLightPosition = new(2, 9);
        }

        #region Private members

        // InvalidateAnimation
        private void InvalidateAnimation()
        {
            AnimationPlayer.Play(IsLit ? "Lit" : "Unlit");
        }

        #endregion

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            InvalidateAnimation();
        }

        // IsLit
        [ScriptProperty]
        public bool IsLit
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;

                    if (field)
                    {
                        InvalidateAnimation();
                        PlaySound(SoundNames.Bonfire, true);
                        AttachedLight?.Lit(Session.State == GameSessionState.Loading);
                    }
                }
            }
        }
    }
}
