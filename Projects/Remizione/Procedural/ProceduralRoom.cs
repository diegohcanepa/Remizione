using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Remizione
{
    /// <summary>
    /// ProceduralRoom
    /// </summary>
    public abstract class ProceduralRoom : GameRoom
    {
        #region Private fields

        private int instanceCount;
        private readonly List<Placeholder> placeholders = [];
        private readonly int randomSeed;
        private readonly NamedCounter spawnCounter = new();

        #endregion

        #region Constructor

        // Constructor
        protected ProceduralRoom(GameSession session, string name, RoomGraph roomGraph)
            : base(session, name)
        {
            if (roomGraph.Config == null)
                throw new InvalidOperationException("RoomGraph has no room config assigned.");

            this.Config = RoomConfig.FindNotNull(roomGraph.Config.Name);
            this.RoomGraph = roomGraph;

            this.AllowGlobalLight = true;
            this.LightingSystem = true;
            this.UnloadMode = Adberration.UnloadMode.Manual;

            int salt = roomGraph.Id;
            this.randomSeed = RandomHelper.GetSeed(Session.Seed, salt);
            this.Placeholders = new ReadOnlyCollection<Placeholder>(placeholders);
            this.Random = new Random(randomSeed);

            // Apply placeholder overrides
            foreach (var phOverride in Config.PlaceholderOverrides)
            {
                if (GetPlaceholder(phOverride.Name) is Placeholder placeholder)
                    phOverride.Apply(placeholder);
            }
        }

        #endregion

        #region Private members

        // CreateRuntimeThingCloneCore
        private GameThing CreateRuntimeThingCloneCore(string staticName)
        {
            if (Session.CreateRuntimeThingClone(staticName, $"{staticName}*{RoomGraph.Id}_{Name}_{instanceCount}") is not GameThing result)
                throw new InvalidOperationException($"Failed to create runtime clone from'{staticName}'.");

            instanceCount++;

            return result;
        }

        // FilterByScope
        private List<ThingConfig> FilterByScope<T>(IList<ThingConfig> configList, ScopeRules scope)
            where T : GameThing
        {
            var outList = new List<ThingConfig>();

            foreach (var config in configList)
            {
                if (!Session.UnlockedPool.IsUnlocked(config.Name))
                    continue;

                var thing = Session.GetStaticThing(config.Name) ?? throw new InvalidOperationException($"There is no static thing named '{config.Name}'. ");

                // Is expected type?
                if (thing is not T)
                    continue;

                // Run constraints
                if (!config.PassesRunConstraints(Session))
                    continue;

                // Scope rules
                if (!config.PassesScope(scope))
                    continue;

                // Passed all checks
                outList.Add(config);
            }

            return outList;
        }

        // GetPlaceholder
        private Placeholder? GetPlaceholder(string name)
        {
            for (var i = 0; i < placeholders.Count; i++)
            {
                if (placeholders[i].Name == name)
                    return placeholders[i];
            }

            return null;
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

        #endregion

        #region Protected members

        // AddPlaceholder
        protected void AddPlaceholder(string name, float fillChance, bool flipImage, string vertices, params string[] allowTags)
        {
            for (var i = 0; i < placeholders.Count; i++)
            {
                if (placeholders[i].Name == name)
                    throw new InvalidOperationException("Duplicated name.");
            }

            var placeholder = new Placeholder(name, fillChance, flipImage, ReadOnlyPolygon.GetVertices(vertices), allowTags);
            placeholders.Add(placeholder);
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

        // Populate
        private void Populate()
        {
            PopulateProps();
            PopulateEnemies();
        }

        // PopulateEnemies
        private void PopulateEnemies()
        {
            var configList = FilterByScope<Enemy>(ThingConfig.All, Config.Scope);
            SpawnInPlaceholders(configList, Config.MaxEnemies, PlaceholderTarget.Enemy);
            SpawnInWalkArea(configList, Config.MaxEnemies);
        }

        // PopulateProps
        private void PopulateProps()
        {
            var configList = FilterByScope<Prop>(ThingConfig.All, Config.Scope);
            SpawnInPlaceholders(configList, Config.MaxProps, PlaceholderTarget.Prop);
            SpawnInWalkArea(configList, Config.MaxProps);
        }

        // Random
        protected Random Random { get; }

        // SpawnInPlaceholders
        private void SpawnInPlaceholders(IList<ThingConfig> configList, int maxInstances, PlaceholderTarget target)
        {
            // 1) Shuffle placeholders
            var placeholders = new List<Placeholder>(Placeholders);
            placeholders.Shuffle(Random);

            // 2) Iterate placeholders
            foreach (var placeholder in placeholders)
            {
                // Already used
                if (placeholder.Used)
                    continue;

                // Test placeholder target
                if (placeholder.Target != PlaceholderTarget.Any)
                {
                    if (placeholder.Target != target)
                        continue;
                }

                // Roll fillChance
                if (Random.NextDouble() > float.Clamp(placeholder.FillChance, 0, 1))
                    continue;

                // Collect candidates
                var candidates = new List<ThingConfig>();
                foreach (var config in configList)
                {
                    if (!config.UsePlaceholder)
                        continue;

                    // Allow tags
                    if (placeholder.AllowTags.Count > 0)
                    {
                        if (!Utils.Intersects(placeholder.AllowTags, config.Tags))
                            continue;
                    }

                    // MaxPerRoom
                    if (!config.PassesMaxPerRoomConstraint(spawnCounter.GetCount(config.Name)))
                        continue;

                    // MaxPerRun
                    if (!config.PassesMaxPerRunConstraint())
                        continue;

                    candidates.Add(config);
                }

                if (candidates.Count == 0)
                    continue;

                // Pick
                var chanceTable = new ChanceTable();
                foreach (var candidate in candidates)
                {
                    chanceTable.Add(candidate.Name, candidate.Weight);
                }

                if (chanceTable.GetValue() is not ChanceTableItem chanceTableItem)
                    continue;

                if (ThingConfig.Find(chanceTableItem.Name) is not ThingConfig chosen)
                    continue;

                // Log spawn in run
                RunManager.SpawnCounter.Increment(chosen.Name);

                // Flag placeholder as used
                placeholder.Used = true;

                var instance = CreateRuntimeThingCloneCore(chosen.Name);
                instance.Position = placeholder.Polygon.BoundingRectangleF.GetPoint(RectanglePoint.Bottom);
                Children.Add(instance);

                // Max per room
                if (maxInstances != -1 && spawnCounter.Increment(chosen.Name) >= maxInstances)
                    return;
            }
        }

        // SpawnInWalkArea
        private void SpawnInWalkArea(IList<ThingConfig> configList, int maxInstances)
        {
            var nameList = new List<string>();

            // Construir candidatos iterando props
            var candidates = new List<ThingConfig>();
            foreach (var config in configList)
            {
                if (config.UsePlaceholder)
                    continue;

                // MaxPerRoom
                if (!config.PassesMaxPerRoomConstraint(spawnCounter.GetCount(config.Name)))
                    continue;

                // MaxPerRun
                if (!config.PassesMaxPerRunConstraint())
                    continue;

                candidates.Add(config);

                // Pick
                var chanceTable = new ChanceTable();
                foreach (var candidate in candidates)
                {
                    chanceTable.Add(candidate.Name, candidate.Weight);
                }

                if (chanceTable.GetValue() is not ChanceTableItem chanceTableItem)
                    continue;

                if (ThingConfig.Find(chanceTableItem.Name) is not ThingConfig chosen)
                    continue;

                var spawnCount = Random.Next(config.MinSpawnAmount, config.MaxSpawnAmount + 1);
                for (var j = 0; j < spawnCount; j++)
                {
                    // Log spawn in run
                    RunManager.SpawnCounter.Increment(chosen.Name);

                    nameList.Add(chosen.Name);

                    // MaxPerRoom
                    if (!config.PassesMaxPerRoomConstraint(spawnCounter.Increment(chosen.Name)))
                        break;

                    // MaxPerRun
                    if (!config.PassesMaxPerRunConstraint())
                        break;
                }
            }

            if (WalkArea != null && nameList.Count > 0)
            {
                var poly = new Polygon(WalkArea.Polygon.Vertices, -30);

                var spawnPoints = GetSpawnPoints(poly.BoundingRectangle, nameList.Count, 18);
                for (var i = 0; i < spawnPoints.Count; i++)
                {
                    var instance = CreateRuntimeThingCloneCore(nameList[i]);
                    instance.Position = spawnPoints[i];
                    Children.Add(instance);
                }
            }
        }

        #endregion

        // Config
        public RoomConfig Config { get; }

        // CreateRuntimeClone
        public GameThing? CreateRuntimeClone(string staticName)
        {
            return CreateRuntimeThingCloneCore(staticName);
        }

        // Placeholders
        public ReadOnlyCollection<Placeholder> Placeholders { get; }

        // RoomGraph
        public RoomGraph RoomGraph { get; }

        // ToString
        public override string ToString()
        {
            return $"{GetType().Name}_{RoomGraph.Id}";
        }
    }
}
