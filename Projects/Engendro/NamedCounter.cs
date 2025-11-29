using System;
using System.Collections.Generic;
using System.Text;

namespace Engendro
{
    /// <summary>
    /// NamedCounter
    /// </summary>
    public sealed class NamedCounter
    {
        private readonly Dictionary<string, int> data = [];

        // Deserialize
        public void Deserialize(string input)
        {
            this.data.Clear();

            var values = input.Split(';', StringSplitOptions.RemoveEmptyEntries);

            foreach (var value in values)
            {
                var v = value.Split(":");
                data.Add(v[0], int.Parse(v[1]));
            }
        }

        // GetCount
        public int GetCount(string name)
        {
            return data.TryGetValue(name, out var value) ? value : 0;
        }

        // Increment
        public void Increment(string name)
        {
            if (data.TryGetValue(name, out var value))
                data[name] = ++value;
            else
                data[name] = 1;
        }

        // Reset
        public void Reset()
        {
            data.Clear();
        }

        // Serialize
        public string Serialize()
        {
            var result = new List<string>();

            foreach (var keyValue in data)
            {
                result.Add($"{keyValue.Key}:{keyValue.Value}");
            }

            return string.Join(";", result);
        }
    }
}
