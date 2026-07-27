using System;

namespace ScaryCastle
{
    // ActionKind
    public enum ActionKind { Script, Proximity, InPlace, Projectile, Self }

    // ActorRank
    public enum ActorRank { Common, MiniBoss, Boss }

    // ApproachBehavior
    public enum ApproachBehavior
    {
        FaceToFace,     // Cara a cara (respetando la dirección del NPC)
        ClosestSide,    // Lado más cercano (sin cruzar al NPC)
        InFront,         // Justo encima (para items o puertas)
        Over,
        Behind,
        ApproachPosition,
        None
    }

    // BodySize
    public enum BodySize { Small, Medium, Large }

    // CombatArchetypeName
    public enum CombatArchetypeName { Lurker, Harasser, Stalker, Tactical, Berserk, Coward, KamikazeFlyer, Volatile }

    // CombatDecisionType
    public enum CombatDecisionType { None, Attack, Curse, Charge, LurkMove, MoveNearby, RandomMove }

    // CombatIntentCategory
    public enum CombatIntentCategory { Basic, Special }

    // ComicTextKind
    public enum ComicTextKind { None, AghGreen, AghPurple, AghRed, BoomPurple, BoomRed, BangBlue, BangRed, CrackBlue, CrackYellow, CuackPurple, CuackYellow, Kapow, KapowStrong, OuchBlue, OuchGreen, PlopRed, PlopYellow, SlapBlue, SlapRed, Zap }

    // ConditionType
    public enum ConditionType { None, Curse, Poison, ChromaticAberration, CoinLoss }

    // ContentFolder
    public enum ContentFolder { Ambience, Atlases, Effects, Music, Fonts, FX, Video, Voices, System, Text }

    // DamageType
    public enum DamageType { None, Physical, Acid, Explosive, Fire, Ice, Lightning }

    // DeityHandKind
    public enum DeityHandKind { Devil, God }

    // Difficulty
    public enum Difficulty { Easy, Normal, Hard }

    // DoorDirection
    public enum DoorDirection { Up, Right, Down, Left }

    // DustParticleKind
    public enum DustParticleKind { None, Dust, Ash }

    // EffectContext
    public enum EffectContext { Collect, Contact, Attack, RunModifier, Update, Use, ProjectileHit }

    // EffectType
    public enum EffectType { None, BronzeKey, Coin, Condition, ComicText, Damage, Death, Energy, ExtraEnergy, ExtraHeart, GoldenKey, Heal }

    // EffectTarget
    public enum EffectTarget { Target, Self }

    // Faction
    public enum Faction { Good, Evil }

    // FallbackMovementKind
    public enum FallbackMovementKind
    {
        None,     // Se queda estático aguantando la posición (útil para torretas o jefes pesados)
        Random,   // Cruza el room de forma errática (el comportamiento viejo)
        Lurk      // Merodea agazapado en órbita corta (para las ratas y alimañas)
    }

    // FloatingMessage
    public enum FloatingMessage { Failed, Success }

    // ImpactType
    public enum ImpactType { Low, Medium, High }

    // InPlaceEffectType
    public enum InPlaceEffectType { None, Lightning }

    // ItemBehavior
    public enum ItemBehavior { Common, PocketItem, PlayerAction, StatModifier }

    // ItemCategory
    public enum ItemCategory { Access, Explosive, Food, Luck, Medicine, Misc, Money, Pills, Sacred }

    // KnockbackIntensity
    public enum KnockbackIntensity { Low, Medium, High }

    // LightKind
    public enum LightKind { Default, Lantern, Fire, Fireplace, MuzzleFlash, Outdoor, Player }

    // LightState
    public enum LightState { Off, On, TurningOn, TurningOff }

    // LockType
    public enum LockType { None, BronzeKey, GoldenKey, GateLever }

    // LogVerb
    public enum LogVerb { None, Consumed, Discarded, Obtained, Found, Lost, Requires, Used }

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
    public enum MessageKind { CannotPlaceItem, HandsFull, InventoryFull, ItemDiscarded, LiftNotAllowed, NotEnoughCoins, NotEnoughFaith, OutOfReach, OutOfLine, PathCleared, ExtraEnergy, ExtraHeart }

    // MeterColor
    public enum MeterColor { Green, Purple, White }

    // MouseCursorIcon
    public enum MouseCursorIcon { Cross, Attack, Down, Eye, Hand, Left, Lift, Magnifier, Right, PickUp, Skull, Talk, Up, Wait }

    // PlaceholderState
    public enum PlaceholderState { Pending, GateLever, Used }

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

    // PocketItemType
    public enum PocketItemType { BronzeKey, Coin, GoldenKey }

    // PositioningMode
    public enum PositioningMode
    {
        None,
        Move,
        MoveOnY
    }

    // ProjectileTrajectoryType
    public enum ProjectileTrajectoryType { Linear, Parabolic }

    // Realm
    public enum Realm { Earthly, Infernal, Celestial }

    // RenderLayer
    public enum RenderLayer { BehindBackground, Background, OverBackground, Default, Foreground, ForegroundNoLight }

    // RoomTheme
    public enum RoomTheme { Castle }

    // RoomCategory
    public enum RoomCategory { Start, Boss, Treasure, Store, Special, Secret, Standard }

    // RunModifierScope
    public enum RunModifierScope { Room, Run }

    // SpeechTextState
    public enum SpeechTextState { Hidden, Typing, Idle }

    // StatName
    public enum StatName { HP, XP }

    // Tag
    public enum Tag { Ceiling, Floor, GateLever, Poison, Pottery, Torch, Trap, Trunk, WallDecoration }

    // TestPolygon
    public enum TestPolygon { Hotspot, Collider }

    // TrapState
    public enum TrapState { None, Idle, Warning, Activating, Active, Cooldown, Disabled }

    // UIControlGroupLayoutStyle
    public enum UIControlGroupLayoutStyle { Vertically, Horizontally }

    // Verb
    public enum Verb { Use, Examine, Talk, Attack, Lift, GoLeft, PickUp, GoRight, GoUp, GoDown }

    // VolumeCategory
    public enum VolumeCategory { Ambient, FX, Music, Voice, Master }
}
