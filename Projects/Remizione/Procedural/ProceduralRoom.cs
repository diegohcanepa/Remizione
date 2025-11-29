using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
            this.Config = RoomConfig.GetConfig(StaticName);
            this.RoomGraph = roomGraph;

            this.AllowGlobalLight = true;
            this.LightingSystem = true;
            this.UnloadMode = Adberration.UnloadMode.Manual;

            int salt = roomGraph.Id;
            this.randomSeed = RandomHelper.GetSeed(Session.Seed, salt);
            this.Placeholders = new ReadOnlyCollection<Placeholder>(placeholders);
            this.Random = new Random(randomSeed);
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

        // FilterByRoomScope
        private List<T> FilterByRoomScope<T>(IList<T> configList, TagScope tagScope)
            where T : ThingConfig
        {
            var outList = new List<T>();

            foreach (var config in configList)
            {
                if (Session.GetStaticThing(config.Name) is null)
                    throw new InvalidOperationException($"There is no static thing named '{config.Name}'. ");

                // Run constraints
                if (!config.PassesRunConstraints(Session))
                    continue;

                // Tag scope
                if (!config.PassesTagScope(tagScope))
                    continue;

                // Passed all checks
                outList.Add(config);
            }

            return outList;
        }

        // GetEnemySpawnPoints
        private List<Vector2> GetEnemySpawnPoints(Rectangle area, int count, int cellSize)
        {
            var cells = new List<Vector2>();

            for (int y = area.Y; y < area.Bottom; y += cellSize)
            {
                for (int x = area.X; x < area.Right; x += cellSize)
                {
                    // Centro de la celda
                    var cx = x + cellSize * 0.5f;
                    var cy = y + cellSize * 0.5f;

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

        // AddWall
        protected void AddWall(string vertices)
        {
            var wall = new RideRoomWall(Session, "36,0;36,46;5,111;0,111;0,0");
            Children.Add(wall);
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
            var enemyList = new List<string>();

            // 1) Filter by room scope
            var filteredEnemies = FilterByRoomScope<EnemyConfig>(EnemyConfig.All, Config.EnemyScope);

            // Construir candidatos iterando props
            var candidates = new List<EnemyConfig>();
            for (int i = 0; i < filteredEnemies.Count; i++)
            {
                var p = filteredEnemies[i];

                // MaxPerRoom
                if (!p.PassesMaxPerRoomConstraint(spawnCounter.GetCount(p.Name)))
                    continue;

                // MaxPerRun
                if (!p.PassesMaxPerRunConstraint())
                    continue;

                candidates.Add(p);

                // Pick
                var chanceTable = new ChanceTable();
                foreach (var enemy in candidates)
                {
                    chanceTable.Add(enemy.Name, enemy.Weight);
                }

                if (chanceTable.GetValue() is not ChanceTableItem chanceTableItem)
                    continue;

                if (EnemyConfig.GetConfig(chanceTableItem.Name) is not EnemyConfig chosen)
                    continue;

                var spawnCount = Random.Next(1, p.MaxAmount + 1);
                for (var j = 0; j < spawnCount; j++)
                {
                    // Log spawn in room
                    spawnCounter.Increment(chosen.Name);

                    // Log spawn in run
                    RunManager.SpawnCounter.Increment(chosen.Name);

                    enemyList.Add(chosen.Name);

                    // MaxPerRoom
                    if (!p.PassesMaxPerRoomConstraint(spawnCounter.GetCount(chosen.Name)))
                        break;

                    // MaxPerRun
                    if (!p.PassesMaxPerRunConstraint())
                        break;
                }
            }

            if (WalkArea != null && enemyList.Count > 0)
            {
                var poly = new Polygon(WalkArea.Polygon.Vertices, -30);

                var spawnPoints = GetEnemySpawnPoints(poly.BoundingRectangle, enemyList.Count, 18);
                for (var i = 0; i < spawnPoints.Count; i++)
                {
                    var instance = CreateRuntimeThingCloneCore(enemyList[i]);
                    instance.Position = spawnPoints[i];
                    Children.Add(instance);
                }
            }
        }

        // PopulateProps
        private void PopulateProps()
        {
            // 1) Filter by room scope
            var filteredProps = FilterByRoomScope<PropConfig>(PropConfig.All, Config.PropScope);

            // 2) Shuffle placeholders
            var placeholders = new List<Placeholder>(Placeholders);
            placeholders.Shuffle(Random);

            // 3) Iterate placeholders
            for (int pi = 0; pi < placeholders.Count; pi++)
            {
                var ph = placeholders[pi];

                if (ph.Used)
                    continue;

                // Roll de fillChance (si falla, placeholder queda vacío)
                if (Random.NextDouble() > float.Clamp(ph.FillChance, 0, 1))
                    continue;

                // Construir candidatos iterando props
                var candidates = new List<PropConfig>();
                for (int i = 0; i < filteredProps.Count; i++)
                {
                    var p = filteredProps[i];

                    // placeholder.allowedTags (si existe) -> requiere intersección
                    if (ph.AllowTags.Count > 0)
                    {
                        if (!Utils.Intersects(ph.AllowTags, p.Tags))
                            continue;
                    }

                    // MaxPerRoom
                    if (!p.PassesMaxPerRoomConstraint(spawnCounter.GetCount(p.Name)))
                        continue;

                    // MaxPerRun
                    if (!p.PassesMaxPerRunConstraint())
                        continue;

                    candidates.Add(p);
                }

                if (candidates.Count == 0)
                    continue;

                // Pick
                var chanceTable = new ChanceTable();
                foreach (var prop in candidates)
                {
                    chanceTable.Add(prop.Name, prop.Weight);
                }

                if (chanceTable.GetValue() is not ChanceTableItem chanceTableItem)
                    continue;

                if (PropConfig.GetConfig(chanceTableItem.Name) is not PropConfig chosen)
                    continue;

                // Log spawn in room
                spawnCounter.Increment(chosen.Name);

                // Log spawn in run
                RunManager.SpawnCounter.Increment(chosen.Name);

                // Flag placeholder as used
                ph.Used = true;

                var instance = CreateRuntimeThingCloneCore(chosen.Name);
                instance.Position = ph.Polygon.BoundingRectangleF.GetPoint(RectanglePoint.Bottom);
                Children.Add(instance);
            }
        }

        // Random
        protected Random Random { get; }

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
        public override string ToString() => $"{GetType().Name}_{RoomGraph.Id}";
    }
}
