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
            AlertIcon = this[nameof(AlertIcon)];
            BossMeter = this[nameof(BossMeter)];
            BossMeterAmount = this[nameof(BossMeterAmount)];
            BronzeKeyIcon = this[nameof(BronzeKeyIcon)];
            CheckMark = this[nameof(CheckMark)];
            Coin = this[nameof(Coin)];
            CoinIcon = this[nameof(CoinIcon)];
            ContextMenuOptionSelector = this[nameof(ContextMenuOptionSelector)];
            CreditsBar = this[nameof(CreditsBar)];
            CurseMeter = CreateReadOnlyCollection(nameof(CurseMeter), 0, 9);
            DialogArrowLarge = this[nameof(DialogArrowLarge)];
            DialogOptionBullet = this[nameof(DialogOptionBullet)];
            DiscardItemIcon = this[nameof(DiscardItemIcon)];
            FaithIcon = this[nameof(FaithIcon)];
            GoldenKeyIcon = this[nameof(GoldenKeyIcon)];
            GooIcon = this[nameof(GooIcon)];
            FaithMeter = CreateReadOnlyCollection(nameof(FaithMeter), 0, 5);
            GreenHearts = CreateReadOnlyCollection(nameof(GreenHearts), 1, 4);
            HeartIcon = this[nameof(HeartIcon)];
            InventoryFaithAmounts = CreateReadOnlyCollection("InventoryFaithAmount", 1, 3);
            InventoryItemAmounts = CreateReadOnlyCollection("InventoryItemAmount", 1, 5);
            InventoryItemSlot = this[nameof(InventoryItemSlot)];
            MessageContainer = this[nameof(MessageContainer)];
            MiniMapCoin = this[nameof(MiniMapCoin)];
            MiniMapCoinAndLoot = this[nameof(MiniMapCoinAndLoot)];
            MiniMapLoot = this[nameof(MiniMapLoot)];
            PickupShadow = this[nameof(PickupShadow)];
            Pixel = this[nameof(Pixel)];
            PopupContainer = this[nameof(PopupContainer)];
            PopupContainerShadow = this[nameof(PopupContainerShadow)];
            PurpleHearts = CreateReadOnlyCollection(nameof(PurpleHearts), 1, 4);
            RedHearts = CreateReadOnlyCollection(nameof(RedHearts), 1, 4);
            CountdownSkullIcon = this[nameof(CountdownSkullIcon)];
            Sack = this[nameof(Sack)];
            SavingIcon = this[nameof(SavingIcon)];
            SkullIcon = this[nameof(SkullIcon)];
            SpeechTextArrow = this[nameof(SpeechTextArrow)];
            SpeechTextPipe = this[nameof(SpeechTextPipe)];
            UIButtonContainerEdge = this[nameof(UIButtonContainerEdge)];
            UIButtonContainerPattern = this[nameof(UIButtonContainerPattern)];
        }

        // AlertIcon
        public AtlasImage AlertIcon { get; }

        // BossMeter
        public AtlasImage BossMeter { get; }

        // BossMeterAmount
        public AtlasImage BossMeterAmount { get; }

        // BronzeKeyIcon
        public AtlasImage BronzeKeyIcon { get; }

        // CheckMark
        public AtlasImage CheckMark { get; }

        // Coin
        public AtlasImage Coin { get; }

        // CoinIcon
        public AtlasImage CoinIcon { get; }

        // ContextMenuOptionSelector
        public AtlasImage ContextMenuOptionSelector { get; }

        // CountdownSkullIcon
        public AtlasImage CountdownSkullIcon { get; }

        // CreditsBar
        public AtlasImage CreditsBar { get; }

        // CurseMeter
        public ReadOnlyCollection<AtlasImage> CurseMeter { get; }

        // DialogArrowLarge
        public AtlasImage DialogArrowLarge { get; }

        // DialogOptionBullet
        public AtlasImage DialogOptionBullet { get; }

        // DiscardItemIcon
        public AtlasImage DiscardItemIcon { get; }

        // FaithIcon
        public AtlasImage FaithIcon { get; }

        // FaithMeter
        public ReadOnlyCollection<AtlasImage> FaithMeter { get; }

        // GoldenKeyIcon
        public AtlasImage GoldenKeyIcon { get; }

        // GooIcon
        public AtlasImage GooIcon { get; }

        // GreenHearts
        public ReadOnlyCollection<AtlasImage> GreenHearts { get; }

        // HeartIcon
        public AtlasImage HeartIcon { get; }

        // InventoryFaithAmounts
        public ReadOnlyCollection<AtlasImage> InventoryFaithAmounts { get; }

        // InventoryItemAmounts
        public ReadOnlyCollection<AtlasImage> InventoryItemAmounts { get; }

        // InventoryItemSlot
        public AtlasImage InventoryItemSlot { get; }

        // MessageContainer
        public AtlasImage MessageContainer { get; }

        // MiniMapCoin
        public AtlasImage MiniMapCoin { get; }

        // MiniMapCoinAndLoot
        public AtlasImage MiniMapCoinAndLoot { get; }

        // MiniMapLoot
        public AtlasImage MiniMapLoot { get; }

        // PickupShadow
        public AtlasImage PickupShadow { get; }

        // Pixel
        public AtlasImage Pixel { get; }

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

        // SpeechTextArrow
        public AtlasImage SpeechTextArrow { get; }

        // SpeechTextPipe
        public AtlasImage SpeechTextPipe { get; }

        // UIButtonContainerEdge
        public AtlasImage UIButtonContainerEdge { get; }

        // UIButtonContainerPattern
        public AtlasImage UIButtonContainerPattern { get; }
    }
}