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
    public enum DamageType { None, Physical, Acid, Explosive, Fire, Ice, Lightning, Poison }

    // DustParticleKind
    public enum DustParticleKind { None, Dust, Ash }

    // Faction
    public enum Faction { Neutral, Good, Evil }

    // GameplayMode
    public enum GameplayMode { Adventure, Action }

    // GridMeasureType
    public enum GridMeasureType { BoundingBox, Collider, Hotspot }

    // HitEffect
    public enum HitEffect { None, Shake, Blink }

    // HUDMessageKind
    public enum HUDMessageKind { CannotPlaceItem, EnoughOfThat, ExtraTime, InventoryFull, CoinRequired }

    // ImpactType
    public enum ImpactType { Low, Medium, High }

    // ImpactWordName
    public enum ImpactWordName { None, AghGreen, AghRed, BoomPurple, BoomRed, BangBlue, BangRed, CrackBlue, CrackYellow, CuackPurple, CuackYellow, Kapow, KapowStrong, OuchBlue, OuchGreen, PlopRed, PlopYellow, SlapBlue, SlapRed, Zap }

    // InventoryVerb
    public enum InventoryVerb { TakeOff, Equip }

    // ItemAction
    public enum ItemAction { None, Place, Throw }

    // ItemCategory
    public enum ItemCategory { None, Consumables, Junk, KeyItems, Gadgets, Trinkets }

    // ItemProperty
    public enum ItemProperty { Chance, Durability, Health }

    // ItemRealm
    public enum ItemRealm { Earthly, Infernal, Celestial }

    // LightKind
    public enum LightKind { Default, Lantern, Fire, Fireplace, Global, MuzzleFlash, Outdoor, Player }

    // LightState
    public enum LightState { Off, On, TurningOn, TurningOff }

    // LogVerb
    public enum LogVerb { Lost, PickedUp, ItemRequired }

    // LootTag
    public enum LootTag { Heal, Weapoon }

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
    public enum PropState { None, Closed, Empty, TurnedOff, Open, Locked, Unlocked }

    // RenderLayer
    public enum RenderLayer { BehindBackground, Background, OverBackground, Default, Foreground, ForegroundNoLight }

    // RoomKind
    public enum RoomKind { Hub, Chamber }

    // RoomSize
    public enum RoomSize { Small, Medium }

    // RunPhase
    public enum RunPhase { None, Start, Mid, End }

    // SpeechBubbleState
    public enum SpeechBubbleState { Hidden, Typing, Idle }

    // StackMode
    public enum StackMode { None, Persistent, Disposable }

    // TestPolygon
    public enum TestPolygon { Hotspot, Collider }

    // ThrowableBounceIntensity
    public enum ThrowableBounceIntensity { Low, Medium, High }

    // UIControlGroupLayoutStyle
    public enum UIControlGroupLayoutStyle { Vertically, Horizontally }

    // VolumeCategory
    public enum VolumeCategory { Ambient, FX, Music, Voice, Master }
}
