using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione.Menus
{
    /// <summary>
    /// MenuScene
    /// </summary>
    public partial class MenuScene : Scene
    {
        private readonly ImageSprite backgroundSprite;
        private readonly TextSprite version;

        #region Constructor

        // Constructor
        protected MenuScene(RemizioneGame game, AtlasImage? backgroundImage, SceneSettings settings = SceneSettings.ExclusiveDraw | SceneSettings.PausePreviousScenes)
            : base(game, settings)
        {
            this.Game = game;
            this.backgroundSprite = new ImageSprite(game, backgroundImage);
            version = Utils.CreateVersionLabel(game);
            Stick = new StickInputController(GamePadThumbStick.Left) { AutoRepeatRate = 250 };
        }

        #endregion

        #region Private members

        // PushSceneAction
        private void PushSceneAction(Scene scene)
        {
            Game.SceneManager.Push(scene);
            TransitionManager.CurrentTransition.Out(TransitionDuration);
        }

        #endregion

        #region Protected members

        // DrawVersionInformation
        protected void DrawVersionInformation(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            version.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            backgroundSprite.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            Stick.Stick = GamePadThumbStick.Left;
            Stick.Update(gameTime);
        }

        // Pop
        protected void Pop()
        {
            TransitionManager.CurrentTransition.In(TransitionDuration, () => { SceneController.Pop(); TransitionManager.CurrentTransition.Out(500); });
        }

        // Stick
        protected StickInputController Stick { get; }

        #endregion

        /*
        // CanHandleInput
        public override bool CanHandleInput
        {
            get
            {
                if (TransitionManager.CurrentTransition.IsRunning)
                {
                    if (TransitionManager.CurrentTransition.TransitionState == TransitionState.In)
                        return false;
                    else if (TransitionManager.CurrentTransition.TransitionState == TransitionState.Out && TransitionManager.CurrentTransition.VisibleRatio > .4f)
                        return false;
                }

                return base.CanHandleInput;
            }
        }
        */

        // Game
        public new RemizioneGame Game { get; }

        // GoToScene
        public void GoToScene(Scene scene)
        {
            TransitionManager.CurrentTransition.In(TransitionDuration, () => PushSceneAction(scene));
        }

        // GoToScene
        public void GoToScene(Scene scene, int transitionDuration)
        {
            TransitionManager.CurrentTransition.In(transitionDuration, () => PushSceneAction(scene));
        }

        // TransitionDuration
        public const int TransitionDuration = 250;
    }
}