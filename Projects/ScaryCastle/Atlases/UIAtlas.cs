using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// UIAtlas abg3340
    /// </summary>
    public sealed partial class UIAtlas : Atlas
    {
        // Constructor
        public UIAtlas()
            : base(EngendroGame.Instance.Content, "UI", ContentManagerExtension.EncodePath(ContentFolder.Atlases, "UI"), false)
        {
            BottomGradient = this[nameof(BottomGradient)];
            CheckMark = this[nameof(CheckMark)];
            Coin = this[nameof(Coin)];
            ContextMenuOptionSelector = this[nameof(ContextMenuOptionSelector)];
            CreditsBar = this[nameof(CreditsBar)];
            DialogArrowLarge = this[nameof(DialogArrowLarge)];
            FaithEmpty = this[nameof(FaithEmpty)];
            FaithHalf = this[nameof(FaithHalf)];
            FaithIcon = this[nameof(FaithIcon)];
            FaithFull = this[nameof(FaithFull)];
            FearEmpty = this[nameof(FearEmpty)];
            FearFull = this[nameof(FearFull)];
            FearIcon = this[nameof(FearIcon)];
            HPEmpty = this[nameof(HPEmpty)];
            HPHalf = this[nameof(HPHalf)];
            HPFull = this[nameof(HPFull)];
            InventorySlot = this[nameof(InventorySlot)];
            InventoryGridContainer = this[nameof(InventoryGridContainer)];
            InventoryInfoContainer = this[nameof(InventoryInfoContainer)];
            InventoryInfoTitleContainer = this[nameof(InventoryInfoTitleContainer)];
            InventoryNavigationBar = this[nameof(InventoryNavigationBar)];
            ItemGridSlot = this[nameof(ItemGridSlot)];
            ItemGridSlotSelected = this[nameof(ItemGridSlotSelected)];
            MessageContainer = this[nameof(MessageContainer)];
            PickupShadow = this[nameof(PickupShadow)];
            Pixel = this[nameof(Pixel)];
            PointingHand = this[nameof(PointingHand)];
            PopupContainer = this[nameof(PopupContainer)];
            PopupContainerShadow = this[nameof(PopupContainerShadow)];
            Sack = this[nameof(Sack)];
            SavingIcon = this[nameof(SavingIcon)];
            Skull = this[nameof(Skull)];
            SkullIcon = this[nameof(SkullIcon)];
            SpeechBubbleCloseArrow = this[nameof(SpeechBubbleCloseArrow)];
            SpeechBubblePipe = this[nameof(SpeechBubblePipe)];
            TalkIcon = this[nameof(TalkIcon)];
            TunnelIcon = this[nameof(TunnelIcon)];
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

        // FaithEmpty
        public AtlasImage FaithEmpty { get; }

        // FaithHalf
        public AtlasImage FaithHalf { get; }

        // FaithIcon
        public AtlasImage FaithIcon { get; }

        // FaithFull
        public AtlasImage FaithFull { get; }

        // FearEmpty
        public AtlasImage FearEmpty { get; }

        // FearFull
        public AtlasImage FearFull { get; }

        // FearIcon
        public AtlasImage FearIcon { get; }

        // InventorySlot
        public AtlasImage InventorySlot { get; }

        // InventoryGridContainer
        public AtlasImage InventoryGridContainer { get; }

        // InventoryInfoContainer
        public AtlasImage InventoryInfoContainer { get; }

        // InventoryInfoTitleContainer
        public AtlasImage InventoryInfoTitleContainer { get; }

        // InventoryNavigationBar
        public AtlasImage InventoryNavigationBar { get; }

        // ItemGridSlot
        public AtlasImage ItemGridSlot { get; }

        // ItemGridSlotSelected
        public AtlasImage ItemGridSlotSelected { get; }

        // HeartEmpty
        public AtlasImage HPEmpty { get; }

        // HeartHalf
        public AtlasImage HPHalf { get; }

        // HeartFull
        public AtlasImage HPFull { get; }

        // MessageContainer
        public AtlasImage MessageContainer { get; }

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

        // Sack
        public AtlasImage Sack { get; }

        // SavingIcon
        public AtlasImage SavingIcon { get; }

        // Skull
        public AtlasImage Skull { get; }

        // SkullIcon
        public AtlasImage SkullIcon { get; }

        // SpeechBubbleCloseArrow
        public AtlasImage SpeechBubbleCloseArrow { get; }

        // SpeechBubblePipe
        public AtlasImage SpeechBubblePipe { get; }

        // TalkIcon
        public AtlasImage TalkIcon { get; }

        // TunnelIcon
        public AtlasImage TunnelIcon { get; }

        // UIButtonContainerEdge
        public AtlasImage UIButtonContainerEdge { get; }

        // UIButtonContainerPattern
        public AtlasImage UIButtonContainerPattern { get; }
    }
}