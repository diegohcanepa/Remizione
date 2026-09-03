using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione.UI
{
    /// <summary>
    /// MonitorFrame
    /// </summary>
    internal static class MonitorFrame
    {
        private static Sprite? image;

        // Draw
        public static void Draw(GameTime gameTime)
        {
            if (RemizioneGame.Effects.CRT.MonitorStyle)
            {
                image ??= new(Atlases.UI.GetImage("MonitorFrame"));
                image.Game.SpriteBatch.Begin(image.Game.Camera);
                image.Draw(gameTime);
                image.Game.SpriteBatch.End();
            }
        }
    }
}
