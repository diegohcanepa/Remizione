using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Remizione.UI;

namespace Remizione
{
    /// <summary>
    /// HUD
    /// </summary>
    public sealed class HUD : GameObject, IInputHandler
    {
        #region Private fields

        private readonly UIHealthMeter healthMeter;
        private readonly UIPrompt prompt;
        private readonly ImageSprite savingIcon;
        private readonly GameSession session;
        private readonly UITokens tokens;

        #endregion

        #region Constructor

        // Constructor
        public HUD(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            this.healthMeter = new(session.Game);
            this.Log = new(Game);
            this.Message = new(Game);
            this.TargetMeter = new(Game);

            // Saving icon
            this.savingIcon = new ImageSprite(Game, Atlases.UI.SavingIcon)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.Area.GetPoint(RectanglePoint.RightTop, -6, 3),
                Scale = ScaleInfo.UIElement.Medium
            };

            // Prompt
            this.prompt = new(session);

            // Bag slot
            this.BagSlot = new(session);

            // Junk slot
            this.JunkSlot = new(session)
            {
                SceneScope = session
            };

            // Thingie slot
            this.ThingieSlot = new(session)
            {
                SceneScope = session
            };

            // Trincket slot
            this.TrincketSlot = new(Game);

            this.tokens = new(Game);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (session.Player != null)
                tokens.Draw(gameTime);

            if (session.IsHUDVisible)
            {
                if (session.GameplayMode == GameplayMode.Run)
                {
                    if (!session.IsConsoleVisible)
                    {
                        BagSlot.Draw(gameTime);
                        JunkSlot.Draw(gameTime);
                        ThingieSlot.Draw(gameTime);
                    }

                    if (session.Room is ProceduralRoom)
                    {
                        TrincketSlot.Draw(gameTime);
                        healthMeter.Draw(gameTime);
                        TargetMeter.Draw(gameTime);
                    }
                }

                Log.Draw(gameTime);
                Message.Draw(gameTime);
            }

            if (!session.IsAwaiting)
                prompt.Draw(gameTime);

            if (savingIcon.Tweens.IsTweening)
            {
                Game.SpriteBatch.Begin(Game.Camera);
                savingIcon.Draw(gameTime);
                Game.SpriteBatch.End();
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            TargetMeter.Update(gameTime);
            BagSlot.Update(gameTime);
            JunkSlot.Update(gameTime);
            ThingieSlot.Update(gameTime);
            TrincketSlot.Update(gameTime);
            healthMeter.Update(gameTime);
            prompt.Update(gameTime);

            Log.Update(gameTime);
            Message.Update(gameTime);

            savingIcon.Update(gameTime);

            if (session.Player != null)
            {
                tokens.Value = session.Player.Tokens;
                tokens.Update(gameTime);
            }
        }

        #endregion

        // BagSlot
        public SackSlot BagSlot { get; }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (session.IsAwaiting)
                return HandleInputResult.Unhandled;

            if (session.IsConsoleVisible || session.GameplayMode == GameplayMode.Adventure)
                return HandleInputResult.Unhandled;

            if (JunkSlot.HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            if (ThingieSlot.HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            if (BagSlot.HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            return HandleInputResult.Unhandled;
        }

        // JunkSlot
        public JunkSlot JunkSlot { get; }

        // Log
        public UILog Log { get; }

        // Message
        public HUDMessage Message { get; }

        // Reset
        public void Reset()
        {
            healthMeter.Actor = session.Player;
            BagSlot.Actor = session.Player;
            JunkSlot.Actor = session.Player;
            ThingieSlot.Actor = session.Player;
            TrincketSlot.Actor = session.Player;
        }

        // ShowSavingIcon
        public void ShowSavingIcon()
        {
            savingIcon.Tweens.OpacityTween = FloatTween.Create(TweenStyle.QuadraticInOut, 1, .8f, 300, 10);
        }

        // TargetMeter
        public UITargetMeter TargetMeter { get; }

        // ThingieSlot
        public ThingieSlot ThingieSlot { get; }

        // TrincketSlot
        public TrinketSlot TrincketSlot { get; }
    }
}
