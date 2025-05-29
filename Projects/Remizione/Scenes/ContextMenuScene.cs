using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remizione.UI;

namespace Remizione
{
    /// <summary>
    /// ContextMenuScene 
    /// </summary>
    public sealed class ContextMenuScene : Scene
    {
        private RectangleF frame;
        private readonly ContextMenu<Verb> menu;
        private readonly GameSession session;

        #region Constructor

        // Constructor
        public ContextMenuScene(GameSession session)
            : base(session.Game, SceneSettings.None)
        {
            this.session = session;
            this.menu = new(session.Game, session.Camera);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (menu.Options.Count == 0)
                return;

            Game.SpriteBatch.Begin(session.Camera, SamplerState.LinearWrap);
            //Game.Shapes.DrawRectangle(frame, Color.Black * .5f);
            Game.SpriteBatch.End();
            menu.Draw(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                SceneController.Pop();
                session.Player?.EndTurn();
                return HandleInputResult.Handled;
            }

            if (Target != null && InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
            {
                if (menu.GetOptionAt(InputManager.DefaultPlayer.Mouse.WorldPosition(session.Camera)) is ContextMenuOption<Verb> option)
                {
                    Target.Verb = option.Key;
                    session.Player?.Interact(Target);
                }
                else
                    session.Player?.EndTurn();

                SceneController.Pop();
                return HandleInputResult.Handled;
            }

            return base.OnHandleInput(gameTime);
        }

        // OnLoadContent
        protected override void OnLoadContent()
        {
            menu.Clear();
            MouseCursor.Instance.State = MouseCursorState.Arrow;
            var verbs = Target?.GetVerbs();

            if (Target == null || session.Player == null || verbs == null)
                return;

            for (int i = 0; i < verbs.Length; i++)
            {
                menu.AddOption(verbs[i], Localization.GetValue(verbs[i]));
            }

            var pos = session.Player.GetOverheadPosition();
            menu.Show(pos, true);

            var bbox = menu.BoundingBox;
            frame = new(bbox.Left - 3, bbox.Top - 2, bbox.Width + 6, bbox.Height + 3);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            menu.Update(gameTime);
        }

        #endregion

        // Target
        public GameThing? Target { get; set; }
    }
}
