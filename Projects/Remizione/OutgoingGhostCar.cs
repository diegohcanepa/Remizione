using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// OutgoingGhostCar
    /// </summary>
    public sealed class OutgoingGhostCar : IsometricProp
    {
        private readonly Light frontLight;
        private static readonly Vector2 frontLightPosition = new Vector2(8, 16);
        private readonly Light rearLight;
        private static readonly Vector2 rearLightPosition = new Vector2(24, 20);

        // Constructor
        public OutgoingGhostCar(GameSession session, string name)
            : base(session, name)
        {
            DamageStyle = DamageStyle.Shake;

            this.frontLight = new Light(session.Game, "<rear>")
            {
                Color = Color.Green,
                Scale = new(2, 1)
            };

            this.rearLight = new Light(session.Game, "<front>")
            {
                Color = Color.Red,
                Scale = new(2, 1)
            };
        }

        #region Protected members

        // OnDrawLights
        protected override void OnDrawLights(GameTime gameTime)
        {
            base.OnDrawLights(gameTime);

            frontLight.Position = this.GetAbsolutePoint(frontLightPosition);
            frontLight.Draw(gameTime);

            rearLight.Position = this.GetAbsolutePoint(rearLightPosition);
            rearLight.Draw(gameTime);
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            IsTurnedOn = false;
            Light?.TurnOff(true);
            this.AnimationPlayer.Play("Off");
        }

        // OnUpdate 
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (IsEmittingLight)
            {
                frontLight.Update(gameTime);
                rearLight.Update(gameTime);
            }
        }

        #endregion

        // DestinationOffset
        [ScriptProperty(CodingContext.EntityDeclaration)]
        public Vector2 DestinationOffset { get; set; }

        // IsEmittingLight
        public override bool IsEmittingLight => IsTurnedOn;

        // IsTurnedOn
        [ScriptProperty]
        public bool IsTurnedOn { get; private set; }

        // Launch
        [ScriptMethod]
        public void Launch()
        {
            Tweens.PositionTween = Vector2Tween.Create(TweenStyle.CubicIn, Position, Position + DestinationOffset, 2000);
        }

        // RoomTheme
        [ScriptProperty]
        public ProceduralRoomTheme RoomTheme { get; set; }

        // TurnOn
        [ScriptMethod]
        public void TurnOn()
        {
            AnimationPlayer.Play("On", false);
            Light?.TurnOn();
            IsTurnedOn = true;
        }
    }
}
