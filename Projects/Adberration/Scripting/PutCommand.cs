using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adberration.Scripting
{
    // PutCommand
    // Syntax: {Thing} into {Entity} {#at:Vector2} [#animation:AnimationName] [#chance:{1..10}] [#depth-offset:Integer] [#flip] [#focus] [#follow] [#index:Integer] [#opacity:Float] [#random-frame] [#scale:Vector2]
    internal sealed class PutCommand : NonAwaitableCommand
    {
        // Constructor
        internal PutCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 3, AnimationArg, AtArg, ChanceArg, DepthOffsetArg, FlipArg, FocusArg, FollowArg, IndexArg, OpacityArg, RandomFrameArg, ScaleArg)
        {
            var thing = AssertEntity<Thing>(0);
            AssertKeyword(1, "into");
            var parent = AssertEntity<Entity>(2);
            Parser.ParseVector2Argument(this, AtArg);
            Parser.ParseInt32Argument(this, ChanceArg);
            Parser.ParseInt32Argument(this, DepthOffsetArg);
            Parser.ParseInt32Argument(this, IndexArg);
            Parser.ParseFloatArgument(this, OpacityArg);
            Parser.ParseVector2Argument(this, ScaleArg);

            if (thing != null && parent != null && !parent.CanParent(thing))
            {
                throw new ScriptException(this, $"[{parent.Name}] cannot parent [{thing.Name}]");
            }
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (HasArg(ChanceArg))
            {
                var chance = Parser.ParseInt32Argument(this, ChanceArg);
                if (DiceExpression.Dice10.Roll() > chance)
                    return;
            }

            var thing = AssertEntity<Thing>(0);
            var parent = AssertEntity<Entity>(2);

            if (thing == null || parent == null)
                return;

            thing.StopMoving();

            // Animation
            if (Body.Args.FindArg(AnimationArg)?.Value is string animationName)
                thing.AnimationPlayer.Play(animationName, true);

            // At
            if (HasArg(AtArg))
                thing.Position = Parser.ParseVector2Argument(this, AtArg, thing.Position);

            // Add thing to room
            if (HasArg(IndexArg))
            {
                var index = Parser.ParseInt32Argument(this, IndexArg, parent.Children.Count);
                index = MathHelper.Clamp(index, 0, parent.Children.Count);
                parent.Children.Insert(index, thing);
            }
            else
            {
                parent.Children.Add(thing);
            }

            thing.Effects = SpriteEffects.None;

            // Face
            if (HasArg(FlipArg))
                thing.Effects = SpriteEffects.FlipHorizontally;

            // DepthOffset
            if (HasArg(DepthOffsetArg))
                thing.DepthOffset = Parser.ParseInt32Argument(this, DepthOffsetArg, thing.DepthOffset);

            // Focus
            if (HasArg(FocusArg))
                Session.Camera.Position = thing.Position;

            // Follow
            if (HasArg(FollowArg))
                Session.Camera.FollowTarget(thing);

            // Opacity
            if (HasArg(OpacityArg))
                thing.Opacity = Parser.ParseFloatArgument(this, OpacityArg, thing.Opacity);

            // RandomFrame
            if (HasArg(RandomFrameArg))
                thing.AnimationPlayer.GoTo(FramePosition.Random);

            // Scale
            if (HasArg(ScaleArg))
                thing.Scale = Parser.ParseVector2Argument(this, ScaleArg, thing.Scale);
        }
    }
}