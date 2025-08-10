namespace Adberration
{
    // Direction
    public enum Direction { Left, Up, Right, Down }

    // EntityKind
    public enum EntityKind { Static, Dynamic, DynamicRuntime, Anonymous }

    // FacingDirection
    public enum FacingDirection { Right, Left }

    // LifetimeScope
    public enum LifetimeScope { Session, Room }

    // GameSessionPersistenceAttributeName
    public enum GameSessionPersistenceAttributeName { AllowSaving, Chapter, Counters, Difficulty, LastSaved, MusicTag, MusicTagRoomScope, MusicSoundName, PlayTime, PreviousRoom, Progress, Room, Flags, Demo, Version }

    // GameSessionState
    public enum GameSessionState { Uninitialized, Idle, LoadingScripts, Loading, AwaitingScripts }

    // LocalizationSource
    public enum LocalizationSource { Script, TextRepository, TextRepositoryOtherwiseScript }

    // MusicTagScope
    public enum MusicTagScope { Session, Script, Room }

    // OperationMode
    public enum OperationMode { Manual, Auto }

    // PersistentTypeScope
    public enum PersistentTypeScope { InheritedAndDeclared, DeclaredOnly }
}
