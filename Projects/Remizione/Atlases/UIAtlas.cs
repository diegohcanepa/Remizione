using Engendro;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// UIAtlas abg3340/midska/taoka24
    /// </summary>
    public sealed partial class UIAtlas : Atlas
    {
        // Constructor
        public UIAtlas()
            : base(EngendroGame.Instance.Content, "UI", ContentManagerExtension.EncodePath(ContentFolder.Atlases, "UI"), false)
        {
            BackButton = this[nameof(BackButton)];
            BackButtonHot = this[nameof(BackButtonHot)];
            CheckMark = this[nameof(CheckMark)];
            CloseButton = this[nameof(CloseButton)];
            CloseButtonHot = this[nameof(CloseButtonHot)];
            ContextMenuOptionSelector = this[nameof(ContextMenuOptionSelector)];
            CraftIcon = this[nameof(CraftIcon)];
            CreditsBar = this[nameof(CreditsBar)];
            DialogArrowLarge = this[nameof(DialogArrowLarge)];
            DialogOptionBullet = this[nameof(DialogOptionBullet)];
            EchoBackground = this[nameof(EchoBackground)];
            FakeItem = this[nameof(FakeItem)];
            GutIcon = this[nameof(GutIcon)];
            HeartIcon = this[nameof(HeartIcon)];
            HeartMeterContainer = this[nameof(HeartMeterContainer)];
            InspectItem = this[nameof(InspectItem)];
            InventoryItemSlots = CreateReadOnlyCollection("InventoryItemSlot", 1, 3);
            ItemPerishStates = CreateReadOnlyCollection("ItemPerishState", 1, 3);
            MiniMapMarker = this[nameof(MiniMapMarker)];
            MouseLeftButtonIcon = this[nameof(MouseLeftButtonIcon)];
            MouseRightButtonIcon = this[nameof(MouseRightButtonIcon)];
            Pixel = this[nameof(Pixel)];
            QuickInventoryBackground = this[nameof(QuickInventoryBackground)];
            Sack = this[nameof(Sack)];
            SavingIcon = this[nameof(SavingIcon)];
            SpeechTextArrow = this[nameof(SpeechTextArrow)];
            SpeechTextPipe = this[nameof(SpeechTextPipe)];
            StatusLaceration = CreateReadOnlyCollection(nameof(StatusLaceration), 1, 3);
            StatusPutrefaction = CreateReadOnlyCollection(nameof(StatusPutrefaction), 1, 3);
            StoneIcon = this[nameof(StoneIcon)];
            UIButtonContainerEdge = this[nameof(UIButtonContainerEdge)];
            UIButtonContainerPattern = this[nameof(UIButtonContainerPattern)];
            WoodIcon = this[nameof(WoodIcon)];
        }

        // BackButton
        public AtlasImage BackButton { get; }

        // BackButtonHot
        public AtlasImage BackButtonHot { get; }

        // CheckMark
        public AtlasImage CheckMark { get; }

        // ContextMenuOptionSelector
        public AtlasImage ContextMenuOptionSelector { get; }

        // CloseButton
        public AtlasImage CloseButton { get; }

        // CloseButtonHot
        public AtlasImage CloseButtonHot { get; }

        // CraftIcon
        public AtlasImage CraftIcon { get; }

        // CreditsBar
        public AtlasImage CreditsBar { get; }

        // DialogArrowLarge
        public AtlasImage DialogArrowLarge { get; }

        // DialogOptionBullet
        public AtlasImage DialogOptionBullet { get; }

        // EchoBackground
        public AtlasImage EchoBackground { get; }

        // FakeItem
        public AtlasImage FakeItem { get; }

        // GutIcon
        public AtlasImage GutIcon { get; }

        // HeartIcon
        public AtlasImage HeartIcon { get; }

        // HeartMeterContainer
        public AtlasImage HeartMeterContainer { get; }

        // InspectItem
        public AtlasImage InspectItem { get; }

        // InventoryItemSlots
        public ReadOnlyCollection<AtlasImage> InventoryItemSlots { get; }

        // ItemPerishStates
        public ReadOnlyCollection<AtlasImage> ItemPerishStates { get; }

        // MiniMapMarker
        public AtlasImage MiniMapMarker { get; }

        // MouseLeftButtonIcon
        public AtlasImage MouseLeftButtonIcon { get; }

        // MouseRightButtonIcon
        public AtlasImage MouseRightButtonIcon { get; }

        // Pixel
        public AtlasImage Pixel { get; }

        // QuickInventoryBackground
        public AtlasImage QuickInventoryBackground { get; }

        // Sack
        public AtlasImage Sack { get; }

        // SavingIcon
        public AtlasImage SavingIcon { get; }

        // SpeechTextArrow
        public AtlasImage SpeechTextArrow { get; }

        // SpeechTextPipe
        public AtlasImage SpeechTextPipe { get; }

        // StatusLaceration
        public ReadOnlyCollection<AtlasImage> StatusLaceration { get; }

        // StatusPutrefaction
        public ReadOnlyCollection<AtlasImage> StatusPutrefaction { get; }

        // StoneIcon
        public AtlasImage StoneIcon { get; }

        // UIButtonContainerEdge
        public AtlasImage UIButtonContainerEdge { get; }

        // UIButtonContainerPattern
        public AtlasImage UIButtonContainerPattern { get; }

        // WoodIcon
        public AtlasImage WoodIcon { get; }
    }
}