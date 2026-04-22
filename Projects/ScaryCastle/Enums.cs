using System;

namespace ScaryCastle
{
    // ActorDirection
    public enum ActorDirection { Down, Up }

    // ActorRole
    public enum ActorRole { Ambient, Guard, Interactive }

    // ApproachBehavior
    public enum ApproachBehavior
    {
        FaceToFace,     // Cara a cara (respetando la dirección del NPC)
        ClosestSide,    // Lado más cercano (sin cruzar al NPC)
        InFront         // Justo encima (para items o puertas)
    }

    // BodySize
    public enum BodySize { Small, Medium, Large }

    // CombatArchetypeName
    public enum CombatArchetypeName { Lurker, Stalker, Tactical, Berserk, Coward }

    // CombatDecisionType
    public enum CombatDecisionType { None, Attack, Flee, Charge, Move }

    // CombatIntentCategory
    public enum CombatIntentCategory { Basic, Special }

    // ContentFolder
    public enum ContentFolder { Ambience, Atlases, Effects, Music, Fonts, FX, Video, Voices, System, Text }

    // ConsumptionType
    public enum ConsumptionType { Quantity, Durability, None }

    // DamageType
    public enum DamageType { Physical, Acid, Curse, Explosive, Fire, Ice, Lightning, Poison }

    // Difficulty
    public enum Difficulty { Easy, Normal, Hard }

    // DoorStyle
    public enum DoorStyle { Wooden, Gate }

    // DustParticleKind
    public enum DustParticleKind { None, Dust, Ash }

    // EffectContext
    public enum EffectContext { Contact, Attack, Update, Use }

    // EffectType
    public enum EffectType { None, Damage, Death, Heal, Luck }

    // EffectTarget
    public enum EffectTarget
    {
        Target,
        Self
    }

    // Faction
    public enum Faction { Good, Evil }

    // FloatingMessage
    public enum FloatingMessage { Failed, Locked, Success }

    // HitEffect
    public enum HitEffect { None, Shake, Blink }

    // ImpactType
    public enum ImpactType { Low, Medium, High }

    // ImpactWordName
    public enum ImpactWordName { None, AghGreen, AghRed, BoomPurple, BoomRed, BangBlue, BangRed, CrackBlue, CrackYellow, CuackPurple, CuackYellow, Kapow, KapowStrong, OuchBlue, OuchGreen, PlopRed, PlopYellow, SlapBlue, SlapRed, Zap }

    // Intensity
    public enum Intensity { Low, Medium, High }

    // InteractionType
    public enum InteractionType { None, Outcome, UseWithOutcome, Cast, Attack }

    // ItemCategory
    public enum ItemCategory { Access, Explosive, Food, Luck, Medicine, Misc, Sacred }

    // ItemProperty
    public enum ItemProperty { Chance, Durability, Health }

    // LightKind
    public enum LightKind { Default, Lantern, Fire, Fireplace, Global, MuzzleFlash, Outdoor, Player }

    // LightState
    public enum LightState { Off, On, TurningOn, TurningOff }

    // LockType
    public enum LockType { None, GoldenKey }

    // LogVerb
    public enum LogVerb { Bought, Found, Requires, Used }

    // LootDropMode
    public enum LootDropMode
    {
        Standard,    // Flujo normal (Saco -> Monedas)
        CoinsOnly,   // Solo monedas
        SackOnly,    // Solo sacos
        Custom,      // Tira un ítem específico definido a mano
        None         // Nada de nada
    }

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
    public enum MessageKind { CannotPlaceItem, Cursed, HandsFull, InventoryFull, NotEnoughCoins, NotEnoughFaith, OutOfReach, PathCleared, Poisoned, SacrificeDone }

    // MouseCursorState
    public enum MouseCursorState { Arrow, Attack, Cross, Down, Hand, Left, Prohibition, Right, Up, Wait }

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
        Ceiling         // Techo (lámparas, telarañas)
    }

    // PlayerNumber
    public enum PlayerNumber { None = -1, One = 0, Two = 1, Three = 2, Four = 3 }

    // Realm
    public enum Realm { Earthly, Infernal, Celestial }

    // RenderLayer
    public enum RenderLayer { BehindBackground, Background, OverBackground, Default, Foreground, ForegroundNoLight }

    // RideDoorDirection
    public enum RideDoorDirection { Up, Right, Down, Left }

    // RoomTheme
    public enum RoomTheme { BlueStone }

    // RoomType
    public enum RoomType { Corridor, SideRoom }

    // SideRoomCategory
    public enum SideRoomCategory { None, Hub, Generic, Save, Treasure }

    // SpeechBubbleState
    public enum SpeechBubbleState { Hidden, Typing, Idle }

    // StatusEffect
    public enum StatusEffect { None, Cursed, Poisoned }

    // TestPolygon
    public enum TestPolygon { Hotspot, Collider }

    // UIControlGroupLayoutStyle
    public enum UIControlGroupLayoutStyle { Vertically, Horizontally }

    // Verb
    public enum Verb { None, Ellipsis, Enter, Exit, Lift, Open, Pull, Take, TalkTo, Use }

    // VolumeCategory
    public enum VolumeCategory { Ambient, FX, Music, Voice, Master }
}
