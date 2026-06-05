using Engendro;
using System.Collections.ObjectModel;

namespace ScaryCastle
{
    /// <summary>
    /// UIAtlas abg3340/midska/taoka24
    /// </summary>
    public sealed partial class UIAtlas : Atlas
    {
        // Constructor
        public UIAtlas()
            : base(EngendroGame.Instance.Content, "UI", ContentManagerExtension.EncodePath(ContentFolder.Atlases, "UI"), false)
        {
            BossMeter = this[nameof(BossMeter)];
            BossMeterAmount = this[nameof(BossMeterAmount)];
            CheckMark = this[nameof(CheckMark)];
            Coin = this[nameof(Coin)];
            ContextMenuOptionSelector = this[nameof(ContextMenuOptionSelector)];
            CreditsBar = this[nameof(CreditsBar)];
            DialogArrowLarge = this[nameof(DialogArrowLarge)];
            DialogOptionBullet = this[nameof(DialogOptionBullet)];
            DiscardItemIcon = this[nameof(DiscardItemIcon)];
            GooIcon = this[nameof(GooIcon)];
            GooIcons = CreateReadOnlyCollection(nameof(GooIcons), 1, 2);
            GreenHearts = CreateReadOnlyCollection(nameof(GreenHearts), 1, 4);
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
            PurpleHearts = CreateReadOnlyCollection(nameof(PurpleHearts), 1, 4);
            RedHearts = CreateReadOnlyCollection(nameof(RedHearts), 1, 4);
            CountdownSkullIcon = this[nameof(CountdownSkullIcon)];
            Sack = this[nameof(Sack)];
            SavingIcon = this[nameof(SavingIcon)];
            SkullIcon = this[nameof(SkullIcon)];
            SpeechBubbleCloseArrow = this[nameof(SpeechBubbleCloseArrow)];
            SpeechBubblePipe = this[nameof(SpeechBubblePipe)];
            UIButtonContainerEdge = this[nameof(UIButtonContainerEdge)];
            UIButtonContainerPattern = this[nameof(UIButtonContainerPattern)];
        }

        // BossMeter
        public AtlasImage BossMeter { get; }

        // BossMeterAmount
        public AtlasImage BossMeterAmount { get; }

        // CheckMark
        public AtlasImage CheckMark { get; }

        // Coin
        public AtlasImage Coin { get; }

        // ContextMenuOptionSelector
        public AtlasImage ContextMenuOptionSelector { get; }

        // CountdownSkullIcon
        public AtlasImage CountdownSkullIcon { get; }

        // CreditsBar
        public AtlasImage CreditsBar { get; }

        // DialogArrowLarge
        public AtlasImage DialogArrowLarge { get; }

        // DialogOptionBullet
        public AtlasImage DialogOptionBullet { get; }

        // DiscardItemIcon
        public AtlasImage DiscardItemIcon { get; }

        // GooIcon
        public AtlasImage GooIcon { get; }

        // GooIcons
        public ReadOnlyCollection<AtlasImage> GooIcons { get; }

        // GreenHearts
        public ReadOnlyCollection<AtlasImage> GreenHearts { get; }

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

        // PurpleHearts
        public ReadOnlyCollection<AtlasImage> PurpleHearts { get; }

        // RedHearts
        public ReadOnlyCollection<AtlasImage> RedHearts { get; }

        // Sack
        public AtlasImage Sack { get; }

        // SavingIcon
        public AtlasImage SavingIcon { get; }

        // SkullIcon
        public AtlasImage SkullIcon { get; }

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