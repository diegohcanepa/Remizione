using Adberration;
using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// VendingMachine
    /// </summary>
    public sealed class VendingMachine : Prop
    {
        private readonly ImageSprite glass;
        private readonly ImageSprite icon;

        // Constructor
        public VendingMachine(GameSession session, string name)
            : base(session, name)
        {
            this.HighlightInteraction = false;

            // Icon
            icon = new ImageSprite(session.Game)
            {
                PivotOrigin = RectanglePoint.Center,
                Scale = new(.4f)
            };

            // Glass
            glass = new ImageSprite(session.Game, Atlases.Environment.GetImage($"{nameof(VendingMachine)}Glass"))
            {
                Opacity = .25f
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
            icon.Draw(gameTime);
            glass.Draw(gameTime);
        }

        // OnParentChanged
        protected override void OnParentChanged(Entity? previousParent)
        {
            base.OnParentChanged(previousParent);

            if (Parent is ProceduralRoom procRoom && Config != null)
                MetaItem = Loot.GetForVending(Session, procRoom.Config, Config.PreferredLootRealm, Config.PreferredLootCategory);
        }

        // OnTransform
        protected override void OnTransform(TransformChange change)
        {
            base.OnTransform(change);

            if (change == TransformChange.Position)
            {
                icon.Position = BoundingBox.GetPoint(RectanglePoint.LeftTop, 10, 12.5f);
                glass.Position = BoundingBox.GetPoint(RectanglePoint.LeftTop, 6, 8);
            }
        }

        #endregion

        // MetaItem
        public MetaItem? MetaItem
        {
            get;
            set
            {
                field = value;
                icon.Image = field?.Image;
            }
        }

        // Use
        [ScriptMethod]
        public void Use()
        {
            AllowInteraction = false;
        }
    }
}
