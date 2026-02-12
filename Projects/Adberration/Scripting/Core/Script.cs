using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Adberration.Scripting
{
    /// <summary>
    /// Script
    /// </summary>
    public class Script
    {
        #region Private fields

        private int currentLineIndex = -1;
        private const string headerSeparator = ":";
        private readonly Stack<bool> ifStack = new();
        private readonly List<string> sourceLines = [];
        private readonly List<Statement> statements = [];

        #endregion

        #region Constructor

        // Constructor
        private Script(Session session, string sourceLine)
        {
            this.Session = session;
            this.Name = "RuntimeScript";
            this.Signature = string.Empty;
            sourceLines.Add(sourceLine);
        }

        // Constructor
        internal Script(Session session, List<string> lines)
        {
            if (lines.Count < 3)
            {
                throw new InvalidOperationException("Not enough lines.");
            }

            this.Session = session;

            var line = lines[0];
            sourceLines.Add(line);
            lines.RemoveAt(0);
            ParseHeader(line, out var name, out var signature);

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(signature))
                throw new InvalidOperationException("Invalid script header.");

            this.Name = name;
            this.Signature = signature;

            ParseBody(lines);
        }

        #endregion

        #region Private members

        // CheckEntity
        private void CheckEntity()
        {
            if (string.IsNullOrWhiteSpace(EntityName))
                throw new InvalidOperationException();

            switch (ScriptType)
            {
                // Enter
                case ScriptType.Enter:
                    if (Session.FindEntity<Room>(EntityName) == null)
                        throw new ScriptException(this, sourceLines[0], $"There is no room named '{EntityName}'.");
                    break;

                // Load / Unload
                case ScriptType.Load:
                case ScriptType.Unload:
                    if (Session.FindEntity<Entity>(EntityName) == null)
                        throw new ScriptException(this, sourceLines[0], $"There is no entity named '{EntityName}'.");
                    break;

                // Outcome
                case ScriptType.Outcome:
                    if (Session.FindEntity<Thing>(EntityName) == null)
                        throw new ScriptException(this, sourceLines[0], $"There is no thing named '{EntityName}'.");
                    break;
            }
        }

        // JumpToNextSelectionStatementBlock
        private void JumpToNextSelectionStatementBlock()
        {
            var ifLevel = 0;
            while (true)
            {
                NextLine();

                if (CurrentStatement == null)
                    continue;

                if (CurrentStatement.StatementType == StatementType.If)
                {
                    ifLevel++;
                }
                else if (CurrentStatement.StatementType == StatementType.Else)
                {
                    if (ifLevel == 0)
                        return;
                }

                else if (CurrentStatement.StatementType == StatementType.Endif)
                {
                    if (ifLevel == 0)
                        return;
                    else
                        ifLevel--;
                }
            }
        }

        // NextLine
        private void NextLine()
        {
            if (IsCompleted)
                return;

            currentLineIndex++;
            if (currentLineIndex >= statements.Count)
                currentLineIndex = -1;
        }

        // ParseHeader
        private void ParseHeader(string line, out string name, out string signature)
        {
            var tokens = Tokenize(line);

            // Script type
            if (Enum.TryParse(tokens[0], out ScriptType scriptType))
                this.ScriptType = scriptType;
            else
                ThrowScriptSyntaxError(this, $"Unrecognized token '{tokens[0]}'.");

            // Form header
            signature = tokens[0];
            if (ScriptType is not ScriptType.NewSession)
            {
                if (tokens.Length < 2)
                    ThrowScriptSyntaxError(this, "Missing header name.");
                else
                    signature += headerSeparator + tokens[1];
            }

            // Persistent
            Persistent = tokens.Contains(ScriptSyntax.PersistentKeyword);
            if (Persistent)
            {
                if (scriptType is not ScriptType.Thing and not ScriptType.Room)
                    ThrowScriptSyntaxError(this, signature, $"The {ScriptSyntax.PersistentKeyword} keyword is not supported in this context.");
            }

            // Cloneable
            Cloneable = tokens.Contains(ScriptSyntax.CloneableKeyword);
            if (Cloneable)
            {
                if (scriptType is not ScriptType.Thing)
                    ThrowScriptSyntaxError(this, signature, $"The {ScriptSyntax.CloneableKeyword} keyword is not supported in this context.");
            }

            // Interruptible
            Interruptible = tokens.Contains(ScriptSyntax.InterruptibleKeyword);
            if (Interruptible)
            {
                if (scriptType is not ScriptType.Routine and not ScriptType.Outcome)
                    ThrowScriptSyntaxError(this, signature, $"The {ScriptSyntax.InterruptibleKeyword} keyword is not supported in this context.");
            }

            // Assign Entity Name
            if (HasCapability(ScriptCapability.EntityContext))
            {
                if (ScriptType != ScriptType.Outcome && tokens[1].Contains(ScriptSyntax.ScriptOverloadSeparator))
                    ThrowScriptSyntaxError(this, signature, "Overload operator is out of context.");

                var index = tokens[1].IndexOf(ScriptSyntax.ScriptOverloadSeparator, StringComparison.Ordinal);
                if (index >= 0)
                {
                    EntityName = tokens[1][..index];
                    OverloadName = tokens[1][(index + 1)..];
                }
                else
                {
                    EntityName = tokens[1];
                }
            }

            name = signature.Substring(signature.IndexOf(headerSeparator, StringComparison.Ordinal) + 1);

            // Parse keywords
            if (tokens.Length > 2 && HasCapability(ScriptCapability.EntityDeclaration))
            {
                var parsed = false;

                ClassName = ParseParameterizedToken(tokens[2], ScriptSyntax.ClassKeyword);
                if (ClassName != null)
                {
                    var entityType = Session.ScriptEnvironment.FindEntityType(ClassName);

                    if (entityType == null)
                    {
                        ThrowScriptSyntaxError(this, $"The entity '{ClassName}' is not registered in code.");
                    }
                    else if (ScriptSyntax.IsDeclarationReservedWord(ClassName))
                    {
                        ThrowScriptSyntaxError(this, $"'{ClassName}' cannot be used because it is a reserved word.");
                    }
                    else if (ScriptType == ScriptType.Room && !typeof(Room).IsAssignableFrom(entityType))
                    {
                        ThrowScriptSyntaxError(this, $"The specified class in {EntityName} must be a Room descendant type.");
                    }
                    else if (ScriptType == ScriptType.Thing && !typeof(Thing).IsAssignableFrom(entityType))
                    {
                        ThrowScriptSyntaxError(this, $"The specified class in {EntityName} must be a Thing descendant type.");
                    }

                    parsed = true;
                }

                if (!parsed)
                {
                    throw new ScriptException($"Unrecognized token '{tokens[2]}'.");
                }
            }
        }

        // ParseBody
        private void ParseBody(List<string> lines)
        {
            var isBodyParsed = false;

            while (lines.Count > 0)
            {
                // Get line
                var line = lines[0];
                lines.RemoveAt(0);

                // Body opened
                if (!isBodyParsed)
                {
                    if (line != ScriptSyntax.CodeBlockStart)
                        throw new InvalidOperationException("Start of code block expected.");

                    isBodyParsed = true;
                    continue;
                }

                // Body closed
                if (line == ScriptSyntax.CodeBlockEnd)
                    break;

                sourceLines.Add(line);

                if (line.StartsWith(ScriptSyntax.CloneKeyword + " ", StringComparison.Ordinal) && lines[0] == ScriptSyntax.CodeBlockStart)
                {
                    var tokens = Tokenize(line);
                    if (tokens.Length < 2)
                        throw new InvalidOperationException("Clone name expected.");

                    ParseInitBody(lines, tokens[1]);
                }
            }
        }

        // ParseInitBody
        private void ParseInitBody(List<string> lines, string newEntityName)
        {
            var isBodyParsed = false;

            while (lines.Count > 0)
            {
                // Get line
                var line = lines[0];

                if (line.StartsWith(ScriptSyntax.CloneKeyword + " ", StringComparison.Ordinal))
                    throw new InvalidOperationException($"The '{ScriptSyntax.CloneKeyword}' keyword is not valid in this context.");

                lines.RemoveAt(0);

                // Body opened
                if (!isBodyParsed)
                {
                    if (line != ScriptSyntax.CodeBlockStart)
                        throw new InvalidOperationException("Start of code block expected.");

                    isBodyParsed = true;
                    continue;
                }

                // Body closed
                if (line == ScriptSyntax.CodeBlockEnd)
                    break;

                var referenceOp = line.EndsWith("()", StringComparison.Ordinal) ? ScriptSyntax.MethodReference : ScriptSyntax.ProperyReference;
                line = $"{referenceOp} {newEntityName}.{line}";
                sourceLines.Add(line);
            }
        }

        // ParseParameterizedToken
        private static string? ParseParameterizedToken(string token, string expectedToken)
        {
            var values = token.Split(':');
            return values.Length < 2 || values[0] != expectedToken ? null : values[1].Trim();
        }

        // ProcessCommand
        private bool ProcessCommand(Command command, GameTime gameTime)
        {
            if (!command.IsExecuted)
            {
                if (command.CanBeginExecution())
                    command.Execute();
                else
                    return false;
            }

            if (command is AwaitableCommand awaitableCommand && awaitableCommand.ShouldAwait && awaitableCommand.IsAwaiting)
            {
                awaitableCommand.Update(gameTime);
                return false;
            }
            else
            {
                command.Done();
                NextLine();
                return true;
            }
        }

        // ProcessElseStatement
        private void ProcessElseStatement()
        {
            if (ifStack.Peek())
                JumpToNextSelectionStatementBlock();
            else
                NextLine();
        }

        // ProcessEndifStatement
        private void ProcessEndifStatement()
        {
            ifStack.Pop();
            NextLine();
        }

        // ProcessSelectionStatement
        private void ProcessSelectionStatement(SelectionStatement statement)
        {
            var result = statement.Evaluate();
            ifStack.Push(result);

            if (!result)
                JumpToNextSelectionStatementBlock();
            else
                NextLine();
        }

        // ThrowScriptSyntaxError
        private static void ThrowScriptSyntaxError(Script script, string errorMessage)
        {
            ThrowScriptSyntaxError(script, script.Signature, errorMessage);
        }

        // ThrowScriptSyntaxError
        private static void ThrowScriptSyntaxError(Script script, string signature, string errorMessage)
        {
            throw new InvalidOperationException($"Script syntax error ({signature}): {errorMessage}");
        }

        // Tokenize
        private static string[] Tokenize(string line)
        {
            var parts = Regex.Matches(line, @"#\S+:""?[^""\s]+""?|[^\s""]+|""[^""]*""");

            var result = new string[parts.Count];

            for (var i = 0; i < parts.Count; i++)
            {
                result[i] = parts[i].Value;
            }

            return result;
        }

        // ValidateConditionalStatements
        private void ValidateConditionalStatements()
        {
            var ifCount = 0;
            var elseCount = 0;
            var endifCount = 0;

            for (var i = 0; i < statements.Count; i++)
            {
                if (statements[i].StatementType == StatementType.If)
                    ifCount++;
                else if (statements[i].StatementType == StatementType.Else)
                    elseCount++;

                // Else before if
                if (elseCount > ifCount)
                    ThrowScriptSyntaxError(this, $"The 'else' block must be preceded by an 'if' block");

                // Endif before if
                if (statements[i].StatementType == StatementType.Endif)
                {
                    endifCount++;
                    if (endifCount > ifCount)
                        ThrowScriptSyntaxError(this, $"The 'endif' must close a preceding 'if'");
                }
            }

            if (ifCount != endifCount)
                ThrowScriptSyntaxError(this, "Missing endif.");
            else if (elseCount > ifCount)
                ThrowScriptSyntaxError(this, "Missing if.");
        }

        #endregion

        #region Internal members

        // Compile
        internal void Compile()
        {
            if (IsCompiled)
                return;

            if (HasCapability(ScriptCapability.EntityContext))
                CheckEntity();

            statements.Clear();

            // Parse code block statements
            for (var i = 1; i < sourceLines.Count; i++)
            {
                var statement = CreateStatement(Session.ScriptEnvironment, this, sourceLines[i]);
                statements.Add(statement);
            }

            ValidateConditionalStatements();

            IsCompiled = true;

            PrepareForExecution();
        }

        // CreateEntity
        internal Entity CreateEntity()
        {
            if (string.IsNullOrWhiteSpace(ClassName))// || string.IsNullOrWhiteSpace(EntityName))
                throw new InvalidOperationException();

            var entityName = EntityName;
            var className = ScriptSyntax.GetDeclaredName(ClassName);

            var result = Session.ScriptEnvironment.CreateEntity(className, entityName) ?? throw new InvalidOperationException("Cannot create entity.");

            return result;
        }

        // CreateStatement
        internal static Statement CreateStatement(ScriptEnvironment scriptEnvironment, Script script, string sourceLine)
        {
            var tokens = Tokenize(sourceLine);
            StatementBody args = new(scriptEnvironment, tokens);
            return scriptEnvironment.CreateStatement(args.Owner, script, sourceLine, args);
        }

        // Discard
        internal void Discard()
        {
            for (var i = 0; i < statements.Count; i++)
            {
                if (statements[i] is Command command)
                    command.Done();
            }

            IsDiscarded = true;
        }

        // EncodeScriptName
        internal static string EncodeScriptName(ScriptType scriptType, string name)
        {
            return $"{scriptType}{headerSeparator}{name}";
        }

        // IsCompleted
        internal bool IsCompleted => currentLineIndex == -1;

        // IsDiscarded
        internal bool IsDiscarded { get; private set; }

        // PrepareForExecution
        internal void PrepareForExecution()
        {
            IsDiscarded = false;
            IsPaused = false;
            currentLineIndex = statements.Count > 0 ? 0 : -1;
        }

        // SetTargetEntity
        internal void SetTargetEntity(string entityName)
        {
            CodeContract.NotEmpty(entityName, nameof(entityName));

            if (!HasCapability(ScriptCapability.SetTargetEntity))
                throw new InvalidOperationException();

            this.EntityName = entityName;

            if (HasCapability(ScriptCapability.EntityDeclaration))
                IsCompiled = false;

            PrepareForExecution();
        }

        // Update
        internal void Update(GameTime gameTime)
        {
            // All commands have been executed
            if (!IsCompiled || IsCompleted || IsPaused || CurrentStatement == null)
                return;

            while (true)
            {
                switch (CurrentStatement.StatementType)
                {
                    // Command
                    case StatementType.AwaitableCommand:
                    case StatementType.NonAwaitableCommmand:
                        if (CurrentStatement is Command command && !ProcessCommand(command, gameTime))
                            return;
                        break;

                    // If
                    case StatementType.If:
                        if (CurrentStatement is SelectionStatement selectionStatement)
                            ProcessSelectionStatement(selectionStatement);
                        break;

                    // Else
                    case StatementType.Else:
                        ProcessElseStatement();
                        break;

                    // Endif
                    case StatementType.Endif:
                        ProcessEndifStatement();
                        break;

                    // Restart
                    case StatementType.Restart:
                        PrepareForExecution();
                        break;

                    // Return
                    case StatementType.Return:
                        currentLineIndex = -1;
                        break;
                }

                if (IsCompleted)
                    break;
            }
        }

        #endregion

        // ClassName
        public string? ClassName { get; private set; }

        // Cloneable
        public bool Cloneable { get; private set; }

        // CurrentStatement
        public Statement? CurrentStatement => currentLineIndex < 0 ? null : statements[currentLineIndex];

        // EntityName
        public string EntityName { get; private set; } = string.Empty;

        // HasCapability
        public bool HasCapability(ScriptCapability value)
        {
            // SetTargetEntity
            if (value == ScriptCapability.SetTargetEntity)
                return ScriptType is ScriptType.Room or ScriptType.Thing or
                       ScriptType.Load or ScriptType.Unload or
                       ScriptType.Enter or ScriptType.Outcome;

            // EntityContext
            if (value == ScriptCapability.EntityContext)
            {
                return ScriptType is not ScriptType.Routine and
                       not ScriptType.NewSession and
                       not ScriptType.Cloning and
                       not ScriptType.Declaration and
                       not ScriptType.Initialization;
            }

            // Awaitable
            if (value == ScriptCapability.Await)
            {
                return ScriptType is ScriptType.Routine or
                       ScriptType.NewSession or
                       ScriptType.Outcome or
                       ScriptType.Enter;
            }

            // Discardable
            if (value == ScriptCapability.Discard)
            {
                return ScriptType == ScriptType.Declaration ||
                       ScriptType == ScriptType.Cloning ||
                       (ScriptType == ScriptType.Thing && !Cloneable);
            }

            // EntityDeclaration
            if (value == ScriptCapability.EntityDeclaration)
                return ScriptType is ScriptType.Thing or ScriptType.Room;

            return false;
        }

        // Interruptible
        public bool Interruptible { get; private set; }

        // IsCompiled
        public bool IsCompiled { get; private set; }

        // IsPaused
        public bool IsPaused { get; internal set; }

        // Name
        public string Name { get; }

        // NextStatement
        public Statement? NextStatement => currentLineIndex + 1 < statements.Count ? statements[currentLineIndex + 1] : null;

        // OverloadName
        public string OverloadName { get; private set; } = string.Empty;

        // Persistent
        public bool Persistent { get; private set; }

        // ScriptType
        public ScriptType ScriptType { get; protected set; }

        // Session
        public Session Session { get; }

        // Signature
        public string Signature { get; }

        // StatementCount
        public int StatementCount => statements.Count;

        // Statements
        public IEnumerable<Statement> Statements => statements;

        // ToString
        public override string ToString()
        {
            return Signature;
        }

        /// <summary>
        /// RuntimeScript
        /// </summary>
        public sealed class RuntimeScript : Script
        {
            // Constructor
            public RuntimeScript(Session session, string sourceLine)
                : base(session, sourceLine)
            {
                ScriptType = ScriptType.Routine;
            }
        }
    }
}
