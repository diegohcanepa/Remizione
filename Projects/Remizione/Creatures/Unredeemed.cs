using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione.Creatures
{
    /// <summary>
    /// Unredeemed
    /// </summary>
    public sealed class Unredeemed : Actor
    {
        private int moveCooldown;

        // Constructor
        public Unredeemed(GameSession session, string name)
            : base(session, name)
        {
            this.BodySize = ActorSize.Small;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            moveCooldown = Randomizer.Next(10000, 30000);
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (!IsCombating && !IsFollowingPath && !IsAlert)
            {
                if (moveCooldown > 0)
                {
                    moveCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                }
                else
                {
                    moveCooldown = Randomizer.Next(10000, 30000);
                    var destination = Position.Random(30, 80);
                    MoveTo(destination);
                }
            }
        }
    }
}
