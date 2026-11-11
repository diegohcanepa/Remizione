using Engendro;

namespace Remizione
{
    /// <summary>
    /// MenuAtlas
    /// </summary>
    public sealed partial class MenuAtlas : Atlas
    {
        // Constructor
        public MenuAtlas()
            : base(EngendroGame.Instance.Content, "Menu", ContentManagerExtension.EncodePath(ContentFolder.Atlases, "Menu"), false)
        {
            BottomOrnament = this[nameof(BottomOrnament)];
            ContainerScreen = this[nameof(ContainerScreen)];
            DemoRibbon = this[nameof(DemoRibbon)];
            DeveloperLogo = this[nameof(DeveloperLogo)];
            FadeCircle = this[nameof(FadeCircle)];
            GameLogo = this[nameof(GameLogo)];
            LanguageIcon = this[nameof(LanguageIcon)];
            MenuItemArrowLeft = this[nameof(MenuItemArrowLeft)];
            MenuItemHighlight = this[nameof(MenuItemHighlight)];
            MenuItemArrowRight = this[nameof(MenuItemArrowRight)];
            MessageBoxOrnamentBottom = this[nameof(MessageBoxOrnamentBottom)];
            MessageBoxOrnamentTop = this[nameof(MessageBoxOrnamentTop)];
            OptionsIcon = this[nameof(OptionsIcon)];
            OptionHighlight = this[nameof(OptionHighlight)];
            PauseMenuBackground = this[nameof(PauseMenuBackground)];
            PublisherLogo = this[nameof(PublisherLogo)];
            StartGameMenuBackground = this[nameof(StartGameMenuBackground)];
            StartGameMenuSelectedSlotContainer = this[nameof(StartGameMenuSelectedSlotContainer)];
            TitleScreen = this[nameof(TitleScreen)];
            TitleScreenForeground = this[nameof(TitleScreenForeground)];
            TitleScreenForegroundLight = this[nameof(TitleScreenForegroundLight)];
            TitleScreenRoofLight = this[nameof(TitleScreenRoofLight)];
        }

        // BottomOrnament
        public AtlasImage BottomOrnament { get; }

        // ContainerScreen
        public AtlasImage ContainerScreen { get; }

        // DemoRibbon
        public AtlasImage DemoRibbon { get; }

        // DeveloperLogo
        public AtlasImage DeveloperLogo { get; }

        // FadeCircle
        public AtlasImage FadeCircle { get; }

        // GameLogo
        public AtlasImage GameLogo { get; }

        // LanguageIcon
        public AtlasImage LanguageIcon { get; }

        // MenuItemArrowLeft
        public AtlasImage MenuItemArrowLeft { get; }

        // MenuItemArrowRight
        public AtlasImage MenuItemArrowRight { get; }

        // MenuItemHighlight
        public AtlasImage MenuItemHighlight { get; }

        // MessageBoxOrnamentBottom
        public AtlasImage MessageBoxOrnamentBottom { get; }

        // MessageBoxOrnamentTop
        public AtlasImage MessageBoxOrnamentTop { get; }

        // OptionHighlight
        public AtlasImage OptionHighlight { get; }

        // OptionsMenu
        public AtlasImage OptionsIcon { get; }

        // PauseMenuBackground
        public AtlasImage PauseMenuBackground { get; }

        // PublisherLogo
        public AtlasImage PublisherLogo { get; }

        // StartGameMenuBackground
        public AtlasImage StartGameMenuBackground { get; }

        // StartGameMenuSelectedSlotContainer
        public AtlasImage StartGameMenuSelectedSlotContainer { get; }

        // TitleScreen
        public AtlasImage TitleScreen { get; }

        // TitleScreenForeground
        public AtlasImage TitleScreenForeground { get; }

        // TitleScreenForegroundLight
        public AtlasImage TitleScreenForegroundLight { get; }

        // TitleScreenRoofLight
        public AtlasImage TitleScreenRoofLight { get; }
    }
}
