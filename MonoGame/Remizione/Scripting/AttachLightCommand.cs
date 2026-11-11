using Adberration.Scripting;
using Engendro;

namespace Remizione.Scripting
{
    // AttachLightCommand
    // Arguments: {Thing} at {Vector2} [#color:Color] [#kind:LightKind] [#pivot:RectanglePoint] [#scale:Vector2]
    internal sealed class AttachLightCommand : NonAwaitableCommand
    {
        // Constructor
        internal AttachLightCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 3, ColorArg, KindArg, PivotArg, ScaleArg)
        {
            AssertEntityNotNull<GameThing>(0);
            AssertKeyword(1, "at");
            Parser.ParseVector2(this, 2);
            Parser.ParseEnumArgument(this, KindArg, LightKind.Default);
            Parser.ParseEnumArgument<RectanglePoint>(this, PivotArg);
            Parser.ParseVector2Argument(this, ScaleArg);
        }

        // OnExecute
        protected override void OnExecute()
        {
            var thing = AssertEntityNotNull<GameThing>(0);
            var position = Parser.ParseVector2(this, 2);
            var color = Parser.ParseColorArgument(this, ColorArg);
            var kind = Parser.ParseEnumArgument(this, KindArg, LightKind.Default);
            var pivot = Parser.ParseEnumArgument(this, PivotArg, RectanglePoint.Center);
            var scale = Parser.ParseVector2Argument(this, ScaleArg);

            thing.AttachedLight = new Light(string.Empty, kind)
            {
                Color = color,
                PivotOrigin = pivot,
                Scale = scale
            };

            thing.AttachedLightPosition = position;
        }
    }
}
