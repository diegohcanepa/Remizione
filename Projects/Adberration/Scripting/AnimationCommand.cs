using Engendro;

namespace Adberration.Scripting
{
    // AnimationCommand
    // Arguments: {Name} [#headless] [#prefix:String] [#zero-padding:Int32]
    [ScriptStatement(CodingContext.EntityDeclaration)]
    internal sealed class AnimationCommand : NonAwaitableCommand
    {
        // Constructor
        internal AnimationCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, HeadlessArg, PrefixArg, ZeroPaddingArg)
        {
            var entity = AssertEntityNotNull<Entity>(Script.EntityName);
            var animationName = RemoveQuotes(0);

            ActiveAnimation = entity.AddAnimation(animationName);
            ActiveAnimation.Headless = HasArg(HeadlessArg);
            ActiveAnimationFramePrefix = Parser.ParseNameArgument(this, PrefixArg);

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
