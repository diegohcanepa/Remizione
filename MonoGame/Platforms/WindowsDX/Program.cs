global using Remizione;

WindowsBridge platformBridge = new(new WindowsFileSystem());
using WindowsGame game = new(platformBridge);
game.Run();

