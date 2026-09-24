using Engendro;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Remizione
{
    /// <summary>
    /// RoomDefinition
    /// </summary>
    public sealed class RoomDefinition : EntityDefinition
    {
        #region Private fields

        private readonly List<Placeholder> placeholders = [];

        #endregion

        #region Constructor

        // Constructor
        public RoomDefinition(JsonElement element)
            : base(element)
        {
            // MinEnemies
            this.MinEnemies = element.GetInt32("minEnemies", 0);

            // MaxEnemies
            this.MaxEnemies = element.GetInt32("maxEnemies", 0);

            if (MinEnemies > MaxEnemies)
                RaiseValidationError(this, $"Minimum enemies exceeds maximum enemies.");

            // MinProps
            this.MinProps = element.GetInt32("minProps", 0);

            // MaxProps
            this.MaxProps = element.GetInt32("maxProps", 0);

            if (MinProps > MaxProps)
                RaiseValidationError(this, $"Minimum props exceeds maximum props.");

            // Placeholders
            if (element.TryGetProperty("placeholders", out JsonElement placeholdersElement))
            {
                foreach (var phElement in placeholdersElement.EnumerateArray())
                {
                    placeholders.Add(new Placeholder(phElement));
                }
            }

            // RunModifiers
            if (element.GetString("runModifiers") is string runModifiersData && !string.IsNullOrWhiteSpace(runModifiersData))
            {
                RunModifiers = new(runModifiersData.Split(','));
                foreach (var value in RunModifiers)
                {
                    if (GameData.RunModifiers.Find(value) is null)
                        RaiseValidationError(this, $"The name '{value}' is not a valid run modifier.");
                }
            }
            else
            {
                RunModifiers = [];
            }

            // Scope
            this.Scope = TagScope.FromJson(element);

            Placeholders = new(placeholders);
        }

        #endregion

        // GetPlaceholders
        public List<Placeholder> GetPlaceholders(PlaceholderContentType contentType)
        {
            var result = new List<Placeholder>();

            for (int i = 0; i < placeholders.Count; i++)
            {
                if (placeholders[i].ContentType == contentType)
                    result.Add(placeholders[i]);
            }

            return result;
        }

        // MaxEnemies
        public int MaxEnemies { get; }

        // MaxProps
        public int MaxProps { get; }

        // MinEnemies
        public int MinEnemies { get; }

        // MinProps
        public int MinProps { get; }

        // Placeholders
        public ReadOnlyPlaceholderCollection Placeholders { get; }

        // RunModifiers
        public ReadOnlyCollection<string> RunModifiers { get; }

        // Scope
        public TagScope Scope { get; }
    }
}
