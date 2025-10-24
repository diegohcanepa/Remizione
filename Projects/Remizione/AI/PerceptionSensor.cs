using Adberration;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// PerceptionSensor
    /// </summary>
    public sealed class PerceptionSensor
    {
        private int elapsedTime;
        private readonly List<GameThing> visibleEnemies = [];

        // Constructor
        public PerceptionSensor(Actor owner)
        {
            this.Owner = owner;
            this.VisibleEnemies = visibleEnemies.AsReadOnly();
        }

        // CanSeeTarget
        public bool CanSeeTarget(GameThing target)
        {
            Vector2 toTarget = target.Position - Owner.Position;

            if (ViewDistance > 0 && toTarget.Length() > ViewDistance)
                return false;

            Vector2 directionToTarget = Vector2.Normalize(toTarget);
            Vector2 forward = Owner.Direction == FacingDirection.Right ? Vector2.UnitX : -Vector2.UnitX;

            float dot = Vector2.Dot(forward, directionToTarget);
            float angleThreshold = MathF.Cos(MathHelper.ToRadians(ViewAngle / 2f));

            return dot >= angleThreshold;
        }

        // CurrentTarget
        public GameThing? CurrentTarget { get; private set; } = null;

        // DefaultRefreshRate
        public const int DefaultRefreshRate = 500;

        // HasLOS
        public bool HasLOS(GameThing target)
        {
            return visibleEnemies.Contains(target);
        }

        // Owner
        public Actor Owner { get; }

        // RefreshRate (ms)
        public int RefreshRate { get; set; } = DefaultRefreshRate;

        // Update
        public void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
            if (elapsedTime < RefreshRate)
                return;

            if (Owner.Room is not GameRoom room)
                return;

            elapsedTime = 0;

            visibleEnemies.Clear();

            CurrentTarget = null;

            float bestDist = float.MaxValue;

            for (var i = 0; i < room.Children.Count; i++)
            {
                // Skip owner
                if (room.Children[i] == Owner)
                    continue;

                if (room.Children[i] is not GameThing target)
                    continue;

                if (target.MaxHP == 0)
                    continue;

                if (target.IsDead)
                    continue;

                // Not an enemy
                if (!Owner.IsEnemy(target))
                    continue;

                // Can see target?
                if (!CanSeeTarget(target))
                    continue;

                visibleEnemies.Add(target);

                var dist = Owner.DistanceTo(target);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    this.CurrentTarget = target;
                }
            }
        }

        // ViewAngle
        public float ViewAngle { get; set; } = 90;

        // ViewDistance
        public float ViewDistance { get; set; }

        // VisibleEnemies
        public ReadOnlyCollection<GameThing> VisibleEnemies { get; }
    }
}
