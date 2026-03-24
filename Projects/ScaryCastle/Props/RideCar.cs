using Adberration.Scripting;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// RideCar
    /// </summary>
    public sealed class RideCar : Prop
    {
        // Constructor
        public RideCar(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Props;
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
        }

        // GetOutRider
        [ScriptMethod]
        public void GetOutRider()
        {
            if (Session.Player != null)
                AnimationPlayer.Play("GetOut" + Session.Player.Name);
        }

        // SyncRider
        [ScriptMethod]
        public void SyncRider()
        {
            if (Session.Player != null)
                AnimationPlayer.Play(Session.Player.Name);
        }
    }
}
