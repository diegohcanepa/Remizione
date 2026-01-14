using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// CloseUpRoom
    /// </summary>
    public class CloseUpRoom : GameRoom
    {
        // Constructor
        public CloseUpRoom(GameSession session, string name)
            : base(session, name)
        {
            AllowPauseMenu = false;
            AllowSaving = false;

            ControlGroup = new UIControlGroup(session.Game);
            ControlGroup.Add(InputBindings.Back);
        }

        // ControlGroup
        public UIControlGroup ControlGroup { get; }

        #region Protected members

        // OnClose
        protected virtual void OnClose()
        {
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            if (AllowInput)
                ControlGroup.Draw(gameTime);
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (IsClosed || !AllowInput)
                return HandleInputResult.Unhandled;

            if (ControlGroup.FirstControl is UIButton control && control.TestPressed(PlayerIndex.One))
            {
                Close();
                return HandleInputResult.Handled;
            }

            return base.OnHandleInput(gameTime);
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            Session.IsMouseVisible = true;
            IsClosed = false;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (AllowInput)
                ControlGroup.Update(gameTime);
        }

        #endregion

        // AllowInput
        [ScriptProperty(CodingContext.Any)]
        public bool AllowInput { get; set; } = true;

        // Close
        public void Close()
        {
            OnClose();
            IsClosed = true;
        }

        // IsClosed
        public bool IsClosed { get; private set; }
    }
}
