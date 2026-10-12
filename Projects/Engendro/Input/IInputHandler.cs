using Microsoft.Xna.Framework;

namespace Engendro.Input
{
    /// <summary>
    /// IInputHandler
    /// </summary>
    public interface IInputHandler
    {
        HandleInputResult HandleInput(GameTime gameTime);
    }
}
