using Engendro;
using Engendro.Input;
using System;
using System.Globalization;

namespace Remizione
{
    /// <summary>
    /// LocalizationHelper
    /// </summary>
    internal static class LocalizationHelper
    {
        // FormatPlayTime
        internal static string FormatPlayTime(TimeSpan value)
        {
            // GetPlaytime
            static string GetPlaytime(string key) => "@Playtime." + key.ToString();

            // Less than a minute
            if (value.TotalMilliseconds <= 60000)
                return TextRepository.GetValue(GetPlaytime("LessThanAMinute"));

            // Hours
            string hourLabel;
            var totalHours = Math.Truncate(value.TotalHours);
            if (totalHours == 0)
                hourLabel = string.Empty;
            else
                hourLabel = TextRepository.GetValue(GetPlaytime(totalHours <= 1 ? "Hour" : "Hours"));

            var hours = string.IsNullOrWhiteSpace(hourLabel) ? string.Empty : totalHours.ToString(CultureInfo.InvariantCulture);

            // Minutes
            string minuteLabel;
            if (value.Minutes == 0)
                minuteLabel = string.Empty;
            else
                minuteLabel = TextRepository.GetValue(GetPlaytime(value.Minutes == 1 ? "Minute" : "Minutes"));

            var minutes = string.IsNullOrWhiteSpace(minuteLabel) ? minuteLabel : value.Minutes.ToString(CultureInfo.InvariantCulture);

            var conjuction = !string.IsNullOrWhiteSpace(hourLabel) && !string.IsNullOrWhiteSpace(minuteLabel) ? TextRepository.GetValue("Misc.And") : string.Empty;

            var result = $"{hours} {hourLabel}".Trim();
            result += " " + conjuction + " ";
            result += $"{minutes} {minuteLabel}".Trim();

            return result.Trim();
        }

        // GetInputBinding
        internal static string GetInputBinding(InputBinding binding)
        {
            return "@InputBindings." + binding.Name;
        }

        // GetMessage
        internal static string GetMessage(MessageKey key)
        {
            return GetMessage(key.ToString());
        }

        // GetMessage
        internal static string GetMessage(string key)
        {
            return "@Messages." + key;
        }

        // GetNoun
        internal static string GetoNoun(string noun)
        {
            return "@Nouns." + noun;
        }

        // GetPlatformMessage
        public static string GetPlatformMessage(PlatformMessageKey key)
        {
            return $"@Platforms.{EngendroGame.RunningPlatform}.Messages.{key}";
        }
    }
}
