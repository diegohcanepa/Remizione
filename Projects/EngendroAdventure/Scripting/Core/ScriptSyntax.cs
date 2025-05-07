using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.ObjectModel;

namespace EngendroAdventure.Scripting
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

        // CodeBlockEnd
        public const string CodeBlockEnd = "}";

        // CodeBlockStart
        public const string CodeBlockStart = "{";

        // ConstantPrefix
        public const string ConstantPrefix = "%";

        // DynamicSuffix
        public const string DynamicSuffix = "*";

        // EqualityOp
        public const string EqualityOp = "==";

        // GetStaticName
        public static string GetStaticName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return name;

            var index = name.IndexOf(DynamicSuffix, StringComparison.OrdinalIgnoreCase);
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

        // InstantiableKeyword
        public const string InstantiableKeyword = "Instantiable";

        // IsDeclarationReservedWord
        public static bool IsDeclarationReservedWord(string value)
        {
            return value == InstantiableKeyword || value == ClassKeyword || value == PersistentKeyword;
        }

        // IsDynamicName
        public static bool IsDynamicName(string name) => name.Contains(DynamicSuffix);

        // IsNumericType
        public static bool IsNumericType(Type type) => SupportedNumericTypes.Contains(type);

        // IsOp
        public const string IsOp = "is";

        // IsRuntimeName
        public static bool IsRuntimeName(string name) => name.Contains(RuntimeNameSuffix);

        // IsSessionMemberReference
        public static bool IsSessionMemberReference(string value)
        {
            return value.StartsWith(SessionPropertyAlias) || value.StartsWith("Session.");
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

        // NewKeyword
        public const string NewKeyword = "new";

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

        // ScriptCompoundSeparator
        public const string ScriptCompoundSeparator = "+";

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
                                                                                                                  typeof(DiceRoll),
                                                                                                                  typeof(FlagCondition),
                                                                                                                  typeof(float),
                                                                                                                  typeof(FloatRange),
                                                                                                                  typeof(InputBinding),
                                                                                                                  typeof(int),
                                                                                                                  typeof(Int32Range),
                                                                                                                  typeof(long),
                                                                                                                  typeof(Rectangle),
                                                                                                                  typeof(RectangleF),
                                                                                                                  typeof(Script),
                                                                                                                  typeof(Sound),
                                                                                                                  typeof(string),
                                                                                                                  typeof(Vector2),
                                                                                                                  typeof(Polygon) ]);

        // ThisKeyword
        public const string ThisKeyword = "this";
    }
}