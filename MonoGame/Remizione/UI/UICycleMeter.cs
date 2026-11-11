using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// UICycleMeter
    /// </summary>
    public sealed class UICycleMeter : SessionGameObject<GameSession>
    {
        private readonly Sprite world;
        private readonly Sprite marker;

        #region Constructor

        // Constructor
        public UICycleMeter(GameSession session)
            : base(session)
        {
            // Planeta (mitad derecha clara, mitad izquierda oscura)
            this.world = new(Atlases.UI.WorldIcon)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightTop, -2, -1)
            };

            // Marcador orbitante (bolita)
            this.marker = new(Atlases.UI.CycleMeterMarker)
            {
                PivotOrigin = RectanglePoint.Center
            };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            world.Draw(gameTime);
            marker.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            UpdateOrbitPosition();

            world.Update(gameTime);
            marker.Update(gameTime);
        }

        #endregion

        #region Private methods

        // UpdateOrbitPosition
        private void UpdateOrbitPosition()
        {
            var totalDuration = Session.CurrentCycle == Cycle.Light ? GameData.WorldSettings.LightCycleDuration : GameData.WorldSettings.DarkCycleDuration;

            if (totalDuration <= 0)
                return;

            // Progreso del ciclo actual: 0.0f (inicio) -> 1.0f (final)
            float t = 1f - MathHelper.Clamp(Session.CycleRemainingTime / totalDuration, 0f, 1f);

            // Ángulo en sentido horario (0 = 12 en punto):
            // Día (Light): 12:00 -> 6:00 recorriendo el semicírculo derecho (0 a PI)
            // Noche (Dark): 6:00 -> 12:00 recorriendo el semicírculo izquierdo (PI a 2*PI)
            float angle = Session.CurrentCycle == Cycle.Light ? t * MathF.PI : MathF.PI + (t * MathF.PI);

            // Centro del planeta
            var worldCenter = world.Position + new Vector2(-world.BoundingBox.Width * .5f, world.BoundingBox.Height * .5f);

            // Radio de la circunferencia externa (+1px de margen sobre el borde)
            float radius = (world.BoundingBox.Width * .5f) + 1f;

            // Desplazamiento trigonométrico en sentido horario (sin para X, -cos para Y)
            float offsetX = (MathF.Sin(angle) * radius) - .5f;
            float offsetY = (-MathF.Cos(angle) * radius) - .5f;

            marker.Position = worldCenter + new Vector2(offsetX, offsetY);
        }

        #endregion
    }
}