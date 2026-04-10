using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// UIHPMeter
    /// </summary>
    public sealed class UIHPMeter : GameObject
    {
        #region Private fields

        private int fullIcons;
        private bool hasHalfIcon;
        private readonly List<IList<AtlasImage>> statusEffectImages = [];
        private readonly Sprite[] icons;
        private int lastKnownMaxValue;
        private int lastKnownStatusEffectAmount;
        private int lastKnownValue;
        private int totalIcons;

        #endregion

        #region Constructor

        // Constructor
        public UIHPMeter(Vector2 margin)
        {
            statusEffectImages.Add(Atlases.UI.RedHearts);
            statusEffectImages.Add(Atlases.UI.PurpleHearts);
            statusEffectImages.Add(Atlases.UI.GreenHearts);

            this.icons = new Sprite[10];
            var pos = Screen.HUDArea.GetPoint(RectanglePoint.LeftTop, margin);

            for (var i = 0; i < icons.Length; i++)
            {
                icons[i] = new(statusEffectImages[0][0])
                {
                    Position = pos
                };

                pos.X += icons[i].BoundingBox.Width + .5f;
            }
        }

        #endregion

        #region Private members

        // Refresh
        private void Refresh()
        {
            if (Actor == null)
                return;

            // 1. Fuentes de verdad
            int hp = Actor.HP;
            int maxHp = Actor.MaxHP;
            int amount = (Actor.StatusEffect != StatusEffect.None) ? Actor.StatusEffectAmount : 0;

            // La "vida segura" es la que no está marcada por el estado
            int safeHp = hp - amount;

            // 2. Dimensionamiento
            totalIcons = maxHp / 2;
            var images = (Actor.StatusEffect != StatusEffect.None)
                ? statusEffectImages[(int)Actor.StatusEffect]
                : statusEffectImages[0];

            for (int i = 0; i < totalIcons; i++)
            {
                if (i >= icons.Length) break;

                // Puntos de este icono (p1 = inferior, p2 = superior)
                int p1 = i * 2 + 1;
                int p2 = i * 2 + 2;

                // --- LÓGICA DE RENDERIZADO ---

                // CASO A: El corazón no tiene vida (puntos por encima del HP actual)
                if (hp < p1)
                {
                    icons[i].RenderImage = statusEffectImages[0][0]; // Vacío
                }

                // CASO B: El corazón está lleno (tiene los 2 puntos de vida)
                else if (hp >= p2)
                {
                    if (safeHp >= p2)
                    {
                        // Ambos puntos son rojos
                        icons[i].RenderImage = statusEffectImages[0][2];
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
                        icons[i].RenderImage = statusEffectImages[0][1];
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
            lastKnownStatusEffectAmount = amount;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (Actor == null)
                return;

            for (var i = 0; i < totalIcons; i++)
            {
                icons[i].Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Actor != null)
            {
                if (lastKnownValue != Actor.HP || 
                    lastKnownMaxValue != Actor.MaxHP ||
                    lastKnownStatusEffectAmount != Actor.StatusEffectAmount)
                    Refresh();
            }
        }

        #endregion

        // Actor
        public Actor? Actor
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;

                    if (field == null)
                    {
                        lastKnownValue = int.MinValue;
                        lastKnownMaxValue = int.MinValue;
                        lastKnownStatusEffectAmount = int.MinValue;
                    }

                    Refresh();
                }
            }
        }
    }
}
