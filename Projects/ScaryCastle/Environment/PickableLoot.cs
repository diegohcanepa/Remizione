using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// PickableLoot
    /// </summary>
    public abstract class PickableLoot : Prop, ILootContainer<ItemDefinition>
    {
        // Constructor
        protected PickableLoot(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Props;
            ApproachBehavior = ApproachBehavior.ClosestSide;
            DepthOffset = 20;
            IgnoreWalkArea = false;
            Loot = ItemDefinition.Definitions.Find(GetType().Name);
            Verb = Verb.PickUp;
        }

        #region Protected members

        // CanCheckCollisions
        protected override bool CanCheckCollisions()
        {
            return Tweens.IsTweeningPosition && base.CanCheckCollisions();
        }

        // OnCollisioning
        protected override void OnCollisioning(GameThing thing, out bool handled)
        {
            if (thing is not PickableLoot)
                Tweens.Reset();

            handled = true;
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();

            if (Room is ProceduralRoom procRoom && procRoom.IsCurrentRoom && procRoom.WalkArea?.RandomWalkablePoint(procRoom.Random, Position, 3, 15) is Vector2 destination)
            {
                var distance = Vector2.Distance(Position, destination);
                var tweenDuration = (int)float.Clamp(distance * 100, 300, 1000);
                Tweens.PositionTween = Vector2Tween.Create(TweenStyle.CubicOut, Position, destination, tweenDuration);
            }
        }

        // OnLootChanged
        protected virtual void OnLootChanged()
        {
        }

        #endregion

        // Loot
        public ItemDefinition? Loot
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    OnLootChanged();
                }
            }
        }
    }
}
