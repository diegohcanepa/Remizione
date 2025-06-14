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
    public enum CombatStateName { Charge, CloseAttack, Decide, Fatigue, Move }

    // ContentFolder
    public enum ContentFolder { Ambience, Atlases, Effects, Music, Fonts, FX, Videos, Voices, System, Text }

    // Cycle
    public enum Cycle { Indulgence, Penance }

    // DerivedStat
    public enum DerivedStat { Spirit, Faith }

    // DustParticleKind
    public enum DustParticleKind { None, Dust, Ash }

    // HitType
    public enum HitType { Default, Critical, Glancing }

    // ImpactType
    public enum ImpactType { Low, Medium, High }

    // InGameMenuOptionName
    public enum InGameMenuOptionName { Attributes, Creatures, Inventory, Gifts, Map, Prayers, SacredWords, QuitToDesktop, Settings }

    // ItemAction
    public enum ItemAction { None, Create, Discard, Use, Wield }

    // ItemContainerCategory
    public enum ItemContainerCategory
    {
        Inventory,
        Gifts,
        Prayers,
        SacredWords
    }

    // ItemProperty
    public enum ItemProperty { BaseDamage, Passive }

    // LightKind
    public enum LightKind { Default, Lantern, Fire, Fireplace, Moon, MuzzleFlash, Outdoor, Lightning }

    // LightState
    public enum LightState { Off, On, TurningOn, TurningOff }

    // LogMessage
    public enum LogMessage { CannoPlaceItem, EnoughOfThat, InventoryFull, NoInventoryBag } 

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

    // RoomAreaKind
    public enum RoomAreaKind { Trigger, Walk }

    // RoomSampler
    public enum RoomSampler { PointClamp, LinearClamp }

    // SacrificeReward
    public enum SacrificeReward { Faith, Spirit }

    // ShadowSpotSize
    public enum ShadowSpotSize { None, Tiny, Small, Average, Large, Huge, Giant }

    // SpeechBubbleState
    public enum SpeechBubbleState { Hidden, Typing, Idle }

    // Stat
    public enum Stat { Strength, Dexterity, Fortitude, Devotion, Mind, Charisma }

    // TerrainKind
    public enum TerrainKind { None }

    // TextSize
    public enum TextSize { Small, Medium, Large }

    // ThrowableBounceIntensity
    public enum ThrowableBounceIntensity { Low, Medium, High }

    // UIControlDisplayMode
    public enum UIControlDisplayMode { ImageAndText, ImageOnly }

    // UIControlGroupLayoutStyle
    public enum UIControlGroupLayoutStyle { Vertically, Horizontally }

    // UIToolbarButton
    public enum UIToolbarButton { None, Inventory }

    // Verb
    public enum Verb { Examine, Insult, Talk, Trade, UseItem }

    // VolumeCategory
    public enum VolumeCategory { Ambient, FX, Music, Voice, Master }

    // WorldBlockTag
    public enum WorldBlockTag { Dark }
}
