using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Environment
    /// </summary>
    public sealed class Environment
    {
        private readonly ColorTween alarmTween = new();
        private readonly GameSession session;

        // Constructor
        public Environment(GameSession session)
        {
            this.session = session;
            this.Lightning = new(session);
            this.Rain = new Rain(session);

            // Global light
            this.GlobalLight ??= new Light(session.Game, "GlobalLight")
            {
                Color = ColorPalette.GlobalLight.Default,
                LightKind = LightKind.Global,
                ImageName = "GlobalLight",
                PivotOrigin = RectanglePoint.Center,
                Position = Screen.Center,
            };

            this.GlobalLight.Prepare(Atlases.Environment);
        }

        #region Internal members

        // GlobalLight
        internal Light GlobalLight { get; }

        // Update
        internal void Update(GameTime gameTime)
        {
            if (session.GameplayMode == GameplayMode.Survival && session.Room is ProceduralRoom)
            {
                if (session.RemainingTime <= GameSettings.TimeCritical && !alarmTween.IsRunning)
                    alarmTween.Start(TweenStyle.QuadraticInOut, ColorPalette.GlobalLight.Default, ColorPalette.GlobalLight.Critical, 400, -1);

                Lightning.Update(gameTime);
                Rain.Update(gameTime);

                if (alarmTween.IsRunning)
                    alarmTween.Update(gameTime);

                GlobalLight.Color = alarmTween.IsRunning ? alarmTween.CurrentValue : ColorPalette.GlobalLight.Default;
            }
        }

        #endregion

        // EnterRoom
        public void EnterRoom(GameRoom room)
        {
            Rain.EnterRoom();
            alarmTween.Stop();
            GlobalLight.Color = ColorPalette.GlobalLight.Default;
        }

        // Lightning
        public Lightning Lightning { get; }

        // Rain
        public Rain Rain { get; }
    }
}
