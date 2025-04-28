namespace Remizione
{
    /// <summary>
    /// Stats
    /// </summary>
    public sealed class Stats
    {
        private readonly Actor actor;

        // Constructor
        public Stats(Actor actor)
        {
            this.actor = actor;
            Apply();
        }

        #region Private members

        // CalculateFP
        private int CalculateFP()
        {
            return 20;

            if (Devotion < 10)
                return 50 + (Devotion - 1) * 6;   // crecimiento inicial

            else if (Devotion <= 20)
                return 104 + (Devotion - 10) * 5; // crecimiento moderado

            else if (Devotion <= 40)
                return 154 + (Devotion - 20) * 3; // soft cap

            else if (Devotion <= 60)
                return 214 + (Devotion - 40);     // +1 FP por punto

            else
                return 234; // hard cap
        }

        // CalculateHP
        private int CalculateHP()
        {
            return 30;

            if (Vigor < 10)
                return 300 + (Vigor - 1) * 15;  // grow faster at begining

            else if (Vigor <= 20)
                return 435 + (Vigor - 10) * 10; // +10 HP per point

            else if (Vigor <= 40)
                return 535 + (Vigor - 20) * 8;  // +8 HP per point

            else if (Vigor <= 60)
                return 695 + (Vigor - 40) * 4;  // +4 HP per point

            else
                return 775; // Max (without buffs)
        }

        // CalculateWillpower
        private int CalculateWillpower()
        {
            if (Endurance <= 10)
                return 90 + (Endurance - 8) * 4; // from 8 a 10 → +4 per point

            else if (Endurance <= 20)
                return 102 + (Endurance - 10) * 2; // from 11 a 20 → +2 per point

            else if (Endurance <= 40)
                return 122 + (Endurance - 20); // from 21 a 40 → +1 per point

            else if (Endurance <= 60)
                return 142 + (Endurance - 40) / 2; // from 41 a 60 → +1 every 2 points

            else
                return 152; // Max
        }

        #endregion

        // Apply
        public void Apply()
        {
            actor.MaxFP = CalculateFP();
            actor.MaxHP = CalculateHP();
            actor.MaxWillpower = CalculateWillpower();
        }

        // Devotion
        public int Devotion { get; set; } = 8;

        // Dexterity
        public int Dexterity { get; set; } = 8;

        // Empathy (Charisma)
        public int Empathy { get; set; } = 8;

        // Endurance
        public int Endurance { get; set; } = 8;

        // GP (XP)
        public int GP { get; set; } = 0;

        // Mind (Intelligence)
        public int Mind { get; set; } = 8;

        // Strength
        public int Strength { get; set; } = 8;

        // Vigor (Constitution)
        public int Vigor { get; set; } = 8;

        // WillpowerDegradationInterval
        public int WillpowerDegradationInterval => 500;
    }
}
