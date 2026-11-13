using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// AshVessel
    /// </summary>
    public class AshVessel : Prop
    {
        private Cycle lastKnownCycle = Cycle.Light;

        // Constructor
        public AshVessel(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Props;
            ApproachBehavior = ApproachBehavior.ClosestSide;
            Collider = new("14,12;15,16;0,16;1,12");
            DepthOffset = 1;
            Hotspot = new("12,0;12,15;3,15;3,0");
            IsStatic = true;
            LabelKey = "Prop.AshVessel";
            ShadowOffset = new(2, -3);
            ShadowSpotSize = 6;

            var anim = AddAnimation(nameof(Cycle.Light));
            anim.AddFrame(nameof(AshVessel) + anim.Name, 10000);

            anim = AddAnimation(nameof(Cycle.Dark));
            anim.AddFrame(nameof(AshVessel) + anim.Name, 10000);
        }

        #region Private members

        // Refresh
        private void Refresh()
        {
            if (Session.CycleManager.CurrentCycle == Cycle.Dark && Session.CycleManager.IsTransitioning)
                return;

            lastKnownCycle = Session.CycleManager.CurrentCycle;
            AnimationPlayer.Play(Session.CycleManager.CurrentCycle.ToString());
            if (lastKnownCycle == Cycle.Light)
            {
                if (IsInViewport)
                {
                    Tweens.XTween = FloatTween.Create(TweenStyle.Linear, X, X + .5f, 40, 4);
                    PlaySound(SoundNames.AshVesselCover);
                }
            }
            IsEnabled = lastKnownCycle == Cycle.Dark;
        }

        #endregion

        #region Protected members

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            Refresh();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (lastKnownCycle != Session.CycleManager.CurrentCycle)
                Refresh();
        }

        #endregion

        // IsEnabled
        [ScriptProperty]
        public bool IsEnabled { get; private set; }
    }
}
