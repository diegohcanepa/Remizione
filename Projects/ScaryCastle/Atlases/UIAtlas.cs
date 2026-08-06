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
            BronzeKeyIcon = this[nameof(BronzeKeyIcon)];
            CheckMark = this[nameof(CheckMark)];
            CloseWindowButton = this[nameof(CloseWindowButton)];
            Coin = this[nameof(Coin)];
            CoinIcon = this[nameof(CoinIcon)];
            ContextMenuOptionSelector = this[nameof(ContextMenuOptionSelector)];
            CreditsBar = this[nameof(CreditsBar)];
            DialogArrowLarge = this[nameof(DialogArrowLarge)];
            DialogOptionBullet = this[nameof(DialogOptionBullet)];
            DiscardItemIcon = this[nameof(DiscardItemIcon)];
            FaithIcon = this[nameof(FaithIcon)];
            FaithMeter = CreateReadOnlyCollection(nameof(FaithMeter), 0, 5);
            FearIcon = this[nameof(FearIcon)];
            GoldenKeyIcon = this[nameof(GoldenKeyIcon)];
            GooIcon = this[nameof(GooIcon)];
            HeartIcon = this[nameof(HeartIcon)];
            InventoryFaithCosts = CreateReadOnlyCollection("InventoryFaithCost", 1, 3);
            InventoryItemAmounts = CreateReadOnlyCollection("InventoryItemAmount", 1, 5);
            InventoryItemSlot = this[nameof(InventoryItemSlot)];
            MagnifierIcon = this[nameof(MagnifierIcon)];
            MessageContainer = this[nameof(MessageContainer)];
            MeterGreen = CreateReadOnlyCollection("GreenMeter", 0, 5);
            MeterOrange = CreateReadOnlyCollection("OrangeMeter", 0, 5);
            MeterPurple = CreateReadOnlyCollection("PurpleMeter", 0, 5);
            MeterSkyBlue = CreateReadOnlyCollection("SkyBlueMeter", 0, 5);
            MeterWhite = CreateReadOnlyCollection("WhiteMeter", 0, 5);
            MiniMapCoin = this[nameof(MiniMapCoin)];
            MiniMapCoinAndLoot = this[nameof(MiniMapCoinAndLoot)];
            MiniMapLoot = this[nameof(MiniMapLoot)];
            MouseLeftButtonIcon = this[nameof(MouseLeftButtonIcon)];
            MouseRightButtonIcon = this[nameof(MouseRightButtonIcon)];
            PickupShadow = this[nameof(PickupShadow)];
            Pixel = this[nameof(Pixel)];
            PopupContainer = this[nameof(PopupContainer)];
            PopupContainerShadow = this[nameof(PopupContainerShadow)];
            QuickInventoryBackground = this[nameof(QuickInventoryBackground)];
            RedHearts = CreateReadOnlyCollection(nameof(RedHearts), 1, 3);
            CountdownSkullIcon = this[nameof(CountdownSkullIcon)];
            Sack = this[nameof(Sack)];
            SavingIcon = this[nameof(SavingIcon)];
            SpeechTextArrow = this[nameof(SpeechTextArrow)];
            SpeechTextPipe = this[nameof(SpeechTextPipe)];
            StaminaIcon = this[nameof(StaminaIcon)];
            StaminaMeter = CreateReadOnlyCollection(nameof(StaminaMeter), 0, 5);
            UIButtonContainerEdge = this[nameof(UIButtonContainerEdge)];
            UIButtonContainerPattern = this[nameof(UIButtonContainerPattern)];
        }

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

        // CloseWindowButton
        public AtlasImage CloseWindowButton { get; }

        // CreditsBar
        public AtlasImage CreditsBar { get; }

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

        // FearIcon
        public AtlasImage FearIcon { get; }

        // GoldenKeyIcon
        public AtlasImage GoldenKeyIcon { get; }

        // GooIcon
        public AtlasImage GooIcon { get; }

        // HeartIcon
        public AtlasImage HeartIcon { get; }

        // InventoryFaithCosts
        public ReadOnlyCollection<AtlasImage> InventoryFaithCosts { get; }

        // InventoryItemAmounts
        public ReadOnlyCollection<AtlasImage> InventoryItemAmounts { get; }

        // InventoryItemSlot
        public AtlasImage InventoryItemSlot { get; }

        // MagnifierIcon
        public AtlasImage MagnifierIcon { get; }

        // MessageContainer
        public AtlasImage MessageContainer { get; }

        // MeterGreen
        public ReadOnlyCollection<AtlasImage> MeterGreen { get; }

        // MeterOrange
        public ReadOnlyCollection<AtlasImage> MeterOrange { get; }

        // MeterPurple
        public ReadOnlyCollection<AtlasImage> MeterPurple { get; }

        // MeterSkyBlue
        public ReadOnlyCollection<AtlasImage> MeterSkyBlue { get; }

        // MeterWhite
        public ReadOnlyCollection<AtlasImage> MeterWhite { get; }

        // MiniMapCoin
        public AtlasImage MiniMapCoin { get; }

        // MiniMapCoinAndLoot
        public AtlasImage MiniMapCoinAndLoot { get; }

        // MiniMapLoot
        public AtlasImage MiniMapLoot { get; }

        // MouseLeftButtonIcon
        public AtlasImage MouseLeftButtonIcon { get; }

        // MouseRightButtonIcon
        public AtlasImage MouseRightButtonIcon { get; }

        // PickupShadow
        public AtlasImage PickupShadow { get; }

        // Pixel
        public AtlasImage Pixel { get; }

        // PopupContainer
        public AtlasImage PopupContainer { get; }

        // PopupContainerShadow
        public AtlasImage PopupContainerShadow { get; }

        // QuickInventoryBackground
        public AtlasImage QuickInventoryBackground { get; }

        // RedHearts
        public ReadOnlyCollection<AtlasImage> RedHearts { get; }

        // Sack
        public AtlasImage Sack { get; }

        // SavingIcon
        public AtlasImage SavingIcon { get; }

        // SpeechTextArrow
        public AtlasImage SpeechTextArrow { get; }

        // SpeechTextPipe
        public AtlasImage SpeechTextPipe { get; }

        // StaminaIcon
        public AtlasImage StaminaIcon { get; }

        // StaminaMeter
        public ReadOnlyCollection<AtlasImage> StaminaMeter { get; }

        // UIButtonContainerEdge
        public AtlasImage UIButtonContainerEdge { get; }

        // UIButtonContainerPattern
        public AtlasImage UIButtonContainerPattern { get; }
    }
}