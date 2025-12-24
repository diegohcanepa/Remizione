using Engendro;

namespace ScaryCastle
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
            HeartEmpty = this[nameof(HeartEmpty)];
            HeartHalf = this[nameof(HeartHalf)];
            Heart = this[nameof(Heart)];
            InventoryCategoryGadget = this[nameof(InventoryCategoryGadget)];
            InventoryCategoryLeftHand = this[nameof(InventoryCategoryLeftHand)];
            InventoryCategoryRightHand = this[nameof(InventoryCategoryRightHand)];
            InventoryGridContainer = this[nameof(InventoryGridContainer)];
            InventoryInfoContainer = this[nameof(InventoryInfoContainer)];
            InventoryInfoTitleContainer = this[nameof(InventoryInfoTitleContainer)];
            InventoryNavigationBar = this[nameof(InventoryNavigationBar)];
            ItemGridSlot = this[nameof(ItemGridSlot)];
            ItemGridSlotSelected = this[nameof(ItemGridSlotSelected)];
            MessageContainer = this[nameof(MessageContainer)];
            MouseCursorArrow = this[nameof(MouseCursorArrow)];
            PickupShadow = this[nameof(PickupShadow)];
            Pixel = this[nameof(Pixel)];
            PointingHand = this[nameof(PointingHand)];
            PopupContainer = this[nameof(PopupContainer)];
            PopupContainerShadow = this[nameof(PopupContainerShadow)];
            SackIcon = this[nameof(SackIcon)];
            SackSlot = this[nameof(SackSlot)];
            SavingIcon = this[nameof(SavingIcon)];
            SpeechBubbleCloseArrow = this[nameof(SpeechBubbleCloseArrow)];
            SpeechBubblePipe = this[nameof(SpeechBubblePipe)];
            TicketIcon = this[nameof(TicketIcon)];
            TicketPriceIcon = this[nameof(TicketPriceIcon)];
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

        // InventoryCategoryGadget
        public AtlasImage InventoryCategoryGadget { get; }

        // inventoryCategoryLeftHand
        public AtlasImage InventoryCategoryLeftHand { get; }

        // inventoryCategoryRightHand
        public AtlasImage InventoryCategoryRightHand { get; }

        // InventoryGridContainer
        public AtlasImage InventoryGridContainer { get; }

        // InventoryInfoContainer
        public AtlasImage InventoryInfoContainer { get; }

        // InventoryInfoTitleContainer
        public AtlasImage InventoryInfoTitleContainer { get; }

        // InventoryNavigationBar
        public AtlasImage InventoryNavigationBar { get; }

        // HeartEmpty
        public AtlasImage HeartEmpty { get; }

        // HeartHalf
        public AtlasImage HeartHalf { get; }

        // Heart
        public AtlasImage Heart { get; }

        // MessageContainer
        public AtlasImage MessageContainer { get; }

        // MouseCursorArrow
        public AtlasImage MouseCursorArrow { get; }

        // ItemGridSlot
        public AtlasImage ItemGridSlot { get; }

        // ItemGridSlotSelected
        public AtlasImage ItemGridSlotSelected { get; }

        // PickupShadow
        public AtlasImage PickupShadow { get; }

        // Pixel
        public AtlasImage Pixel { get; }

        // PointingHand
        public AtlasImage PointingHand { get; }

        // PopupContainer
        public AtlasImage PopupContainer { get; }

        // PopupContainerShadow
        public AtlasImage PopupContainerShadow { get; }

        // SackIcon
        public AtlasImage SackIcon { get; }

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

        // TicketpriceIcon
        public AtlasImage TicketPriceIcon { get; }

        // TicketSlot
        public AtlasImage TicketSlot { get; }

        // UIButtonContainerEdge
        public AtlasImage UIButtonContainerEdge { get; }

        // UIButtonContainerPattern
        public AtlasImage UIButtonContainerPattern { get; }
    }
}