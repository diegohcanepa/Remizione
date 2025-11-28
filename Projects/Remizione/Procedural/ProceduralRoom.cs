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
            var result = new List<PropConfig>();
            var scope = Config.PropScopeRule;

            foreach (var prop in props)
            {
                if (PropConfig.GetConfig(prop.StaticName) is not PropConfig p)
                    continue;

                // DenyPools
                if (scope.DenyPools.Count > 0 && p.Pools.Count > 0)
                {
                    var skip = false;
                    for (int i = 0; i < scope.DenyPools.Count; i++)
                    {
                        for (int j = 0; j < p.Pools.Count; j++)
                        {
                            if (StringEquals(scope.DenyPools[i], p.Pools[j]))
                            {
                                skip = true;
                                break;
                            }
                        }

                        if (skip)
                            break;
                    }

                    if (skip)
                        continue;
                }

                // denyTags
                if (scope.DenyTags.Count > 0 && p.Tags.Count > 0)
                {
                    var skip = false;
                    for (int i = 0; i < scope.DenyTags.Count; i++)
                    {
                        for (int j = 0; j < p.Tags.Count; j++)
                        {
                            if (StringEquals(scope.DenyTags[i], p.Tags[j]))
                            {
                                skip = true;
                                break;
                            }
                        }

                        if (skip)
                            break;
                    }

                    if (skip)
                        continue;
                }

                // AllowPools (if present require intersection)
                if (scope.AllowPools.Count > 0 && p.Pools.Count > 0)
                {
                    var ok = false;

                    for (int i = 0; i < scope.AllowPools.Count; i++)
                    {
                        for (int j = 0; j < p.Pools.Count; j++)
                        {
                            if (StringEquals(scope.AllowPools[i], p.Pools[j]))
                            {
                                ok = true;
                                break;
                            }
                        }

                        if (ok)
                            break;
                    }

                    if (!ok)
                        continue;
                }
                else
                {
                    // AllowTags: if contains "any" accept; else require intersection if list not empty
                    if (scope.AllowTags.Count > 0 && p.Tags.Count > 0)
                    {
                        var ok = false;
                        for (int i = 0; i < scope.AllowTags.Count; i++)
                        {
                            for (int j = 0; j < p.Tags.Count; j++)
                            {
                                if (StringEquals(scope.AllowTags[i], p.Tags[j]))
                                {
                                    ok = true;
                                    break;
                                }
                            }

                            if (ok)
                                break;
                        }

                        if (!ok)
                            continue;
                    }
                }

                // Passed all checks
                result.Add(p);
            }

            return result;
        }

        // StringEquals
        private static bool StringEquals(string a, string b)
        {
            return string.Compare(a, b, StringComparison.OrdinalIgnoreCase) == 0;
        }

        #endregion

        #region Protected members

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
                        bool ok = false;
                        for (int t = 0; t < ph.AllowedTags.Count; t++)
                        {
                            for (int pt = 0; pt < p.Tags.Count; pt++)
                            {
                                if (StringEquals(ph.AllowedTags[t], p.Tags[pt])) { ok = true; break; }
                            }
                            if (ok) break;
                        }
                        if (!ok) continue;
                    }

                    // MaxPerRoom: <=0 => ilimitado; >0 chequeamos contador
                    if (p.MaxPerRoom > 0)
                    {
                        int spawned = 0;
                        spawnedCounts.TryGetValue(p.Name, out spawned);
                        if (spawned >= p.MaxPerRoom) continue;
                    }

                    candidates.Add(p);
                }

                if (candidates.Count == 0)
                    continue;

                // Pick
                var chanceTable = new ChanceTable();
                foreach (var prop in candidates)
                {
                    chanceTable.Add(prop.Name, 1, prop.Weight);
                }

                if (chanceTable.GetValue() is not ChanceTableItem chanceTableItem)
                    continue;

                if (PropConfig.GetConfig(chanceTableItem.Name) is not PropConfig chosen)
                    continue;

                // incrementar contador si aplica
                if (chosen.MaxPerRoom > 0)
                {
                    int prev = 0;
                    spawnedCounts.TryGetValue(chosen.Name, out prev);
                    spawnedCounts[chosen.Name] = prev + 1;
                }

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

        // AddPlaceholder
        public Placeholder AddPlaceholder(string name, float fillChance, bool flipImage, string vertices, params string[] allowedTags)
        {
            var result = new Placeholder(name, fillChance, flipImage, ReadOnlyPolygon.GetVertices(vertices), allowedTags);
            placeholders.Add(result);
            return result;
        }

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
