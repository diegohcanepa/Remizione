using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Engendro
{
    /// <summary>
    /// TextRepository
    /// </summary>
    public static class TextRepository
    {
        #region Private fields

        [ThreadStatic]
        private static readonly StringBuilder sb = new();
        private static FrozenDictionary<string, string> texts = FrozenDictionary<string, string>.Empty;

        #endregion

        public delegate void Notify();

        #region Private members

        // CreateDictionary
        private static Dictionary<string, string> CreateDictionary(string fileName)
        {
            Dictionary<string, string> result = [];
            XmlReaderSettings settings = new() { CloseInput = true };

            using (var stm = TitleContainer.OpenStream(fileName))
            {
                var input = !XOREncryptor.IsEncryptedXml(stm) ? stm : XOREncryptor.AsStream(stm, XOREncryptor.EncryptionKey);

                if (input == null)
                    return result;

                using XmlReader r = XmlReader.Create(input, settings);
                r.MoveToContent();

                if (r.GetAttribute("PublishVersion") is string publishVersionValue)
                    PublishVersion = XmlConvert.ToInt32(publishVersionValue);

                r.Read();
                r.Read();

                while (r.Name == "String")
                {
                    if (r["Key"] is string key)
                    {
                        var value = r.ReadElementContentAsString();

                        // Normalize carriage returns and line feeds
                        value = value.Replace("\r\n", "\n").Replace('\r', '\n').Replace("\n", Environment.NewLine);

                        result[key] = value;
                    }

                    r.Read();
                }
            }

            return result;
        }

        // LoadCore
        private static void LoadCore(string fileName, LanguagePackage? languagePackage)
        {
            FileName = fileName;
            texts = CreateDictionary(fileName).ToFrozenDictionary();
            ContentVersion++;
            LanguagePackage = languagePackage;
            Loaded?.Invoke();
        }

        #endregion

        // AsDictionary
        public static Dictionary<string, string> AsDictionary(LanguagePackage languagePackage)
        {
            return AsDictionary(languagePackage.Path);
        }

        // AsDictionary
        public static Dictionary<string, string> AsDictionary(string fileName)
        {
            return CreateDictionary(fileName);
        }

        // Clear
        public static void Clear()
        {
            texts = FrozenDictionary<string, string>.Empty;
        }

        // ContainsKey
        public static bool ContainsKey(string key)
        {
            return texts.ContainsKey(key);
        }

        // ContentVersion
        public static int ContentVersion { get; private set; }

        // FileName
        public static string FileName { get; private set; } = string.Empty;

        // FindMissingGlyphs
        public static char[] FindMissingGlyphs(SpriteFont spriteFont, LanguagePackage languagePackage)
        {
            HashSet<char> result = [];
            
            var glyphDictionary = spriteFont.GetGlyphs();

            foreach (var value in AsDictionary(languagePackage).Values)
            {
                if (value == null)
                    continue;

                for (var i = 0; i < value.Length; i++)
                {
                    var c = value[i];
                    if (!glyphDictionary.ContainsKey(c))
                    {
                        result.Add(c);
                    }
                }
            }

            return [.. result];
        }

        // GetValue
        public static string GetValue(string key, params (string name, string value)[] replacements)
        {
            if (string.IsNullOrWhiteSpace(key))
                return string.Empty;

            if (IsKeyReference(key))
                key = key[1..];

            if (!texts.TryGetValue(key, out var result))
                return string.Empty;

            if (replacements.Length == 0)
                return result;

            // Optimizar reemplazos con StringBuilder
            sb.Clear();
            sb.Append(result);

            for (var i = 0; i < replacements.Length; i++)
            {
                sb.Replace($"{{{replacements[i].name}}}", replacements[i].value);
            }

            return sb.ToString();
        }

        // IsEmpty
        public static bool IsEmpty => texts.Count == 0;

        // IsKeyReference
        public static bool IsKeyReference(string text)
        {
            return text.StartsWith(KeyReferenceSymbol, StringComparison.InvariantCulture);
        }

        // KeyReferenceSymbol
        public static readonly string KeyReferenceSymbol = "@";

        // Keys
        public static IEnumerable<string> Keys => texts.Keys;

        // LanguagePackage
        public static LanguagePackage? LanguagePackage { get; private set; }

        // Load
        public static void Load(LanguagePackage languagePackage)
        {
            LoadCore(languagePackage.Path, languagePackage);
        }

        // Load
        public static void Load(string fileName)
        {
            LoadCore(fileName, null);
        }

        // Loaded
        public static event Notify? Loaded;

        // PublishVersion
        public static int PublishVersion { get; private set; }

        // Values
        public static IEnumerable<string> Values => texts.Values;
    }
}
