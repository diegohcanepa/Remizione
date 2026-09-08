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
            CheckMark = this[nameof(CheckMark)];
            CloseWindowButton = this[nameof(CloseWindowButton)];
            ContextMenuOptionSelector = this[nameof(ContextMenuOptionSelector)];
            CreditsBar = this[nameof(CreditsBar)];
            DialogArrowLarge = this[nameof(DialogArrowLarge)];
            DialogOptionBullet = this[nameof(DialogOptionBullet)];
            DiscardItemIcon = this[nameof(DiscardItemIcon)];
            EchoBackground = this[nameof(EchoBackground)];
            FakeItem = this[nameof(FakeItem)];
            HeartIcon = this[nameof(HeartIcon)];
            InventoryItemAmounts = CreateReadOnlyCollection("InventoryItemAmount", 1, 5);
            InventoryMeterSlot = this[nameof(InventoryMeterSlot)];
            MessageContainer = this[nameof(MessageContainer)];
            MiniMapNodes = CreateReadOnlyCollection<MapNodeState>("MiniMapNode");
            MouseLeftButtonIcon = this[nameof(MouseLeftButtonIcon)];
            MouseRightButtonIcon = this[nameof(MouseRightButtonIcon)];
            Pixel = this[nameof(Pixel)];
            PopupContainer = this[nameof(PopupContainer)];
            PopupContainerShadow = this[nameof(PopupContainerShadow)];
            QuickInventoryBackground = this[nameof(QuickInventoryBackground)];
            RedHearts = CreateReadOnlyCollection(nameof(RedHearts), 1, 3);
            Sack = this[nameof(Sack)];
            SavingIcon = this[nameof(SavingIcon)];
            SpeechTextArrow = this[nameof(SpeechTextArrow)];
            SpeechTextPipe = this[nameof(SpeechTextPipe)];
            UIButtonContainerEdge = this[nameof(UIButtonContainerEdge)];
            UIButtonContainerPattern = this[nameof(UIButtonContainerPattern)];
        }

        // CheckMark
        public AtlasImage CheckMark { get; }

        // ContextMenuOptionSelector
        public AtlasImage ContextMenuOptionSelector { get; }

        // CloseWindowButton
        public AtlasImage CloseWindowButton { get; }

        // CreditsBar
        public AtlasImage CreditsBar { get; }

        // DialogArrowLarge
        public AtlasImage DialogArrowLarge { get; }

        // DialogOptionBullet
        public AtlasImage DialogOptionBullet { get; }

        // DiscardItemIcon
        public AtlasImage DiscardItemIcon { get; }

        // EchoBackground
        public AtlasImage EchoBackground { get; }

        // FakeItem
        public AtlasImage FakeItem { get; }

        // HeartIcon
        public AtlasImage HeartIcon { get; }

        // InventoryItemAmounts
        public ReadOnlyCollection<AtlasImage> InventoryItemAmounts { get; }

        // InventoryMeterSlot
        public AtlasImage InventoryMeterSlot { get; }

        // MessageContainer
        public AtlasImage MessageContainer { get; }

        // MiniMapRooms
        public ReadOnlyCollection<AtlasImage> MiniMapNodes { get; }

        // MouseLeftButtonIcon
        public AtlasImage MouseLeftButtonIcon { get; }

        // MouseRightButtonIcon
        public AtlasImage MouseRightButtonIcon { get; }

        // Pixel
        public AtlasImage Pixel { get; }

        // PopupContainer
        public AtlasImage PopupContainer { get; }

        // PopupContainerShadow
        public AtlasImage PopupContainerShadow { get; }

        // QuickInventoryBackground
        public AtlasImage QuickInventoryBackground { get; }

        // RedHearts
        public ReadOnlyCollection<AtlasImage> RedHearts { get; }

        // Sack
        public AtlasImage Sack { get; }

        // SavingIcon
        public AtlasImage SavingIcon { get; }

        // SpeechTextArrow
        public AtlasImage SpeechTextArrow { get; }

        // SpeechTextPipe
        public AtlasImage SpeechTextPipe { get; }

        // UIButtonContainerEdge
        public AtlasImage UIButtonContainerEdge { get; }

        // UIButtonContainerPattern
        public AtlasImage UIButtonContainerPattern { get; }
    }
}