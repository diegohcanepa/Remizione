using Engendro;

namespace Remizione
{
    /// <summary>
    /// UIAtlas
    /// </summary>
    public sealed partial class UIAtlas : Atlas
    {
        // Constructor
        public UIAtlas(EngendroGame game)
            : base(game.Content, "UI", ContentManagerExtension.EncodePath(ContentFolder.Atlases, "UI"), false)
        {
            BottomGradient = this[nameof(BottomGradient)];
            CheckMark = this[nameof(CheckMark)];
            CoinIcon = this[nameof(CoinIcon)];
            ContextMenuOptionSelector = this[nameof(ContextMenuOptionSelector)];
            CreditsBar = this[nameof(CreditsBar)];
            DialogArrowLarge = this[nameof(DialogArrowLarge)];
            EquipmentSlot = this[nameof(EquipmentSlot)];
            HeartEmptyIcon = this[nameof(HeartEmptyIcon)];
            HeartHalfIcon = this[nameof(HeartHalfIcon)];
            HeartHalfIconWithShadow = this[nameof(HeartHalfIconWithShadow)];
            HeartIcon = this[nameof(HeartIcon)];
            HeartIconWithShadow = this[nameof(HeartIconWithShadow)];
            InventoryCategoryNotEmpty = this[nameof(InventoryCategoryNotEmpty)];
            InventoryGridContainer = this[nameof(InventoryGridContainer)];
            InventoryInfoContainer = this[nameof(InventoryInfoContainer)];
            InventoryInfoTitleContainer = this[nameof(InventoryInfoTitleContainer)];
            InventoryNavigationBar = this[nameof(InventoryNavigationBar)];
            ItemGridSlot = this[nameof(ItemGridSlot)];
            ItemGridSlotSelected = this[nameof(ItemGridSlotSelected)];
            MessageContainer = this[nameof(MessageContainer)];
            MouseCursorArrow = this[nameof(MouseCursorArrow)];
            Pixel = this[nameof(Pixel)];
            PopupContainer = this[nameof(PopupContainer)];
            PopupContainerShadow = this[nameof(PopupContainerShadow)];
            SackSlot = this[nameof(SackSlot)];
            SavingIcon = this[nameof(SavingIcon)];
            SpeechBubbleCloseArrow = this[nameof(SpeechBubbleCloseArrow)];
            SpeechBubblePipe = this[nameof(SpeechBubblePipe)];
            TicketIcon = this[nameof(TicketIcon)];
            TicketSlot = this[nameof(TicketSlot)];
            UIButtonContainerEdge = this[nameof(UIButtonContainerEdge)];
            UIButtonContainerPattern = this[nameof(UIButtonContainerPattern)];
        }

        // BottomGradient
        public AtlasImage BottomGradient { get; }

        // CheckMark
        public AtlasImage CheckMark { get; }

        // CoinIcon
        public AtlasImage CoinIcon { get; }

        // ContextMenuOptionSelector
        public AtlasImage ContextMenuOptionSelector { get; }

        // CreditsBar
        public AtlasImage CreditsBar { get; }

        // DialogArrowLarge
        public AtlasImage DialogArrowLarge { get; }

        // EquipmentSlot
        public AtlasImage EquipmentSlot { get; }

        // InventoryCategoryNotEmpty
        public AtlasImage InventoryCategoryNotEmpty { get; }

        // InventoryGridContainer
        public AtlasImage InventoryGridContainer { get; }

        // InventoryInfoContainer
        public AtlasImage InventoryInfoContainer { get; }

        // InventoryInfoTitleContainer
        public AtlasImage InventoryInfoTitleContainer { get; }

        // InventoryNavigationBar
        public AtlasImage InventoryNavigationBar { get; }

        // HeartEmptyIcon
        public AtlasImage HeartEmptyIcon { get; }

        // HeartHalfIcon
        public AtlasImage HeartHalfIcon { get; }

        // HeartHalfIconWithShadow
        public AtlasImage HeartHalfIconWithShadow { get; }

        // HeartIcon
        public AtlasImage HeartIcon { get; }

        // HeartIconWithShadow
        public AtlasImage HeartIconWithShadow { get; }

        // MessageContainer
        public AtlasImage MessageContainer { get; }

        // MouseCursorArrow
        public AtlasImage MouseCursorArrow { get; }

        // ItemGridSlot
        public AtlasImage ItemGridSlot { get; }

        // ItemGridSlotSelected
        public AtlasImage ItemGridSlotSelected { get; }

        // Pixel
        public AtlasImage Pixel { get; }

        // PopupContainer
        public AtlasImage PopupContainer { get; }

        // PopupContainerShadow
        public AtlasImage PopupContainerShadow { get; }

        // SackSlot
        public AtlasImage SackSlot { get; }

        // SavingIcon
        public AtlasImage SavingIcon { get; }

        // SpeechBubbleCloseArrow
        public AtlasImage SpeechBubbleCloseArrow { get; }

        // SpeechBubblePipe
        public AtlasImage SpeechBubblePipe { get; }

        // TicketIcon
        public AtlasImage TicketIcon { get; }

        // TicketSlot
        public AtlasImage TicketSlot { get; }

        // UIButtonContainerEdge
        public AtlasImage UIButtonContainerEdge { get; }

        // UIButtonContainerPattern
        public AtlasImage UIButtonContainerPattern { get; }
    }
}