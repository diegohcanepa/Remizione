using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;

namespace Remizione
{
    /// <summary>
    /// Credits
    /// </summary>
    public sealed class Credits : GameObject
    {
        #region Private fields

        private readonly ImageSprite bottomBar;
        private readonly ImageSprite topBar;
        private readonly List<Credit> lines = new();

        #endregion

        #region Constructor

        // Constructor
        public Credits(EngendroGame game, string text)
            : base(game)
        {
            bottomBar = new ImageSprite(game, Atlases.UI.CreditsBar)
            {
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = Screen.Area.GetPoint(RectanglePoint.LeftBottom, 0, 3)
            };

            topBar = new ImageSprite(game, Atlases.UI.CreditsBar)
            {
                PivotOrigin = RectanglePoint.LeftTop,
                Effects = SpriteEffects.FlipVertically,
                Position = Screen.Area.GetPoint(RectanglePoint.LeftTop, 0, -3)
            };

            var lineReader = new LineReader(text);

            float y = Screen.Area.Bottom - 20;

            while (true)
            {
                var group = ReadGroup(lineReader);
                if (group.Length == 0)
                    break;

                for (int i = 0; i < group.Length; i++)
                {
                    var isGroupHeading = lines.Count > 0 && group.Length > 1 && i == 0;
                    var color = isGroupHeading ? ColorPalette.CreditHeading : ColorPalette.CreditLine;

                    Vector2 scale;
                    if (group.Length == 1)
                        scale = ScaleInfo.CreditTitle;
                    else
                        scale = isGroupHeading ? ScaleInfo.CreditHeading : ScaleInfo.CreditLine;

                    var line = new Credit(this, group[i], new Vector2(Screen.Center.X, y), color, scale);
                    lines.Add(line);
                    y += line.BoundingBox.Height;
                }

                y += group.Length == 1 ? 15 : 25;
            }
        }

        #endregion

        #region Private members

        // ReadGroup
        private static string[] ReadGroup(LineReader reader)
        {
            var list = new List<string>();

            while (true)
            {
                var text = reader.Read();
                if (text == null)
                    break;

                if (!string.IsNullOrWhiteSpace(text))
                    list.Add(text);
                else
                    break;
            }

            return list.ToArray();
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);

            for (int i = 0; i < lines.Count; i++)
            {
                lines[i].Draw(gameTime);
            }

            topBar.Draw(gameTime);
            bottomBar.Draw(gameTime);

            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            var done = true;
            for (int i = 0; i < lines.Count; i++)
            {
                lines[i].Update(gameTime);
                if (!lines[i].Done)
                    done = false;
            }

            if (done)
                Hide();
        }

        #endregion

        // Hide
        public void Hide() => IsRunning = false;

        // IsActiveInGameLoop
        public override bool IsActiveInGameLoop => IsRunning;

        // IsRunning
        public bool IsRunning { get; private set; } = true;

        // Speed
        public const int Speed = -10;

        /// <summary>
        /// LineReader
        /// </summary>
        private sealed class LineReader
        {
            private int index;
            private readonly List<string> lines = new();

            // Constructor
            internal LineReader(string text)
            {
                using var r = new StringReader(text);
                while (true)
                {
                    var value = r.ReadLine();
                    if (value == null)
                        break;
                    else
                        lines.Add(value);
                }
            }

            // AllLinesRead
            internal bool AllLinesRead => index == lines.Count;

            // Read
            internal string? Read()
            {
                if (AllLinesRead)
                    return null;

                var result = lines[index];
                index++;
                return result;
            }
        }
    }
}
