using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// UIFearMeter
    /// </summary>
    public sealed class UIFearMeter : GameObject
    {
        private readonly string displayText = TextRepository.GetValue("Misc.FearSyncope");
        private readonly Sprite[] icons;
        private int lastKnownAmount = -1;
        private int lastSecondsValue = -1;
        private readonly GameSession session;
        private readonly TextSprite text;
        private readonly FloatTween tween = new();

        #region Constructor

        // Constructor
        public UIFearMeter(GameSession session)
        {
            this.session = session;

            this.icons = new Sprite[10];

            for (var i = 0; i < icons.Length; i++)
            {
                this.icons[i] = new()
                {
                    PivotOrigin = RectanglePoint.Center,
                };
            }

            this.text = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Orange,
                Scale = ScaleInfo.Text.Huge,
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, 2, -12)
            };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!RunManager.HasContent)
                return;

            if (RunManager.HasContent && session.FearManager.CurrentFear == session.FearManager.MaximumFear && !session.FearManager.IsDeadByFear)
                text.Draw(gameTime);

            for (var i = 0; i < RunManager.MaximumFear; i++)
            {
                if (icons[i].IsEmpty)
                    break;

                icons[i].Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (!RunManager.HasContent)
                return;

            if (lastKnownAmount != session.FearManager.CurrentFear)
            {
                var blink = session.FearManager.CurrentFear > lastKnownAmount;
                lastKnownAmount = session.FearManager.CurrentFear;
                Refresh(blink);
            }

            for (var i = 0; i < RunManager.MaximumFear; i++)
            {
                if (icons[i].IsEmpty)
                    break;

                icons[i].Update(gameTime);
            }

            // Evitamos lógica pesada: si no llegamos al máximo o ya morimos, no actualizamos el texto
            if (!RunManager.HasContent || session.FearManager.CurrentFear < session.FearManager.MaximumFear || session.FearManager.IsDeadByFear)
                return;

            // El manager nos da los milisegundos precisos, nosotros solo formateamos para el humano
            int currentSeconds = Math.Max(0, (int)Math.Ceiling(session.FearManager.RemainingDeathTime / 1000f));

            if (currentSeconds != lastSecondsValue)
            {
                lastSecondsValue = currentSeconds;
                text.Text = $"{displayText}: {currentSeconds}";
            }
        }

        #endregion

        // Refresh
        public void Refresh(bool blink)
        {
            var pos = Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, 6, -8);

            for (var i = 0; i < icons.Length; i++)
            {
                icons[i].Position = pos;
                icons[i].RenderImage = Atlases.UI.FearEmpty;
                pos.X += icons[i].BoundingBox.Width + 1;
            }

            for (var i = 0; i < session.FearManager.CurrentFear; i++)
            {
                icons[i].RenderImage = Atlases.UI.FearFull;

                if (blink)
                {
                    if (i == session.FearManager.CurrentFear - 1)
                    {
                        tween.Start(TweenStyle.Linear, 1, .5f, 400, 10);
                        icons[i].Tweens.OpacityTween = tween;
                    }
                    else
                    {
                        icons[i].Tweens.OpacityTween = null;
                        icons[i].Opacity = 1;
                    }
                }
            }
        }

        // Reset
        public void Reset()
        {
            lastKnownAmount = -1;
            Refresh(false);
        }
    }
}
