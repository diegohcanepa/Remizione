using Adberration.Scripting;
using System;
using System.Text.RegularExpressions;

namespace Adberration
{
    /// <summary>
    /// NameValidator
    /// </summary>
    internal static class NameValidator
    {
        #region Internal members

        // Validate
        internal static NameValidationError Validate(string name)
        {
            // Check for null
            if (name == null)
            {
                return NameValidationError.Null;
            }

            // Check empty
            if (string.IsNullOrWhiteSpace(name))
            {
                return NameValidationError.Empty;
            }

            // Check characters
            if (Regex.Count(name, @"[0-9a-zA-Z-_]") < name.Length)
            {
                return NameValidationError.InvalidCharacters;
            }

            // Check if name is a reserved word
            if (ScriptEnvironment.IsReservedWord(name))
            {
                return NameValidationError.ReservedWord;
            }

            return NameValidationError.None;
        }

        #endregion

        // CheckName
        public static void CheckName(string name)
        {
            var error = Validate(name);
            if (error != NameValidationError.None)
            {
                throw new ArgumentException(GetErrorMessage(error), nameof(name));
            }
        }

        // GetErrorMessage
        public static string? GetErrorMessage(NameValidationError error)
        {
            return error switch
            {
                NameValidationError.Null => "A 'Name Identifier' cannot be null.",

                NameValidationError.Empty => "A 'Name Identifier' cannot be null, empty, or consists of white-space characters.",

                NameValidationError.InvalidCharacters => "A 'Name Identifier' can contain only [0-9, a-z, A-Z, _] characters.",

                NameValidationError.ReservedWord => $"The name is a reserved word",

                NameValidationError.None => null,

                _ => throw new NotImplementedException(),
            };
        }
    }
}
