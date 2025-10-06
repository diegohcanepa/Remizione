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
            BottomGradient = this[nameof(BottomGradient)];
            CheckMark = this[nameof(CheckMark)];
            ContextMenuOptionSelector = this[nameof(ContextMenuOptionSelector)];
            CreditsBar = this[nameof(CreditsBar)];
            DialogArrowLarge = this[nameof(DialogArrowLarge)];
            EquipmentSlot = this[nameof(EquipmentSlot)];
            HeartEmptyIcon = this[nameof(HeartEmptyIcon)];
            HeartHalfIcon = this[nameof(HeartHalfIcon)];
            HeartIcon = this[nameof(HeartIcon)];
            HeartIconWithShadow = this[nameof(HeartIconWithShadow)];
            InventoryCategoryNotEmpty = this[nameof(InventoryCategoryNotEmpty)];
            InventoryGridContainer = this[nameof(InventoryGridContainer)];
            InventoryInfoContainer = this[nameof(InventoryInfoContainer)];
            InventoryInfoTitleContainer = this[nameof(InventoryInfoTitleContainer)];
            InventoryNavigationBar = this[nameof(InventoryNavigationBar)];
            InventorySlot = this[nameof(InventorySlot)];
            InventorySlotLockIcon = this[nameof(InventorySlotLockIcon)];
            InventorySlotQuestionIcon = this[nameof(InventorySlotQuestionIcon)];
            InventorySlotSelected = this[nameof(InventorySlotSelected)];
            InventorySlotTrinketsIcon = this[nameof(InventorySlotTrinketsIcon)];
            MessageContainer = this[nameof(MessageContainer)];
            MouseCursorArrow = this[nameof(MouseCursorArrow)];
            Pixel = this[nameof(Pixel)];
            PopupContainer = this[nameof(PopupContainer)];
            PopupContainerShadow = this[nameof(PopupContainerShadow)];
            ProhibitionIcon = this[nameof(ProhibitionIcon)];
            ProhibitionMark = this[nameof(ProhibitionMark)];
            SackSlot = this[nameof(SackSlot)];
            SavingIcon = this[nameof(SavingIcon)];
            SpeechBubbleCloseArrow = this[nameof(SpeechBubbleCloseArrow)];
            SpeechBubblePipe = this[nameof(SpeechBubblePipe)];
            TrinketSlot = this[nameof(TrinketSlot)];
            UITextButtonContainerEdge = this[nameof(UITextButtonContainerEdge)];
            UITextButtonContainerPattern = this[nameof(UITextButtonContainerPattern)];
        }

        // BottomGradient
        public AtlasImage BottomGradient { get; }

        // CheckMark
        public AtlasImage CheckMark { get; }

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

        // InventorySlot
        public AtlasImage InventorySlot { get; }

        // InventorySlotLockIcon
        public AtlasImage InventorySlotLockIcon { get; }

        // InventorySlotQuestionIcon
        public AtlasImage InventorySlotQuestionIcon { get; }

        // InventorySlotSelected
        public AtlasImage InventorySlotSelected { get; }

        // InventorySlotTrinketsIcon
        public AtlasImage InventorySlotTrinketsIcon { get; }

        // HeartEmptyIcon
        public AtlasImage HeartEmptyIcon { get; }

        // HeartHalfIcon
        public AtlasImage HeartHalfIcon { get; }

        // HeartIcon
        public AtlasImage HeartIcon { get; }

        // HeartIconWithShadow
        public AtlasImage HeartIconWithShadow { get; }

        // MessageContainer
        public AtlasImage MessageContainer { get; }

        // MouseCursorArrow
        public AtlasImage MouseCursorArrow { get; }

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

        // SackSlot
        public AtlasImage SackSlot { get; }

        // SavingIcon
        public AtlasImage SavingIcon { get; }

        // SpeechBubbleCloseArrow
        public AtlasImage SpeechBubbleCloseArrow { get; }

        // SpeechBubblePipe
        public AtlasImage SpeechBubblePipe { get; }

        // TrinketSlot
        public AtlasImage TrinketSlot { get; }

        // UITextButtonContainerEdge
        public AtlasImage UITextButtonContainerEdge { get; }

        // UITextButtonContainerPattern
        public AtlasImage UITextButtonContainerPattern { get; }
    }
}