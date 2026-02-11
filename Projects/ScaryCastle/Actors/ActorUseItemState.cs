using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ActorUseItemState
    /// </summary>
    public sealed class ActorUseItemState : ActorActionState
    {
        // Constructor
        public ActorUseItemState(Actor owner, string animationName)
            : base(owner, animationName)
        {
        }

        #region Protected members

        // OnExecuteAction
        protected override void OnExecuteAction()
        {
        }

        #endregion

        // Item
        public Item? Item { get; set; }
    }
}
