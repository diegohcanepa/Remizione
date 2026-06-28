using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScaryCastle
{
    /// <summary>
    /// HUDConditionMeter
    /// </summary>
    internal class HUDConditionMeter : HUDElement
    {
        private readonly Sprite meter = new() { Position = new(14, 13) };

        // Constructor
        public HUDConditionMeter(GameSession session)
            : base(session)
        {
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (Session.Player == null || Session.Player.ConditionTimer <= 0)
                return;

            meter.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Session.Player == null || Session.Player.Condition == ConditionType.None)
                return;

            var index = 9 - (Session.Player.ConditionTimer * 9 / GameSettings.ConditionCooldown);

            if (Session.Player.Condition == ConditionType.Curse)
                meter.RenderImage = Atlases.UI.CurseMeter[index];

            else if (Session.Player.Condition == ConditionType.Poison)
                meter.RenderImage = Atlases.UI.PoisonMeter[index];
        }

        #endregion
    }
}
