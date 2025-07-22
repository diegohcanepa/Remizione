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
            : base(game.Content, "UI", ContentHelper.EncodePath(ContentFolder.Atlases, "UI"), false)
        {
            BagSlot = this[nameof(BagSlot)];
            BottomGradient = this[nameof(BottomGradient)];
            CheckMark = this[nameof(CheckMark)];
            CloseWindowButton = this[nameof(CloseWindowButton)];
            ContextMenuOptionSelector = this[nameof(ContextMenuOptionSelector)];
            CreditsBar = this[nameof(CreditsBar)];
            DialogArrowLarge = this[nameof(DialogArrowLarge)];
            HeartEmptyIcon = this[nameof(HeartEmptyIcon)];
            HeartHalfIcon = this[nameof(HeartHalfIcon)];
            HeartIcon = this[nameof(HeartIcon)];
            InventoryCategoryConsumables = this[nameof(InventoryCategoryConsumables)];
            InventoryCategoryEquipment = this[nameof(InventoryCategoryEquipment)];
            InventoryCategoryKeyItems = this[nameof(InventoryCategoryKeyItems)];
            InventoryCategorySkills = this[nameof(InventoryCategorySkills)];
            InventoryGridContainer = this[nameof(InventoryGridContainer)];
            InventoryInfoContainer = this[nameof(InventoryInfoContainer)];
            InventoryInfoTitleContainer = this[nameof(InventoryInfoTitleContainer)];
            InventoryNavigationBar = this[nameof(InventoryNavigationBar)];
            InventorySlot = this[nameof(InventorySlot)];
            InventorySlotLockIcon = this[nameof(InventorySlotLockIcon)];
            InventorySlotQuestionIcon = this[nameof(InventorySlotQuestionIcon)];
            InventorySlotSelected = this[nameof(InventorySlotSelected)];
            MessageContainer = this[nameof(MessageContainer)];
            MiniHeartHalfIcon = this[nameof(MiniHeartHalfIcon)];
            MiniHeartIcon = this[nameof(MiniHeartIcon)];
            MouseCursorArrow = this[nameof(MouseCursorArrow)];
            MouseCursorCross = this[nameof(MouseCursorCross)];
            MouseCursorCrossOn = this[nameof(MouseCursorCrossOn)];
            MouseCursorWait = this[nameof(MouseCursorWait)];
            Pixel = this[nameof(Pixel)];
            PopupContainer = this[nameof(PopupContainer)];
            PopupContainerShadow = this[nameof(PopupContainerShadow)];
            ProhibitionIcon = this[nameof(ProhibitionIcon)];
            ProhibitionMark = this[nameof(ProhibitionMark)];
            QuickSlot = this[nameof(QuickSlot)];
            SavingIcon = this[nameof(SavingIcon)];
            SpeechBubbleCloseArrow = this[nameof(SpeechBubbleCloseArrow)];
            SpeechBubblePipe = this[nameof(SpeechBubblePipe)];
            TicketGoldenIcon = this[nameof(TicketGoldenIcon)];
            TicketGreenIcon = this[nameof(TicketGreenIcon)];
            TicketWhiteIcon = this[nameof(TicketWhiteIcon)];
            UITextButtonContainerEdge = this[nameof(UITextButtonContainerEdge)];
            UITextButtonContainerPattern = this[nameof(UITextButtonContainerPattern)];
        }

        // BagSlot
        public AtlasImage BagSlot { get; }

        // BottomGradient
        public AtlasImage BottomGradient { get; }

        // CheckMark
        public AtlasImage CheckMark { get; }

        // CloseWindowButton
        public AtlasImage CloseWindowButton { get; }

        // ContextMenuOptionSelector
        public AtlasImage ContextMenuOptionSelector { get; }

        // CreditsBar
        public AtlasImage CreditsBar { get; }

        // DialogArrowLarge
        public AtlasImage DialogArrowLarge { get; }

        // InventoryCategoryConsumables
        public AtlasImage InventoryCategoryConsumables { get; }

        // InventoryCategoryEquipment
        public AtlasImage InventoryCategoryEquipment { get; }

        // InventoryCategoryKeyItems
        public AtlasImage InventoryCategoryKeyItems { get; }

        // InventoryCategorySkills
        public AtlasImage InventoryCategorySkills { get; }

        // InventoryGridContainer
        public AtlasImage InventoryGridContainer { get; }

        // InventoryInfoContainer
        public AtlasImage InventoryInfoContainer { get; }

        // InventoryInfoTitleContainer
        public AtlasImage InventoryInfoTitleContainer { get; }

        // InventoryNavigationBar
        public AtlasImage InventoryNavigationBar { get; }

        // InventorySlot
        public AtlasImage InventorySlot { get; }

        // InventorySlotLockIcon
        public AtlasImage InventorySlotLockIcon { get; }

        // InventorySlotQuestionIcon
        public AtlasImage InventorySlotQuestionIcon { get; }

        // InventorySlotSelected
        public AtlasImage InventorySlotSelected { get; }

        // HeartEmptyIcon
        public AtlasImage HeartEmptyIcon { get; }

        // HeartHalfIcon
        public AtlasImage HeartHalfIcon { get; }

        // HeartIcon
        public AtlasImage HeartIcon { get; }

        // MessageContainer
        public AtlasImage MessageContainer { get; }

        // MiniHeartHalfIcon
        public AtlasImage MiniHeartHalfIcon { get; }

        // MiniHeartIcon
        public AtlasImage MiniHeartIcon { get; }

        // MouseCursorArrow
        public AtlasImage MouseCursorArrow { get; }

        // MouseCursorCross
        public AtlasImage MouseCursorCross { get; }

        // MouseCursorCrossOn
        public AtlasImage MouseCursorCrossOn { get; }

        // MouseCursorWait
        public AtlasImage MouseCursorWait { get; }

        // Pixel
        public AtlasImage Pixel { get; }

        // PopupContainer
        public AtlasImage PopupContainer { get; }

        // PopupContainerShadow
        public AtlasImage PopupContainerShadow { get; }

        // ProhibitionIcon
        public AtlasImage ProhibitionIcon { get; }

        // ProhibitionMark
        public AtlasImage ProhibitionMark { get; }

        // QuickSlot
        public AtlasImage QuickSlot { get; }

        // SavingIcon
        public AtlasImage SavingIcon { get; }

        // SpeechBubbleCloseArrow
        public AtlasImage SpeechBubbleCloseArrow { get; }

        // SpeechBubblePipe
        public AtlasImage SpeechBubblePipe { get; }

        // TicketGoldenIcon
        public AtlasImage TicketGoldenIcon { get; }

        // TicketGreenIcon
        public AtlasImage TicketGreenIcon { get; }

        // TicketWhiteIcon
        public AtlasImage TicketWhiteIcon { get; }

        // UITextButtonContainerEdge
        public AtlasImage UITextButtonContainerEdge { get; }

        // UITextButtonContainerPattern
        public AtlasImage UITextButtonContainerPattern { get; }
    }
}