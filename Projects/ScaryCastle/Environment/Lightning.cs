using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System.Linq;

namespace ScaryCastle
{
    /// <summary>
    /// Lightning
    /// </summary>
    public sealed class Lightning : GameThing
    {
        private int cooldown;
        private Item? item = null!;
        private readonly ImageSprite impactArea;
        private Vector2 position;
        private readonly GameSession session;
        private GameThing? source = null!;

        // Constructor
        public Lightning(GameSession session)
            : base(session, string.Empty)
        {
            this.Atlas = Atlases.Environment;
            this.session = session;
            this.Scale = ScaleInfo.UIElement.Small;

            var animation = AddAnimation("Default");
            animation.AddFrameSequence("Lightning", 80, 1, 3);

            this.impactArea = new(Game, Atlases.Environment.GetImage("ImpactArea"))
            {
                Color = ColorPalette.Text.Red * .6f,
                PivotOrigin = RectanglePoint.Center
            };
            impactArea.Tweens.OpacityTween = FloatTween.Create(TweenStyle.CubicInOut, 1, .7f, 100, -1);
        }

        #region Private members

        // ShowCore
        private void ShowCore()
        {
            this.Position = position;
            this.AnimationPlayer.Play("Default", false);
            InputManager.DefaultPlayer.GamePad.Vibrate(200, .4f, .4f);
            session.Camera.Shake(TweenStyle.Linear, new Vector2(1.5f), 66, 4);
            Sound.Play(SoundNames.Lightning);

            if (Room != null)
            {
                foreach (var enemy in Room.Children.OfType<ProceduralActor>())
                {
                    var dist = DistanceTo(enemy);
                    if (dist <= 10)
                        EffectDescriptor.Apply(item.Definition.EffectDescriptors, source, enemy);
                }
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (cooldown > 0)
            {
                impactArea.Draw(gameTime);
                return;
            }

            base.OnDraw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (cooldown > 0)
            {
                impactArea.Update(gameTime);

                cooldown -= gameTime.ElapsedGameTime.Milliseconds;

                if (cooldown <= 0)
                    ShowCore();

                return;
            }

            base.OnUpdate(gameTime);

            if (!AnimationPlayer.IsPlaying)
                Unparent();
        }

        #endregion

        // Show
        public void Show(Vector2 position, int delay, GameThing source, Item item)
        {
            this.position = position;
            this.cooldown = delay;
            this.source = source;
            this.item = item;
            this.impactArea.Position = position;
        }
    }
}
