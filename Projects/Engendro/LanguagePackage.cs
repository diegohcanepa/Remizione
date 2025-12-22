using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Engendro
{
    /// <summary>
    /// LanguagePackage
    /// </summary>
    public sealed class LanguagePackage
    {
        private static readonly List<LanguagePackage> packageList = [];

        #region Constructor

        // Static constructor
        static LanguagePackage()
        {
            Packages = new ReadOnlyCollection<LanguagePackage>(packageList);
        }

        // Constructor
        private LanguagePackage(string languageTag, string path)
        {
            CodeContract.NotEmpty(languageTag, nameof(languageTag));
            CodeContract.NotEmpty(path, nameof(path));

            this.LanguageTag = languageTag;
            this.Path = path;
            this.CultureInfo = CultureInfo.GetCultureInfo(languageTag) ?? CultureInfo.InvariantCulture;
        }

        #endregion

        // Add
        public static LanguagePackage Add(string languageTag, string path)
        {
            if (FindPackage(languageTag) != null)
            {
                throw new InvalidOperationException("A package with the same language tag already exists.");
            }

            LanguagePackage result = new(languageTag, path);
            packageList.Add(result);
            return result;
        }

        // CultureInfo
        public CultureInfo CultureInfo { get; }

        // FileExtension
        public static string FileExtension => "lpkg";

        // FindPackage
        public static LanguagePackage? FindPackage(string languageTag)
        {
            return FindPackage(languageTag, false);
        }

        // FindPackage
        public static LanguagePackage? FindPackage(string languageTag, bool closestMatch)
        {
            CodeContract.NotEmpty(languageTag, nameof(languageTag));

            // Try exact match
            for (var i = 0; i < packageList.Count; i++)
            {
                if (string.Equals(packageList[i].LanguageTag, languageTag, StringComparison.CurrentCultureIgnoreCase))
                {
                    return packageList[i];
                }
            }

            // Try closest match
            if (closestMatch)
            {
                // Try country code only 'en'
                var countryRegion = languageTag.Split('-');
                foreach (var package in packageList)
                {
                    if (string.Compare(package.LanguageTag.Split('-')[0], countryRegion[0], true) == 0)
                    {
                        return package;
                    }
                }
            }

            return null;
        }

        // LanguageTag
        public string LanguageTag { get; }

        // Packages
        public static ReadOnlyCollection<LanguagePackage> Packages { get; }

        // Path
        public string Path { get; }

        // ToString
        public override string ToString()
        {
            return LanguageTag;
        }
    }
}
