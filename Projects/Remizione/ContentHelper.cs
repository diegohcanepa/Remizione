using Microsoft.Xna.Framework.Content;
using System;
using System.IO;

namespace Remizione
{
    /// <summary>
    /// ContentHelper
    /// </summary>
    internal static class ContentHelper
    {
        // EncodeAudioPath
        internal static string EncodeAudioPath(ContentFolder folder)
        {
            if (folder == ContentFolder.FX || folder == ContentFolder.Ambience || folder == ContentFolder.Music || folder == ContentFolder.Voices)
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
