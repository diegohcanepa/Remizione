using Engendro;
using System;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// PlayerStat
    /// </summary>
    public sealed class PlayerStat : INamedObject
    {
        // Constructor
        public PlayerStat(JsonElement element)
        {
            // Name
            this.Name = element.GetString("name") ?? throw new InvalidOperationException("Missing name.");

            if (element.GetString("values") is string values)
            {
                var valueList = values.Split(';');
                if (valueList.Length != 5)
                    throw new InvalidOperationException("You must specify 5 ratio values.");

                var ratios = new float[5];

                for (var i = 0; i < ratios.Length; i++)
                {
                    ratios[i] = float.Parse(valueList[i]);
                }

                Values = new(ratios);
            }
            else
            {
                throw new InvalidOperationException("Missing values property.");
            }
        }

        // CurrentValue
        public float CurrentValue => Level == 0 ? 0 : Values[Level];

        // Data
        public static DataContainer<PlayerStat> Data { get; } = new(element => new PlayerStat(element));

        // Level
        public int Level
        {
            get;
            set
            {
                if (value != field)
                {
                    field = int.Clamp(value, 0, 5);
                }
            }
        }

        // Name
        public string Name { get; }

        // Values
        public ReadOnlyCollection<float> Values { get; }
    }
}
