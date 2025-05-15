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
    public enum CombatStateSignal { Attack, CloseAttack, Decide, EndTurn, Fatigue, Move }

    // CombatStateName
    public enum CombatStateName { Charge, CloseAttack, Decide, Fatigue, Move }

    // CombatTurnState
    public enum CombatTurnState { None, Waiting, WaitingInput, Busy }

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
    public enum InGameMenuOptionName { Attributes, Creatures, Inventory, Manifestations, Map, Prayers, QuitToDesktop, Settings }

    // ItemAction
    public enum ItemAction { None, Discard, Use, Wield }

    // ItemCategory
    public enum ItemCategory { Skill, Consumable, Spell, Throwable, Weapon }

    // ItemContainerCategory
    public enum ItemContainerCategory { Inventory, Skills }

    // ItemProperty
    public enum ItemProperty { BaseDamage }

    // LightKind
    public enum LightKind { Default, Lantern, Fire, Fireplace, LightBulb, Moon, MuzzleFlash, Outdoor, Lightning }

    // LightState
    public enum LightState { Off, On, TurningOn, TurningOff }

    // LogMessage
    public enum LogMessage { EnoughOfThat, InventoryFull }

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
    public enum MouseCursorState { Bag, Cross, CrossOn, Target, TargetOn, Wait, Arrow }

    // PlacementDistributionStrategy
    public enum PlacementDistributionStrategy { Random, Clump, NoiseMap }

    // PlacementPhase
    public enum PlacementPhase { None, Terrain, NonSolidDecoration, NaturalObject, ArtificialObject, Actor }

    // PlayerNumber
    public enum PlayerNumber { None = -1, One = 0, Two = 1, Three = 2, Four = 3 }

    // QTEResult
    public enum QTEResult { Failure, Success }

    // RenderLayer
    public enum RenderLayer { BehindBackground, Background, Doors, Default, Foreground, ForegroundNoLight }

    // RoomAreaKind
    public enum RoomAreaKind { Trigger, Walk }

    // RoomSampler
    public enum RoomSampler { PointClamp, LinearClamp }

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

    // UpgradeHardness
    public enum UpgradeHardness { Easy, Normal, Hard }

    // Verb
    public enum Verb { Examine, Insult, Talk, Trade }

    // VolumeCategory
    public enum VolumeCategory { Ambient, FX, Music, Voice, Master }

    // WorldBlockTag
    public enum WorldBlockTag { Dark }
}
