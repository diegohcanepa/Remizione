using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    public sealed class UIHPMeter : SessionGameObject<GameSession>
    {
        #region Private fields

        private Actor? actor;
        private readonly List<Sprite> displaySprites = [];

        private readonly ReadOnlyCollection<AtlasImage> redHeartImages;
        private readonly AtlasImage aoeHeartImage;
        private readonly AtlasImage shieldHeartImage;

        private int lastHp = int.MinValue;
        private int lastMaxHp = int.MinValue;
        private int lastShields = int.MinValue;
        private int lastAoeCount = int.MinValue;

        #endregion

        #region Constructor

        public UIHPMeter(GameSession session)
            : base(session)
        {
            redHeartImages = Atlases.UI.FleshinessHearts;
            aoeHeartImage = Atlases.UI.AoeHeart;
            shieldHeartImage = Atlases.UI.ShieldHeart;
        }

        #endregion

        #region Private members

        private void Refresh()
        {
            if (actor == null)
                return;

            displaySprites.Clear();

            var startPos = Screen.HUDArea.GetPoint(RectanglePoint.LeftTop, 5, 2);
            Vector2 currentPos = startPos;
            float spacing = 0.5f;

            // 1. CORAZONES (Fleshiness)
            int hp = actor.HP;
            int maxHp = actor.MaxHP;
            int redContainers = maxHp / 2;

            for (int i = 0; i < redContainers; i++)
            {
                int p1 = (i * 2) + 1;
                int p2 = (i * 2) + 2;

                AtlasImage img = redHeartImages[0];
                if (hp >= p2) img = redHeartImages[2];
                else if (hp == p1) img = redHeartImages[1];

                var sprite = new Sprite(img)
                {
                    PivotOrigin = RectanglePoint.Center,
                    Position = currentPos
                };

                displaySprites.Add(sprite);
                currentPos.X += sprite.BoundingBox.Width + spacing;
            }

            currentPos.Y += 1;

            // 2. CORAZONES (AoE)
            for (int i = 0; i < actor.AoeHearts; i++)
            {
                var sprite = new Sprite(aoeHeartImage)
                {
                    PivotOrigin = RectanglePoint.Center,
                    Position = currentPos
                };

                displaySprites.Add(sprite);
                currentPos.X += sprite.BoundingBox.Width + spacing;
            }

            // 3. CORAZONES (Escudo)
            for (int i = 0; i < actor.ShieldHearts; i++)
            {
                var sprite = new Sprite(shieldHeartImage)
                {
                    PivotOrigin = RectanglePoint.Center,
                    Position = currentPos
                };

                displaySprites.Add(sprite);
                currentPos.X += sprite.BoundingBox.Width + spacing;
            }

            lastHp = hp;
            lastMaxHp = maxHp;
            lastShields = actor.ShieldHearts;
            lastAoeCount = actor.AoeHearts;

            if (displaySprites.Count > 0)
            {
                float totalWidth = currentPos.X - startPos.X;
                BoundingBox = new RectangleF(startPos.X, startPos.Y, totalWidth, displaySprites[0].BoundingBox.Height);
            }
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

            if (lastHp != actor.HP ||
                lastMaxHp != actor.MaxHP ||
                lastShields != actor.ShieldHearts ||
                lastAoeCount != actor.AoeHearts)
            {
                Refresh();
            }
        }

        #endregion

        // BoundingBox
        public RectangleF BoundingBox { get; private set; }
    }
}