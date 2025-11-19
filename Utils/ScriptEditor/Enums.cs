namespace Engendro
{
    // NodeImage
    public enum NodeImage { Project, FolderClosed, FolderOpened }

    // OpenDocumentAction
    public enum OpenDocumentBehavior { None, Select, SelectAndEdit }

    // ScriptType
    public enum ScriptType
    {
        Unknown,
        Declaration,
        Initialization,
        Cloning,
        Room,
        TransientRoom,
        Thing,
        TransientThing,
        Load,
        Unload,
        Enter,
        Outcome,
        Routine,
        EnterRoom,
        NewSession
    }

    // VariableKind
    public enum VariableKind { Counter, Flag }

    // XmlAttributeName
    public enum XmlAttributeName { Project, OutputFileName, Build, Documents, Folder, IsExpanded, IsSelected, IsDocumentOpen, IsDocumentPinned, IsDocumentSelected, Name, Script, SourceCode, EncryptionKey, CaretOffset }
}
