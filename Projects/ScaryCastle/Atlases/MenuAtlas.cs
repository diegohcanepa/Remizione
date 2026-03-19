using Engendro;

namespace ScaryCastle
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
            ControllerAdvice = this[nameof(ControllerAdvice)];
            ControlsKeyboard = this[nameof(ControlsKeyboard)];
            ControlsNintendoSwitch = this[nameof(ControlsNintendoSwitch)];
            ControlsPlayStation4 = this[nameof(ControlsPlayStation4)];
            ControlsPlayStation5 = this[nameof(ControlsPlayStation5)];
            ControlsXboxOne = this[nameof(ControlsXboxOne)];
            ControlsWindows = this[nameof(ControlsWindows)];
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
            RatShadow = this[nameof(RatShadow)];
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

        // ControllerAdvice
        public AtlasImage ControllerAdvice { get; }

        // ControlsKeyboard
        public AtlasImage ControlsKeyboard { get; }

        // ControlsNintendoSwitch
        public AtlasImage ControlsNintendoSwitch { get; }

        // ControlsPlayStation4
        public AtlasImage ControlsPlayStation4 { get; }

        // ControlsPlayStation5
        public AtlasImage ControlsPlayStation5 { get; }

        // ControlsWindows
        public AtlasImage ControlsWindows { get; }

        // ControlsXboxOne
        public AtlasImage ControlsXboxOne { get; }

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

        // RatShadow
        public AtlasImage RatShadow { get; }

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
