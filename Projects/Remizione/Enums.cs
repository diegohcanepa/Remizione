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

    // ControlImageSource
    public enum ControlImageSource { Default, InputBindingName, ImageName }

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

    // ItemAction
    public enum ItemAction { None, Examine, Upgrade }

    // ItemCategory
    public enum ItemCategory { Skills, KeyItems, Consumables, Throwables, Spells, Amulets }

    // ItemEffectTiming
    public enum ItemEffectTiming { OnBeginUse, OnEndUse, AfterAllEffects }

    // ItemName
    public enum ItemName { None, UnarmedAttack, Cross, Lockpick, Stamina, Health, ZabulContact }

    // ItemStorageCategory
    public enum ItemStorageCategory { Inventory, Skills }

    // LightKind
    public enum LightKind { Default, Lantern, Fire, Fireplace, LightBulb, Moon, MuzzleFlash, Outdoor, Lightning }

    // LightState
    public enum LightState { Off, On, TurningOn, TurningOff }

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

    // MessageKey
    public enum MessageKey { None, Fatigue, NoStamina }

    // MouseCursorState
    public enum MouseCursorState { Cross, CrossOn, Target, TargetOn, Wait, Arrow }

    // PlacementDistributionStrategy
    public enum PlacementDistributionStrategy { Random, Clump, NoiseMap }

    // PlacementPhase
    public enum PlacementPhase { None, Terrain, NonSolidDecoration, NaturalObject, ArtificialObject, Actor }

    // PlatformMessageKey
    public enum PlatformMessageKey { ChangeUser, ControllerDisconnected, PressAnyButton, PressAnyKeyOrButton, SignIn }

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
    public enum Stat { Strength, Constitution, Dexterity, Devotion, Empathy, Mind }

    // TerrainKind
    public enum TerrainKind { None }

    // TextSize
    public enum TextSize { Small, Medium, Large }

    // ThrowableBounceIntensity
    public enum ThrowableBounceIntensity { Low, Medium, High }

    // UIControlSize
    public enum UIControlSize { Large, Small }

    // UIControlDisplayMode
    public enum UIControlDisplayMode { ImageAndText, ImageOnly }

    // UIControlGroupLayoutStyle
    public enum UIControlGroupLayoutStyle { Vertically, Horizontally }

    // Verb
    public enum Verb { Examine, Insult, Talk, Trade }

    // VolumeCategory
    public enum VolumeCategory { Ambient, FX, Music, Voice, Master }

    // WorldBlockTag
    public enum WorldBlockTag { Dark }
}
