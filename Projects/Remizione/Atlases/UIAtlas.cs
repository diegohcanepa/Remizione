using Engendro;
using System.Collections.ObjectModel;

namespace Remizione
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
            CheckMark = this[nameof(CheckMark)];
            CloseWindowButton = this[nameof(CloseWindowButton)];
            ContextMenuOptionSelector = this[nameof(ContextMenuOptionSelector)];
            CreditsBar = this[nameof(CreditsBar)];
            DialogArrowLarge = this[nameof(DialogArrowLarge)];
            DialogOptionBullet = this[nameof(DialogOptionBullet)];
            DiscardItemIcon = this[nameof(DiscardItemIcon)];
            DroolCost = CreateReadOnlyCollection("DroolCost", 1, 3);
            DroolIcon = this[nameof(DroolIcon)];
            DroolMeter = CreateReadOnlyCollection(nameof(DroolMeter), 0, 5);
            EchoBackground = this[nameof(EchoBackground)];
            HeartIcon = this[nameof(HeartIcon)];
            InventoryItemAmounts = CreateReadOnlyCollection("InventoryItemAmount", 1, 5);
            InventoryItemSlot = this[nameof(InventoryItemSlot)];
            InventoryMeterSlot = this[nameof(InventoryMeterSlot)];
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
            MiniMapNodes = CreateReadOnlyCollection<MapNodeState>("MiniMapNode");
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
            StaminaCosts = CreateReadOnlyCollection("StaminaCost", 1, 3);
            StaminaIcon = this[nameof(StaminaIcon)];
            UIButtonContainerEdge = this[nameof(UIButtonContainerEdge)];
            UIButtonContainerPattern = this[nameof(UIButtonContainerPattern)];
        }

        // CheckMark
        public AtlasImage CheckMark { get; }

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

        // DroolCost
        public ReadOnlyCollection<AtlasImage> DroolCost { get; }

        // DroolIcon
        public AtlasImage DroolIcon { get; }

        // DroolMeter
        public ReadOnlyCollection<AtlasImage> DroolMeter { get; }

        // EchoBackground
        public AtlasImage EchoBackground { get; }

        // HeartIcon
        public AtlasImage HeartIcon { get; }

        // InventoryItemAmounts
        public ReadOnlyCollection<AtlasImage> InventoryItemAmounts { get; }

        // InventoryItemSlot
        public AtlasImage InventoryItemSlot { get; }

        // InventoryMeterSlot
        public AtlasImage InventoryMeterSlot { get; }

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

        // MiniMapRooms
        public ReadOnlyCollection<AtlasImage> MiniMapNodes { get; }

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

        // StaminaCosts
        public ReadOnlyCollection<AtlasImage> StaminaCosts { get; }

        // StaminaIcon
        public AtlasImage StaminaIcon { get; }

        // UIButtonContainerEdge
        public AtlasImage UIButtonContainerEdge { get; }

        // UIButtonContainerPattern
        public AtlasImage UIButtonContainerPattern { get; }
    }
}