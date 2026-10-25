namespace Remizione
{
    /// <summary>
    /// StatusSet
    /// </summary>
    public sealed class StatusSet
    {
        // ApplyAmount
        public bool ApplyAmount(StatusType type, int amount)
        {
            if (amount <= 0)
                return false;

            switch (type)
            {
                case StatusType.Laceration:
                    Laceration += amount;
                    if (Laceration >= Threshold)
                    {
                        Laceration = 0;
                        return true;
                    }
                    break;

                case StatusType.Putrefaction:
                    Poison += amount;
                    if (Poison >= Threshold)
                    {
                        Poison = 0;
                        return true;
                    }
                    break;

                default:
                    break;
            }

            return false;
        }

        // Clear
        public void Clear(StatusType type)
        {
            switch (type)
            {
                case StatusType.Laceration:
                    Laceration = 0;
                    break;

                case StatusType.Putrefaction:
                    Poison = 0;
                    break;

                default:
                    break;
            }
        }

        // GetValue
        public int GetValue(StatusType type)
        {
            return type switch
            {
                StatusType.Laceration => Laceration,
                StatusType.Putrefaction => Poison,
                _ => 0
            };
        }

        // Laceration
        public int Laceration { get; set; }

        // Poison
        public int Poison { get; set; }

        // Threshold
        public const float Threshold = 4;
    }
}