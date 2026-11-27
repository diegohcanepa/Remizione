using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// FallingProp
    /// </summary>
    public sealed class FallingProp : GameThing
    {
        #region Constants

        private const int FallDuration = 250;

        #endregion

        #region Private fields

        private float depth;
        private readonly Actor owner;
        private readonly FloatTween yTween = new();

        #endregion

        #region Constructor

        public FallingProp(Actor owner, Prop prop)
            : base(prop.Session, string.Empty)
        {
            this.owner = owner;
            this.Prop = prop;
            this.Atlas = Atlases.Environment;
            this.PivotOrigin = RectanglePoint.Center;
            this.IgnoreWalkArea = true;

            var animation = AddAnimation(AnimationNames.Default);
            animation.AddFrame(prop.GetHeldPropImageName(), 1000);
        }

        #endregion

        #region Private members

        // Break
        private void Break()
        {
            Tweens.Reset();

            if (Prop.DeathSound != null)
                PlaySound(Prop.DeathSound);

            RenderLayer = RenderLayer.Background;
            DepthOffset = 0;
            Prop.Position = this.Position;

            if (Room != null)
                Prop.SpawnRemains(Room);

            Unparent();

            Session.Camera.Shake(TweenStyle.Linear, Vector2.One, 40, 6);
        }

        // CheckCollision
        private void CheckCollision()
        {
            if (Prop.Definition == null || Room == null)
                return;

            for (int i = 0; i < Room.Children.Count; i++)
            {
                var child = Room.Children[i] as GameThing;

                if (child == null || child == owner || child == this)
                    continue;

                if (child.CanBeHit() && !child.IsDead)
                {
                    if (child.RuntimeHotspot.BoundingRectangleF.Intersects(BoundingBox))
                    {
                        EffectDescriptor.Apply(Prop.Definition.EffectDescriptors, owner, child, EffectContext.Contact);
                        Break();
                        break;
                    }
                }
            }
        }

        #endregion

        #region Protected members

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            CheckCollision();
        }

        #endregion

        // Depth
        public override float Depth
        {
            get { return depth; }
        }

        // Fall
        public void Fall()
        {
            if (owner.Room == null)
                return;

            var startPos = owner.GetHeldPropPosition() ?? owner.Position;

            RenderLayer = RenderLayer.Default;
            depth = owner.Depth + .01f;
            Position = startPos;

            yTween.Start(TweenStyle.QuadraticIn, Y, owner.Y + 1, FallDuration, Break);
            Tweens.YTween = yTween;

            owner.Room.Children.Add(this);
        }

        // Prop
        public Prop Prop { get; }
    }
}