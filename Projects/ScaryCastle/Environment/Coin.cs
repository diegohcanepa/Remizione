using Adberration;
using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// Coin
    /// </summary>
    public sealed class Coin : Prop
    {
        // Constructor
        public Coin(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Environment;

            ApproachBehavior = ApproachBehavior.ClosestSide;
            Collider = new Polygon("0,0;5,0;5,4;0,4");
            DepthOffset = -20;
            DisplayNameKey = "Prop.Coin";
            Hotspot = new Polygon("0,0;5,0;5,4;0,4");
            IgnoreWalkArea = false;
            RenderLayer = RenderLayer.Background;
            ShadowSpotSize = 0;
            
            var animation = AddAnimation("Default");
            animation.AddFrame("Coin01", 1500);
            animation.AddFrame("Coin02", 100);
            animation.AddFrame("Coin03", 100);
            animation.AddFrame("Coin04", 100);
        }

        // CanCheckCollisions
        protected override bool CanCheckCollisions()
        {
            return Tweens.IsTweeningPosition && base.CanCheckCollisions();
        }

        // OnCollisioning
        protected override void OnCollisioning(GameThing thing, out bool handled)
        {
            if (thing is not Coin)
                Tweens.Reset();

            handled = true;
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            
            if (Room is GameRoom gameRoom &&  gameRoom.WalkArea?.RandomWalkablePoint(Position, 3, 15) is Vector2 destination)
            {
                var distance = Vector2.Distance(Position, destination);
                var tweenDuration = (int)float.Clamp(distance * 100, 300, 1000);
                Tweens.PositionTween = Vector2Tween.Create(TweenStyle.CubicOut, Position, destination, tweenDuration);
            }
        }
    }
}
