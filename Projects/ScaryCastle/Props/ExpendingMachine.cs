using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// ExpendingMachine
    /// </summary>
    public sealed class ExpendingMachine : Prop
    {
        // Constructor
        public ExpendingMachine(GameSession session, string name)
            : base(session, name)
        {
        }

        // Use
        [ScriptMethod]
        public void Use()
        {
            AllowInteraction = false;
            if (Room is GameRoom room)
            {
                room.Session.ObjectPools.Pickups.Get()?.Drop(room, BoundingBox.GetPoint(RectanglePoint.Bottom), MetaItem.FindNotNull("Apple"), BoundingBox.Bottom + Random.Shared.Next(10), 0);
                room.Session.ObjectPools.Pickups.Get()?.Drop(room, BoundingBox.GetPoint(RectanglePoint.Bottom), MetaItem.FindNotNull("RottenApple"), BoundingBox.Bottom + Random.Shared.Next(10), 400);
                room.Session.ObjectPools.Pickups.Get()?.Drop(room, BoundingBox.GetPoint(RectanglePoint.Bottom), MetaItem.FindNotNull("Firecracker"), BoundingBox.Bottom + Random.Shared.Next(20), 800);
            }
        }
    }
}
