using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// LightningInvocation
    /// </summary>
    public sealed class LightningInvocation : Invocation
    {
        // Constructor
        public LightningInvocation(IGameAction action, GameThing target)
            : base(action, target)
        {
            var animation = AddAnimation(AnimationNames.Default);
            animation.AddFrameSequence("Lightning", 80, 1, 3);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            this.AnimationPlayer.Play(AnimationNames.Default, false);
            InputManager.DefaultPlayer.GamePad.Vibrate(200, .4f, .4f);
            Session.Camera.Shake(TweenStyle.Linear, new Vector2(1.5f), 66, 4);
            Sound.Play(SoundNames.Lightning);
        }

        #endregion
    }
}
