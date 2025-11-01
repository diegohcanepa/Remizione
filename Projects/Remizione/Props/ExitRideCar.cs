using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ExitRideCar
    /// </summary>
    public sealed class ExitRideCar : IsometricProp
    {
        // Constructor
        public ExitRideCar(GameSession session, string name)
            : base(session, name)
        {
            HitEffect = HitEffect.Shake;
        }

        #region Protected members

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            IsTurnedOn = false;
            this.AnimationPlayer.Play("Off");
        }

        #endregion

        // IsEmittingLight
        public override bool IsEmittingLight => IsTurnedOn;

        // IsTurnedOn
        [ScriptProperty]
        public bool IsTurnedOn { get; private set; }

        // TurnOn
        [ScriptMethod]
        public void TurnOn()
        {
            AnimationPlayer.Play("On", false);
            IsTurnedOn = true;
        }
    }
}
