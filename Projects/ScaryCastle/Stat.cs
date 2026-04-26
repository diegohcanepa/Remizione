using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// Stat
    /// </summary>
    public sealed class Stat
    {
        private bool isDirty = true;
        private readonly List<StatModifier> modifiers = new(20);

        // Constrcutor
        public Stat(float baseValue)
        {
            BaseValue = baseValue;
            Value = baseValue;
        }

        #region Private members

        // CalculateValue
        private float CalculateValue()
        {
            float finalValue = BaseValue;
            float multiplierSum = 0;

            // Bucle manual: Primero sumas planas, luego acumulamos multiplicadores
            for (int i = 0; i < modifiers.Count; i++)
            {
                multiplierSum += modifiers[i].Value;
            }

            // Aplicamos la suma de multiplicadores al final
            // Ejemplo: Base 1 + Mod(0.10) + Mod(0.05) = 1.15
            return finalValue * (1.0f + multiplierSum);
        }

        #endregion

        // AddModifier
        public void AddModifier(StatModifier mod)
        {
            modifiers.Add(mod);
            isDirty = true;
        }

        // BaseValue
        public float BaseValue { get; }

        // Clear
        public void Clear()
        {
            modifiers.Clear();
            isDirty = true;
        }

        // RemoveModifiers
        public void RemoveModifiers(object source)
        {
            for (int i = modifiers.Count - 1; i >= 0; i--)
            {
                if (modifiers[i].Source == source)
                {
                    modifiers.RemoveAt(i);
                    isDirty = true;
                }
            }
        }

        // Value
        public float Value
        {
            get
            {
                if (isDirty)
                {
                    field = CalculateValue();
                    isDirty = false;
                }

                return field;
            }
        }
    }
}
