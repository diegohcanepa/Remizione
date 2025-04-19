using Engendro;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// Fonts
    /// </summary>
    public static class Fonts
    {
        private static readonly Dictionary<string, SpriteFont> assets = [];

        #region Private members

        // LoadFont
        private static SpriteFont LoadFont(ContentManager content, string fontName)
        {
            var assetName = ContentHelper.EncodePath(ContentFolder.Fonts, fontName);
            return content.Load<SpriteFont>(assetName);
        }

        // OnTextRepositoryLoaded
        private static void OnTextRepositoryLoaded()
        {
            if (TextRepository.LanguagePackage is null)
                return;

            Regular.SpriteFont = assets[RegularAssetName];
            Regular.SpriteFont.LineSpacing += 4;
            Regular.SpriteFont.Spacing = 4;

            Outline.SpriteFont = assets[OutlineAssetName];
            Outline.SpriteFont.LineSpacing += 4;
            Outline.SpriteFont.Spacing = -6;

            Speech.SpriteFont = assets[SpeechAssetName];
            Speech.SpriteFont.LineSpacing += 2;
            Speech.SpriteFont.Spacing = 5;
        }

        #endregion

        // GetSpriteFont
        public static SpriteFont GetSpriteFont(string assetName) => assets[assetName];

        // Initialize
        public static void Initialize(ContentManager content)
        {
            assets[RegularAssetName] = LoadFont(content, RegularAssetName);
            assets[OutlineAssetName] = LoadFont(content, OutlineAssetName);
            assets[SpeechAssetName] = LoadFont(content, SpeechAssetName);

            TextRepository.Loaded += OnTextRepositoryLoaded;
        }

        // Outline
        // Ancient Modern Tales, Regular, 48, Antialiased, Outline 6, Shadow Offset 0
        public static Font Outline { get; } = new Font();

        // OutlineAssetName
        public const string OutlineAssetName = "Outline";

        // Regular
        // Ancient Modern Tales, Regular, 48, Antialiased
        public static Font Regular { get; } = new Font();

        // RegularAssetName
        public const string RegularAssetName = "Regular";

        // Speech
        // SB Navigator, Regular, 48, Antialiased
        public static Font Speech { get; } = new Font();

        // SpeechAssetName
        public const string SpeechAssetName = "Speech";
    }
}
