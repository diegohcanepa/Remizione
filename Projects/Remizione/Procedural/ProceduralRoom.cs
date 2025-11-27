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

        // FilterPropsByPlaceholder
        private List<Prop> GetStaticProps(Placeholder placeholder)
        {
            var result = new List<Prop>();

            foreach (var thing in Session.StaticThings)
            {
                // Is a Prop?
                if (thing is not Prop prop)
                    continue;

                // Has config data?
                if (PropConfig.GetConfig(prop.StaticName) is not PropConfig propConfig)
                    continue;

                // Match placeholder type?
                // TODO: PlaceholderType y PlaceholderSize podrian ir a Prop y sacarlos de GameThing
                if (prop.PlaceholderType != placeholder.PlaceholderType)
                    continue;

                // Match placeholder size?
                if (placeholder.Size != PlaceholderSize.Any && prop.PlaceholderSize != placeholder.Size)
                    continue;

                // Tags
                if (placeholder.AllowedTags.Count > 0)
                {
                    var tagMatched = false;

                    for (int i = 0; i < placeholder.AllowedTags.Count; i++)
                    {
                        string reqTag = placeholder.AllowedTags[i];

                        for (int j = 0; j < propConfig.Tags.Count; j++)
                        {
                            if (StringEquals(reqTag, propConfig.Tags[j]))
                            {
                                tagMatched = true;
                                break;
                            }
                        }

                        if (tagMatched)
                            break;
                    }

                    if (!tagMatched)
                        continue;
                }

                var propScope = Config.PropScopeRule;

                // DenyPools
                if (propScope.DenyPools.Count > 0 && propConfig.Pools.Count > 0)
                {
                    bool denied = false;

                    for (int i = 0; i < propScope.DenyPools.Count; i++)
                    {
                        for (int j = 0; j < propConfig.Pools.Count; j++)
                        {
                            if (StringEquals(propScope.DenyPools[i], propConfig.Pools[j]))
                            {
                                denied = true;
                                break;
                            }
                        }

                        if (denied)
                            break;
                    }

                    if (denied)
                        continue;
                }

                // DenyTags
                if (propScope.DenyTags.Count > 0)
                {
                    var denied = false;

                    for (int i = 0; i < propScope.DenyTags.Count; i++)
                    {
                        for (int j = 0; j < propConfig.Tags.Count; j++)
                        {
                            if (StringEquals(propScope.DenyTags[i], propConfig.Tags[j]))
                            {
                                denied = true;
                                break;
                            }
                        }

                        if (denied)
                            break;
                    }

                    if (denied)
                        continue;
                }

                // AllowPools (if present, require intersection)
                if (propScope.AllowPools.Count > 0)
                {
                    var ok = false;

                    if (propConfig.Pools.Count > 0)
                    {
                        for (int i = 0; i < propScope.AllowPools.Count; i++)
                        {
                            for (int j = 0; j < propConfig.Pools.Count; j++)
                            {
                                if (StringEquals(propScope.AllowPools[i], propConfig.Pools[j]))
                                {
                                    ok = true;
                                    break;
                                }
                            }

                            if (ok)
                                break;
                        }
                    }

                    if (!ok)
                        continue;
                }
                else
                {
                    // AllowTags: if list not empty, require tag intersection
                    if (propScope.AllowTags.Count > 0)
                    {
                        var ok = false;

                        for (int i = 0; i < propScope.AllowTags.Count; i++)
                        {
                            for (int j = 0; j < propConfig.Tags.Count; j++)
                            {
                                if (StringEquals(propScope.AllowTags[i], propConfig.Tags[j]))
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

                /*
                // MaxPerRoom
                if (prop.MaxPerRoom > 0)
                {
                    int spawned = 0;
                    roomInstance.SpawnedCounts.TryGetValue(prop.Name, out spawned);
                    if (spawned >= prop.MaxPerRoom) continue;
                }
                */

                result.Add(prop);
            }

            return result;
        }

        // FilterPropsByRoomScope
        private List<PropConfig> FilterPropsByRoomScope(List<Prop> props)
        {
            var outList = new List<PropConfig>();
            var scope = Config.PropScopeRule;

            foreach (var prop in props)
            {
                if (PropConfig.GetConfig(prop.StaticName) is not PropConfig p)
                    continue;

                // DenyPools
                if (scope.DenyPools.Count > 0 && p.Pools != null)
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
                if (scope.DenyTags.Count > 0)
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
                if (scope.AllowPools.Count > 0)
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
                    if (scope.AllowTags.Count > 0)
                    {
                        var ok = false;
                        for (int a = 0; a < scope.AllowTags.Count; a++)
                        {
                            for (int pt = 0; pt < p.Tags.Count; pt++)
                            {
                                if (StringEquals(scope.AllowTags[a], p.Tags[pt]))
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
                outList.Add(p);
            }

            return outList;
        }

        // PopulateThing
        private void PopulateThing(GameThing thing, PlacementData placementData)
        {
            var count = Random.Next(placementData.Rolls.Minimum, placementData.Rolls.Maximum + 1);

            /*
            for (int i = 0; i < count; i++)
            {
                SpawnThing(thing, finalCol, finalRow, placementData);
                placementData.LogSpawn(thing.StaticName);
                if (!placementData.CanSpawn(thing.StaticName))
                    return;
            }
            */
        }

        // SpawnThing
        private void SpawnThing(GameThing thing, int col, int row, PlacementData placementData)
        {
            /*
            var sizeInCells = grid.GetRequiredGridSpace(thing);
            var ltPos = grid.GetPixelArea(col, row).GetPoint(RectanglePoint.LeftTop) + new Vector2(.5f);
            var rect = new RectangleF(ltPos.X, ltPos.Y, sizeInCells.Width * grid.CellSize, sizeInCells.Height * grid.CellSize);
            var instance = CreateRuntimeThingCloneCore(thing.StaticName);
            instance.Position = rect.GetPoint(RectanglePoint.Bottom);
            Children.Add(instance);

            if (placementData.MaximumPerRun > 0)
                RunManager.LogSpawn(thing.StaticName);
            */
        }

        // StringEquals
        private bool StringEquals(string a, string b)
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
            var placeholders = new List<Placeholder>(Placeholders);
            placeholders.Shuffle(Random);

            // Iterate placeholders
            foreach (var placeholder in placeholders)
            {
                // Roll fill chance
                if (Random.NextDouble() > placeholder.FillChance)
                    continue;

                // Get available candidates for the placeholder
                var staticProps = GetStaticProps(placeholder);
                staticProps.Shuffle(Random);


                // Filtered things
                foreach (var thing in staticProps)
                {
                    var placementDataList = data.GetList(thing.StaticName);
                    if (placementDataList == null)
                        continue;

                    for (var i = 0; i < placementDataList.Count; i++)
                    {
                        var placementData = placementDataList[i];

                        if (!placementData.IsAvailable(this, thing, Random))
                            continue;

                        var instance = CreateRuntimeThingCloneCore(thing.StaticName);
                        instance.Position = placeholder.Polygon.BoundingRectangleF.GetPoint(RectanglePoint.Bottom);
                        Children.Add(instance);
                        placeholder.Used = true;
                        break;
                    }
                }
            }
        }

        // Random
        protected Random Random { get; }

        #endregion

        // AddPlaceholder
        public Placeholder AddPlaceholder(string name, PlaceholderType type, PlaceholderSize size, float fillChance, bool flipImage, string vertices, string[] allowedTags)
        {
            var result = new Placeholder(name, type, size, fillChance, flipImage, ReadOnlyPolygon.GetVertices(vertices), allowedTags);
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
