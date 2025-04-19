namespace TextRepositoryEditor
{
    // AttributeName
    public enum AttributeName { None, Project, LanguagePackageFolder, LanguagePackage, OutputFolder, Folder, IsExpanded, Text, LocalizableText, SourceCode, EncryptionKey, LCID, Comments, Context, AllowValidation, IsLiteral, LiteralText, Emitter, ValidationScope, PublishVersion, AllowEmpty, ImportResult, LastImportedFile, LastImported }

    // EditingAction
    public enum EditingAction { None, Copy, Cut }

    // NodeImage
    public enum NodeImage { Project, LanguagePackageFolderClosed, LanguagePackageFolderOpen, LanguagePackage, FolderClosed, FolderOpen, Text, TextWithErrors, TextLiteral, ImportsFolderClosed, ImportsFolderOpen }

    // ErrorLevel
    public enum ErrorLevel { Hint, Warning, Error }

    // FilterType
    public enum FilterType { Texts, Names, Comments }

    // ItemMoveDirection
    public enum ItemMoveDirection { Up, Down }

    // ImportResult
    public enum ImportResult { None, Unchanged, New, Updated, NotFound }

    // ImportCultureStructureSynchronization
    public enum ImportCultureStructureSynchronization { Enabled, Disabled }

    // LocalizableTextContentConstraint
    public enum LocalizableTextConstraint { MustContainText, None }
}
