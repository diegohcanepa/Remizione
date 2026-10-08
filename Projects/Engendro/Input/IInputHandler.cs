using Microsoft.Xna.Framework;

namespace Engendro.Input
{
    /// <summary>
    /// IInputHandler
    /// </summary>
    public interface IInputHandler
    {
        // HandleInput
        HandleInputResult HandleInput(GameTime gameTime);
    }
}
