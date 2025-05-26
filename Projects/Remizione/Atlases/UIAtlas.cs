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
            BagIcon = this[nameof(BagIcon)];
            BottomGradient = this[nameof(BottomGradient)];
            CharacterSheetContainer = this[nameof(CharacterSheetContainer)];
            CharacterSheetStatContainer = this[nameof(CharacterSheetStatContainer)];
            CheckMark = this[nameof(CheckMark)];
            CloseWindowButton = this[nameof(CloseWindowButton)];
            ContextMenuOptionSelector = this[nameof(ContextMenuOptionSelector)];
            CreditsBar = this[nameof(CreditsBar)];
            FaithIcon = this[nameof(FaithIcon)];
            InventorySlot = this[nameof(InventorySlot)];
            InventorySlotSelection = this[nameof(InventorySlotSelection)];
            ItemMenuContainer = this[nameof(ItemMenuContainer)];
            ItemMenuContainerSelection = this[nameof(ItemMenuContainerSelection)];
            MessageContainer = this[nameof(MessageContainer)];
            MissingInputBinding = this[nameof(MissingInputBinding)];
            MouseCursorArrow = this[nameof(MouseCursorArrow)];
            MouseCursorCross = this[nameof(MouseCursorCross)];
            MouseCursorCrossOn = this[nameof(MouseCursorCrossOn)];
            MouseCursorTarget = this[nameof(MouseCursorTarget)];
            MouseCursorTargetOn = this[nameof(MouseCursorTargetOn)];
            MouseCursorWait = this[nameof(MouseCursorWait)];
            Pixel = this[nameof(Pixel)];
            PopupContainer = this[nameof(PopupContainer)];
            PopupContainerShadow = this[nameof(PopupContainerShadow)];
            PrayerIcon = this[nameof(PrayerIcon)];
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

        // BagIcon
        public AtlasImage BagIcon { get; }

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

        // InventorySlotSelection
        public AtlasImage InventorySlotSelection { get; }

        // ItemMenuContainer
        public AtlasImage ItemMenuContainer { get; }

        // ItemMenuContainerSelection
        public AtlasImage ItemMenuContainerSelection { get; }

        // FaithIcon
        public AtlasImage FaithIcon { get; }

        // MessageContainer
        public AtlasImage MessageContainer { get; }

        // MissingInputBinding
        public AtlasImage MissingInputBinding { get; }

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

        // PrayerIcon
        public AtlasImage PrayerIcon { get; }

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