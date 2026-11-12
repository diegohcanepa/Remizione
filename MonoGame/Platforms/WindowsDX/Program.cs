global using Remizione;
using System.Threading;

using var mutex = new Mutex(true, "Remizione_SingleInstance", out bool createdNew);

if (!createdNew)
    return;

WindowsBridge platformBridge = new(new WindowsFileSystem());
using WindowsGame game = new(platformBridge);
game.Run();

