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
            FaithIcon = this[nameof(FaithIcon)];
            InventoryCategoryMarker = this[nameof(InventoryCategoryMarker)];
            InventoryGridContainer = this[nameof(InventoryGridContainer)];
            InventoryInfoContainer = this[nameof(InventoryInfoContainer)];
            InventorySlot = this[nameof(InventorySlot)];
            InventorySlotSelection = this[nameof(InventorySlotSelection)];
            ItemMenuContainer = this[nameof(ItemMenuContainer)];
            MessageContainer = this[nameof(MessageContainer)];
            MissingInputBinding = this[nameof(MissingInputBinding)];
            MissingItem = this[nameof(MissingItem)];
            MouseCursorArrow = this[nameof(MouseCursorArrow)];
            MouseCursorCross = this[nameof(MouseCursorCross)];
            MouseCursorCrossOn = this[nameof(MouseCursorCrossOn)];
            MouseCursorTarget = this[nameof(MouseCursorTarget)];
            MouseCursorTargetOn = this[nameof(MouseCursorTargetOn)];
            MouseCursorWait = this[nameof(MouseCursorWait)];
            Pixel = this[nameof(Pixel)];
            PopupContainer = this[nameof(PopupContainer)];
            PopupContainerShadow = this[nameof(PopupContainerShadow)];
            ProhibitionIcon = this[nameof(ProhibitionIcon)];
            ProhibitionMark = this[nameof(ProhibitionMark)];
            QuickSlot = this[nameof(QuickSlot)];
            QuickSlotShadow = this[nameof(QuickSlotShadow)];
            SavingIcon = this[nameof(SavingIcon)];
            SpeechBubbleCloseArrow = this[nameof(SpeechBubbleCloseArrow)];
            SpeechBubblePipe = this[nameof(SpeechBubblePipe)];
            SpiritIcon = this[nameof(SpiritIcon)];
            UIControlShade = this[nameof(UIControlShade)];
            UnreadSign = this[nameof(UnreadSign)];
        }

        // BottomGradient}
        public AtlasImage BottomGradient { get; }

        // CheckMark
        public AtlasImage CheckMark { get; }

        // ContextMenuOptionSelector
        public AtlasImage ContextMenuOptionSelector { get; }

        // CreditsBar
        public AtlasImage CreditsBar { get; }

        // InventoryCategoryMarker
        public AtlasImage InventoryCategoryMarker { get; }

        // InventoryGridContainer
        public AtlasImage InventoryGridContainer { get; }

        // InventoryInfoContainer
        public AtlasImage InventoryInfoContainer { get; }

        // InventorySlot
        public AtlasImage InventorySlot { get; }

        // InventorySlotSelection
        public AtlasImage InventorySlotSelection { get; }

        // ItemMenuContainer
        public AtlasImage ItemMenuContainer { get; }

        // FaithIcon
        public AtlasImage FaithIcon { get; }

        // MessageContainer
        public AtlasImage MessageContainer { get; }

        // MissingInputBinding
        public AtlasImage MissingInputBinding { get; }

        // MissingItem
        public AtlasImage MissingItem { get; }

        // MouseCursorArrow
        public AtlasImage MouseCursorArrow { get; }

        // MouseCursorCross
        public AtlasImage MouseCursorCross { get; }

        // MouseCursorCrossOn
        public AtlasImage MouseCursorCrossOn { get; }

        // MouseCursorTarget
        public AtlasImage MouseCursorTarget { get; }

        // MouseCursorTargetOn
        public AtlasImage MouseCursorTargetOn { get; }

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

        // QuickSlotShadow
        public AtlasImage QuickSlotShadow { get; }

        // SavingIcon
        public AtlasImage SavingIcon { get; }

        // SpeechBubbleCloseArrow
        public AtlasImage SpeechBubbleCloseArrow { get; }

        // SpeechBubblePipe
        public AtlasImage SpeechBubblePipe { get; }

        // SpiritIcon
        public AtlasImage SpiritIcon { get; }

        // UIControlShade
        public AtlasImage UIControlShade { get; }

        // UnreadSign
        public AtlasImage UnreadSign { get; }
    }
}