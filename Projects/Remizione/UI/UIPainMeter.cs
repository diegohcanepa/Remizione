using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Remizione
{
    public sealed class UIPainMeter : SessionGameObject<GameSession>
    {
        #region Private fields

        private const int MaxPoints = 5;

        private Actor? actor;
        private readonly Sprite[] displaySprites = new Sprite[MaxPoints];

        private int lastHp = int.MinValue;

        #endregion

        #region Constructor

        // Constructor
        public UIPainMeter(GameSession session)
            : base(session)
        {

            for (int i = 0; i < MaxPoints; i++)
            {
                displaySprites[i] = new Sprite
                {
                    PivotOrigin = RectanglePoint.Center
                };
            }
        }

        #endregion

        #region Private members

        // Refresh
        private void Refresh()
        {
            if (actor == null)
                return;

            int hp = actor.HP;

            // Fórmula aplicada directamente para determinar la resistencia restante del bloque actual
            int meterValue = (hp > 0 && hp % MaxPoints == 0) ? MaxPoints : (hp % MaxPoints);

            var startPos = Screen.HUDArea.GetPoint(RectanglePoint.LeftTop, 4, 10);
            Vector2 currentPos = startPos;
            float spacing = .25f;

            for (int i = 0; i < MaxPoints; i++)
            {
                // Solo se actualiza la imagen y la posición, sin recrear instancias
                displaySprites[i].RenderImage = i < meterValue
                    ? Atlases.UI.PainHeartFull
                    : Atlases.UI.PainHeartEmpty;

                displaySprites[i].Position = currentPos;
                currentPos.X += displaySprites[i].BoundingBox.Width + spacing;
            }

            lastHp = hp;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (actor == null)
                return;

            for (int i = 0; i < MaxPoints; i++)
            {
                displaySprites[i].Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Session.Player != actor)
            {
                actor = Session.Player;
                Refresh();
            }

            if (actor == null)
                return;

            if (lastHp != actor.HP)
                Refresh();
        }

        #endregion
    }
}