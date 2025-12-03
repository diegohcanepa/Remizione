using Engendro;

namespace Adberration.Scripting
{
    /// <summary>
    /// LocalizableCommand
    /// </summary>
    public abstract class LocalizableCommand : AwaitableCommand
    {
        // Constructor
        protected LocalizableCommand(Script script, string source, StatementBody body, int clauseCount, params string[] argList)
            : base(script, source, body, clauseCount, argList)
        {
            AssertLocalizationId();
        }

        #region Protected members

        // AssertLocalizationId
        protected int AssertLocalizationId()
        {
            var result = 0;

            var value = Body.Args.GetArg(LocalizationIdArg)?.Value;
            if (value != null)
            {
                if (value.Trim() == "?")
                {
                    result = -1;
                }
                else
                {
                    result = Parser.ParseInt32(this, value);
                    if (result <= 0)
                    {
                        throw new ScriptException(this, "Localization Id must be greater than zero.");
                    }
                }
            }

            return result;
        }

        // LocalizableText
        protected internal string LocalizableText()
        {
            return Body.Clauses[TextClauseIndex];
        }

        // TextClauseIndex
        protected abstract int TextClauseIndex { get; }

        #endregion

        #region Internal members

        // EncodeTextKey
        internal string EncodeTextKey()
        {
            var value = Body.Args.GetArg(LocalizationIdArg)?.Value;
            return value == null ? string.Empty : (ScriptSyntax.RootLocalizationImportsKey + "." + Script.Name + "." + value);
        }

        // GetDisplayText
        internal protected string GetDisplayText()
        {
            return GetDisplayText(Session.LocalizationSource);
        }

        // GetDisplayText
        internal protected string GetDisplayText(LocalizationSource textSource)
        {
            var result = Parser.ParseQuotedString(this, TextClauseIndex);

            if (!Literal && !TextRepository.IsKeyReference(result))
            {
                if (textSource is LocalizationSource.TextRepository or
                    LocalizationSource.TextRepositoryOtherwiseScript)
                {
                    var key = EncodeTextKey();
                    var textRepositoryValue = TextRepository.GetValue(key);

                    if (string.IsNullOrWhiteSpace(textRepositoryValue))
                    {
                        if (textSource == LocalizationSource.TextRepositoryOtherwiseScript)
                        {
                            textRepositoryValue = result;
                        }
                    }

                    result = textRepositoryValue;
                }
            }

            return result;
        }

        // GetLocalizationId
        internal int GetLocalizationId()
        {
            return AssertLocalizationId();
        }

        // GetTextEmitterName
        internal protected virtual string GetTextEmitterName()
        {
            return string.Empty;
        }

        // HasLocalizationArg
        internal bool HasLocalizationArg => HasArg(LocalizationIdArg);

        // Literal
        internal bool Literal => HasArg(LiteralArg);

        #endregion
    }
}
