using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Engendro
{
    /// <summary>
    /// CodeContract
    /// </summary>
    public static class CodeContract
    {
        // EqualOrGreaterThanZero
        public static void EqualOrGreaterThanZero(float value, string paramName)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(paramName, "Value must be equal or greater than zero.");
        }

        // GreaterThanZero
        public static void GreaterThanZero(float value, string paramName)
        {
            if (value < 1)
                throw new ArgumentOutOfRangeException(paramName, "Value must be greater than zero.");
        }

        // IsValidName
        public static bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || Regex.Count(name, @"[0-9a-zA-Z-_]") < name.Length)
                return false;
            else
                return true;
        }

        // NotDisposed
        public static void NotDisposed(string objectName, bool isDisposed)
        {
            NotEmpty(objectName, nameof(objectName));

            if (isDisposed)
                throw new ObjectDisposedException(objectName, $"The {objectName} object was used after being disposed.");
        }

        // NotDuplicate
        public static void NotDuplicate<T>(ICollection<T> collection, T item, string paramName)
        {
            if (collection.Contains(item))
                throw new ArgumentException("Item already exists in the collection.", paramName);
        }

        // NotDuplicate
        public static void NotDuplicate<T>(NamedObjectReadOnlyCollection<T> collection, string name, string paramName) where T : class, INamedObject
        {
            if (collection.Contains(name))
                throw new ArgumentException("Name already exists in the collection.", paramName);
        }

        // NotEmpty
        public static void NotEmpty(string? value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null, empty or contain only whitespaces.", paramName);
        }

        // NotLoaded
        public static void NotLoaded(string objectName, bool isLoaded)
        {
            NotEmpty(objectName, nameof(objectName));
            if (!isLoaded)
                throw new InvalidOperationException($"The {objectName} object is not loaded.");
        }

        // ThrowDuplicatedNameException
        public static void ThrowDuplicatedNameException(string paramName)
        {
            throw new ArgumentException("Name must be unique.", paramName);
        }

        // ValidName
        public static void ValidName(string name, string parameName)
        {
            if (!IsValidName(name))
                throw new ArgumentException("Value must contain only [0-9, a-z, A-Z, _] characters.", parameName);
        }

        // ValidRange
        public static void ValidRange(float value, float min, float max, string paramName)
        {
            if (!value.IsBetween(min, max))
                throw new ArgumentOutOfRangeException(paramName, $"Value must be between {min} and {max}.");
        }

        // ValidRatio
        public static void ValidRatio(float value, string paramName)
        {
            if (!value.IsBetween(0, 1))
                throw new ArgumentOutOfRangeException(paramName, $"Value must be between 0 and 1.");
        }
    }
}
