using Engendro;

namespace Remizione
{
    /// <summary>
    /// Trunk
    /// </summary>
    public class Trunk : IsometricProp
    {
        // Constructor
        public Trunk(GameSession session, string name)
            : base(session, name)
        {
            HitEffect = HitEffect.Shake;
        }

        #region Protected members

        // CanInteractCore
        protected override bool CanInteractCore(Actor requester)
        {
            return PropState != PropState.Open && base.CanInteractCore(requester);
        }

        // OnPropStateChanged
        protected override void OnPropStateChanged()
        {
            AnimationPlayer.Play(PropState == PropState.Open ? AnimationNames.Open : AnimationNames.Closed, false);

            if (LoadState != LoadState.Loaded)
                return;

            if (PropState == PropState.Open)
            {
                PlaySound(SoundNames.TrunkOpen);

                if (Session.Room is ProceduralRoom room && GetLoot() is MetaItem metaItem)
                {
                    if (room.CreateRuntimeClone(nameof(Loot)) is Loot loot)
                    {
                        var bbox = BoundingBox;
                        var start = bbox.GetPoint(RectanglePoint.LeftTop, 10, 10);
                        var end = bbox.GetPoint(RectanglePoint.LeftTop, 17, 25);
                        loot.DropJumping(room, start, end, metaItem);
                    }
                }

            }
        }

        #endregion
    }
}
