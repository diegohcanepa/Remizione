using System;
using System.Collections.Generic;

namespace Engendro
{
    /// <summary>
    /// AchievementManager
    /// </summary>
    public static class AchievementManager
    {
        private static readonly Dictionary<string, Achievement> achievements = [];

        // Add
        public static Achievement Add(string id)
        {
            if (FindAchievement(id) != null)
            {
                throw new InvalidOperationException($"Duplicates are not allowed: {id}.");
            }

            Achievement result = new(achievements.Count, id);
            achievements.Add(id, result);
            return result;
        }

        // Count
        public static int Count => achievements.Count;

        // FindAchievement
        public static Achievement? FindAchievement(string name)
        {
            return achievements.TryGetValue(name, out var value) ? value : null;
        }

        // GetAchievements
        public static Achievement[] GetAchievements()
        {
            return new List<Achievement>(achievements.Values).ToArray();
        }
    }
}
