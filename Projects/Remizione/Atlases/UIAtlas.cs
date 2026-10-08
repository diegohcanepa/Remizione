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
            AoeHeart = this[nameof(AoeHeart)];
            CheckMark = this[nameof(CheckMark)];
            CloseWindowButton = this[nameof(CloseWindowButton)];
            ContextMenuOptionSelector = this[nameof(ContextMenuOptionSelector)];
            CreditsBar = this[nameof(CreditsBar)];
            DialogArrowLarge = this[nameof(DialogArrowLarge)];
            DialogOptionBullet = this[nameof(DialogOptionBullet)];
            EchoBackground = this[nameof(EchoBackground)];
            ExamineItem = this[nameof(ExamineItem)];
            FakeItem = this[nameof(FakeItem)];
            FleshinessHearts = CreateReadOnlyCollection(nameof(FleshinessHearts), 1, 3);
            InventoryItemSlots = CreateReadOnlyCollection("InventoryItemSlot", 1, 3);
            MiniMapMarker = this[nameof(MiniMapMarker)];
            MouseLeftButtonIcon = this[nameof(MouseLeftButtonIcon)];
            MouseRightButtonIcon = this[nameof(MouseRightButtonIcon)];
            Pixel = this[nameof(Pixel)];
            PopupContainer = this[nameof(PopupContainer)];
            PopupContainerShadow = this[nameof(PopupContainerShadow)];
            QuickInventoryBackground = this[nameof(QuickInventoryBackground)];
            Sack = this[nameof(Sack)];
            SavingIcon = this[nameof(SavingIcon)];
            ShieldHeart = this[nameof(ShieldHeart)];
            SpeechTextArrow = this[nameof(SpeechTextArrow)];
            SpeechTextPipe = this[nameof(SpeechTextPipe)];
            TrajectoryDot = this[nameof(TrajectoryDot)];
            TrajectoryTargetMark = this[nameof(TrajectoryTargetMark)];
            UIButtonContainerEdge = this[nameof(UIButtonContainerEdge)];
            UIButtonContainerPattern = this[nameof(UIButtonContainerPattern)];
        }

        // AoeHeart
        public AtlasImage AoeHeart { get; }

        // CheckMark
        public AtlasImage CheckMark { get; }

        // ContextMenuOptionSelector
        public AtlasImage ContextMenuOptionSelector { get; }

        // CloseWindowButton
        public AtlasImage CloseWindowButton { get; }

        // CreditsBar
        public AtlasImage CreditsBar { get; }

        // DialogArrowLarge
        public AtlasImage DialogArrowLarge { get; }

        // DialogOptionBullet
        public AtlasImage DialogOptionBullet { get; }

        // EchoBackground
        public AtlasImage EchoBackground { get; }

        // ExamineItem
        public AtlasImage ExamineItem { get; }

        // FakeItem
        public AtlasImage FakeItem { get; }

        // FleshinessHearts
        public ReadOnlyCollection<AtlasImage> FleshinessHearts { get; }

        // InventoryItemSlots
        public ReadOnlyCollection<AtlasImage> InventoryItemSlots { get; }

        // MiniMapMarker
        public AtlasImage MiniMapMarker { get; }

        // MouseLeftButtonIcon
        public AtlasImage MouseLeftButtonIcon { get; }

        // MouseRightButtonIcon
        public AtlasImage MouseRightButtonIcon { get; }

        // Pixel
        public AtlasImage Pixel { get; }

        // PopupContainer
        public AtlasImage PopupContainer { get; }

        // PopupContainerShadow
        public AtlasImage PopupContainerShadow { get; }

        // QuickInventoryBackground
        public AtlasImage QuickInventoryBackground { get; }

        // Sack
        public AtlasImage Sack { get; }

        // SavingIcon
        public AtlasImage SavingIcon { get; }

        // ShieldHeart
        public AtlasImage ShieldHeart { get; }

        // SpeechTextArrow
        public AtlasImage SpeechTextArrow { get; }

        // SpeechTextPipe
        public AtlasImage SpeechTextPipe { get; }

        // TrajectoryDot
        public AtlasImage TrajectoryDot { get; }

        // TrajectoryTargetMark
        public AtlasImage TrajectoryTargetMark { get; }

        // UIButtonContainerEdge
        public AtlasImage UIButtonContainerEdge { get; }

        // UIButtonContainerPattern
        public AtlasImage UIButtonContainerPattern { get; }
    }
}