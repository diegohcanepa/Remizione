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
        private readonly List<Sprite> activeIcons = [];
        private readonly Dictionary<RunModifierKind, Sprite> icons = [];
        private int lastKnownVersion = -1;

        #region Constructor

        // Constructor
        public UIRunModifiers(GameSession session)
            : base(session)
        {
            foreach (var modifierKind in Enum.GetValues<RunModifierKind>())
            {
                icons[modifierKind] = new()
                {
                    PivotOrigin = RectanglePoint.Top,
                    RenderImage = Atlases.UI.GetImage($"{nameof(RunModifier)}{modifierKind}Icon"),
                    Scale = ScaleInfo.UIElement.Medium
                };
            }
        }

        #endregion

        #region Private members

        // Refresh
        private void Refresh()
        {
            activeIcons.Clear();

            if (Session.CurrentRun?.Modifiers is not RunModifierManager runModifierManager)
                return;

            if (runModifierManager.Count == 0)
                return;

            foreach (var modifier in runModifierManager.Modifiers)
            {
                activeIcons.Add(icons[modifier.Kind]);
            }

            float spacing = 1;
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
                var sprite = activeIcons[i];
                float halfWidth = sprite.BoundingBox.Width / 2f;

                // Como el PivotOrigin es 'Top' (centro superior del sprite), 
                // sumamos la mitad de su propio ancho para centrar el pivote en la posición X actual
                sprite.Position = new Vector2(currentX + halfWidth, topMargin);

                // Desplazamos la X para el siguiente ícono en la fila
                currentX += sprite.BoundingBox.Width + spacing;
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            for (var i = 0; i < activeIcons.Count; i++)
            {
                if (activeIcons[i].RenderImage == null)
                    break;
                else
                    activeIcons[i].Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Session.CurrentRun?.Modifiers == null)
                return;

            if (lastKnownVersion != Session.CurrentRun.Modifiers.ContentVersion)
            {
                Refresh();
                lastKnownVersion = Session.CurrentRun.Modifiers.ContentVersion;
            }
        }

        #endregion
    }
}
