using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// Drool
    /// </summary>
    public sealed class Drool : Prop
    {
        // Constructor
        public Drool(GameSession session, string name)
            : base(session, name)
        {
            ApproachBehavior = ApproachBehavior.Over;
            AutoPlayAnimation = false;
            Atlas = Atlases.Props;
            CollisionDetection = false;
            DisplayNameKey = "Prop.Drool";
            PivotOrigin = RectanglePoint.Center;
            Opacity = .7f;
            RenderLayer = RenderLayer.Background;
            TerrainParticleColor = new(75, 133, 150);
            TerrainSound = Sound.Find(SoundNames.FootstepWater);
        }

        #region Protected members

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            AnimationPlayer.GoTo(FramePosition.Random);
        }

        #endregion

        // Consume
        [ScriptMethod]
        public void Consume()
        {
            var tween = new Vector2Tween()
            {
                StartDelay = 300
            };

            tween.Start(TweenStyle.CubicIn, Scale, Vector2.Zero, 1500, Unparent);

            Tweens.ScaleTween = tween;
        }
    }
}
