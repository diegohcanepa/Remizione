using System;

namespace Remizione
{
    // ActorDirection
    public enum ActorDirection { Down, Up }

    // ActorSize
    public enum ActorSize { Small, Medium, Large }

    // Affinity
    public enum Affinity { Good, Evil }

    // AIStateName
    public enum AIStateName { Attack, Charge, Chase, CloseAttack, Decide, Idle, Move, Patrol, RangeAttack }

    // ContentFolder
    public enum ContentFolder { Ambience, Atlases, Effects, Music, Fonts, FX, Videos, Voices, System, Text }

    // DamageIntensity
    public enum DamageIntensity { Base, Critical }

    // DamageKind
    public enum DamageKind { None, Physical, Fire, Cold, Lightning, Acid, Poison }

    // DustParticleKind
    public enum DustParticleKind { None, Dust, Ash }

    // GameplayMode
    public enum GameplayMode { Adventure, Run }

    // HitEffect
    public enum HitEffect { None, Shake, Blink }

    // HitTestSource
    public enum HitTestSource { Hotspot, Collider }

    // HUDMessageKind
    public enum HUDMessageKind { CannotPlaceItem, EnoughOfThat, InventoryFull, PowerRestored }

    // ImpactType
    public enum ImpactType { Low, Medium, High }

    // ImpactWordName
    public enum ImpactWordName { None, AghGreen, AghRed, BoomPurple, BoomRed, BangBlue, BangRed, CrackBlue, CrackYellow, CuackPurple, CuackYellow, Kapow, KapowStrong, OuchBlue, OuchGreen, PlopRed, PlopYellow, SlapBlue, SlapRed, Zap }

    // InventoryCategory
    public enum InventoryCategory { None, Consumables, Junk, KeyItems, Traits, Trinkets }

    // InventoryVerb
    public enum InventoryVerb { Equip, Unequip }

    // ItemAction
    public enum ItemAction { None, Throw }

    // ItemProperty
    public enum ItemProperty { Durability, Health, SkillChance }

    // LightKind
    public enum LightKind { Default, Lantern, Fire, Fireplace, Global, MuzzleFlash, Outdoor, Player }

    // LightState
    public enum LightState { Off, On, TurningOn, TurningOff }

    // LogVerb
    public enum LogVerb { Lost, PickedUp }

    // MenuItemName
    public enum MenuItemName { Options, Resume, Start, Exit, ExitToMainMenu, Yes, No, Cancel, Accept, Credits, WishlistNow, Memoirs }

    // Message
    public enum Message { None }

    // MessageBoxOptions
    [Flags]
    public enum MessageBoxOptions
    {
        Yes = 1,
        No = 2,
        Cancel = 4,
        Accept = 8
    }

    // PlacementDistributionStrategy
    public enum PlacementDistributionStrategy { Random, Clump, NoiseMap }

    // PlacementPhase
    public enum PlacementPhase { None, Terrain, NonSolidDecoration, Tower, NaturalObject, ArtificialObject, Creature }

    // PlayerNumber
    public enum PlayerNumber { None = -1, One = 0, Two = 1, Three = 2, Four = 3 }

    // PropState
    public enum PropState { None, Closed, Open, Locked, Empty }

    // RainDropImpactKind
    public enum RainDropImpactKind { None, Ground, Water }

    // RenderLayer
    public enum RenderLayer { BehindBackground, Background, Doors, Default, Foreground, ForegroundNoLight }

    // SpeechBubbleState
    public enum SpeechBubbleState { Hidden, Typing, Idle }

    // ThrowableBounceIntensity
    public enum ThrowableBounceIntensity { Low, Medium, High }

    // UIControlGroupLayoutStyle
    public enum UIControlGroupLayoutStyle { Vertically, Horizontally }

    // VolumeCategory
    public enum VolumeCategory { Ambient, FX, Music, Voice, Master }
}
