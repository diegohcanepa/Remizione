using Engendro;
using Engendro.Input;
using System;
using System.Globalization;

namespace Remizione
{
    /// <summary>
    /// Localization
    /// </summary>
    internal static class Localization
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

        // GetItemDescription
        internal static string GetItemDescription(MetaItem item)
        {
            return TextRepository.GetValue($"Item.{item.Name}.Description");
        }

        // GetItemName
        internal static string GetItemName(MetaItem item)
        {
            return TextRepository.GetValue($"Item.{item.Name}.Name");
        }

        // GetValue
        internal static string GetValue<TEnum>(TEnum value) where TEnum : Enum
        {
            return TextRepository.GetValue($"@{typeof(TEnum).Name}.{value}");
        }

        // GetValue
        internal static string GetValue(InputBinding binding) => TextRepository.GetValue($"InputBinding.{binding.Name}");
    }
}
