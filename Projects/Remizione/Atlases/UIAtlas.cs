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
            CharacterSheetContainer = this[nameof(CharacterSheetContainer)];
            CharacterSheetStatContainer = this[nameof(CharacterSheetStatContainer)];
            CheckMark = this[nameof(CheckMark)];
            CloseWindowButton = this[nameof(CloseWindowButton)];
            ContextMenuOptionSelector = this[nameof(ContextMenuOptionSelector)];
            CreditsBar = this[nameof(CreditsBar)];
            FaithGainIcon = this[nameof(FaithGainIcon)];
            FaithIcon = this[nameof(FaithIcon)];
            GraceGainIcon = this[nameof(GraceGainIcon)];
            GraceIcon = this[nameof(GraceIcon)];
            InventorySlot = this[nameof(InventorySlot)];
            InventorySlotSelected = this[nameof(InventorySlotSelected)];
            InventorySlotSelection = this[nameof(InventorySlotSelection)];
            ItemMenuContainer = this[nameof(ItemMenuContainer)];
            ItemMenuContainerSelection = this[nameof(ItemMenuContainerSelection)];
            MessageContainer = this[nameof(MessageContainer)];
            MouseCursorArrow = this[nameof(MouseCursorArrow)];
            MouseCursorCross = this[nameof(MouseCursorCross)];
            MouseCursorCrossOn = this[nameof(MouseCursorCrossOn)];
            MouseCursorWait = this[nameof(MouseCursorWait)];
            Pixel = this[nameof(Pixel)];
            PopupContainer = this[nameof(PopupContainer)];
            PopupContainerShadow = this[nameof(PopupContainerShadow)];
            ProhibitionIcon = this[nameof(ProhibitionIcon)];
            ProhibitionMark = this[nameof(ProhibitionMark)];
            SavingIcon = this[nameof(SavingIcon)];
            SpeechBubbleCloseArrow = this[nameof(SpeechBubbleCloseArrow)];
            SpeechBubblePipe = this[nameof(SpeechBubblePipe)];
            SpiritGainIcon = this[nameof(SpiritGainIcon)];
            SpiritIcon = this[nameof(SpiritIcon)];
            UIControlContainerEdgeLarge = this[nameof(UIControlContainerEdgeLarge)];
            UIControlContainerPatternLarge = this[nameof(UIControlContainerPatternLarge)];
            UIControlContainerEdgeSmall = this[nameof(UIControlContainerEdgeSmall)];
            UIControlContainerPatternSmall = this[nameof(UIControlContainerPatternSmall)];
            UIControlShade = this[nameof(UIControlShade)];
            UnreadSign = this[nameof(UnreadSign)];
        }

        // BottomGradient
        public AtlasImage BottomGradient { get; }

        // CharacterSheetContainer
        public AtlasImage CharacterSheetContainer { get; }

        // CharacterSheetStatContainer
        public AtlasImage CharacterSheetStatContainer { get; }

        // CheckMark
        public AtlasImage CheckMark { get; }

        // CloseWindowButton
        public AtlasImage CloseWindowButton { get; }

        // ContextMenuOptionSelector
        public AtlasImage ContextMenuOptionSelector { get; }

        // CreditsBar
        public AtlasImage CreditsBar { get; }

        // InventorySlot
        public AtlasImage InventorySlot { get; }

        // InventorySlotSelected
        public AtlasImage InventorySlotSelected { get; }

        // InventorySlotSelection
        public AtlasImage InventorySlotSelection { get; }

        // ItemMenuContainer
        public AtlasImage ItemMenuContainer { get; }

        // ItemMenuContainerSelection
        public AtlasImage ItemMenuContainerSelection { get; }

        // FaithGainIcon
        public AtlasImage FaithGainIcon { get; }

        // FaithIcon
        public AtlasImage FaithIcon { get; }

        // GraceGainIcon
        public AtlasImage GraceGainIcon { get; }

        // GraceIcon
        public AtlasImage GraceIcon { get; }

        // MessageContainer
        public AtlasImage MessageContainer { get; }

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

        // SavingIcon
        public AtlasImage SavingIcon { get; }

        // SpeechBubbleCloseArrow
        public AtlasImage SpeechBubbleCloseArrow { get; }

        // SpeechBubblePipe
        public AtlasImage SpeechBubblePipe { get; }

        // SpiritGainIcon
        public AtlasImage SpiritGainIcon { get; }

        // SpiritIcon
        public AtlasImage SpiritIcon { get; }

        // UIControlContainerEdgeLeftLarge
        public AtlasImage UIControlContainerEdgeLarge { get; }

        // UIControlContainerPatternLarge
        public AtlasImage UIControlContainerPatternLarge { get; }

        // UIControlContainerEdgeLeftSmall
        public AtlasImage UIControlContainerEdgeSmall { get; }

        // UIControlContainerPatternSmall
        public AtlasImage UIControlContainerPatternSmall { get; }

        // UIControlShade
        public AtlasImage UIControlShade { get; }

        // UnreadSign
        public AtlasImage UnreadSign { get; }
    }
}