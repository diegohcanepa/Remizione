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

        // Constitution (Resistencia física)
        // Puntos de golpe(HP)
        // Resistencia a enfermedades, venenos, fatiga
        // Tiradas de salvación del cuerpo
        public int Constitution { get; set; } = 8;

        // Devotion (Fe o fuerza espiritual)
        // Puntos de espíritu o "mana sagrado"
        // Poder y precisión de los conjuros sagrados
        // Salvaciones contra corrupción, maldiciones, tentaciones
        // Influencia en rituales
        public int Devotion { get; set; } = 8;

        // Dexterity (Agilidad y reflejos)
        // Tiradas de ataque con armas ligeras o a distancia
        // Iniciativa
        // Clase de armadura(evasión)
        // Salvaciones contra trampas, fuego, explosiones
        public int Dexterity { get; set; } = 8;

        // Empathy (Carisma/emoción)
        // Interacciones sociales: persuasión, intimidación, mentira
        // Atraer aliados o manipular enemigos
        // Habilidad para consolar, redimir, o engañar
        // Influye en eventos basados en emociones
        public int Empathy { get; set; } = 8;

        // Mind (Inteligencia/razón)
        // Tiradas de habilidad mental(investigación, conocimiento)
        // Capacidad para entender acertijos, runas, lenguas antiguas
        // Defensa contra ilusiones y control mental
        public int Mind { get; set; } = 8;

        // Strength (Fuerza física)
        // Tiradas de ataque con armas cuerpo a cuerpo
        // Daño con armas pesadas
        // Tiradas para forzar cosas, romper, cargar
        // Salvaciones contra agarres, empujones
        public int Strength { get; set; } = 8;

        #endregion

        // AngerDegradationInterval
        public int AngerDegradationInterval => 500;

        // AngerGainPerLevel
        public int AngerGainPerLevel { get; private set; } = 10;

        // Apply
        public void Apply()
        {
            actor.MaxFP = GetMaxFP();
            actor.MaxHP = GetMaxHP();
            actor.MaxAnger = GetMaxAnger();
        }

        // FPGainPerLevel
        public int FPGainPerLevel { get; private set; } = 10;

        // GetAngerCostForAttack
        public int GetAngerCostForAttack(AttackType attackType)
        {
            return attackType switch
            {
                AttackType.Light => 2,
                AttackType.Medium => 3,
                AttackType.Heavy => 4,
                _ => throw new NotImplementedException(),
            };
        }

        // GetAngerCostFromMovement
        public int GetAngerCostFromMovement(float distance)
        {
            const int basePixelUnit = 20;
            return (int)Math.Ceiling(distance / basePixelUnit);
        }

        // GetDefense
        public int GetDefense(int armorBonus = 0, int miscBonus = 0)
        {
            return 10 + GetModifier(PrimaryStat.Dexterity) + armorBonus + miscBonus;
        }

        // GetMaxAnger
        public int GetMaxAnger()
        {
            return actor.Level * (AngerGainPerLevel + GetModifier(PrimaryStat.Dexterity));
        }

        // GetMaxFP
        public int GetMaxFP()
        {
            return actor.Level * (FPGainPerLevel + GetModifier(PrimaryStat.Devotion));
        }

        // GetMaxHP
        public int GetMaxHP()
        {
            return actor.Level * (HPGainPerLevel + GetModifier(PrimaryStat.Constitution));
        }

        // GetStatValue
        public int GetStatValue(PrimaryStat stat)
        {
            return stat switch
            {
                PrimaryStat.Constitution => Constitution,
                PrimaryStat.Devotion => Devotion,
                PrimaryStat.Dexterity => Dexterity,
                PrimaryStat.Empathy => Empathy,
                PrimaryStat.Mind => Mind,
                PrimaryStat.Strength => Strength,
                _ => throw new System.NotImplementedException()
            };
        }

        // GetModifier
        public int GetModifier(PrimaryStat stat)
        {
            return (GetStatValue(stat) - 10) / 2;
        }

        // GP (XP)
        public int GP { get; set; } = 0;

        // HPGainPerLevel
        public int HPGainPerLevel { get; private set; } = 8;

        // PerformSkillCheck
        public int PerformSkillCheck(PrimaryStat stat) => DiceBag.Dice20.Roll() + GetStatValue(stat);

        // RollAttack
        public int RollAttack(bool useDexterity = false)
        {
            int modifier = useDexterity ? GetModifier(PrimaryStat.Dexterity) : GetModifier(PrimaryStat.Strength);
            return DiceBag.Dice20.Roll() + modifier;
        }

        // RollInitiative
        public int RollInitiative()
        {
            return DiceBag.Dice20.Roll() + GetModifier(PrimaryStat.Dexterity);
        }

        // RollSavingThrow
        public int RollSavingThrow(PrimaryStat stat)
        {
            return stat switch
            {
                PrimaryStat.Mind => DiceBag.Dice20.Roll() + GetModifier(PrimaryStat.Mind),
                PrimaryStat.Constitution => DiceBag.Dice20.Roll() + GetModifier(PrimaryStat.Constitution),
                PrimaryStat.Devotion => DiceBag.Dice20.Roll() + GetModifier(PrimaryStat.Devotion),
                PrimaryStat.Empathy => DiceBag.Dice20.Roll() + GetModifier(PrimaryStat.Empathy),
                _ => DiceBag.Dice20.Roll()
            };
        }
    }
}
