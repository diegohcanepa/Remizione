using Adberration;
using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// UIConditionMeter
    /// </summary>
    internal class UIConditionMeter : SessionGameObject<GameSession>
    {
        private readonly Sprite meter = new() { PivotOrigin = RectanglePoint.Top, Scale = ScaleInfo.UIElement.Medium };

        // Constructor
        public UIConditionMeter(GameSession session)
            : base(session)
        {
        }

        // IsVisible
        public bool IsVisible { get; private set; }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (IsVisible)
                meter.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Session.Player != null && Session.Player.Condition != ConditionType.None && Session.Player.ConditionTimer > 0)
            {
                var index = 9 - (Session.Player.ConditionTimer * 9 / GameSettings.ConditionCooldown);

                if (Session.Player.Condition == ConditionType.Curse)
                    meter.RenderImage = Atlases.UI.CurseMeter[index];

                else if (Session.Player.Condition == ConditionType.Poison)
                    meter.RenderImage = Atlases.UI.PoisonMeter[index];

                IsVisible = true;
            }
            else
            {
                IsVisible = false;
            }
        }

        #endregion

        // Position
        public Vector2 Position
        {
            get => meter.Position;
            set => meter.Position = value;
        }
    }
}
