using Microsoft.Xna.Framework.Content;
using System;
using System.IO;

namespace ScaryCastle
{
    /// <summary>
    /// ContentManagerExtension
    /// </summary>
    internal static class ContentManagerExtension
    {
        // EncodeAudioPath
        internal static string EncodeAudioPath(ContentFolder folder)
        {
            if (folder is ContentFolder.FX or ContentFolder.Ambience or ContentFolder.Music or ContentFolder.Voices)
                return Path.Combine("Audio", folder.ToString());

            throw new ArgumentException("Invalid folder.", nameof(folder));
        }

        // EncodePath
        internal static string EncodePath(ContentFolder folder, string assetName)
        {
            return Path.Combine(folder.ToString(), assetName);
        }

        // EncodePath
        internal static string EncodePath(ContentManager content, ContentFolder folder, string assetName)
        {
            return Path.Combine(content.RootDirectory, folder.ToString(), assetName);
        }
    }
}
