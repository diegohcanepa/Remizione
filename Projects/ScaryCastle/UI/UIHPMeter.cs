using Engendro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// UIHPMeter
    /// </summary>
    public sealed class UIHPMeter : HUDElement
    {
        #region Private fields

        private Actor? actor;
        private readonly Sprite[] icons;
        private readonly Dictionary<ConditionType, IList<AtlasImage>> imageGroups = [];
        private int lastFilledIconIndex = -1;
        private int lastKnownMaxValue;
        private int lastKnownConditionAmount;
        private int lastKnownValue;
        private readonly Vector2Tween scaleTween = new();
        private int totalIcons;

        #endregion

        #region Constructor

        // Constructor
        public UIHPMeter(GameSession session)
            : base(session)
        {
            imageGroups.Add(ConditionType.None, Atlases.UI.RedHearts);
            imageGroups.Add(ConditionType.Curse, Atlases.UI.PurpleHearts);
            imageGroups.Add(ConditionType.Poison, Atlases.UI.GreenHearts);

            this.icons = new Sprite[10];
            var pos = Screen.HUDArea.GetPoint(RectanglePoint.LeftTop, new(11, 3));

            for (var i = 0; i < icons.Length; i++)
            {
                icons[i] = new(imageGroups[0][0])
                {
                    PivotOrigin = RectanglePoint.Center,
                    Position = pos
                };

                pos.X += icons[i].BoundingBox.Width + .5f;
            }

            scaleTween.Start(TweenStyle.Linear, Vector2.One, Vector2.One * 1.1f, 300, -1);
        }

        #endregion

        #region Private members

        // Refresh
        private void Refresh()
        {
            if (actor == null)
                return;

            // 1. Fuentes de verdad
            int hp = actor.HP;
            int maxHp = actor.MaxHP;
            int amount = (actor.Condition != ConditionType.None) ? actor.ConditionAmount : 0;

            // La "vida segura" es la que no está marcada por el estado
            int safeHp = hp - amount;

            // 2. Dimensionamiento
            totalIcons = maxHp / 2;
            var images = imageGroups[actor.Condition];

            for (int i = 0; i < totalIcons; i++)
            {
                if (i >= icons.Length)
                    break;

                // Puntos de este icono (p1 = inferior, p2 = superior)
                int p1 = (i * 2) + 1;
                int p2 = (i * 2) + 2;

                // --- LÓGICA DE RENDERIZADO ---

                // CASO A: El corazón no tiene vida (puntos por encima del HP actual)
                if (hp < p1)
                {
                    icons[i].RenderImage = imageGroups[0][0]; // Vacío
                }

                // CASO B: El corazón está lleno (tiene los 2 puntos de vida)
                else if (hp >= p2)
                {
                    if (safeHp >= p2)
                    {
                        // Ambos puntos son rojos
                        icons[i].RenderImage = imageGroups[0][2];
                    }
                    else if (safeHp >= p1)
                    {
                        // El punto inferior es rojo, el superior es veneno
                        // AQUÍ USAS TU NUEVO ARTE (Frame 3)
                        icons[i].RenderImage = images[3];
                    }
                    else
                    {
                        // Ambos puntos son veneno
                        icons[i].RenderImage = images[2];
                    }
                }

                // CASO C: El corazón está por la mitad (solo tiene 1 punto de vida)
                else // hp == p1
                {
                    if (safeHp >= p1)
                    {
                        // El único punto que tiene es rojo
                        icons[i].RenderImage = imageGroups[0][1];
                    }
                    else
                    {
                        // El único punto que tiene es veneno
                        icons[i].RenderImage = images[1];
                    }
                }
            }

            lastKnownValue = hp;
            lastKnownMaxValue = maxHp;
            lastKnownConditionAmount = amount;
            lastFilledIconIndex = actor.IsDead ? 0 : ((actor.HP + 1) / 2) - 1;
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
                    lastKnownConditionAmount = int.MinValue;
                }

                Refresh();
            }

            if (actor == null || actor.HP == 0)
                return;

            scaleTween.Update(gameTime);

            if (lastKnownValue != actor.HP || lastKnownMaxValue != actor.MaxHP || lastKnownConditionAmount != actor.ConditionAmount)
                Refresh();

            if (totalIcons > 0)
            {
                if (actor.HP <= 2 || actor.ConditionAmount > 0)
                    icons[lastFilledIconIndex].Scale = scaleTween.CurrentValue;
                else
                    icons[lastFilledIconIndex].Scale = Vector2.One;
            }
        }

        #endregion
    }
}
