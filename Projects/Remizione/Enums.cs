using System;

namespace Remizione
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

    // ContentFolder
    public enum ContentFolder { Ambience, Atlases, Effects, Music, Fonts, FX, Video, Voice, System, Text }

    // DamageType
    public enum DamageType { None, Physical, Acid, Explosive, Fire, Ice, Lightning, Poison }

    // Difficulty
    public enum Difficulty { Easy, Normal, Hard }

    // DoorDirection
    public enum DoorDirection { Up, Right, Down, Left }

    // DustParticleKind
    public enum DustParticleKind { None, Dust, Ash }

    // EffectContext
    public enum EffectContext { Collect, Contact, Attack, RemainsContact, RunModifier, Update, Use, ProjectileHit, Status, ApplyStatus }

    // EffectType
    public enum EffectType { None, Damage, Death, EnergyGain, EnergyLoss, EnergyRestore, HPGain, HPLoss, HPRestore, MaxEnergyGain, MaxEnergyLoss, MaxHPGain, MaxHPLoss, Status }

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

    // ItemCategory
    public enum ItemCategory { Access, Food, Luck, Medicine, Misc, Sacred }

    // KnockbackIntensity
    public enum KnockbackIntensity { None, Low, Medium, High }

    // LightKind
    public enum LightKind { Default, Lantern, Fire, Fireplace, Global, MuzzleFlash, Outdoor, Player, LootOrb }

    // LightState
    public enum LightState { Off, On, TurningOn, TurningOff }

    // LockType
    public enum LockType { None, BronzeKey, GoldenKey, GateLever, TrapDoorKey }

    // MapNodeState
    public enum MapNodeState { Current, Start, Visited, NotVisited, End };

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
    public enum MessageKind { CannotPlaceItem, GateOpened, HandsFull, InventoryFull, ItemDiscarded, LiftNotAllowed, NotEnoughCoins, OutOfReach, OutOfLine, PathCleared }

    // MeterColor
    public enum MeterColor { Green, Orange, Purple, SkyBlue, White }

    // MouseCursorIcon
    public enum MouseCursorIcon { Cross, Arrow, Attack, Down, Eye, Hand, Left, Lift, Magnifier, Rest, Right, Sack, Skull, Talk, Up, Wait }

    // NameValidationRule
    public enum NameValidationRule
    {
        AllowDuplicates, // No requiere ser único
        Unique,          // Único entre nombres de su mismo tipo
        Strict           // Único estricto (no puede colisionar con categorías, realms ni ids)
    }

    // PlaceholderSpawnRule
    public enum PlaceholderSpawnRule
    {
        /// <summary>
        /// Evaluá tanto el FillChance del placeholder como el ratio individual del prop.
        /// Si el prop falla su tirada, el slot queda vacío orgánicamente.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Solo evalúa el FillChance del placeholder. Si aprueba, FORZA la aparición de un prop
        /// eligiendo uno del pool por ChanceTable (ignora el ratio individual del prop).
        /// </summary>
        PlaceholderChanceOnly = 1,

        /// <summary>
        /// El placeholder ignora su FillChance (slot siempre activo) y la aparición 
        /// depende 100% del ratio individual del prop.
        /// </summary>
        ContentChanceOnly = 2
    }

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

    // PlayerStat
    public enum PlayerStat { Grace, Willpower }

    // PositioningMode
    public enum PositioningMode
    {
        None,
        Move,
        MoveOnY
    }

    // ProjectileTrajectoryType
    public enum ProjectileTrajectoryType { Linear, Parabolic }

    // PuzzleKind
    public enum PuzzleKind { BronzeKey, GateLever }

    // Realm
    public enum Realm { Earthly, Infernal, Celestial }

    // RemainsKind
    public enum RemainsKind { None, Custom, Bones, Guts, ToxicGuts }

    // RenderLayer
    public enum RenderLayer { BehindBackground, Background, OverBackground, Default, Foreground, ForegroundNoLight }

    // RoomTheme
    public enum RoomTheme { Castle }

    // RoomCategory
    public enum RoomCategory { Start, End, Treasure, Store, Special, Secret, Standard }

    // RunModifierScope
    public enum RunModifierScope { Room, Run }

    // RunStage
    public enum RunStage { Start, End }

    // SpawnScope
    public enum SpawnScope
    {
        Anywhere,       // Props universales, antorchas, sangre (entran en End, Store, Standard, etc.)
        StandardOnly,   // Enemigos comunes, trampas estándar (NO entran en End, Store, Treasure)
        RestrictedOnly  // Requiere coincidencia exacta con RequiredRoomCategory (Bosses, NPCs, Pedestales)
    }

    // SpeechTextState
    public enum SpeechTextState { Hidden, Typing, Idle }

    // StatusType
    public enum StatusType { Poison }

    // Tag
    public enum Tag { Ceiling, Floor, GateLever, Grate, Obstacle, Poison, Pottery, Torch, Trap, Trapdoor, Trunk, WallDecoration, Window }

    // TestPolygon
    public enum TestPolygon { Hotspot, Collider }

    // TraitType
    public enum TraitType { Lockpicking, Luck }

    // TrapState
    public enum TrapState { None, Idle, Warning, Activating, Active, Cooldown, Disabled }

    // UIControlGroupLayoutStyle
    public enum UIControlGroupLayoutStyle { Vertically, Horizontally }

    // Verb
    public enum Verb { None, Use, Examine, Rest, Talk, Attack, Lift, GoLeft, GoRight, GoUp, GoDown, PickUp }

    // VolumeCategory
    public enum VolumeCategory { Ambient, FX, Music, Voice, Master }
}
