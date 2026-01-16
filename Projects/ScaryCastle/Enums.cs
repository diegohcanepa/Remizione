using System;

namespace ScaryCastle
{
    // ActorDirection
    public enum ActorDirection { Down, Up }

    // ActorSize
    public enum ActorSize { Small, Medium, Large }

    // AIStateName
    public enum AIStateName { Attack, Charge, Chase, CloseAttack, Decide, Idle, Move, Patrol, RangeAttack }

    // ContentFolder
    public enum ContentFolder { Ambience, Atlases, Effects, Music, Fonts, FX, Video, Voices, System, Text }

    // ConsumptionType
    public enum ConsumptionType { Quantity, Durability, None }

    // DamageType
    public enum DamageType { None, Physical, Acid, Explosive, Fire, Ice, Lightning, Poison }

    // Difficulty
    public enum Difficulty { Easy, Normal, Hard }

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

    // ImpactType
    public enum ImpactType { Low, Medium, High }

    // ImpactWordName
    public enum ImpactWordName { None, AghGreen, AghRed, BoomPurple, BoomRed, BangBlue, BangRed, CrackBlue, CrackYellow, CuackPurple, CuackYellow, Kapow, KapowStrong, OuchBlue, OuchGreen, PlopRed, PlopYellow, SlapBlue, SlapRed, Zap }

    // ItemCategory
    public enum ItemCategory { Explosive, Food, Luck, Medicine, Misc }

    // ItemProperty
    public enum ItemProperty { Chance, Durability, Health }

    // LightKind
    public enum LightKind { Default, Lantern, Fire, Fireplace, Global, MuzzleFlash, Outdoor, Player }

    // LightState
    public enum LightState { Off, On, TurningOn, TurningOff }

    // LockType
    public enum LockType { None, Padlock }

    // LogVerb
    public enum LogVerb { Bought, Found, Requires, Used }

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

    // MessageKind
    public enum MessageKind { CannotPlaceItem, NotEnoughCoins }

    // MouseCursorState
    public enum MouseCursorState { None, Arrow, Cross, CrossOn, Wait }

    // PlaceholderTarget
    public enum PlaceholderTarget { Prop, Enemy, Any }

    // PlacementType
    public enum PlacementType
    {
        Floor,          // Suelo libre (lejos de paredes)
        WallFrontBase,  // Apoyado contra la pared de arriba (Vending Machine)
        WallFrontHang,  // Colgado en la pared de arriba (Cuadros, Antorchas)
        WallLeftBase,   // Apoyado contra la pared izquierda
        WallLeftHang,   // Colgado en la pared izquierda
        WallRightBase,  // Apoyado contra la pared derecha
        WallRightHang,  // Colgado en la pared derecha
        Ceiling,        // Techo (lámparas, telarañas)
        WalkArea        // Cualquier lugar del suelo navegable
    }

    // PlayerNumber
    public enum PlayerNumber { None = -1, One = 0, Two = 1, Three = 2, Four = 3 }

    // PropState
    public enum PropState { None, Closed, Empty, TurnedOff, Open, Locked, Unlocked }

    // Realm
    public enum Realm { Earthly, Infernal, Celestial }

    // RenderLayer
    public enum RenderLayer { BehindBackground, Background, OverBackground, Default, Foreground, ForegroundNoLight }

    // RideDoorDirection
    public enum RideDoorDirection { Up, Right, Down, Left }

    // RoomType
    public enum RoomType { Connector, Start, Exit }

    // SpeechBubbleState
    public enum SpeechBubbleState { Hidden, Typing, Idle }

    // TestPolygon
    public enum TestPolygon { Hotspot, Collider }

    // ThrowableBounceIntensity
    public enum ThrowableBounceIntensity { Low, Medium, High }

    // UIControlGroupLayoutStyle
    public enum UIControlGroupLayoutStyle { Vertically, Horizontally }

    // Verb
    public enum Verb { Enter, Exit, Use }

    // VolumeCategory
    public enum VolumeCategory { Ambient, FX, Music, Voice, Master }
}
