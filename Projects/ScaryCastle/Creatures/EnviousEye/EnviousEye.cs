using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// EnviousEye
    /// </summary>
    public sealed class EnviousEye : Actor
    {
        // Constructor
        public EnviousEye(GameSession session, string name)
            : base(session, name)
        {
            AnimationSettings.SupressAll();
            Brain = new ActorBrain(this);
            FastMoveFactor = 3;
            Guts = 7;
            ShadowSpotSize = 0;

            AddCard("CardTest3");
        }

        #region Protected members

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
        }

        #endregion

        /// <summary>
        /// ActorBrain
        /// </summary>
        public sealed class ActorBrain : Brain
        {
            // Constructor
            public ActorBrain(Actor owner)
                : base(owner)
            {
            }

            // PickCard
            public override Card? PickCard(Arena arena)
            {
                if (Owner.Cards.Count == 0)
                    return null;
                else
                    return Owner.Cards[0];
            }
        }
    }
}
