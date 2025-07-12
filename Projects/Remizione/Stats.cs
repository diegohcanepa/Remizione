using Engendro;
using System;

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
            //Apply();
        }

        #region Primary stats

        // Devotion (Fe o fuerza espiritual)
        // Puntos de espíritu o "mana sagrado"
        // Poder y precisión de los conjuros sagrados
        // Salvaciones contra corrupción, maldiciones, tentaciones
        // Influencia en rituales
        public int Devotion { get; set; } = 10;

        // Dexterity (Agilidad y reflejos)
        // Tiradas de ataque con armas ligeras o a distancia
        // Iniciativa
        // Clase de armadura(evasión)
        // Salvaciones contra trampas, fuego, explosiones
        public int Dexterity { get; set; } = 10;

        // Fortitude (Resistencia del alma y espiritual)
        // Puntos de golpe(HP)
        // Resistencia a enfermedades, venenos, fatiga
        // Tiradas de salvación del cuerpo
        public int Fortitude { get; set; } = 10;

        // Mind (Inteligencia/razón)
        // Tiradas de habilidad mental(investigación, conocimiento)
        // Capacidad para entender acertijos, runas, lenguas antiguas
        // Defensa contra ilusiones y control mental
        public int Mind { get; set; } = 10;

        // Charisma
        // Interacciones sociales: persuasión, intimidación, mentira
        // Atraer aliados o manipular enemigos
        // Habilidad para consolar, redimir, o engañar
        // Influye en eventos basados en emociones
        public int Charisma { get; set; } = 10;

        // Strength (Fuerza física)
        // Tiradas de ataque con armas cuerpo a cuerpo
        // Daño con armas pesadas
        // Tiradas para forzar cosas, romper, cargar
        // Salvaciones contra agarres, empujones
        public int Strength { get; set; } = 8;

        #endregion

        /*
        // Apply
        public void Apply()
        {
            actor.MaxFaith = GetMaxFaith();
            actor.MaxHP = GetMaxHP();
        }
        */

        // FaithGainPerLevel
        public int FaithGainPerLevel { get; private set; } = 20;

        // GetDefense
        public int GetDefense(int armorBonus = 0, int miscBonus = 0)
        {
            return 10 + GetModifier(StatModifier.Dexterity) + armorBonus + miscBonus;
        }

        // GetMaxHP
        public int GetMaxHP()
        {
            return actor.Level * (HPGainPerLevel + GetModifier(StatModifier.Fortitude));
        }

        // GetModifier
        public int GetModifier(StatModifier statModifier)
        {
            if (statModifier == StatModifier.None)
                return 0;
            else
                return (GetStatValue(statModifier) - 10) / 2;
        }

        // GetStatValue
        public int GetStatValue(StatModifier statModifier)
        {
            return statModifier switch
            {
                StatModifier.Fortitude => Fortitude,
                StatModifier.Devotion => Devotion,
                StatModifier.Dexterity => Dexterity,
                StatModifier.Charisma => Charisma,
                StatModifier.Mind => Mind,
                StatModifier.Strength => Strength,
                _ => throw new NotImplementedException()
            };
        }

        // HPGainPerLevel
        public int HPGainPerLevel { get; private set; } = 8;

        // PerformSkillCheck
        public int PerformSkillCheck(StatModifier statModifier) => DiceExpression.Dice20.Roll() + GetStatValue(statModifier);

        // RollAttack
        public int RollAttack(AttackRollStat stat, int bonus, out bool criticalHit)
        {
            int modifier = stat == AttackRollStat.Dexterity ? GetModifier(StatModifier.Dexterity) : GetModifier(StatModifier.Strength);
            var d20 = DiceExpression.Dice20.Roll();
            criticalHit = d20 == 20;
            return d20 + modifier + bonus;
        }

        // RollSavingThrow
        public int RollSavingThrow(Stat stat)
        {
            return stat switch
            {
                Stat.Mind => DiceExpression.Dice20.Roll() + GetModifier(StatModifier.Mind),
                Stat.Fortitude => DiceExpression.Dice20.Roll() + GetModifier(StatModifier.Fortitude),
                Stat.Devotion => DiceExpression.Dice20.Roll() + GetModifier(StatModifier.Devotion),
                Stat.Charisma => DiceExpression.Dice20.Roll() + GetModifier(StatModifier.Charisma),
                _ => DiceExpression.Dice20.Roll()
            };
        }
    }
}
