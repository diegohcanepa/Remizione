using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ActorUseItemState
    /// </summary>
    public sealed class ActorUseItemState : ActorActionState
    {
        // Constructor
        public ActorUseItemState(string animationName)
            : base(animationName)
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
