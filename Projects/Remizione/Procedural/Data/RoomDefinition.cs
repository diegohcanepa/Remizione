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
        private readonly List<string> walls = [];

        #endregion

        #region Constructor

        // Constructor
        public RoomDefinition(JsonElement element)
            : base(element)
        {
            // AllowEnemies
            this.AllowEnemies = element.GetBool("allowEnemies", true);

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

            // WalkArea
            WalkArea = string.Empty;
            if (element.TryGetProperty("walkArea", out JsonElement walkAreaElement))
            {
                WalkArea = walkAreaElement.GetString() ?? string.Empty;
                Polygon.GetVertices(WalkArea);
            }

            // Walls
            if (element.TryGetProperty("walls", out JsonElement wallsElement))
            {
                foreach (var item in wallsElement.EnumerateArray())
                {
                    var value = item.GetString() ?? string.Empty;
                    Polygon.GetVertices(value);
                    walls.Add(value);
                }
            }

            Placeholders = new(placeholders);
            Walls = walls.AsReadOnly();
        }

        #endregion

        // AllowEnemies
        public bool AllowEnemies { get; }

        // Placeholders
        public ReadOnlyPlaceholderCollection Placeholders { get; }

        // RunModifiers
        public ReadOnlyCollection<string> RunModifiers { get; }

        // Scope
        public TagScope Scope { get; }

        // WalkArea
        public string WalkArea { get; }

        // Walls
        public ReadOnlyCollection<string> Walls { get; }
    }
}
