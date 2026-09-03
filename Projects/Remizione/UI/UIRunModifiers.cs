using Engendro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// UIRunModifiers
    /// </summary>
    public sealed class UIRunModifiers : GameObject
    {
        private readonly List<ModifierIcon> activeIcons = [];
        private readonly Dictionary<string, ModifierIcon> icons = [];
        private int lastKnownVersion = -1;
        private readonly Run run;

        #region Constructor

        // Constructor
        public UIRunModifiers(Run run)
        {
            this.run = run;

            foreach (var modifier in run.Modifiers.All)
            {
                icons[modifier.Name] = new(modifier.Definition.Image);
            }
        }

        #endregion

        #region Private members

        // Refresh
        private void Refresh()
        {
            activeIcons.Clear();

            if (run.Modifiers.ActiveCount == 0)
                return;

            foreach (var modifier in run.Modifiers.ActiveModifiers)
            {
                activeIcons.Add(icons[modifier.Name]);
            }

            float spacing = 0;
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
            float topMargin = 2; // Margen desde el borde superior de la pantalla

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
                icon.Position = new Vector2(currentX + halfWidth, topMargin + (icon.BoundingBox.Height / 2));

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
            if (lastKnownVersion != run.Modifiers.ContentVersion)
            {
                Refresh();
                lastKnownVersion = run.Modifiers.ContentVersion;
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
            private readonly Sprite sprite;

            // Constructor
            public ModifierIcon(AtlasImage image)
            {
                sprite = new()
                {
                    PivotOrigin = RectanglePoint.Center,
                    RenderImage = image
                };
            }

            #region Protected members

            // OnDraw
            protected override void OnDraw(GameTime gameTime)
            {
                sprite.Draw(gameTime);
            }

            #endregion

            // BoundingBox
            public RectangleF BoundingBox => sprite.BoundingBox;

            // Position
            public Vector2 Position
            {
                get => sprite.Position;
                set => sprite.Position = value;
            }
        }
    }
}
