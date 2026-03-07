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
        InFront         // Justo encima (para items o puertas)
    }

    // BodySize
    public enum BodySize { Small, Medium, Large }

    // CombatBehaviorArchetype
    public enum CombatBehaviorArchetype { Tactical, Berserk, Coward, Sniper, Swarmer }

    // CombatIntentCategory
    public enum CombatIntentCategory { Basic, Special }

    // ContentFolder
    public enum ContentFolder { Ambience, Atlases, Effects, Music, Fonts, FX, Video, Voices, System, Text }

    // ConsumptionType
    public enum ConsumptionType { Quantity, Durability, None }

    // DamageType
    public enum DamageType { Physical, Acid, Explosive, Fire, Ice, Lightning, Poison }

    // Difficulty
    public enum Difficulty { Easy, Normal, Hard }

    // DustParticleKind
    public enum DustParticleKind { None, Dust, Ash }

    // EffectType
    public enum EffectType { None, Damage, Death, Heal, Luck, AddCondition, RemoveCondition }

    // EffectAreaRange
    public enum EffectAreaRange { None, Small, Medium, Large }

    // EffectTarget
    public enum EffectTarget
    {
        Target,
        Self
    }

    // Faction
    public enum Faction { Neutral, Good, Evil }

    // HitEffect
    public enum HitEffect { None, Shake, Blink }

    // ImpactType
    public enum ImpactType { Low, Medium, High }

    // ImpactWordName
    public enum ImpactWordName { None, AghGreen, AghRed, BoomPurple, BoomRed, BangBlue, BangRed, CrackBlue, CrackYellow, CuackPurple, CuackYellow, Kapow, KapowStrong, OuchBlue, OuchGreen, PlopRed, PlopYellow, SlapBlue, SlapRed, Zap }

    // InteractionType
    public enum InteractionType { None, Outcome, UseWithOutcome, Cast }

    // ItemCategory
    public enum ItemCategory { Access, Explosive, Food, Luck, Medicine, Misc, Sacred }

    // ItemVerb
    public enum ItemVerb { None, Eat, Ignite }

    // ItemProperty
    public enum ItemProperty { Chance, Durability, Health }

    // LargHandStyle
    public enum LargHandStyle { God, Devil }

    // LightKind
    public enum LightKind { Default, Lantern, Fire, Fireplace, Global, MuzzleFlash, Outdoor, Player }

    // LightState
    public enum LightState { Off, On, TurningOn, TurningOff }

    // LockType
    public enum LockType { None, GoldenKey }

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
    public enum MessageKind { CannotPlaceItem, InventoryFull, NotEnoughCoins }

    // MouseCursorState
    public enum MouseCursorState { Arrow, Cross, Down, Hand, Left, Prohibition, Right, Up, Wait }

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

    // Realm
    public enum Realm { Earthly, Infernal, Celestial }

    // RenderLayer
    public enum RenderLayer { BehindBackground, Background, OverBackground, Default, Foreground, ForegroundNoLight }

    // RideDoorDirection
    public enum RideDoorDirection { Up, Right, Down, Left }

    // RoomType
    public enum RoomType { Connector, Start, End }

    // SpeechBubbleState
    public enum SpeechBubbleState { Hidden, Typing, Idle }

    // TestPolygon
    public enum TestPolygon { Hotspot, Collider }

    // ThrownObjectType
    public enum ThrownObjectType { None, Bible }

    // UIControlGroupLayoutStyle
    public enum UIControlGroupLayoutStyle { Vertically, Horizontally }

    // Verb
    public enum Verb { Enter, Exit, Use }

    // VolumeCategory
    public enum VolumeCategory { Ambient, FX, Music, Voice, Master }
}
