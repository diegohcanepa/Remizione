using System;

namespace Remizione
{
    // ActorDirection
    public enum ActorDirection { Down, Up }

    // ActorSize
    public enum ActorSize { Small, Medium, Large }

    // Affinity
    public enum Affinity { Good, Neutral, Evil }

    // AttackRollStat
    public enum AttackRollStat { Strength, Dexterity }

    // CombatStateSignal
    public enum CombatStateSignal { Attack, CloseAttack, Decide, Fatigue, Move }

    // CombatStateName
    public enum CombatStateName { Charge, CloseAttack, Decide, Move }

    // ContentFolder
    public enum ContentFolder { Ambience, Atlases, Effects, Music, Fonts, FX, Videos, Voices, System, Text }

    // DerivedStat
    public enum DerivedStat { HP, Faith, Tickets }

    // DustParticleKind
    public enum DustParticleKind { None, Dust, Ash }

    // GameplayMode
    public enum GameplayMode { Adventure, Survival }

    // HitTestSource
    public enum HitTestSource { Hotspot, Collider }

    // HUDMessageKind
    public enum HUDMessageKind { CannotPlaceItem, EnoughOfThat, InventoryFull, NotEnoughFaith }

    // ImpactType
    public enum ImpactType { Low, Medium, High }

    // ImpactWordKind
    public enum ImpactWordKind { None, Kapow, Zap }

    // InGameMenuOptionName
    public enum InGameMenuOptionName { Attributes, Creatures, Inventory, Gifts, Map, Prayers, SacredWords, QuitToDesktop, Settings }

    // InventoryOption
    public enum InventoryOption { Inspect, Select, Discard }

    // ItemAction
    public enum ItemAction { UseWith, Consume, Throw }

    // ItemProperty
    public enum ItemProperty { BaseDamage, Passive }

    // LightKind
    public enum LightKind { Default, Lantern, Fire, Fireplace, Global, MuzzleFlash, Outdoor, Player }

    // LightState
    public enum LightState { Off, On, TurningOn, TurningOff }

    // LogVerb
    public enum LogVerb { Discarded, Lost, PickedUp, Restored }

    // MenuItemName
    public enum MenuItemName { Options, Resume, Start, Exit, ExitToMainMenu, Yes, No, Cancel, Accept, Credits, WishlistNow, Memoirs }

    // MessageBoxOptions
    [Flags]
    public enum MessageBoxOptions
    {
        Yes = 1,
        No = 2,
        Cancel = 4,
        Accept = 8
    }

    // Message
    public enum Message { None, Critical, Miss, NoFaith, ThreatsNearby }

    // MetaItemCategory
    public enum MetaItemCategory { None, Consumable, Equipment, Misc }

    // MouseCursorState
    public enum MouseCursorState { None, Cross, CrossOn, Wait, Arrow }

    // PlacementDistributionStrategy
    public enum PlacementDistributionStrategy { Random, Clump, NoiseMap }

    // PlacementPhase
    public enum PlacementPhase { None, Terrain, NonSolidDecoration, NaturalObject, ArtificialObject, Actor }

    // PlayerNumber
    public enum PlayerNumber { None = -1, One = 0, Two = 1, Three = 2, Four = 3 }

    // QTEResult
    public enum QTEResult { Failure, Success }

    // RainDropImpactKind
    public enum RainDropImpactKind { None, Ground, Water }

    // RenderLayer
    public enum RenderLayer { BehindBackground, Background, Doors, Default, Foreground, ForegroundNoLight }

    // RoomSampler
    public enum RoomSampler { PointClamp, LinearClamp }

    // SpeechBubbleState
    public enum SpeechBubbleState { Hidden, Typing, Idle }

    // Stat
    public enum Stat { Strength, Dexterity, Fortitude, Devotion, Mind, Charisma }

    // StatModifier
    public enum StatModifier { None, Strength, Dexterity, Fortitude, Devotion, Mind, Charisma }

    // TerrainKind
    public enum TerrainKind { None }

    // TextSize
    public enum TextSize { Small, Medium, Large }

    // ThrowableBounceIntensity
    public enum ThrowableBounceIntensity { Low, Medium, High }

    // UIControlGroupLayoutStyle
    public enum UIControlGroupLayoutStyle { Vertically, Horizontally }

    // UIToolbarButton
    public enum UIToolbarButton { None, Inventory }

    // VolumeCategory
    public enum VolumeCategory { Ambient, FX, Music, Voice, Master }

    // WorldBlockTag
    public enum WorldBlockTag { Dark }
}
