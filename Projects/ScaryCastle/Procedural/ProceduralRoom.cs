using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ScaryCastle
{
    /// <summary>
    /// ProceduralRoom
    /// </summary>
    public abstract class ProceduralRoom : GameRoom
    {
        #region Private fields

        private readonly CounterBank enemiesSpawnCounter = new();
        private int instanceCount;
        private readonly List<Placeholder> placeholders = [];
        private readonly CounterBank propsSpawnCounter = new();
        private readonly int randomSeed;
        private readonly HashSet<Placeholder> usedPlaceholders = [];

        #endregion

        #region Constructor

        // Constructor
        protected ProceduralRoom(GameSession session, string name, RoomNode roomNode)
            : base(session, name)
        {
            this.RoomNode = roomNode;
            this.LightingSystem = true;
            this.UnloadMode = UnloadMode.Manual;

            int salt = roomNode.Index;
            this.randomSeed = RandomHelper.GetSeed(Session.Seed, salt);
            this.Random = new Random(randomSeed);
            this.Placeholders = placeholders.AsReadOnly();

            this.MonitorStyle = true;
        }

        #endregion

        #region Private members

        // CalculateEnemyBudget
        private int CalculateEnemyBudget()
        {
            // 1. Definimos el presupuesto estrictamente por la zona geográfica
            var (min, max) = RoomNode.TopographicDifficulty switch
            {
                Difficulty.Easy => (1, 1),   // Muy tranquilo
                Difficulty.Normal => (2, 2), // Reto estándar
                Difficulty.Hard => (2, 3),   // Presión alta
                _ => (0, 0)
            };

            // 2. Variación aleatoria (-1, 0, +1) para inyectar imprevisibilidad
            int finalBudget = Random.Next(min - 1, max + 2);

            // 3. Clamp final para garantizar un límite mínimo y máximo absoluto en el cuarto
            return Math.Clamp(finalBudget, 1, 4); // Nunca 0, nunca más de 4 patrullas/patotas base
        }

        // GetSpawnPoints
        private List<Vector2> GetSpawnPoints(ReadOnlyPolygon polygon, int count, int cellSize)
        {
            var cells = new List<Vector2>();
            var area = polygon.BoundingRectangle;

            for (int y = area.Top; y < area.Bottom; y += cellSize)
            {
                for (int x = area.Left; x < area.Right; x += cellSize)
                {
                    float cx = x + (cellSize * 0.5f);
                    float cy = y + (cellSize * 0.5f);

                    float offsetRange = cellSize * 0.25f;
                    cx += (float)((Random.NextDouble() * offsetRange * 2) - offsetRange);
                    cy += (float)((Random.NextDouble() * offsetRange * 2) - offsetRange);

                    var candidate = new Vector2(cx, cy);

                    if (polygon.Contains(candidate))
                    {
                        cells.Add(candidate);
                    }
                }
            }

            for (int i = cells.Count - 1; i > 0; i--)
            {
                int j = Random.Next(i + 1);
                (cells[i], cells[j]) = (cells[j], cells[i]);
            }

            if (cells.Count > count)
                cells.RemoveRange(count, cells.Count - count);

            return cells;
        }

        // Populate
        private void Populate()
        {
            SpawnProps();
            if (RoomNode.Definition.AllowEnemies)
                SpawnEnemies();
        }

        // SpawnEnemies
        private void SpawnEnemies()
        {
            if (WalkArea == null || Session.CurrentRun == null)
                return;

            var candidates = GetCandidateDefinitions<ActorDefinition, Actor>(
                ActorDefinition.Definitions.All);

            if (candidates.Count == 0)
                return;

            var table = new ChanceTable();
            foreach (var c in candidates)
            {
                // Intensidad eliminada. Ajuste puro por choque de dificultades.
                var finalWeight = AdjustWeight(RoomNode.TopographicDifficulty, c.Difficulty, c.SpawnWeight);
                table.Add(c.Name, finalWeight);
            }

            var pendingSpawns = new List<ActorDefinition>();
            int remainingInstances = CalculateEnemyBudget();
            int safety = (candidates.Count * 2) + 10;

            while (table.Count > 0 && remainingInstances > 0 && safety-- > 0)
            {
                if (table.GetValue() is not ChanceTableItem item)
                    break;

                if (ActorDefinition.Definitions.Find(item.Name) is not ActorDefinition chosen)
                    continue;

                int packSize = chosen.RollPackSize(Random);
                int targetSpawnCount = Math.Min(packSize, remainingInstances);
                int successfulGroupSpawns = 0;

                for (int p = 0; p < targetSpawnCount; p++)
                {
                    int pendingCount = 0;
                    for (int i = 0; i < pendingSpawns.Count; i++)
                    {
                        if (pendingSpawns[i].Name == chosen.Name)
                            pendingCount++;
                    }

                    int currentInRoom = enemiesSpawnCounter.GetCount(chosen.Name) + pendingCount;

                    if (!chosen.PassesMaxPerRoomConstraint(currentInRoom))
                        break;

                    pendingSpawns.Add(chosen);
                    successfulGroupSpawns++;
                    remainingInstances--;
                }

                if (successfulGroupSpawns == 0)
                {
                    table.Remove(item.Name);
                }
            }

            if (pendingSpawns.Count == 0)
                return;

            var safePoly = new Polygon(WalkArea.Polygon.Vertices, -45);
            var points = GetSpawnPoints(safePoly, pendingSpawns.Count, 45);

            for (int i = 0; i < points.Count; i++)
            {
                var chosenDef = pendingSpawns[i];
                var instance = CreateThingClone<Actor>(chosenDef.Name);
                instance.Position = points[i];
                Children.Add(instance);

                enemiesSpawnCounter.Increment(chosenDef.Name);
                Session.CurrentRun.Spawns.Increment(chosenDef.Name);
            }
        }

        // SpawnProps
        private void SpawnProps()
        {
            if (Session.CurrentRun == null || Placeholders.Count == 0)
                return;

            var candidates = GetCandidateDefinitions<PropDefinition, Prop>(PropDefinition.Definitions.All);

            if (candidates.Count == 0)
                return;

            var shuffledPlaceholders = new List<Placeholder>(Placeholders);
            shuffledPlaceholders.Shuffle(Random);

            foreach (var placeholder in shuffledPlaceholders)
            {
                if (usedPlaceholders.Contains(placeholder))
                    continue;

                if (!placeholder.FillChance.Roll(Random))
                    continue;

                var table = new ChanceTable();

                foreach (var def in candidates)
                {
                    if (!def.Placements.Contains(placeholder.Placement))
                        continue;

                    if (placeholder.AllowTags.Count > 0 && !placeholder.AllowTags.Intersects(def.Tags))
                        continue;

                    if (!def.PassesMaxPerRoomConstraint(propsSpawnCounter.GetCount(def.Name)))
                        continue;

                    if (!def.PassesMaxPerRunConstraint(Session.CurrentRun.Spawns.GetCount(def.Name)))
                        continue;

                    // Intensidad eliminada.
                    var finalWeight = AdjustWeight(RoomNode.TopographicDifficulty, def.Difficulty, def.SpawnWeight);
                    table.Add(def.Name, finalWeight);
                }

                if (table.GetValue() is not ChanceTableItem item)
                    continue;

                if (PropDefinition.Definitions.Find(item.Name) is not PropDefinition chosen)
                    continue;

                usedPlaceholders.Add(placeholder);

                var instance = CreateThingClone<Prop>(chosen.Name);
                instance.Position = placeholder.Position;
                Children.Add(instance);

                Session.CurrentRun.Spawns.Increment(chosen.Name);
                propsSpawnCounter.Increment(chosen.Name);
            }
        }

        #endregion

        #region Protected members

        // AddPlaceholder
        protected void AddPlaceholder(Placeholder placeholder)
        {
            placeholders.Add(placeholder);
        }

        // AdjustWeight
        protected static float AdjustWeight(Difficulty roomDiff, Difficulty thingDiff, float baseWeight)
        {
            int distance = (int)roomDiff - (int)thingDiff;

            float roomMultiplier = distance switch
            {
                2 => 0.05f,   // Ej: Sala Hard (2), Enemigo Easy (0) -> Desalentamos apariciones tontas en el final
                1 => 0.25f,   // Ej: Sala Normal (1), Enemigo Easy (0)
                0 => 1.0f,    // Matching perfecto
                -1 => 0.10f,  // Out of Depth leve: Sala Easy, Enemigo Normal -> Sorpresa controlada
                -2 => 0.02f,  // Out of Depth severo: Sala Easy, Enemigo Hard -> Rareza extrema ("salvajada")
                _ => 1.0f
            };

            return baseWeight * roomMultiplier;
        }

        // GetCandidateDefinitions
        protected List<TDefinition> GetCandidateDefinitions<TDefinition, TThing>(IList<TDefinition> definitions, Func<TDefinition, bool>? predicate = null)
            where TDefinition : ThingDefinition where TThing : GameThing
        {
            var outList = new List<TDefinition>();
            if (Session.CurrentRun == null)
                return outList;

            foreach (var definition in definitions)
            {
                if (predicate != null && !predicate(definition))
                    continue;

                if (definition.RoomTheme.HasValue && definition.RoomTheme != RoomNode.Definition.Theme)
                    continue;

                if (definition.RequiresDeadEnd && RoomNode.ConnectionCount() > 1)
                    continue;

                var thing = Session.FindDeclaredThing(definition.Name) ?? throw new InvalidOperationException($"There is no declared thing named '{definition.Name}'. ");

                if (thing is not TThing)
                    continue;

                if (!definition.PassesRunConstraints(Session.RunCount))
                    continue;

                if (!definition.PassesMaxPerRunConstraint(Session.CurrentRun.Spawns))
                    continue;

                if (!TagScope.Test(RoomNode.Definition.Scope, RoomNode.Definition.Pools, RoomNode.Definition.Tags))
                    continue;

                outList.Add(definition);
            }

            return outList;
        }

        // OnChildAdded
        protected override void OnChildAdded(Entity child)
        {
            base.OnChildAdded(child);

            if (child is Sack)
                SackCount++;
        }

        // OnChildRemoved
        protected override void OnChildRemoved(Entity child)
        {
            base.OnChildAdded(child);

            if (child is Sack)
                SackCount--;
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();

            CustomWidth = (int)BoundingBox.Width;
            CustomHeight = (int)BoundingBox.Height;

            OnPopulating();
            Populate();
            OnPopulated();
        }

        // OnPopulating
        protected virtual void OnPopulating()
        {
        }

        // OnPopulated
        protected virtual void OnPopulated()
        {
        }

        // Random
        protected Random Random { get; }

        #endregion

        // CreateThingClone
        public T CreateThingClone<T>(string declaredName) where T : GameThing
        {
            if (Session.CreateThingClone(declaredName, $"{declaredName}*{RoomNode.Index}_{Name}_{instanceCount}") is not T result)
                throw new InvalidOperationException($"Failed to create runtime clone from'{declaredName}'.");

            instanceCount++;

            return result;
        }

        // IsProcedural
        public override bool IsProcedural => true;

        // Placeholders
        public ReadOnlyCollection<Placeholder> Placeholders { get; }

        // RoomNode
        public RoomNode RoomNode { get; }

        // SackCount
        public int SackCount { get; private set; }

        // ToString
        public override string ToString()
        {
            return RoomNode.ToString();
        }
    }
}