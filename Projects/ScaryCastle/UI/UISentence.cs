using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// UISentence
    /// </summary>
    public sealed class UISentence : GameObject
    {
        private readonly TextSprite sentence;
        private readonly GameSession session;
        private GameThing? target;
        private readonly string useVerb;
        private readonly string withPreposition;

        // Constructor
        public UISentence(GameSession session)
            : base(session.Game)
        {
            this.session = session;
            this.useVerb = Localization.GetValue(Verb.Use);
            this.withPreposition = TextRepository.GetValue("Misc.WithPreposition");

            this.sentence = new(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.OrangeLight,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom, 0, -10),
                Scale = ScaleInfo.UISentence
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (session.IsCurrentScene && target != null)
            {
                Game.SpriteBatch.Begin(Game.Camera);
                //sentence.Draw(gameTime);
                Game.SpriteBatch.End();
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (session.IsCurrentScene && session.Player?.InteractiveTarget is GameThing currentTarget)
            {
                if (currentTarget != target)
                {
                    target = currentTarget;
                    var targetText = currentTarget.GetInteractPrompt() ?? currentTarget.LocalizedDisplayName;

                    if (session.Inventory.HeldItem == null)
                    {
                        MouseCursor.Highlight = false;
                        sentence.Text = targetText;
                    }
                    else
                    {
                        MouseCursor.Highlight = true;
                        sentence.Text = $"{useVerb} {session.Inventory.HeldItem.Definition.LocalizedDisplayName} {withPreposition} {targetText}";
                    }

                    MouseCursor.Text = sentence.Text;
                }
            }
            else
            {
                MouseCursor.Text = null;
                MouseCursor.Highlight = false;
                sentence.Text = null;
                target = null;
            }

            sentence.Update(gameTime);
        }

        #endregion
    }
}
