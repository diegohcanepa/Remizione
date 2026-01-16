using Engendro;
using Microsoft.Xna.Framework;
using ScaryCastle.Procedural;
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

        private readonly MultiCounter enemiesSpawnCounter = new();
        private int instanceCount;
        private readonly List<Placeholder> placeholders = [];
        private readonly MultiCounter propsSpawnCounter = new();
        private readonly int randomSeed;

        #endregion

        #region Constructor

        // Constructor
        protected ProceduralRoom(GameSession session, string name, RoomGraph roomGraph)
            : base(session, name)
        {
            if (roomGraph.Definition == null)
                throw new InvalidOperationException("RoomGraph has no room definition assigned.");

            this.Definition = RoomDefinition.Get(roomGraph.Definition.Name);
            this.RoomGraph = roomGraph;

            this.AllowGlobalLight = true;
            this.LightingSystem = true;
            this.UnloadMode = Adberration.UnloadMode.Manual;

            int salt = roomGraph.Index;
            this.randomSeed = RandomHelper.GetSeed(Session.Seed, salt);
            this.Random = new Random(randomSeed);
            this.Placeholders = placeholders.AsReadOnly();
        }

        #endregion

        #region Private members

        // AdjustWeightByDifficulty
        private static float AdjustWeightByDifficulty(Difficulty roomDifficulty, Difficulty thingDifficulty, float thingWeight)
        {
            float finalWeight = thingWeight;

            // Si el cuarto es Difícil, bajamos la chance de los "Flojitos"
            if (roomDifficulty == Difficulty.Hard && thingDifficulty == Difficulty.Easy)
            {
                finalWeight *= .2f; // El multiplicador bizarro
            }
            // Si el cuarto es Difícil y el enemigo también, lo potenciamos
            else if (roomDifficulty == Difficulty.Hard && thingDifficulty == Difficulty.Hard)
            {
                finalWeight *= 2.5f;
            }

            return finalWeight;
        }

        // ApplyPrimaryFilter
        private List<ThingDefinition> ApplyPrimaryFilter<T>(IList<ThingDefinition> definitions)
            where T : GameThing
        {
            var outList = new List<ThingDefinition>();

            foreach (var definition in definitions)
            {
                // Filtro Techo: No permitimos que aparezcan cosas más difíciles que el cuarto
                if (definition.Difficulty > Definition.Difficulty)
                    continue;

                // Thing requires a dead end room
                if (definition.RequiresDeadEnd && RoomGraph.GetConnectionCount() > 1)
                    continue;

                var thing = Session.FindDeclaredThing(definition.Name) ?? throw new InvalidOperationException($"There is no static thing named '{definition.Name}'. ");

                // Is expected type?
                if (thing is not T)
                    continue;

                // Run constraints
                if (!definition.PassesFloorConstraints(Session))
                    continue;

                // Scope rules
                if (!definition.PassesScope(Definition.Scope))
                    continue;

                // Passed all checks
                outList.Add(definition);
            }

            return outList;
        }

        // CreateRuntimeThingCloneCore
        private GameThing CreateRuntimeThingCloneCore(string staticName)
        {
            if (Session.CreateRuntimeThingClone(staticName, $"{staticName}*{RoomGraph.Index}_{Name}_{instanceCount}") is not GameThing result)
                throw new InvalidOperationException($"Failed to create runtime clone from'{staticName}'.");

            instanceCount++;

            return result;
        }

        // GetSpawnPoints
        private List<Vector2> GetSpawnPoints(Rectangle area, int count, int cellSize)
        {
            var cells = new List<Vector2>();

            for (int y = area.Y; y < area.Bottom; y += cellSize)
            {
                for (int x = area.X; x < area.Right; x += cellSize)
                {
                    // Centro de la celda
                    var cx = x + (cellSize * 0.5f);
                    var cy = y + (cellSize * 0.5f);

                    // Solo agregamos si el centro cae dentro
                    if (cx >= area.Left && cx <= area.Right &&
                        cy >= area.Top && cy <= area.Bottom)
                    {
                        cells.Add(new Vector2(cx, cy));
                    }
                }
            }

            // Mezclamos
            for (int i = cells.Count - 1; i > 0; i--)
            {
                int j = Random.Next(i + 1);
                (cells[i], cells[j]) = (cells[j], cells[i]);
            }

            // Devolvemos solo los que pidieron
            if (cells.Count > count)
                cells.RemoveRange(count, cells.Count - count);

            return cells;
        }

        // Populate
        private void Populate()
        {
            PopulateProps();
            PopulateEnemies();
        }

        // PopulateEnemies
        private void PopulateEnemies()
        {
            var definitions = ApplyPrimaryFilter<Enemy>(ThingDefinition.All);
            SpawnInPlaceholders(definitions, Definition.MaxEnemies, enemiesSpawnCounter, PlaceholderTarget.Enemy);
            SpawnInWalkArea(definitions, Definition.MaxEnemies, enemiesSpawnCounter);
        }

        // PopulateProps
        private void PopulateProps()
        {
            var definitions = ApplyPrimaryFilter<Prop>(ThingDefinition.All);
            SpawnInPlaceholders(definitions, Definition.MaxProps, propsSpawnCounter, PlaceholderTarget.Prop);
            SpawnInWalkArea(definitions, Definition.MaxProps, propsSpawnCounter);
        }

        // SpawnInPlaceholders
        private void SpawnInPlaceholders(IList<ThingDefinition> definitions, int maxInstances, MultiCounter spawnCounter, PlaceholderTarget target)
        {
            if (Placeholders.Count == 0 || maxInstances == 0)
                return;

            // 1) Shuffle placeholders
            var placeholders = new List<Placeholder>(Placeholders);
            placeholders.Shuffle(Random);

            // 2) Iterate placeholders
            foreach (var placeholder in placeholders)
            {
                // Already used
                if (placeholder.Used)
                    continue;

                // Functional filter (Prop vs Enemy)
                if (placeholder.Target != PlaceholderTarget.Any && placeholder.Target != target)
                    continue;

                // Roll fillChance
                if (!placeholder.FillChance.Roll(Random))
                    continue;

                // Collect candidates
                var candidates = new List<ThingDefinition>();
                foreach (var definition in definitions)
                {
                    // Is compatible with placehokder placement?
                    if (!definition.Placements.Contains(placeholder.Placement))
                        continue;

                    // MaxPerRoom
                    if (!definition.PassesMaxPerRoomConstraint(spawnCounter.GetCount(definition.Name)))
                        continue;

                    // MaxPerRun
                    if (!definition.PassesMaxPerRunConstraint())
                        continue;

                    candidates.Add(definition);
                }

                if (candidates.Count == 0)
                    continue;

                // Pick
                var chanceTable = new ChanceTable();
                foreach (var c in candidates)
                {
                    var finalWeight = AdjustWeightByDifficulty(Definition.Difficulty, c.Difficulty, c.SpawnWeight);
                    chanceTable.Add(c.Name, finalWeight);
                }

                if (chanceTable.GetValue() is not ChanceTableItem chanceTableItem)
                    continue;

                if (ThingDefinition.Find(chanceTableItem.Name) is not ThingDefinition chosen)
                    continue;

                // Log spawn in run
                RunManager.SpawnCounter.Increment(chosen.Name);
                spawnCounter.Increment(chosen.Name);

                // Flag placeholder as used
                placeholder.Used = true;

                var instance = CreateRuntimeThingCloneCore(chosen.Name);
                instance.Position = placeholder.Position;
                Children.Add(instance);

                // Max per room
                if (maxInstances != -1 && spawnCounter.Increment(chosen.Name) >= maxInstances)
                    return;
            }
        }

        // SpawnInWalkArea
        private void SpawnInWalkArea(IList<ThingDefinition> definitions, int maxInstances, MultiCounter spawnCounter)
        {
            if (WalkArea == null || maxInstances == 0)
                return;

            // 1) Collect candidates
            var candidates = new List<ThingDefinition>();
            foreach (var definition in definitions)
            {
                // Allowed if list is empty or contains WalkAea enum value
                if (!definition.Placements.Contains(PlacementType.WalkArea))
                    continue;

                if (!definition.PassesMaxPerRoomConstraint(spawnCounter.GetCount(definition.Name)))
                    continue;

                if (!definition.PassesMaxPerRunConstraint())
                    continue;

                candidates.Add(definition);
            }

            if (candidates.Count == 0)
                return;

            // 2) Build chance table
            var table = new ChanceTable();
            foreach (var c in candidates)
            {
                var finalWeight = AdjustWeightByDifficulty(Definition.Difficulty, c.Difficulty, c.SpawnWeight);
                table.Add(c.Name, finalWeight);
            }

            var spawnedNames = new List<string>();
            int remainingInstances = maxInstances;

            // Corte blando
            var continueChance = 1f;
            const float decay = 0.7f; // ajustable

            // seguridad
            int safety = candidates.Count;

            // 3) Pick groups
            while (table.Count > 0 && safety-- > 0)
            {
                if (maxInstances > 0 && remainingInstances <= 0)
                    break;

                // roll de continuación (solo si es infinito)
                if (maxInstances <= 0)
                {
                    if (Random.NextDouble() > continueChance)
                        break;

                    continueChance *= decay;
                }

                if (table.GetValue() is not ChanceTableItem item)
                    break;

                table.Remove(item.Name);

                if (ThingDefinition.Find(item.Name) is not ThingDefinition chosen)
                    continue;

                int min = Math.Max(1, chosen.MinSpawnAmount);
                int max = Math.Max(min, chosen.MaxSpawnAmount);
                int amount = Random.Next(min, max + 1);

                // respetar maxInstances si existe
                if (maxInstances > 0)
                    amount = Math.Min(amount, remainingInstances);

                for (int i = 0; i < amount; i++)
                {
                    spawnCounter.Increment(chosen.Name);
                    RunManager.SpawnCounter.Increment(chosen.Name);
                    spawnedNames.Add(chosen.Name);

                    if (maxInstances > 0)
                        remainingInstances--;
                }
            }

            if (spawnedNames.Count == 0)
                return;

            // 4) Spawn positions
            var poly = new Polygon(WalkArea.Polygon.Vertices, -30);
            var points = GetSpawnPoints(poly.BoundingRectangle, spawnedNames.Count, 18);

            for (int i = 0; i < points.Count; i++)
            {
                var instance = CreateRuntimeThingCloneCore(spawnedNames[i]);
                instance.Position = points[i];
                Children.Add(instance);
            }
        }

        #endregion

        #region Protected members

        // AddPlaceholder
        protected void AddPlaceholder(Placeholder placeholder)
        {
            placeholders.Add(placeholder);
        }

        // DropLoot
        protected virtual void DropLoot()
        {
        }

        // GetDropLootPosition
        protected Vector2 GetDropLootPosition()
        {
            return WalkArea != null ? WalkArea.Polygon.BoundingRectangleF.Center : BoundingBox.Center;
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

        // CreateRuntimeClone
        public GameThing? CreateRuntimeClone(string staticName)
        {
            return CreateRuntimeThingCloneCore(staticName);
        }

        // Definition
        public RoomDefinition Definition { get; }

        // IsProcedural
        public override bool IsProcedural => true;

        // Placeholders
        public ReadOnlyCollection<Placeholder> Placeholders { get; }

        // RoomGraph
        public RoomGraph RoomGraph { get; }

        // ToString
        public override string ToString()
        {
            return RoomGraph.ToString();
        }
    }
}
