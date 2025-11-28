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

        #endregion

        #region Constructor

        // Constructor
        protected ProceduralRoom(GameSession session, string name, RoomGraph roomGraph)
            : base(session, name)
        {
            this.Config = RoomConfig.GetConfig(StaticName);
            this.RoomGraph = roomGraph;

            AllowGlobalLight = true;
            LightingSystem = true;
            UnloadMode = Adberration.UnloadMode.Manual;

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

        // FilterPropsByRoomScope
        private List<PropConfig> FilterPropsByRoomScope(IList<Prop> props)
        {
            var outList = new List<PropConfig>();
            var scope = Config.PropScopeRule;

            foreach (var prop in props)
            {
                if (PropConfig.GetConfig(prop.StaticName) is not PropConfig propConfig)
                    continue;

                // Run constraints
                if (!PassesRunConstraints(propConfig))
                    continue;

                // DenyPools
                if (scope.DenyPools.Count > 0)
                {
                    if (Intersects(scope.DenyPools, propConfig.Pools))
                        continue;
                }

                // DenyTags
                if (scope.DenyTags.Count > 0)
                {
                    if (Intersects(scope.DenyTags, propConfig.Tags))
                        continue;
                }

                // AllowPools (si existe, requiere intersección)
                if (scope.AllowPools.Count > 0)
                {
                    if (!Intersects(scope.AllowPools, propConfig.Pools))
                        continue;
                }
                else
                {
                    // AllowTags VACÍO -> aceptar todo (equivalente a "any")
                    if (scope.AllowTags.Count > 0)
                    {
                        // si hay al menos una tag en allow, requerimos intersección
                        if (!Intersects(scope.AllowTags, propConfig.Tags))
                            continue;
                    }

                    // si AllowTags está vacío o es null, no filtramos por tags (aceptamos)
                }

                // Passed all checks
                outList.Add(propConfig);
            }

            return outList;
        }

        // Intersects
        private static bool Intersects(ReadOnlyCollection<string> listA, ReadOnlyCollection<string> listB)
        {
            if (listA.Count == 0 || listB.Count == 0)
                return false;

            for (int i = 0; i < listA.Count; i++)
            {
                var va = listA[i];

                for (int j = 0; j < listB.Count; j++)
                {
                    if (string.Equals(va, listB[j], StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }

            return false;
        }

        // PassesRunConstraints
        private bool PassesRunConstraints(PropConfig propConfig)
        {
            // MaxPerRun
            if (propConfig.MaxPerRun > 0)
            {
                int spawnedRun = RunManager.GetSpawnCount(propConfig.Name);
                if (spawnedRun >= propConfig.MaxPerRun)
                    return false;
            }

            // RequiredRuns
            if (propConfig.RequiredRuns > 0)
            {
                if (Session.TotalRuns < propConfig.RequiredRuns)
                    return false;
            }

            // RequiredCompletedRuns
            if (propConfig.RequiredCompletedRuns > 0)
            {
                if (Session.CompletedRuns < propConfig.RequiredCompletedRuns)
                    return false;
            }

            return true;
        }

        #endregion

        #region Protected members

        // AddPlaceholder
        protected void AddPlaceholder(string name, float fillChance, bool flipImage, string vertices, params string[] allowedTags)
        {
            for (var i = 0; i < placeholders.Count; i++)
            {
                if (placeholders[i].Name == name)
                    throw new InvalidOperationException("Duplicated name.");
            }

            var placeholder = new Placeholder(name, fillChance, flipImage, ReadOnlyPolygon.GetVertices(vertices), allowedTags);
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
            // 1) Filter by room scope
            var filteredProps = FilterPropsByRoomScope(Session.StaticProps);

            // 2) Initialize spawnedCounts
            var spawnedCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < filteredProps.Count; i++)
            {
                spawnedCounts[filteredProps[i].Name] = 0;
            }

            // 3) Shuffle placeholders
            var placeholders = new List<Placeholder>(Placeholders);
            placeholders.Shuffle(Random);

            // 4) Iterate placeholders
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
                    if (ph.AllowedTags.Count > 0)
                    {
                        if (!Intersects(ph.AllowedTags, p.Tags))
                            continue;
                    }

                    // MaxPerRoom: <=0 => ilimitado; >0 chequeamos contador
                    if (p.MaxPerRoom > 0)
                    {
                        spawnedCounts.TryGetValue(p.Name, out var spawned);
                        if (spawned >= p.MaxPerRoom)
                            continue;
                    }

                    // MaxPerRun
                    if (p.MaxPerRun > 0)
                    {
                        var spawnedCount = RunManager.GetSpawnCount(p.Name);
                        if (spawnedCount >= p.MaxPerRun)
                            continue;
                    }

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
                if (chosen.MaxPerRoom > 0)
                {
                    spawnedCounts.TryGetValue(chosen.Name, out var prev);
                    spawnedCounts[chosen.Name] = prev + 1;
                }

                // Log spawn in run
                RunManager.LogSpawn(chosen.Name);

                // marcar placeholder usado
                ph.Used = true;

                var instance = CreateRuntimeThingCloneCore(chosen.Name);
                instance.Position = ph.Polygon.BoundingRectangleF.GetPoint(RectanglePoint.Bottom);
                Children.Add(instance);
            }
        }

        // Random
        protected Random Random { get; }

        #endregion

        // CanPlaceThingAt
        public bool CanPlaceThingAt(GameThing thing, Vector2 position)
        {
            if (!thing.Collider.IsEmpty)
            {
                var box = new RectangleF(position, thing.Collider.BoundingRectangleF.Size);

                for (int i = 0; i < CulledThings.Count; i++)
                {
                    if (CulledThings[i] == thing)
                        continue;

                    if (CulledThings[i] is IHoleArea holeArea)
                    {
                        if (holeArea.Polygon.BoundingRectangleF.Intersects(box))
                            return false;
                    }
                }
            }

            return true;
        }

        // Config
        public RoomConfig Config { get; }

        // CreateRuntimeClone
        public GameThing? CreateRuntimeClone(string staticName)
        {
            return CreateRuntimeThingCloneCore(staticName);
        }

        // Placeholders
        public ReadOnlyCollection<Placeholder> Placeholders { get; }

        // PlaceRuntimeCloneAt
        public GameThing? PlaceRuntimeCloneAt(GameThing thing, Vector2 position)
        {
            GameThing? result = null;

            if (CanPlaceThingAt(thing, position))
            {
                result = Session.CreateRuntimeThingClone(thing.StaticName, string.Empty) as GameThing;
                if (result != null)
                {
                    result.Position = position;
                    Children.Add(result);
                }
                else
                    return null;
            }

            return result;
        }

        // RoomGraph
        public RoomGraph RoomGraph { get; }

        // ToString
        public override string ToString() => $"ProcRoom_{RoomGraph.Id}";
    }
}
