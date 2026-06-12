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
            CheckMark = this[nameof(CheckMark)];
            Coin = this[nameof(Coin)];
            CoinIcon = this[nameof(CoinIcon)];
            ContextMenuOptionSelector = this[nameof(ContextMenuOptionSelector)];
            CreditsBar = this[nameof(CreditsBar)];
            DialogArrowLarge = this[nameof(DialogArrowLarge)];
            DialogOptionBullet = this[nameof(DialogOptionBullet)];
            DiscardItemIcon = this[nameof(DiscardItemIcon)];
            GooIcon = this[nameof(GooIcon)];
            GooIcons = CreateReadOnlyCollection(nameof(GooIcons), 1, 2);
            GreenHearts = CreateReadOnlyCollection(nameof(GreenHearts), 1, 4);
            InventoryItemAmounts = CreateReadOnlyCollection("InventoryItemAmount", 1, 5);
            InventoryItemSlot = this[nameof(InventoryItemSlot)];
            InventorySkillSlot = this[nameof(InventorySkillSlot)];
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
            SpeechTextArrow = this[nameof(SpeechTextArrow)];
            UIButtonContainerEdge = this[nameof(UIButtonContainerEdge)];
            UIButtonContainerPattern = this[nameof(UIButtonContainerPattern)];
        }

        // AlertIcon
        public AtlasImage AlertIcon { get; }

        // BossMeter
        public AtlasImage BossMeter { get; }

        // BossMeterAmount
        public AtlasImage BossMeterAmount { get; }

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

        // InventoryItemAmounts
        public ReadOnlyCollection<AtlasImage> InventoryItemAmounts { get; }

        // InventoryItemSlot
        public AtlasImage InventoryItemSlot { get; }

        // InventorySkillSlot
        public AtlasImage InventorySkillSlot { get; }

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

        // SpeechTextArrow
        public AtlasImage SpeechTextArrow { get; }

        // UIButtonContainerEdge
        public AtlasImage UIButtonContainerEdge { get; }

        // UIButtonContainerPattern
        public AtlasImage UIButtonContainerPattern { get; }
    }
}