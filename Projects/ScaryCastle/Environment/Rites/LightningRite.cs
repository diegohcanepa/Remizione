using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// LightningRite
    /// </summary>
    public sealed class LightningRite : Rite
    {
        // Constructor
        public LightningRite(GameSession session, Item item)
            : base(session, item, session.InteractionData.LastKnownCastPosition)
        {
            var animation = AddAnimation("Default");
            animation.AddFrameSequence("Lightning", 80, 1, 3);
        }

        #region Protected members

        // OnCast
        protected override void OnCast()
        {
            this.AnimationPlayer.Play("Default", false);
            InputManager.DefaultPlayer.GamePad.Vibrate(200, .4f, .4f);
            Session.Camera.Shake(TweenStyle.Linear, new Vector2(1.5f), 66, 4);
            Sound.Play(SoundNames.Lightning);
        }

        #endregion
    }
}
