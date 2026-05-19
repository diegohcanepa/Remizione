using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// Monitor
    /// </summary>
    public sealed class Monitor : GameRoom
    {
        private bool pushText;
        private readonly TextSprite[] lines;
        private readonly TextSprite subtitle;
        private readonly TextSprite title;
        private int lineIndex;

        // Constructor
        public Monitor(GameSession session, string name)
            : base(session, name)
        {
            title = CreateText(TextRepository.GetValue("Monitor.Title"));
            title.PivotOrigin = RectanglePoint.Top;
            title.Position = Screen.Area.GetPoint(RectanglePoint.Top, 0, 25);

            subtitle = CreateText(TextRepository.GetValue("Monitor.Subtitle"));
            subtitle.PivotOrigin = RectanglePoint.Top;
            subtitle.Position = title.BoundingBox.GetPoint(RectanglePoint.Bottom);

            lines = new TextSprite[6];
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = CreateText();
            }
        }

        #region Private members

        // CreateText
        private static TextSprite CreateText(string? text = null)
        {
            return new(Fonts.Common)
            {
                Color = Color.White,
                Opacity = .9f,
                MaximumWidth = 163,
                Scale = ScaleInfo.Text.ExtraLarge,
                Text = text,
                X = 33
            };
        }

        // Layout
        private void Layout()
        {
            var y = subtitle.BoundingBox.Bottom + 5;
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i].Y = y;
                y += lines[i].BoundingBox.Height + 4;
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            Game.SpriteBatch.Begin(Game.Camera);

            title.Draw(gameTime);
            subtitle.Draw(gameTime);

            for (var i = 0; i < lines.Length; i++)
            {
                lines[i].Draw(gameTime);
            }

            Game.SpriteBatch.End();
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            lineIndex = -1;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            for (var i = 0; i < lines.Length; i++)
            {
                lines[i].Update(gameTime);
            }
        }

        #endregion

        // IsTypingText
        public bool IsTypingText => lineIndex < 0 ? false : lines[lineIndex].IsTyping;

        // TypeText
        public void TypeText(string text, bool fast, bool color)
        {
            if (!pushText)
            {
                lineIndex++;
                if (lineIndex == lines.Length - 3)
                {
                    pushText = true;
                    lineIndex--;
                }
            }

            if (pushText)
            {
                // Offset texts
                for (int i = 1; i < lines.Length; i++)
                {
                    lines[i - 1].Color = lines[i].Color;
                    lines[i - 1].Text = lines[i].Text;
                }
            }

            Layout();

            lines[lineIndex].Color = color ? Color.LightBlue : Color.White;
            lines[lineIndex].Text = text;

            if (!fast)
                lines[lineIndex].StartTyping(Sound.Get(SoundNames.Keyboard)?.PopInstance());
        }
    }
}