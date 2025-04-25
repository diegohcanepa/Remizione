using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// OverlayTextManager
    /// </summary>
    public sealed class OverlayTextManager : GameObject
    {
        private readonly List<(string Name, TextSprite TextSprite)> texts = [];

        // Constructor
        public OverlayTextManager(RemizioneGame game)
            : base(game)
        {
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (texts.Count > 0)
            {
                Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp, null);
                for (var i = 0; i < texts.Count; i++)
                {
                    texts[i].TextSprite.Draw(gameTime);
                }
                Game.SpriteBatch.End();
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (texts.Count > 0)
            {
                for (var i = 0; i < texts.Count; i++)
                {
                    texts[i].TextSprite.Update(gameTime);
                }
            }
        }

        #endregion

        // Clear
        public void Clear()
        {
            texts.Clear();
        }

        // Count
        public int Count => texts.Count;

        // GetText
        public TextSprite GetText(int index)
        {
            return texts[index].TextSprite;
        }

        // GetText
        public TextSprite? GetText(string name)
        {
            for (var i = 0; i < texts.Count; i++)
            {
                if (texts[i].Name == name)
                {
                    return texts[i].TextSprite;
                }
            }

            return null;
        }

        // Hide
        public void Hide(string name, int fadeDuration)
        {
            for (var i = texts.Count - 1; i >= 0; i--)
            {
                if (texts[i].Name == name)
                {
                    if (fadeDuration <= 0)
                    {
                        texts.RemoveAt(i);
                    }
                    else
                    {
                        var textSprite = texts[i].TextSprite;
                        textSprite.Tweens.OpacityTween = FloatTween.Create(TweenStyle.CubicIn, textSprite.Opacity, 0, fadeDuration, () => Hide(name, 0));
                    }
                }
            }
        }

        // Show
        public TextSprite Show(string name, string text, Color color, RectanglePoint pivotOrigin, Vector2 position, Vector2 scale, int maximumWidth, int fadeDuration)
        {
            TextSprite textSprite = new(Game, Fonts.Main)
            {
                Color = color,
                MaximumWidth = maximumWidth,
                PivotOrigin = pivotOrigin,
                Position = position,
                Scale = scale,
                Text = text
            };

            if (fadeDuration > 0)
            {
                textSprite.Tweens.OpacityTween = FloatTween.Create(TweenStyle.CubicIn, 0, 1, fadeDuration);
            }

            texts.Add((name, textSprite));

            return textSprite;
        }
    }
}
