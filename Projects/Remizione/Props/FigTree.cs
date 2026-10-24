using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// FigTree
    /// </summary>
    public class FigTree : Prop
    {
        private readonly Sprite florishedImage = new() { PivotOrigin = RectanglePoint.Bottom };

        #region Constructor

        // Constructor
        public FigTree(GameSession session, string name)

            : base(session, name)
        {
            Atlas = Atlases.Props;
            DepthOffset = -2;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
        }

        // OnTransform
        protected override void OnTransform(TransformChange change)
        {
            base.OnTransform(change);

            if (change == TransformChange.Position)
                florishedImage.Position = Position;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
        }

        #endregion
    }
}
