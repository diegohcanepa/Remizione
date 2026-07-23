using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// UIRunModifiers
    /// </summary>
    public sealed class UIRunModifiers : SessionGameObject<GameSession>
    {
        private readonly List<ModifierIcon> activeIcons = [];
        private readonly Dictionary<string, ModifierIcon> icons = [];
        private int lastKnownVersion = -1;

        #region Constructor

        // Constructor
        public UIRunModifiers(GameSession session)
            : base(session)
        {
            foreach (var modifier in Session.RunModifiers.All)
            {
                icons[modifier.Name] = new(modifier);
            }
        }

        #endregion

        #region Private members

        // Refresh
        private void Refresh()
        {
            activeIcons.Clear();

            if (Session.RunModifiers.ActiveCount == 0)
                return;

            foreach (var modifier in Session.RunModifiers.ActiveModifiers)
            {
                activeIcons.Add(icons[modifier.Name]);
            }

            float spacing = 2;
            float w = 0;

            // Calculamos el ancho total sumando los bounding boxes y los espacios intermedios
            for (var i = 0; i < activeIcons.Count; i++)
            {
                w += activeIcons[i].BoundingBox.Width;
                if (i < activeIcons.Count - 1)
                    w += spacing;
            }

            // Buscamos el centro horizontal de la pantalla. 
            float screenCenterX = Screen.Area.Width / 2;
            float topMargin = 4; // Margen desde el borde superior de la pantalla

            // El punto de partida (X) a la izquierda de la fila centrada
            float startX = screenCenterX - (w / 2f);

            // Iteramos para posicionar cada ícono uno al lado del otro
            float currentX = startX;
            for (var i = 0; i < activeIcons.Count; i++)
            {
                var icon = activeIcons[i];
                float halfWidth = icon.BoundingBox.Width / 2f;

                // Como el PivotOrigin es 'Top' (centro superior del sprite), 
                // sumamos la mitad de su propio ancho para centrar el pivote en la posición X actual
                icon.Position = new Vector2(currentX + halfWidth, topMargin);

                // Desplazamos la X para el siguiente ícono en la fila
                currentX += icon.BoundingBox.Width + spacing;
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            for (var i = 0; i < activeIcons.Count; i++)
            {
                activeIcons[i].Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (lastKnownVersion != Session.RunModifiers.ContentVersion)
            {
                Refresh();
                lastKnownVersion = Session.RunModifiers.ContentVersion;
            }

            for (var i = 0; i < activeIcons.Count; i++)
            {
                activeIcons[i].Update(gameTime);
            }
        }

        #endregion

        /// <summary>
        /// ModifierIcon
        /// </summary>
        private sealed class ModifierIcon : GameObject
        {
            private readonly FlatMeter meter;
            private readonly RunModifier modifier;
            private readonly Sprite sprite;

            // Constructor
            public ModifierIcon(RunModifier modifier)
            {
                this.modifier = modifier;

                sprite = new()
                {
                    PivotOrigin = RectanglePoint.Top,
                    RenderImage = modifier.Definition.Image,
                    Scale = ScaleInfo.UIElement.Medium
                };

                meter = new(modifier.Definition.MeterBackColor, modifier.Definition.MeterForeColor, Color.Transparent, new(8, 1), 0)
                {
                    MaximumValue = modifier.Definition.Cooldown
                };
            }

            #region Protected members

            // OnDraw
            protected override void OnDraw(GameTime gameTime)
            {
                sprite.Draw(gameTime);

                if (meter.MaximumValue > 0)
                    meter.Draw(gameTime);
            }

            // OnUpdate
            protected override void OnUpdate(GameTime gameTime)
            {
                if (meter.MaximumValue > 0)
                {
                    meter.Value = modifier.Timer;
                    meter.Update(gameTime);
                }
            }

            #endregion

            // BoundingBox
            public RectangleF BoundingBox => sprite.BoundingBox;

            // Position
            public Vector2 Position
            {
                get => sprite.Position;
                set
                {
                    sprite.Position = value;
                    meter.Position = sprite.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, 1);
                }
            }
        }
    }
}
