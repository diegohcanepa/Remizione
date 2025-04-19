using Engendro;
using EngendroAdventure.Scripting;

namespace Remizione.Scripting
{
    // SetThingLightCommand
    // Arguments: {Thing} at {Vector2} [#color:Color] [#kind:LightKind] [#pivot:RectanglePoint] [#scale:Vector2]
    internal sealed class SetThingLightCommand : NonAwaitableCommand
    {
        // Constructor
        internal SetThingLightCommand(Script script, string source, StatementBody body)
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
            var pivot = Parser.ParseEnumArgument<RectanglePoint>(this, PivotArg, RectanglePoint.Middle);
            var scale = Parser.ParseVector2Argument(this, ScaleArg);

            thing.Light = new Light(Game, "")
            {
                Color = color,
                LightKind = kind,
                PivotOrigin = pivot,
                Scale = scale
            };

            thing.LightPosition = position;
        }
    }
}
