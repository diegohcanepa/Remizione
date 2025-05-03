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

            Main.SpriteFont = assets[MainAssetName];
            Main.SpriteFont.LineSpacing += 4;
            Main.SpriteFont.Spacing = 4;

            MainOutline.SpriteFont = assets[MainOutlineAssetName];
            MainOutline.SpriteFont.LineSpacing += 4;
            MainOutline.SpriteFont.Spacing = -6;

            Common.SpriteFont = assets[CommonAssetName];
            Common.SpriteFont.LineSpacing += 2;
            Common.SpriteFont.Spacing = 5;

            CommonOutline.SpriteFont = assets[CommonOutlineAssetName];
            CommonOutline.SpriteFont.LineSpacing += 2;
            CommonOutline.SpriteFont.Spacing = -7;
        }

        #endregion

        // GetSpriteFont
        public static SpriteFont GetSpriteFont(string assetName) => assets[assetName];

        // Initialize
        public static void Initialize(ContentManager content)
        {
            assets[CommonAssetName] = LoadFont(content, CommonAssetName);
            assets[CommonOutlineAssetName] = LoadFont(content, CommonOutlineAssetName);
            assets[MainAssetName] = LoadFont(content, MainAssetName);
            assets[MainOutlineAssetName] = LoadFont(content, MainOutlineAssetName);

            TextRepository.Loaded += OnTextRepositoryLoaded;
        }

        // Main
        // Ancient Modern Tales, Regular, 48, Antialiased
        public static Font Main { get; } = new Font();

        // MainAssetName
        public const string MainAssetName = "Main";

        // MainOutline
        // Ancient Modern Tales, Regular, 48, Antialiased, Outline 6, Shadow Offset 0
        public static Font MainOutline { get; } = new Font();

        // MainOutlineAssetName
        public const string MainOutlineAssetName = "MainOutline";

        // Common
        // SB Navigator, Regular, 48, Antialiased
        public static Font Common { get; } = new Font();

        // CommonAssetName
        public const string CommonAssetName = "Common";

        // CommonOutine
        // SB Navigator, Regular, 48, Antialiased
        public static Font CommonOutline { get; } = new Font();

        // CommonOutlineAssetName
        public const string CommonOutlineAssetName = "CommonOutline";
    }
}
