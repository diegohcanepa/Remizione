using System;

namespace ScaryCastle
{
    // ActorDirection
    public enum ActorDirection { Down, Up }

    // ApproachBehavior
    public enum ApproachBehavior
    {
        FaceToFace,     // Cara a cara (respetando la dirección del NPC)
        ClosestSide,    // Lado más cercano (sin cruzar al NPC)
        InFront,         // Justo encima (para items o puertas)
        Over
    }

    // BodySize
    public enum BodySize { Small, Medium, Large }

    // CombatArchetypeName
    public enum CombatArchetypeName { Lurker, Harasser, Stalker, Tactical, Berserk, Coward }

    // CombatDecisionType
    public enum CombatDecisionType { None, Attack, Flee, Charge, Move }

    // CombatIntentCategory
    public enum CombatIntentCategory { Basic, Special }

    // ComicTextKind
    public enum ComicTextKind { None, AghGreen, AghRed, BoomPurple, BoomRed, BangBlue, BangRed, CrackBlue, CrackYellow, CuackPurple, CuackYellow, Kapow, KapowStrong, OuchBlue, OuchGreen, PlopRed, PlopYellow, SlapBlue, SlapRed, Zap }

    // ConditionType
    public enum ConditionType { None, Curse, Poison, ChromaticAberration, CoinLoss }

    // ContentFolder
    public enum ContentFolder { Ambience, Atlases, Effects, Music, Fonts, FX, Video, Voices, System, Text }

    // ConsumptionType
    public enum ConsumptionType { Quantity, Durability, None }

    // DamageType
    public enum DamageType { None, Physical, Acid, Explosive, Fire, Ice, Lightning }

    // DeityHandKind
    public enum DeityHandKind { Devil, God }

    // Difficulty
    public enum Difficulty { Easy, Normal, Hard }

    // DoorStyle
    public enum DoorStyle { Wooden, Gate }

    // DustParticleKind
    public enum DustParticleKind { None, Dust, Ash }

    // EffectContext
    public enum EffectContext { Contact, Attack, Update, Use }

    // EffectType
    public enum EffectType { None, Condition, Damage, Death, Goo, Heal }

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

    // Intensity
    public enum Intensity { Low, Medium, High }

    // InteractionContextMode
    public enum InteractionContextMode { Default, Attack, Lift }

    // InteractionType
    public enum InteractionType { None, Outcome, UseWithOutcome, Cast, Attack, Lift }

    // ItemCategory
    public enum ItemCategory { Access, Explosive, Food, Luck, Medicine, Misc, Money, Sacred }

    // ItemProperty
    public enum ItemProperty { Chance, Durability, Health }

    // KnockbackIntensity
    public enum KnockbackIntensity { Low, Medium, High }

    // LightKind
    public enum LightKind { Default, Alarm, Lantern, Fire, Fireplace, Global, MuzzleFlash, Outdoor, Player }

    // LightState
    public enum LightState { Off, On, TurningOn, TurningOff }

    // LockType
    public enum LockType { None, GoldenKey }

    // LogVerb
    public enum LogVerb { Bought, Found, Lost, Requires, Used }

    // LootDropMode
    public enum LootDropMode
    {
        Standard,    // Flujo normal (Saco -> Monedas)
        CoinsOnly,   // Solo monedas
        SackOnly,    // Solo sacos
        Custom,      // Tira un ítem específico definido a mano
        None         // Nada de nada
    }

    // LootDropTrigger
    public enum LootDropTrigger
    {
        OnDeath,
        OnImpact
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
    public enum MessageKind { CannotPlaceItem, HandsFull, HurryUp, InventoryFull, ItemDiscarded, LiftNotAllowed, NotEnoughCoins, NotEnoughGoo, OutOfReach, PathCleared, PullCorridorLever }

    // MouseCursorState
    public enum MouseCursorState { Arrow, Cross, Down, Hand, Left, Prohibition, Right, Up, Wait }

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
    public enum RoomTheme { Castle }

    // RoomType
    public enum RoomType { Corridor, SideRoom }

    // SideRoomCategory
    public enum SideRoomCategory { None, Hub, Generic, Save, Treasure }

    // SpawnLocation
    public enum SpawnLocation { CorridorOrSideRoom, Corridor, SideRoom, Gate }

    // SpeechBubbleState
    public enum SpeechBubbleState { Hidden, Typing, Idle }

    // TestPolygon
    public enum TestPolygon { Hotspot, Collider }

    // UIControlGroupLayoutStyle
    public enum UIControlGroupLayoutStyle { Vertically, Horizontally }

    // Verb
    public enum Verb { None, Ellipsis, Enter, Exit, Lift, Open, Pull, Take, TalkTo, Throw, Use }

    // VolumeCategory
    public enum VolumeCategory { Ambient, FX, Music, Voice, Master }
}
