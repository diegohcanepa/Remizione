using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione.UI
{
    /// <summary>
    /// UIProgressMeter
    /// </summary>
    public class UIProgressMeter : GameObject
    {
        private int lastKnownValue = -1;
        private readonly TextSprite progress;
        private readonly GameSession session;
        private readonly TextSprite title;

        // Constructor
        public UIProgressMeter(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            // Title
            this.title = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.TerraDark,
                PivotOrigin = RectanglePoint.Top,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Top),
                Scale = ScaleInfo.Text.Large,
                Text = TextRepository.GetValue("Misc.Threshold")
            };

            // Progress
            this.progress = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.TerraDark,
                PivotOrigin = RectanglePoint.Top,
                Position = title.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, -2),
                Scale = ScaleInfo.Text.VeryLarge,
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp, BlendState.AlphaBlend, null);
            title.Draw(gameTime);
            progress.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (lastKnownValue != session.Level)
            {
                lastKnownValue = session.Level;
                progress.Text = $"{session.Level}/{GameSettings.MaximumLevel}";
            }
        }

        #endregion
    }
}
