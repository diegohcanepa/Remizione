using Engendro;
using System.Collections.ObjectModel;

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
            CheckMark = this[nameof(CheckMark)];
            ContextMenuOptionSelector = this[nameof(ContextMenuOptionSelector)];
            CreditsBar = this[nameof(CreditsBar)];
            InventoryCategoryMarker = this[nameof(InventoryCategoryMarker)];
            InventoryGridContainer = this[nameof(InventoryGridContainer)];
            InventoryInfoContainer = this[nameof(InventoryInfoContainer)];
            InventorySlot = this[nameof(InventorySlot)];
            InventorySlotSelection = this[nameof(InventorySlotSelection)];
            MessageContainer = this[nameof(MessageContainer)];
            MissingInputBinding = this[nameof(MissingInputBinding)];
            MissingItem = this[nameof(MissingItem)];
            MouseCursorDefault = this[nameof(MouseCursorDefault)];
            MouseCursorTargetOff = this[nameof(MouseCursorTargetOff)];
            MouseCursorTargetOn = this[nameof(MouseCursorTargetOn)];
            Pixel = this[nameof(Pixel)];
            PopupContainer = this[nameof(PopupContainer)];
            PopupContainerShadow = this[nameof(PopupContainerShadow)];
            ProhibitionIcon = this[nameof(ProhibitionIcon)];
            ProhibitionMark = this[nameof(ProhibitionMark)];
            QuickSlot = this[nameof(QuickSlot)];
            QuickSlotShadow = this[nameof(QuickSlotShadow)];
            ReactionMeter = CreateReadOnlyCollection("ReactionMeter", 1, 9);
            SavingIcon = this[nameof(SavingIcon)];
            SpeechBubbleCloseArrow = this[nameof(SpeechBubbleCloseArrow)];
            SpeechBubblePipe = this[nameof(SpeechBubblePipe)];
            UIControlContainerEdgeLarge = this[nameof(UIControlContainerEdgeLarge)];
            UIControlContainerPatternLarge = this[nameof(UIControlContainerPatternLarge)];
            UIControlContainerEdgeSmall = this[nameof(UIControlContainerEdgeSmall)];
            UIControlContainerPatternSmall = this[nameof(UIControlContainerPatternSmall)];
            UIControlShade = this[nameof(UIControlShade)];
            UnreadSign = this[nameof(UnreadSign)];
        }

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

        // MessageContainer
        public AtlasImage MessageContainer { get; }

        // MissingInputBinding
        public AtlasImage MissingInputBinding { get; }

        // MissingItem
        public AtlasImage MissingItem { get; }

        // MouseCursorDefault
        public AtlasImage MouseCursorDefault { get; }

        // MouseCursorTargetOff
        public AtlasImage MouseCursorTargetOff { get; }

        // MouseCursorTargetOn
        public AtlasImage MouseCursorTargetOn { get; }

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

        // ReactionMeter
        public ReadOnlyCollection<AtlasImage> ReactionMeter { get; }

        // UIControlContainerEdgeLarge
        public AtlasImage UIControlContainerEdgeLarge { get; }

        // UIControlContainerPatternLarge
        public AtlasImage UIControlContainerPatternLarge { get; }

        // UIControlContainerEdgeSmall
        public AtlasImage UIControlContainerEdgeSmall { get; }

        // UIControlContainerPatternSmall
        public AtlasImage UIControlContainerPatternSmall { get; }

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

        // UIControlShade
        public AtlasImage UIControlShade { get; }

        // UnreadSign
        public AtlasImage UnreadSign { get; }
    }
}