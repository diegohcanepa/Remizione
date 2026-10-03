using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// UIThrowTrajectory
    /// </summary>
    public static class UIThrowTrajectory
    {
        #region Private fields

        private const float StepIntervalPx = 4;

        private static readonly Sprite dotSprite = new(Atlases.UI.TrajectoryDot)
        {
            Color = ColorPalette.Text.Terra,
            Opacity = .6f,
            PivotOrigin = RectanglePoint.Center,
            Scale = ScaleInfo.UIElement.Medium
        };

        private static readonly Sprite targetMarkerSprite = new(Atlases.UI.TrajectoryTargetMark)
        {
            Color = ColorPalette.Text.Terra,
            Opacity = .6f,
            PivotOrigin = RectanglePoint.Center,
            Scale = ScaleInfo.UIElement.Medium
        };

        #endregion

        // Draw
        public static void Draw(GameTime gameTime, Actor thrower, Prop prop, Vector2 targetWorldPos)
        {
            if (prop.ThrownDistance == ThrowDistance.None)
                return;

            Vector2 start = thrower.Position;

            // 1. Calculamos la posición límite real usando la compresión plana de perspectiva
            Vector2 end = ThrownProp.ClampToEffectiveRange(start, targetWorldPos, prop.ThrownDistanceInPixels);

            Vector2 direction = end - start;
            float totalDistance = direction.Length();

            if (totalDistance <= 1f)
                return;

            direction.Normalize();

            // 2. Dibujamos los puntos intermedios a intervalos regulares
            for (var current = StepIntervalPx; current < totalDistance; current += StepIntervalPx)
            {
                Vector2 dotPosition = start + (direction * current);
                dotSprite.Position = dotPosition;
                dotSprite.Draw(gameTime);
            }

            // 3. Dibujamos el marcador de impacto final (más grande / distinto) en el punto límite 'end'
            targetMarkerSprite.Position = end;
            targetMarkerSprite.Draw(gameTime);
        }
    }
}