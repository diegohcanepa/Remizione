using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Engendro
{
    /// <summary>
    /// Atlas
    /// </summary>
    public partial class Atlas : IDisposable, INamedObject
    {
        #region Private fields

        private readonly Dictionary<string, AtlasImage> images = [];
        private static readonly Dictionary<string, Atlas> instancesByName = [];
        private static readonly List<Atlas> instanceList = [];

        #endregion

        #region Constructor

        // Constructor
        public Atlas(ContentManager content, string name, string assetName, bool useFolderNames)
        {
            CodeContract.NotEmpty(name, nameof(name));

            // Content
            this.Content = content;

            // Name
            this.Name = name;

            // Name must be unique
            if (instancesByName.ContainsKey(name))
                CodeContract.ThrowDuplicatedNameException(nameof(name));

            // Check texture name
            CodeContract.NotEmpty(assetName, nameof(assetName));

            // Load texture
            this.Texture = content.Load<Texture2D>(assetName);
            this.AssetName = assetName;
            this.UseFolderNames = useFolderNames;

            // Load atlas data
            var dataFile = Path.Combine(content.RootDirectory, assetName);
            dataFile = Path.ChangeExtension(dataFile, "xml");
            using var stm = TitleContainer.OpenStream(dataFile);
            LoadXmlData(stm);

            instancesByName.Add(name, this);
            instanceList.Add(this);
        }

        #endregion

        #region Private members

        // GetImageCore
        private AtlasImage? GetImageCore(string imageName, LanguagePackage? languagePackage, string separator)
        {
            CodeContract.NotEmpty(imageName, nameof(imageName));

            AtlasImage? result = null;

            if (languagePackage != null)
            {
                var localizedImageName = imageName + separator + languagePackage.LanguageTag;
                images.TryGetValue(localizedImageName, out result);
            }

            if (result == null)
                images.TryGetValue(imageName, out result);

            return result;
        }

        // LoadXmlData
        private void LoadXmlData(Stream input)
        {
            using XmlReader r = XmlReader.Create(input);

            while (r.ReadToFollowing("sprite"))
            {
                if (r["n"] is string name)
                {
                    name = Path.ChangeExtension(name, null);

                    if (r["x"] is string xValue &&
                        r["y"] is string yValue &&
                        r["w"] is string wValue &&
                        r["h"] is string hValue)
                    {
                        var x = XmlConvert.ToInt32(xValue);
                        var y = XmlConvert.ToInt32(yValue);
                        var w = XmlConvert.ToInt32(wValue);
                        var h = XmlConvert.ToInt32(hValue);

                        images[name] = new AtlasImage(name, this, new Rectangle(x, y, w, h));
                    }
                }
            }
        }

        #endregion

        #region Protected members

        // CreateReadOnlyCollection
        protected static ReadOnlyCollection<T> CreateReadOnlyCollection<T>(params T[] items)
        {
            return new(items);
        }

        // CreateReadOnlyCollection
        protected ReadOnlyCollection<AtlasImage> CreateReadOnlyCollection(string prefix, int start, int end)
        {
            return CreateReadOnlyCollection(prefix, start, end, string.Empty);
        }

        // CreateReadOnlyCollection
        protected ReadOnlyCollection<AtlasImage> CreateReadOnlyCollection(string prefix, int start, int end, string format)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(start, end);

            AtlasImage[] list = new AtlasImage[end - start + 1];

            for (var i = 0; i < list.Length; i++)
            {
                var imageName = prefix + (start + i).ToString(format, CultureInfo.InvariantCulture);
                list[i] = this[imageName];
            }

            return new ReadOnlyCollection<AtlasImage>(list);
        }

        // Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                Texture.Dispose();
                images.Clear();
                instanceList.Remove(this);
                instancesByName.Remove(Name);
            }

            IsDisposed = true;
        }

        #endregion

        // AssetName
        public string AssetName { get; }

        // Contains
        public bool Contains(string imageName)
        {
            return images.ContainsKey(imageName);
        }

        // Content
        public ContentManager Content { get; }

        // Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // DisposeFromContent
        public static int DisposeFromContent(ContentManager content)
        {
            List<Atlas> list = [];

            for (var i = 0; i < instanceList.Count; i++)
            {
                var item = instanceList[i];
                if (item.Content == content)
                {
                    list.Add(item);
                }
            }

            for (var i = 0; i < list.Count; i++)
            {
                list[i].Dispose();
            }

            return list.Count;
        }

        // GetImage
        public AtlasImage? GetImage(string imageName)
        {
            return GetImageCore(imageName, null, string.Empty);
        }

        // GetImageNoNull
        public AtlasImage GetImageNotNull(string imageName)
        {
            return GetImageCore(imageName, null, string.Empty) ?? throw new KeyNotFoundException($"Image '{imageName}' not found in atlas '{Name}'.");
        }

        // GetInstance
        public static Atlas? GetInstance(string name)
        {
            CodeContract.NotEmpty(name, nameof(name));
            instancesByName.TryGetValue(name, out var result);
            return result;
        }

        // GetLocalizedImage
        public AtlasImage? GetLocalizedImage(string imageName, LanguagePackage languagePackage, string separator = "-")
        {
            return GetImageCore(imageName, languagePackage, separator);
        }

        // Index
        public AtlasImage this[string imageName]
        {
            get
            {
                CodeContract.NotEmpty(imageName, nameof(imageName));
                return images[imageName];
            }
        }

        // IsDisposed
        public bool IsDisposed { get; private set; }

        // Name
        public string Name { get; }

        // Texture
        public Texture2D Texture { get; }

        // ToString
        public override string ToString()
        {
            return $"{Name} [{AssetName}]";
        }

        // UseFolderNames
        public bool UseFolderNames { get; }
    }
}