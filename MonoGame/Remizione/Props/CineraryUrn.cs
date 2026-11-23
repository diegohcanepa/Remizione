using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// CineraryUrn
    /// </summary>
    public class CineraryUrn : Prop
    {
        private Cycle lastKnownCycle = Cycle.Light;

        // Constructor
        public CineraryUrn(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Props;
            ApproachBehavior = ApproachBehavior.ClosestSide;
            Collider = new("14,12;15,16;0,16;1,12");
            DeathSound = Sound.Find(SoundNames.CineraryUrnBreak);
            DepthOffset = 1;
            Hotspot = new("12,0;12,15;3,15;3,0");
            IsHittable = true;
            IsStatic = true;
            LabelKey = "Prop.CineraryUrn";
            ShadowOffset = new(2, -3);
            ShadowSpotSize = 6;

            var anim = AddAnimation(nameof(Cycle.Light));
            anim.AddFrame(nameof(CineraryUrn) + anim.Name, 10000);

            anim = AddAnimation(nameof(Cycle.Dark));
            anim.AddFrame(nameof(CineraryUrn) + anim.Name, 10000);
        }

        #region Private members

        // Refresh
        private void Refresh()
        {
            if (Session.CycleManager.CurrentCycle == Cycle.Dark && Session.CycleManager.IsTransitioning)
                return;

            lastKnownCycle = Session.CycleManager.CurrentCycle;
            AnimationPlayer.Play(Session.CycleManager.CurrentCycle.ToString());
            //if (lastKnownCycle == Cycle.Light)
            {
                if (IsInViewport)
                {
                    Tweens.XTween = FloatTween.Create(TweenStyle.Linear, X, X + .75f, 50, 6);
                    PlaySound(SoundNames.VesselCover);
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

        // OfferAndShatter
        [ScriptMethod]
        public void OfferAndShatter()
        {
            Session.PlayerData.Ashes -= AshCost;
            Die();
        }
    }
}
