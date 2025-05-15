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
            Apply();
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

        // Apply
        public void Apply()
        {
            actor.MaxFaith = GetMaxFaith();
            actor.MaxHP = GetMaxHP();
        }

        // MovePenalty
        public int MovePenalty { get; private set; } = 2;

        // FaithGainPerLevel
        public int FaithGainPerLevel { get; private set; } = 20;

        // FaithPenaltyCooldown
        public int FaithPenaltyCooldown
        {
            get
            {
                int baseCooldown = 100;
                int modifier = GetModifier(Stat.Devotion);

                // Increase cooldown by 100ms for each modifier point
                return Math.Max(250, baseCooldown + (modifier * 100));
            }
        }

        // GetDefense
        public int GetDefense(int armorBonus = 0, int miscBonus = 0)
        {
            return 10 + GetModifier(Stat.Dexterity) + armorBonus + miscBonus;
        }

        // GetMaxFaith
        public int GetMaxFaith()
        {
            return actor.Level * (FaithGainPerLevel + GetModifier(Stat.Devotion));
        }

        // GetMaxHP
        public int GetMaxHP()
        {
            return actor.Level * (HPGainPerLevel + GetModifier(Stat.Fortitude));
        }

        // GetModifier
        public int GetModifier(Stat stat)
        {
            return (GetStatValue(stat) - 10) / 2;
        }

        // GetStatValue
        public int GetStatValue(Stat stat)
        {
            return stat switch
            {
                Stat.Fortitude => Fortitude,
                Stat.Devotion => Devotion,
                Stat.Dexterity => Dexterity,
                Stat.Charisma => Charisma,
                Stat.Mind => Mind,
                Stat.Strength => Strength,
                _ => throw new System.NotImplementedException()
            };
        }

        // GP (XP)
        public int GP { get; set; } = 0;

        // HPGainPerLevel
        public int HPGainPerLevel { get; private set; } = 8;

        // PerformSkillCheck
        public int PerformSkillCheck(Stat stat) => DiceBag.Dice20.Roll() + GetStatValue(stat);

        // RollAttack
        public int RollAttack(AttackRollStat stat, int bonus, out bool criticalHit)
        {
            int modifier = stat == AttackRollStat.Dexterity ? GetModifier(Stat.Dexterity) : GetModifier(Stat.Strength);
            var d20 = DiceBag.Dice20.Roll();
            criticalHit = d20 == 20;
            return d20 + modifier + bonus;
        }

        // RollInitiative
        public int RollInitiative()
        {
            return DiceBag.Dice20.Roll() + GetModifier(Stat.Dexterity);
        }

        // RollInitiative
        public bool RollInitiative(Actor target)
        {
            var targetInitiative = target.Stats.RollInitiative();
            var thisInitiative = RollInitiative();

            return thisInitiative >= targetInitiative;
        }

        // RollSavingThrow
        public int RollSavingThrow(Stat stat)
        {
            return stat switch
            {
                Stat.Mind => DiceBag.Dice20.Roll() + GetModifier(Stat.Mind),
                Stat.Fortitude => DiceBag.Dice20.Roll() + GetModifier(Stat.Fortitude),
                Stat.Devotion => DiceBag.Dice20.Roll() + GetModifier(Stat.Devotion),
                Stat.Charisma => DiceBag.Dice20.Roll() + GetModifier(Stat.Charisma),
                _ => DiceBag.Dice20.Roll()
            };
        }
    }
}
