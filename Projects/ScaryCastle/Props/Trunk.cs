using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// Trunk
    /// </summary>
    public class Trunk : BreakableProp
    {
        private int breakCooldoown;

        // Constructor
        public Trunk(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Environment;
            DeathSound = Sound.Find(SoundNames.WoodDebris);
            DisplayNameKey = "Prop.Trunk";
            HitEffect = HitEffect.Shake;
            HurtSound = Sound.Find(SoundNames.ImpactA);
            PropState = PropState.Closed;
        }

        #region Protected members

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            breakCooldoown = 1000;
        }

        /*
        // OnPropStateChanged
        protected override void OnPropStateChanged(PropState previousState)
        {
            AnimationPlayer.Play(PropState == PropState.Open ? AnimationNames.Open : AnimationNames.Closed, false);

            //AllowInteraction = PropState == PropState.Closed;

            if (LoadState != LoadState.Loaded)
                return;

            if (PropState == PropState.Open)
                PlaySound(SoundNames.TrunkOpen);
        }
        */

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (IsBroken)
                return;

            if (PropState == PropState.Open)
            {
                if (breakCooldoown > 0)
                {
                    breakCooldoown -= gameTime.ElapsedGameTime.Milliseconds;
                }
                else if (breakCooldoown <= 0)
                {
                    HP = int.MinValue;
                    Die();
                }
            }
        }

        #endregion

        // CanInteract
        public override bool CanInteract(Actor requester)
        {
            return PropState != PropState.Open && base.CanInteract(requester);
        }
    }
}
