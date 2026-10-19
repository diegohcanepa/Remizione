using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// UIHeartMeter
    /// </summary>
    public sealed class UIHeartMeter : SessionGameObject<GameSession>
    {
        #region Private fields

        private const int PointsPerHeart = 10;
        private const int PointsPerHalfHeart = 5;

        private Actor? actor;
        private readonly List<Sprite> displaySprites = [];

        private readonly ReadOnlyCollection<AtlasImage> redHeartImages;

        private int lastHp = int.MinValue;
        private int lastMaxHp = int.MinValue;

        #endregion

        #region Constructor

        // Constructor
        public UIHeartMeter(GameSession session)
            : base(session)
        {
            redHeartImages = Atlases.UI.FleshinessHearts;
        }

        #endregion

        #region Private members

        // Refresh
        private void Refresh()
        {
            if (actor == null)
                return;

            displaySprites.Clear();

            var startPos = Screen.HUDArea.GetPoint(RectanglePoint.LeftTop, 5, 2);
            Vector2 currentPos = startPos;
            float spacing = 0.5f;

            // 1. Fleshiness
            int hp = actor.HP;
            int maxHp = actor.MaxHP;
            int redContainers = (maxHp + PointsPerHeart - 1) / PointsPerHeart;
            BoundingBox = RectangleF.Empty;

            for (int i = 0; i < redContainers; i++)
            {
                int baseHp = i * PointsPerHeart;
                int halfThreshold = baseHp + PointsPerHalfHeart;

                AtlasImage img;
                if (hp > halfThreshold)
                    img = redHeartImages[2]; // Lleno (11 a 20 puntos dentro del contenedor)
                else if (hp > baseHp)
                    img = redHeartImages[1]; // Mitad (1 a 10 puntos dentro del contenedor)
                else
                    img = redHeartImages[0]; // Vacío (0 puntos o menos)

                var sprite = new Sprite(img)
                {
                    PivotOrigin = RectanglePoint.Center,
                    Position = currentPos
                };

                displaySprites.Add(sprite);
                currentPos.X += sprite.BoundingBox.Width + spacing;

                BoundingBox = RectangleF.Union(BoundingBox, sprite.BoundingBox);
            }

            lastHp = hp;
            lastMaxHp = maxHp;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (actor == null)
                return;

            for (int i = 0; i < displaySprites.Count; i++)
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

            if (lastHp != actor.HP || lastMaxHp != actor.MaxHP)
                Refresh();
        }

        #endregion

        // BoundingBox
        public RectangleF BoundingBox { get; private set; }
    }
}