using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Environment
    /// </summary>
    public sealed class Environment
    {
        private SoundInstance? alarmSound;
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
            Lightning.Update(gameTime);
            Rain.Update(gameTime);

            if (session.GameplayMode == GameplayMode.Survival && session.Room is ProceduralRoom)
            {
                if (session.RemainingTime.IsBetween(1, GameSettings.TimeCritical) && !alarmTween.IsRunning)
                {
                    alarmSound = Sound.Play(SoundNames.ExitAlarm, true);
                    alarmTween.Start(TweenStyle.QuadraticInOut, ColorPalette.GlobalLight.Default, ColorPalette.GlobalLight.Critical, 400, -1);
                }

                if (alarmTween.IsRunning)
                    alarmTween.Update(gameTime);

                GlobalLight.Color = alarmTween.IsRunning ? alarmTween.CurrentValue : ColorPalette.GlobalLight.Default;
            }
        }

        #endregion

        // EnterRoom
        public void EnterRoom()
        {
            Rain.EnterRoom();
            alarmTween.Stop();
            GlobalLight.Color = ColorPalette.GlobalLight.Default;
        }

        // ExitRoom
        public void ExitRoom()
        {
            alarmSound?.Stop(3000);
        }

        // Lightning
        public Lightning Lightning { get; }

        // Rain
        public Rain Rain { get; }
    }
}
