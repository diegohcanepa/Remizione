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
            DialogArrowLarge = this[nameof(DialogArrowLarge)];
            FaithIcon = this[nameof(FaithIcon)];
            HeartEmptyIcon = this[nameof(HeartEmptyIcon)];
            HeartHalfIcon = this[nameof(HeartHalfIcon)];
            HeartIcon = this[nameof(HeartIcon)];
            HPIcon = this[nameof(HPIcon)];
            InventorySlot = this[nameof(InventorySlot)];
            InventorySlotSelected = this[nameof(InventorySlotSelected)];
            ItemMenuContainer = this[nameof(ItemMenuContainer)];
            ItemMenuContainerSelection = this[nameof(ItemMenuContainerSelection)];
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
            TicketsIcon = this[nameof(TicketsIcon)];
            UITextButtonContainerEdge = this[nameof(UITextButtonContainerEdge)];
            UITextButtonContainerPattern = this[nameof(UITextButtonContainerPattern)];
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

        // DialogArrowLarge
        public AtlasImage DialogArrowLarge { get; }

        // InventorySlot
        public AtlasImage InventorySlot { get; }

        // InventorySlotSelected
        public AtlasImage InventorySlotSelected { get; }

        // ItemMenuContainer
        public AtlasImage ItemMenuContainer { get; }

        // ItemMenuContainerSelection
        public AtlasImage ItemMenuContainerSelection { get; }

        // FaithIcon
        public AtlasImage FaithIcon { get; }

        // HeartEmptyIcon
        public AtlasImage HeartEmptyIcon { get; }

        // HeartHalfIcon
        public AtlasImage HeartHalfIcon { get; }

        // HeartIcon
        public AtlasImage HeartIcon { get; }

        // HPIcon
        public AtlasImage HPIcon { get; }

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

        // TicketsIcon
        public AtlasImage TicketsIcon { get; }

        // UITextButtonContainerEdge
        public AtlasImage UITextButtonContainerEdge { get; }

        // UITextButtonContainerPattern
        public AtlasImage UITextButtonContainerPattern { get; }
    }
}