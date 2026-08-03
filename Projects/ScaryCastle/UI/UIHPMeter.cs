using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;

namespace ScaryCastle
{
    /// <summary>
    /// UIHPMeter
    /// </summary>
    public sealed class UIHPMeter : SessionGameObject<GameSession>
    {
        #region Private fields

        private Actor? actor;
        private readonly Sprite[] icons;
        private readonly ReadOnlyCollection<AtlasImage> heartImages;
        private int lastKnownValue = int.MinValue;
        private int lastKnownMaxValue = int.MinValue;
        private int totalIcons;

        #endregion

        #region Constructor

        // Constructor
        public UIHPMeter(GameSession session)
            : base(session)
        {
            // Directamente referenciamos las imágenes de los corazones rojos:
            // Frame 0 = Vacío, Frame 1 = Medio corazón, Frame 2 = Corazón lleno
            heartImages = Atlases.UI.RedHearts;

            icons = new Sprite[10];
            var pos = Screen.HUDArea.GetPoint(RectanglePoint.LeftTop, new(15, 3));

            for (var i = 0; i < icons.Length; i++)
            {
                icons[i] = new(heartImages[0])
                {
                    PivotOrigin = RectanglePoint.Center,
                    Position = pos
                };

                pos.X += icons[i].BoundingBox.Width + 0.5f;
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
            int maxHp = actor.MaxHP;

            totalIcons = maxHp / 2;

            for (int i = 0; i < totalIcons; i++)
            {
                if (i >= icons.Length)
                    break;

                int p1 = (i * 2) + 1; // Primer punto de vida de este icono
                int p2 = (i * 2) + 2; // Segundo punto de vida de este icono

                if (hp >= p2)
                {
                    icons[i].RenderImage = heartImages[2]; // Corazón Lleno
                }
                else if (hp == p1)
                {
                    icons[i].RenderImage = heartImages[1]; // Medio Corazón
                }
                else
                {
                    icons[i].RenderImage = heartImages[0]; // Corazón Vacío
                }
            }

            lastKnownValue = hp;
            lastKnownMaxValue = maxHp;

            var w = (totalIcons * icons[0].BoundingBox.Width) + (.5f * totalIcons);
            
            BoundingBox = new(icons[0].Position.X, icons[0].Position.Y, w, icons[0].BoundingBox.Height);

            Session.StatusHUD.Statuses.Refresh();
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (actor == null)
                return;

            for (var i = 0; i < totalIcons; i++)
            {
                icons[i].Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Session.Player != actor)
            {
                actor = Session.Player;

                if (actor == null)
                {
                    lastKnownValue = int.MinValue;
                    lastKnownMaxValue = int.MinValue;
                }

                Refresh();
            }

            if (actor == null || actor.IsDead)
                return;

            if (lastKnownValue != actor.HP || lastKnownMaxValue != actor.MaxHP)
            {
                Refresh();
            }
        }

        #endregion

        // BoundingBox
        public RectangleF BoundingBox { get; private set; }
    }
}