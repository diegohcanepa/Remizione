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
            Common.SpriteFont.Spacing = 1;

            CommonOutline.SpriteFont = assets[CommonOutlineAssetName];
            CommonOutline.SpriteFont.LineSpacing -= 5;
            CommonOutline.SpriteFont.Spacing = -8;

            Monitor.SpriteFont = assets[MonitorAssetName];
            Monitor.SpriteFont.LineSpacing += 2;
            Monitor.SpriteFont.Spacing = 3;
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
            assets[MonitorAssetName] = LoadFont(content, MonitorAssetName);

            TextRepository.Loaded += OnTextRepositoryLoaded;
        }

        // Common
        // Bubblegum Sans, Regular, 48, Antialiased, Outline 6
        public static Font Common { get; } = new();

        // CommonAssetName
        public const string CommonAssetName = "Common";

        // CommonOutline
        // Bubblegum Sans, Regular, 48, Antialiased, Outline 6
        public static Font CommonOutline { get; } = new();

        // CommonOutlineAssetName
        public const string CommonOutlineAssetName = "CommonOutline";

        // Monitor
        // SB Navigator, Regular, 48
        public static Font Monitor { get; } = new();

        // MonitorAssetName
        public const string MonitorAssetName = "Monitor";
    }
}
