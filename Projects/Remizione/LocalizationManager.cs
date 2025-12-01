using Engendro;
using System.Globalization;
using System.IO;

namespace Remizione
{
    /// <summary>
    /// LocalizationManager
    /// </summary>
    internal static class LocalizationManager
    {
        internal const string Abbreviations = "Languages.Abbreviations";
        internal const string English = "en-us";
        internal const string French = "fr-fr";
        internal const string German = "de-de";
        internal const string Russian = "ru-ru";
        internal const string Spanish = "es-es";
        internal const string SpanishLatinAmerica = "es-419";

        // OnTextRepositoryLoaded
        private static void OnTextRepositoryLoaded()
        {
            if (TextRepository.LanguagePackage is not null)
            {
                TextSprite.Abbreviations.Clear();

                if (TextRepository.GetValue(Abbreviations) is string abbreviations)
                {
                    foreach (var value in abbreviations.Split('|'))
                    {
                        TextSprite.Abbreviations.Add(value);
                    }
                }
            }
        }

        // GetAvailableLanguageTags
        internal static string[] GetAvailableLanguageTags()
        {
            return [SpanishLatinAmerica, English];
        }

        // Initialize
        internal static void Initialize(RemizioneGame game)
        {
            TextRepository.Loaded += OnTextRepositoryLoaded;

            var names = GetAvailableLanguageTags();
            for (var i = 0; i < names.Length; i++)
            {
                var fileName = Path.ChangeExtension(names[i].ToLower(CultureInfo.InvariantCulture), LanguagePackage.FileExtension);
                var path = ContentManagerExtension.EncodePath(game.Content, ContentFolder.Text, fileName);
                LanguagePackage.Add(names[i], path);
            }

            UserSettingsData userSettings = UserSettingsData.Load(game);

            // TODO: UNCOMMENT DURING DEV ONLY
            if (LanguagePackage.GetPackage(SpanishLatinAmerica) is LanguagePackage languagePackage)
                TextRepository.Load(languagePackage);
        }
    }
}
