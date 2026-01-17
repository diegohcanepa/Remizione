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
            Coin = this[nameof(Coin)];
            ContextMenuOptionSelector = this[nameof(ContextMenuOptionSelector)];
            CreditsBar = this[nameof(CreditsBar)];
            DialogArrowLarge = this[nameof(DialogArrowLarge)];
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
            MouseCursorCross = this[nameof(MouseCursorCross)];
            MouseCursorCrossOn = this[nameof(MouseCursorCrossOn)];
            MouseCursorDown = this[nameof(MouseCursorDown)];
            MouseCursorLeft = this[nameof(MouseCursorLeft)];
            MouseCursorRight = this[nameof(MouseCursorRight)];
            MouseCursorUp = this[nameof(MouseCursorUp)];
            MouseCursorWait = this[nameof(MouseCursorWait)];
            PickupShadow = this[nameof(PickupShadow)];
            Pixel = this[nameof(Pixel)];
            PointingHand = this[nameof(PointingHand)];
            PopupContainer = this[nameof(PopupContainer)];
            PopupContainerShadow = this[nameof(PopupContainerShadow)];
            Sack = this[nameof(Sack)];
            SavingIcon = this[nameof(SavingIcon)];
            SpeechBubbleCloseArrow = this[nameof(SpeechBubbleCloseArrow)];
            SpeechBubblePipe = this[nameof(SpeechBubblePipe)];
            UIButtonContainerEdge = this[nameof(UIButtonContainerEdge)];
            UIButtonContainerPattern = this[nameof(UIButtonContainerPattern)];
        }

        // BottomGradient
        public AtlasImage BottomGradient { get; }

        // CheckMark
        public AtlasImage CheckMark { get; }

        // Coin
        public AtlasImage Coin { get; }

        // ContextMenuOptionSelector
        public AtlasImage ContextMenuOptionSelector { get; }

        // CreditsBar
        public AtlasImage CreditsBar { get; }

        // DialogArrowLarge
        public AtlasImage DialogArrowLarge { get; }

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

        // MouseCursorCross
        public AtlasImage MouseCursorCross { get; }

        // MouseCursorCrossOn
        public AtlasImage MouseCursorCrossOn { get; }

        // MouseCursorDown
        public AtlasImage MouseCursorDown { get; }

        // MouseCursorLeft
        public AtlasImage MouseCursorLeft { get; }

        // MouseCursorRight
        public AtlasImage MouseCursorRight { get; }

        // MouseCursorUp
        public AtlasImage MouseCursorUp { get; }

        // MouseCursorWait
        public AtlasImage MouseCursorWait { get; }

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
        public AtlasImage Sack { get; }

        // SavingIcon
        public AtlasImage SavingIcon { get; }

        // SpeechBubbleCloseArrow
        public AtlasImage SpeechBubbleCloseArrow { get; }

        // SpeechBubblePipe
        public AtlasImage SpeechBubblePipe { get; }

        // UIButtonContainerEdge
        public AtlasImage UIButtonContainerEdge { get; }

        // UIButtonContainerPattern
        public AtlasImage UIButtonContainerPattern { get; }
    }
}