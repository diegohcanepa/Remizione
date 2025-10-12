using System;

namespace Remizione
{
    // ActorDirection
    public enum ActorDirection { Down, Up }

    // ActorSize
    public enum ActorSize { Small, Medium, Large }

    // AIStateName
    public enum AIStateName { Attack, Charge, Chase, CloseAttack, Decide, Idle, Move, Patrol, RangeAttack }

    // ContentFolder
    public enum ContentFolder { Ambience, Atlases, Effects, Music, Fonts, FX, Video, Voices, System, Text }

    // DamageType
    public enum DamageType { Physical, Acid, Explosive, Fire, Ice, Lightning, Poison }

    // DustParticleKind
    public enum DustParticleKind { None, Dust, Ash }

    // Faction
    public enum Faction { Neutral, Good, Evil }

    // GameplayMode
    public enum GameplayMode { Adventure, Run }

    // GridMeasureType
    public enum GridMeasureType { BoundingBox, Collider, Hotspot }

    // HitEffect
    public enum HitEffect { None, Shake, Blink }

    // HUDMessageKind
    public enum HUDMessageKind { CannotPlaceItem, EnoughOfThat, ExtraTime, InventoryFull, MagneticCardRequired }

    // ImpactType
    public enum ImpactType { Low, Medium, High }

    // ImpactWordName
    public enum ImpactWordName { None, AghGreen, AghRed, BoomPurple, BoomRed, BangBlue, BangRed, CrackBlue, CrackYellow, CuackPurple, CuackYellow, Kapow, KapowStrong, OuchBlue, OuchGreen, PlopRed, PlopYellow, SlapBlue, SlapRed, Zap }

    // InventoryCategory
    public enum InventoryCategory { None, Consumables, Junk, KeyItems, Quirks, Thingies, Trinkets }

    // InventoryVerb
    public enum InventoryVerb { Equip, Unequip }

    // ItemAction
    public enum ItemAction { None, Place, Throw }

    // ItemProperty
    public enum ItemProperty { Chance, Durability, Health }

    // LightKind
    public enum LightKind { Default, Lantern, Fire, Fireplace, Global, MuzzleFlash, Outdoor, Player }

    // LightState
    public enum LightState { Off, On, TurningOn, TurningOff }

    // LogVerb
    public enum LogVerb { Lost, PickedUp, ItemRequired }

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
    public enum PlacementPhase { None, Terrain, Building, Treasure, NaturalObject, ArtificialObject, Creature }

    // PlayerNumber
    public enum PlayerNumber { None = -1, One = 0, Two = 1, Three = 2, Four = 3 }

    // PropState
    public enum PropState { None, Closed, Empty, Open, Locked, Unlocked }

    // RainDropImpactKind
    public enum RainDropImpactKind { None, Ground, Water }

    // RenderLayer
    public enum RenderLayer { BehindBackground, Background, OverBackground, Default, Foreground, ForegroundNoLight }

    // RoomKind
    public enum RoomKind { RideRoom }

    // RoomSize
    public enum RoomSize { Small, Medium }

    // RunPhase
    public enum RunPhase { None, Start, Mid, End }

    // SpeechBubbleState
    public enum SpeechBubbleState { Hidden, Typing, Idle }

    // TestPolygon
    public enum TestPolygon { Hotspot, Collider }

    // ThrowableBounceIntensity
    public enum ThrowableBounceIntensity { Low, Medium, High }

    // UIControlGroupLayoutStyle
    public enum UIControlGroupLayoutStyle { Vertically, Horizontally }

    // VolumeCategory
    public enum VolumeCategory { Ambient, FX, Music, Voice, Master }
}
