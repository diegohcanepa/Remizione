using Engendro;

namespace Remizione.Menus
{
    /// <summary>
    /// LanguageOptionValue
    /// </summary>
    public readonly struct LanguageMenuOptionValue
    {
        public LanguageMenuOptionValue(string languageTag)
        {
            this.LanguageTag = languageTag;
        }

        public string LanguageTag { get; }
    }

    /// <summary>
    /// LanguageMenuOption
    /// </summary>
    public sealed class LanguageOption : Option<LanguageMenuOptionValue>
    {
        // Constructor
        internal LanguageOption(RemizioneGame game)
            : base(game, "@Menu.Options.Language", game.CurrentSession == null)
        {
            IconImage = Atlases.Menu.LanguageIcon;

            foreach (var languagePackage in LanguagePackage.Packages)
            {
                AddValue($"@Languages.{languagePackage.LanguageTag}", new LanguageMenuOptionValue(languagePackage.LanguageTag));
            }

            if (TextRepository.LanguagePackage != null)
            {
                Value = new LanguageMenuOptionValue(TextRepository.LanguagePackage.LanguageTag);
            }
        }

        // OnValueChanged
        protected override void OnValueChanged(LanguageMenuOptionValue newValue)
        {
            if (LanguagePackage.FindPackage(newValue.LanguageTag) is LanguagePackage languagePackage)
            {
                TextRepository.Load(languagePackage);
            }
        }
    }
}
