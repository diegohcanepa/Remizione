using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione.Menus
{
    /// <summary>
    /// SplashScene
    /// </summary>
    public partial class SplashScene : Scene
    {
        #region Private fields

        private const int defaultDuration = 1500;
        private readonly ImageSprite gradientSpot;
        private readonly Timer gradientSpotStartTimer;

        #endregion

        #region Constructors

        // Constructor
        public SplashScene(RemizioneGame game)
            : this(game, null, Vector2.Zero, 0, 0)
        {
        }

        // Constructor
        public SplashScene(RemizioneGame game, AtlasImage? image, Vector2 imagePosition, float imageScale)
            : this(game, image, imagePosition, imageScale, defaultDuration)
        {
        }

        // Constructor
        public SplashScene(RemizioneGame game, AtlasImage? image, Vector2 imagePosition, float imageScale, int duration)
            : base(game, SceneSettings.ExclusiveDraw)
        {
            BackgroundColor = Color.Black;

            Image = new ImageSprite(game, image)
            {
                PivotOrigin = RectanglePoint.Middle,
                Position = imagePosition,
                Scale = new Vector2(imageScale)
            };

            gradientSpot = new ImageSprite(game, Atlases.Menu.FadeCircle)
            {
                PivotOrigin = RectanglePoint.Middle,
                Scale = new Vector2(1.8f)
            };

            Duration = duration;

            this.gradientSpotStartTimer = new Timer(ShowGradientSpot);
        }

        #endregion

        #region Private members

        // SetNextScene
        private void SetNextScene()
        {
            if (NextScene != null)
            {
                Game.SceneManager.Pop();
                Game.SceneManager.Push(NextScene);
            }
        }

        // ShowGradientSpot
        private void ShowGradientSpot()
        {
            TransitionManager.CurrentTransition.Out(FadeDuration);
            gradientSpot.Position = Screen.HUDArea.GetPoint(RectanglePoint.Left);
            gradientSpot.Tweens.XTween = FloatTween.Create(TweenStyle.CubicIn, gradientSpot.X, 240, 3000, GoToNextScene);
        }

        #endregion

        #region Protected members

        // FadeDuration 
        protected int FadeDuration { get; set; } = 1000;

        // GoToNextScene
        protected void GoToNextScene()
        {
            GoToNextScene(false);
        }

        // GoToNextScene
        protected void GoToNextScene(bool immediate)
        {
            TransitionManager.CurrentTransition.In(immediate ? 1 : FadeDuration, SetNextScene);
        }

        // Image
        protected ImageSprite Image { get; }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);
            Image.Draw(gameTime);

            if (!Image.IsEmpty)
            {
                gradientSpot.Draw(gameTime);
            }

            Game.SpriteBatch.End();
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            base.OnLoadContent();

            TransitionManager.CurrentTransition.In(0);

            if (Image.IsEmpty)
            {
                TransitionManager.CurrentTransition.Out(FadeDuration);
            }
            else
            {
                Image.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicIn, Image.Scale, Image.Scale * 1.3f, 6000);
                gradientSpotStartTimer.Start(1000);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            gradientSpot.Update(gameTime);
            gradientSpotStartTimer.Update(gameTime);
            Image.Update(gameTime);
        }

        #endregion

        // Duration
        public int Duration { get; }

        // NextScene
        public Scene? NextScene { get; set; }
    }
}
