namespace Adberration.Scripting
{
    // CodingContext
    public enum CodingContext { Any, Declaration, EntityDeclaration, Execution, Instantiation, Initialization }

    // ComparisonOperator
    public enum ComparisonOperator { Equality, Inequality, LessThan, LessThanOrEqual, GreaterThan, GreaterThanOrEqual }

    // CompilationPhase
    internal enum CompilationPhase { None, Declarations, Instantiation, Routines, Outcomes }

    // NameValidationError
    public enum NameValidationError { None, Null, Empty, InvalidCharacters, ReservedWord }

    // ScriptCapability
    public enum ScriptCapability
    {
        SetTargetEntity,
        EntityContext,
        Await,
        Discard,
        EntityDeclaration
    }

    // ScriptType
    public enum ScriptType { Declaration, Initialization, Instantiation, Room, Thing, Load, Unload, Enter, EnterRoom, Outcome, Routine, NewSession }

    // StatementType
    public enum StatementType { If, Else, Endif, Restart, Return, AwaitableCommand, NonAwaitableCommmand, LocalizationComment }
}
