using Engendro;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ScaryCastle
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
            var assetName = ContentManagerExtension.EncodePath(ContentFolder.Fonts, fontName);
            return content.Load<SpriteFont>(assetName);
        }

        // OnTextRepositoryLoaded
        private static void OnTextRepositoryLoaded()
        {
            if (TextRepository.LanguagePackage is null)
                return;

            Common.SpriteFont = assets[CommonAssetName];
            Common.SpriteFont.LineSpacing += 2;
            Common.SpriteFont.Spacing = 3;

            CommonOutline.SpriteFont = assets[CommonOutlineAssetName];
            CommonOutline.SpriteFont.LineSpacing += 2;
            CommonOutline.SpriteFont.Spacing = -5;
        }

        #endregion

        // GetSpriteFont
        public static SpriteFont GetSpriteFont(string assetName)
        {
            return assets[assetName];
        }

        // Initialize
        public static void Initialize(ContentManager content)
        {
            assets[CommonAssetName] = LoadFont(content, CommonAssetName);
            assets[CommonOutlineAssetName] = LoadFont(content, CommonOutlineAssetName);

            TextRepository.Loaded += OnTextRepositoryLoaded;
        }

        // Common
        // SB Navigator, Regular, 48, Antialiased
        public static Font Common { get; } = new Font();

        // CommonAssetName
        public const string CommonAssetName = "Common";

        // CommonOutine
        // SB Navigator, Regular, 48, Antialiased, Outline 6
        public static Font CommonOutline { get; } = new Font();

        // CommonOutlineAssetName
        public const string CommonOutlineAssetName = "CommonOutline";
    }
}
