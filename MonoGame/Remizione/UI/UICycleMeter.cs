using Engendro;
using Microsoft.Xna.Framework;
using Remizione.UI;
using System;

namespace Remizione
{
    /// <summary>
    /// UICycleMeter
    /// </summary>
    public sealed class UICycleMeter : HUDComponent
    {
        private readonly Sprite icon;
        private readonly Sprite marker;

        #region Constructor

        // Constructor
        public UICycleMeter(GameSession session)
            : base(session)
        {
            // Planeta (mitad derecha clara, mitad izquierda oscura)
            this.icon = new(Atlases.UI.WorldIcon)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightTop, -2, -1)
            };

            // Marcador orbitante (bolita)
            this.marker = new(Atlases.UI.CycleMeterMarker)
            {
                PivotOrigin = RectanglePoint.Center
            };

            BoundingBox = icon.BoundingBox;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            icon.Draw(gameTime);
            marker.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            UpdateOrbitPosition();

            icon.Update(gameTime);
            marker.Update(gameTime);
        }

        #endregion

        #region Private methods

        // UpdateOrbitPosition
        private void UpdateOrbitPosition()
        {
            var totalDuration = CycleManager.GetCycleDuration(Session.CycleManager.CurrentCycle);

            if (totalDuration <= 0)
                return;

            // Progreso del ciclo actual: 0.0f (inicio) -> 1.0f (final)
            float t = 1f - MathHelper.Clamp(Session.CycleManager.CycleRemainingTime / totalDuration, 0f, 1f);

            // Ángulo en sentido horario (0 = 12 en punto):
            // Día (Light): 12:00 -> 6:00 recorriendo el semicírculo derecho (0 a PI)
            // Noche (Dark): 6:00 -> 12:00 recorriendo el semicírculo izquierdo (PI a 2*PI)
            float angle = Session.CycleManager.CurrentCycle == Cycle.Light ? t * MathF.PI : MathF.PI + (t * MathF.PI);

            // Centro del planeta
            var worldCenter = icon.Position + new Vector2(-icon.BoundingBox.Width * .5f, icon.BoundingBox.Height * .5f);

            // Radio de la circunferencia externa (+1px de margen sobre el borde)
            float radius = (icon.BoundingBox.Width * .5f) + 1f;

            // Desplazamiento trigonométrico en sentido horario (sin para X, -cos para Y)
            float offsetX = (MathF.Sin(angle) * radius) - .5f;
            float offsetY = (-MathF.Cos(angle) * radius) - .5f;

            marker.Position = worldCenter + new Vector2(offsetX, offsetY);
        }

        #endregion
    }
}