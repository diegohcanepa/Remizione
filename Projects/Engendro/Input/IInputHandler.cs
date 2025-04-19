using Microsoft.Xna.Framework;

namespace Engendro.Input
{
    /// <summary>
    /// IInputHandler
    /// </summary>
    public interface IInputHandler
    {
        // CanHandleInput
        bool CanHandleInput { get; }

        // HandleInput
        HandleInputResult HandleInput(GameTime gameTime);
    }
}
