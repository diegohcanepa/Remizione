using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Snail
    /// </summary>
    public sealed class Snail : Actor
    {
        // Constructor
        public Snail(GameSession session, string name)
            : base(session, name)
        {
            Scale = new(.75f);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (!IsMoving)
            {
                if (IsFlippedHorizontally)
                    MoveTo(new(X + 30, Y));
                else
                    MoveTo(new(X - 30, Y));
            }
        }
    }
}
