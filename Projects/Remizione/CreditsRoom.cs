using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// CreditsRoom
    /// </summary>
    public sealed class CreditsRoom : GameRoom
    {
        private readonly UITextButton button;
        private int buttonDisplayCooldown = 10000;
        private readonly Credits credits;
        private int creditsCooldown = 2000;

        // Constructor
        public CreditsRoom(GameSession session, string name)
            : base(session, name)
        {
            AllowPauseMenu = false;
            AtlasName = string.Empty;

            this.button = new UITextButton(Game, InputBindings.Exit)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom)
            };

            LightingSystem = false;

            this.credits = new Credits(Game, TextRepository.GetValue("Misc.Credits"));
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            if (creditsCooldown <= 0)
                credits.Draw(gameTime);

            if (buttonDisplayCooldown < 0)
                button.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (buttonDisplayCooldown >= 0)
                buttonDisplayCooldown -= gameTime.ElapsedGameTime.Milliseconds;
            else
                button.Update(gameTime);

            if (creditsCooldown <= 0)
                credits.Update(gameTime);
            else
                creditsCooldown -= gameTime.ElapsedGameTime.Milliseconds;

            if (buttonDisplayCooldown <= 0 && credits.IsRunning)
            {
                if (button.TestPressed(0))
                {
                    credits.Hide();
                    buttonDisplayCooldown = int.MaxValue;
                }
            }
        }

        #endregion

        // IsShowingCredits
        public bool IsShowingCredits => credits.IsRunning;
    }
}
