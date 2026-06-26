using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle.Scripting
{
    // AddLightCommand
    // Arguments: {Name} at {Vector2} [#color:Color] [#kind:LightKind] [#off] [#passes:Integer] [#pivot:RectanglePoint] [#scale:Vector2]
    internal sealed class AddLightCommand : NonAwaitableCommand
    {
        // Constructor
        internal AddLightCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 3, ColorArg, KindArg, PivotArg, PassesArg, ScaleArg, OffArg)
        {
            var room = AssertEntityNotNull<GameRoom>(Script.EntityName);

            var name = Parser.ParseName(this, 0);
            AssertKeyword(1, "at");

            var result = room.AddLight(name);

            // Color
            result.Color = Parser.ParseColorArgument(this, ColorArg, Color.White);

            // LightKind
            result.LightKind = Parser.ParseEnumArgument(this, KindArg, LightKind.Default);

            // Passes
            if (HasArg(PassesArg))
                result.Passes = Parser.ParseInt32Argument(this, PassesArg);

            // Pivot
            if (HasArg(PivotArg))
                result.PivotOrigin = Parser.ParseEnumArgument<RectanglePoint>(this, PivotArg);

            // Position
            result.Position = Parser.ParseVector2(this, 2);

            // Scale
            if (HasArg(ScaleArg))
                result.Scale = Parser.ParseVector2Argument(this, ScaleArg);

            // Off
            if (HasArg(OffArg))
                result.TurnOff(true);
        }
    }
}
