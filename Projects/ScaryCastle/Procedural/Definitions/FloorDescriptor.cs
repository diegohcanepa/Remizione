using Engendro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// FloorDescriptor
    /// </summary>
    public sealed class FloorDescriptor
    {
        // Constructor
        public FloorDescriptor(JsonElement element)
        {
            // RoomCount
            if (element.GetInt32("roomCount") is not int roomCount)
                throw new InvalidOperationException("Missing roomCount property.");
            else
            {
                this.RoomCount = roomCount;
                CodeContract.ValidRange(roomCount, 5, 20, "roomCount");
            }

            // Theme
            if (element.GetEnum<RoomTheme>("theme") is not RoomTheme theme)
                throw new InvalidOperationException("Missing theme property.");
            else
                this.Theme = theme;

            // AallowedPuzzles
            List<PuzzleKind> puzzlesList = [];

            if (element.TryGetProperty("allowedPuzzles", out JsonElement allowedPuzzlesElement))
            {
                foreach (var puzzleElement in allowedPuzzlesElement.EnumerateArray())
                {
                    puzzlesList.Add(Enum.Parse<PuzzleKind>(puzzleElement.ToString()));
                }
            }

            AllowedPuzzles = puzzlesList.AsReadOnly();
        }

        // AllowedPuzzles
        public ReadOnlyCollection<PuzzleKind> AllowedPuzzles { get; }

        // RoomCount
        public int RoomCount { get; }

        // Theme
        public RoomTheme Theme { get; }
    }
}
