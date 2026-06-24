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
        Over
    }

    // BodySize
    public enum BodySize { Small, Medium, Large }

    // CombatArchetypeName
    public enum CombatArchetypeName { Lurker, Harasser, Stalker, Tactical, Berserk, Coward, KamikazeFlyer, Volatile }

    // CombatDecisionType
    public enum CombatDecisionType { None, Attack, Curse, Charge, Flee, MoveNearby, RandomMove }

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

    // DustParticleKind
    public enum DustParticleKind { None, Dust, Ash }

    // EffectContext
    public enum EffectContext { Collect, Contact, Attack, Update, Use, ProjectileHit }

    // EffectType
    public enum EffectType { None, Coin, Condition, ComicText, Damage, Death, Energy, ExtraEnergy, ExtraHeart, Heal }

    // EffectTarget
    public enum EffectTarget { Target, Self }

    // Faction
    public enum Faction { Good, Evil }

    // FloatingMessage
    public enum FloatingMessage { Failed, Success }

    // GateEventType
    public enum GateEventType { Nothing, Good, Bad }

    // HitEffect
    public enum HitEffect { None, Shake, Blink }

    // ImpactType
    public enum ImpactType { Low, Medium, High }

    // InPlaceEffectType
    public enum InPlaceEffectType { None, Lightning }

    // InteractionKind
    public enum InteractionKind { Use, Examine, Talk, Attack, Lift, GoLeft, GoRight, GoUp, GoDown }

    // ItemBehavior
    public enum ItemBehavior { Common, Currency, PlayerAction, StatModifier }

    // ItemCategory
    public enum ItemCategory { Access, Explosive, Food, Luck, Medicine, Misc, Money, Pills, Sacred }

    // KnockbackIntensity
    public enum KnockbackIntensity { Low, Medium, High }

    // LightKind
    public enum LightKind { Default, Ambient, Alarm, Lantern, Fire, Fireplace, MuzzleFlash, Outdoor, Player }

    // LightState
    public enum LightState { Off, On, TurningOn, TurningOff }

    // LockType
    public enum LockType { None, GoldenKey }

    // LogVerb
    public enum LogVerb { None, Consumed, Obtained, Found, Lost, Requires, Used }

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
    public enum MessageKind { CannotPlaceItem, FullGoo, HandsFull, InventoryFull, ItemDiscarded, LiftNotAllowed, NotEnoughCoins, NotEnoughGoo, OutOfReach, OutOfLine, PathCleared, ExtraEnergy, ExtraHeart }

    // MouseCursorState
    public enum MouseCursorState { Cross, Attack, Down, Examine, Hand, Left, Lift, Right, Skull, Talk, Up, Wait }

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

    // RideDoorDirection
    public enum RideDoorDirection { Up, Right, Down, Left }

    // RoomTheme
    public enum RoomTheme { Castle }

    // RoomCategory
    public enum RoomCategory { Start, Boss, Treasure, Save, Special, Secret, Standard }

    // SpeechTextState
    public enum SpeechTextState { Hidden, Typing, Idle }

    // StatName
    public enum StatName { HP, XP }

    // TestPolygon
    public enum TestPolygon { Hotspot, Collider }

    // UIControlGroupLayoutStyle
    public enum UIControlGroupLayoutStyle { Vertically, Horizontally }

    // Verb
    public enum Verb { None, Attack, Ellipsis, Enter, Exit, Lift, Open, Pull, Take, TalkTo, Throw, Use }

    // VolumeCategory
    public enum VolumeCategory { Ambient, FX, Music, Voice, Master }
}
