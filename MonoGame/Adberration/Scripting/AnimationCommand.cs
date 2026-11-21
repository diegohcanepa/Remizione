using Engendro;

namespace Adberration.Scripting
{
    // AnimationCommand
    // Arguments: {Name} [#zero-padding:Int32]
    internal sealed class AnimationCommand : NonAwaitableCommand
    {
        // Constructor
        internal AnimationCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, ZeroPaddingArg)
        {
            var entity = AssertEntityNotNull<Entity>(Script.EntityName);
            var animationName = RemoveQuotes(0);

            ActiveAnimation = entity.AddAnimation(animationName);
            ActiveAnimationFramePrefix = entity.Atlas?.HasFolderNamesPrepended == true ? null : entity.DeclaredName;

            ZeroPaddingLength = Parser.ParseInt32Argument(this, ZeroPaddingArg, 2);
        }

        // ActiveAnimation
        internal static SpriteAnimation? ActiveAnimation { get; private set; }

        // ActiveAnimationFramePrefix
        internal static string? ActiveAnimationFramePrefix { get; private set; } = string.Empty;

        // ZeroPaddingLength
        internal static int ZeroPaddingLength { get; private set; }
    }
}
