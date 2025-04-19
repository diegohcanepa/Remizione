global using Remizione;

#if WINDOWS

// Restrict the app to a single instance
WindowsGame.Mutex = new System.Threading.Mutex(true, "RemizioneWindows", out var createdNew);
if (!createdNew)
{
    return;
}

#endif

WindowsBridge platformBridge = new(new WindowsFileSystem());
using WindowsGame game = new(platformBridge);
game.Run();

