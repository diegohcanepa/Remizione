using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.ObjectModel;

namespace Adberration.Scripting
{
    /// <summary>
    /// ScriptSyntax
    /// </summary>
    public static class ScriptSyntax
    {
        // AdditionAssignmentOp
        public const string AdditionAssignmentOp = "+=";

        // AnyEntityOp
        public const string AnyEntityOp = "?";

        // ArgumentListSeparator1
        public const char ArgumentListSeparator1 = ';';

        // ArgumentPrefix
        public const string ArgumentPrefix = "#";

        // AssignmentOp
        public const string AssignmentOp = "=";

        // AwaitKeyword
        public const string AwaitKeyword = "await";

        // ClassKeyword
        public const string ClassKeyword = "Class";

        // CloneableKeyword
        public const string CloneableKeyword = "Cloneable";

        // CodeBlockEnd
        public const string CodeBlockEnd = "}";

        // CodeBlockStart
        public const string CodeBlockStart = "{";

        // ConstantPrefix
        public const string ConstantPrefix = "%";

        // CloneKeyword
        public const string CloneKeyword = "clone";

        // CloneSuffix
        public const string CloneSuffix = "*";

        // EqualityOp
        public const string EqualityOp = "==";

        // GetDeclaredName
        public static string GetDeclaredName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return name;

            var index = name.IndexOf(CloneSuffix, StringComparison.OrdinalIgnoreCase);
            if (index != -1)
            {
                name = name.Substring(0, index);
            }

            return name;
        }

        // GreaterThanOp
        public const string GreaterThanOp = ">";

        // GreaterThanOp
        public const string GreaterThanOrEqualOp = ">=";

        // InOp
        public const string InOp = "in";

        // InequalityOp
        public const string InequalityOp = "!=";

        // IsClonedName
        public static bool IsClonedName(string name)
        {
            return name.Contains(CloneSuffix);
        }

        // IsDeclarationReservedWord
        public static bool IsDeclarationReservedWord(string value)
        {
            return value is CloneableKeyword or ClassKeyword or PersistentKeyword;
        }

        // IsNumericType
        public static bool IsNumericType(Type type)
        {
            return SupportedNumericTypes.Contains(type);
        }

        // IsOp
        public const string IsOp = "is";

        // IsRuntimeName
        public static bool IsRuntimeName(string name)
        {
            return name.Contains(RuntimeNameSuffix);
        }

        // IsSessionMemberReference
        public static bool IsSessionMemberReference(string value)
        {
            return value.StartsWith(SessionPropertyAlias, StringComparison.Ordinal) || value.StartsWith("Session.", StringComparison.Ordinal);
        }

        // LessOp
        public const string LessOp = "<";

        // LessThanOp
        public const string LessThanOrEqualOp = "<=";

        // LineComment
        public const string LineComment = "//";

        // LogicalNegation
        public const string LogicalNegation = "!";

        // LogicalDisjunction
        public const string LogicalDisjunction = "|";

        // MemberSeparator
        public const string MemberSeparator = ".";

        // MethodReference
        public const string MethodReference = "=>>";

        // NotInOp
        public const string NotInOp = "not-in";

        // NullValue
        public const string NullValue = "null";

        // PersistentKeyword
        public const string PersistentKeyword = "Persistent";

        // ProperyReference
        public const string ProperyReference = "=>";

        // PropertyValueEndTag
        public const string PropertyValueEndTag = "}";

        // PropertyValueStartTag
        public const string PropertyValueStartTag = "{";

        // RangeDelimiter
        public const string RangeDelimiter = "..";

        // RootLocalizationImportsKey
        public const string RootLocalizationImportsKey = "IMPORTS";

        // RuntimeNameSuffix
        public const string RuntimeNameSuffix = "__";

        // RuntimeRoomNamePrefix
        public const string RuntimeRoomNamePrefix = "<room>";

        // SessionKeyword
        public const string SessionKeyword = "Session";

        // SessionPropertyAlias
        public const string SessionPropertyAlias = "$";

        // StringQuoteTag
        public const string StringQuoteTag = "[quote]";

        // SubtractionAssignmentOp
        public const string SubtractionAssignmentOp = "-=";

        // SupportedNumericTypes
        public static ReadOnlyCollection<Type> SupportedNumericTypes { get; } = new ReadOnlyCollection<Type>([typeof(int), typeof(long), typeof(float)]);

        // SupportedPropertyTypes
        public static ReadOnlyCollection<Type> SupportedPropertyTypes { get; } = new ReadOnlyCollection<Type>([ typeof(Atlas),
                                                                                                                  typeof(bool),
                                                                                                                  typeof(Color),
                                                                                                                  typeof(DiceExpression),
                                                                                                                  typeof(FlagCondition),
                                                                                                                  typeof(float),
                                                                                                                  typeof(FloatRange),
                                                                                                                  typeof(InputBinding),
                                                                                                                  typeof(int),
                                                                                                                  typeof(Int32Range),
                                                                                                                  typeof(long),
                                                                                                                  typeof(Rectangle),
                                                                                                                  typeof(RectangleF),
                                                                                                                  typeof(Size),
                                                                                                                  typeof(Script),
                                                                                                                  typeof(Sound),
                                                                                                                  typeof(string),
                                                                                                                  typeof(Vector2),
                                                                                                                  typeof(Polygon) ]);

        // ThisKeyword
        public const string ThisKeyword = "this";
    }
}