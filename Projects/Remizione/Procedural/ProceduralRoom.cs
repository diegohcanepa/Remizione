using Adberration;
using Engendro;
using Engendro.Collections;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Remizione
{
    /// <summary>
    /// ProceduralRoom
    /// </summary>
    public class ProceduralRoom : GameRoom
    {
        #region Private fields

        private readonly CounterBank actorsSpawnCounter = new();
        private readonly List<Placeholder> placeholders = [];
        private readonly CounterBank propsSpawnCounter = new();
        private readonly List<Vector2> occupiedPositions = [];

        #endregion

        #region Constructor

        // Constructor
        public ProceduralRoom(GameSession session, string name)
            : base(session, name)
        {
            this.Definition = GameData.Rooms.Get(name);

            this.LightingSystem = true;
            this.UnloadMode = UnloadMode.Manual;
            this.DustParticleKind = DustParticleKind.Ash;
            this.placeholders.AddRange(Definition.Placeholders);
        }

        #endregion

        #region Private members

        // GetCandidateDefinitions
        private List<TDefinition> GetCandidateDefinitions<TDefinition>(IList<TDefinition> definitions)
            where TDefinition : ThingDefinition
        {
            var outList = new List<TDefinition>();

            foreach (var definition in definitions)
            {
                if (definition.SpawnWeight == 0)
                    continue;

                if (!Session.IsUnlocked(definition))
                    continue;

                if (Session.GetProceduralThing(definition.Name) == null)
                    continue;

                if (!TagScope.Test(Definition.Scope, Definition.Pools, definition.Tags))
                    continue;

                outList.Add(definition);
            }

            return outList;
        }

        // GetSpawnPoints
        private List<Vector2> GetSpawnPoints(Polygon polygon, int count, int cellSize, Random rng)
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
                    cx += (float)((rng.NextDouble() * offsetRange * 2) - offsetRange);
                    cy += (float)((rng.NextDouble() * offsetRange * 2) - offsetRange);

                    var candidate = new Vector2(cx, cy);

                    if (polygon.Contains(candidate))
                    {
                        cells.Add(candidate);
                    }
                }
            }

            for (int i = cells.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (cells[i], cells[j]) = (cells[j], cells[i]);
            }

            if (cells.Count > count)
                cells.RemoveRange(count, cells.Count - count);

            return cells;
        }

        // PopulatePlaceholders
        private int PopulatePlaceholders<TDefinition>(
            List<Placeholder> targetPlaceholders,
            IList<TDefinition> candidateDefs,
            CounterBank counterBank)
            where TDefinition : ThingDefinition
        {
            if (targetPlaceholders.Count == 0 || candidateDefs.Count == 0)
                return 0;

            int spawnedCount = 0;
            var shuffled = new List<Placeholder>(targetPlaceholders);
            shuffled.Shuffle(Session.Random);

            for (int i = 0; i < shuffled.Count; i++)
            {
                var ph = shuffled[i];

                // 1. CONDICIÓN DE SLOT
                if (ph.SpawnRule != PlaceholderSpawnRule.ContentChanceOnly)
                {
                    if (!ph.FillChance.Roll(Session.Random))
                        continue;
                }

                // 2. FILTRADO Y EVALUACIÓN
                var phTable = new ChanceTable();

                for (int j = 0; j < candidateDefs.Count; j++)
                {
                    var def = candidateDefs[j];

                    if (!def.RequiresPlaceholder)
                        continue;

                    if (ph.AllowTags.Count > 0 && !ph.AllowTags.Intersects(def.Tags))
                        continue;

                    if (!def.PassesMaxPerRoomConstraint(counterBank.GetCount(def.Name)))
                        continue;

                    if (ph.SpawnRule != PlaceholderSpawnRule.PlaceholderChanceOnly)
                    {
                        if (Session.Random.NextSingle() > MathHelper.Clamp(def.SpawnWeight, 0f, 1f))
                            continue;
                    }

                    if (def.SpawnWeight > 0f)
                        phTable.Add(def.Name, (int)def.SpawnWeight, def);
                }

                if (phTable.Count == 0)
                    continue;

                // 3. INSTANCIACIÓN FINAL
                if (phTable.GetItem(Session.Random)?.Context is TDefinition chosen)
                {
                    SpawnThing<GameThing>(chosen.Name, ph.Position, counterBank);
                    spawnedCount++;
                }
            }

            return spawnedCount;
        }

        // SpawnActors
        private void SpawnActors()
        {
            var candidateDefs = GetCandidateDefinitions(GameData.Actors);
            if (candidateDefs.Count == 0)
                return;

            #region Placeholders

            var actorPlaceholders = new List<Placeholder>();
            for (int i = 0; i < placeholders.Count; i++)
            {
                if (placeholders[i].ContentType == PlaceholderContentType.Actor)
                    actorPlaceholders.Add(placeholders[i]);
            }

            // Llamada directa al helper parametrizado
            var spawnedCount = PopulatePlaceholders(actorPlaceholders, candidateDefs, actorsSpawnCounter);

            #endregion

            #region WalkArea

            if (WalkArea == null)
                return;

            int targetTotal = Session.Random.Next(Definition.MinEnemies, Definition.MaxEnemies + 1);
            int remainingToSpawn = targetTotal - spawnedCount;

            if (remainingToSpawn <= 0)
                return;

            var freeCandidates = new List<ActorDefinition>();
            for (int i = 0; i < candidateDefs.Count; i++)
            {
                var def = candidateDefs[i];
                if (!def.RequiresPlaceholder && def.PassesMaxPerRoomConstraint(actorsSpawnCounter.GetCount(def.Name)))
                {
                    freeCandidates.Add(def);
                }
            }

            if (freeCandidates.Count == 0)
                return;

            var safePoly = new Polygon(WalkArea.Polygon.Vertices, -45);
            var points = GetSpawnPoints(safePoly, remainingToSpawn, 45, Session.Random);

            for (int i = 0; i < points.Count; i++)
            {
                if (freeCandidates.Count == 0)
                    break;

                if (occupiedPositions.Contains(points[i]))
                    continue;

                var chosen = freeCandidates.GetRandomItem(Session.Random);
                if (chosen == null)
                    continue;

                SpawnThing<Actor>(chosen.Name, points[i], actorsSpawnCounter);
                occupiedPositions.Add(points[i]);

                if (!chosen.PassesMaxPerRoomConstraint(actorsSpawnCounter.GetCount(chosen.Name)))
                {
                    freeCandidates.Remove(chosen);
                }
            }

            #endregion
        }

        // SpawnProps
        private void SpawnProps()
        {
            var candidates = GetCandidateDefinitions(GameData.Props);
            if (candidates.Count == 0)
                return;

            #region Placeholders

            var propPlaceholders = new List<Placeholder>();
            for (int i = 0; i < placeholders.Count; i++)
            {
                if (placeholders[i].ContentType == PlaceholderContentType.Prop)
                    propPlaceholders.Add(placeholders[i]);
            }

            // Llamada directa al helper parametrizado
            PopulatePlaceholders(propPlaceholders, candidates, propsSpawnCounter);

            #endregion

            #region WalkArea

            if (WalkArea == null)
                return;

            var freeCandidates = new List<PropDefinition>();
            for (int i = 0; i < candidates.Count; i++)
            {
                var def = candidates[i];
                if (!def.RequiresPlaceholder && def.PassesMaxPerRoomConstraint(propsSpawnCounter.GetCount(def.Name)))
                {
                    freeCandidates.Add(def);
                }
            }

            if (freeCandidates.Count == 0)
                return;

            int maxAttemptsInRoom = Session.Random.Next(1, 3);

            while (maxAttemptsInRoom > 0 && freeCandidates.Count > 0)
            {
                maxAttemptsInRoom--;

                var chosen = freeCandidates.GetRandomItem(Session.Random);
                if (chosen == null)
                    continue;

                if (Session.Random.NextSingle() > MathHelper.Clamp(chosen.SpawnWeight, 0f, 1f))
                    continue;

                Vector2 spawnPosition = WalkArea.RandomWalkablePoint(Session.Random);

                if (spawnPosition == Vector2.Zero || occupiedPositions.Contains(spawnPosition))
                    continue;

                occupiedPositions.Add(spawnPosition);

                SpawnThing<Prop>(chosen.Name, spawnPosition, propsSpawnCounter);

                if (!chosen.PassesMaxPerRoomConstraint(propsSpawnCounter.GetCount(chosen.Name)))
                    freeCandidates.Remove(chosen);
            }

            #endregion
        }

        // SpawnThing
        private T SpawnThing<T>(string name, Vector2 position, CounterBank counterBank)
            where T : GameThing
        {
            var instance = CreateThingClone<T>(name);
            instance.Position = position;
            Children.Add(instance);
            counterBank.Increment(name);

            if (instance is ISpawnNotification spawnNotification)
                spawnNotification.OnSpawned(this);

            return instance;
        }

        #endregion

        #region Protected members

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();

            CustomWidth = (int)BoundingBox.Width;
            CustomHeight = (int)BoundingBox.Height;

            SpawnProps();
            SpawnActors();
        }

        #endregion

        // CreateThingClone
        public sealed override T CreateThingClone<T>(string declaredName)
        {
            if (Session.CreateThingClone(declaredName, string.Empty) is not T result)
                throw new InvalidOperationException($"Failed to create runtime clone from'{declaredName}'.");

            return result;
        }

        // Definition
        public RoomDefinition Definition { get; }

        // Difficulty
        public Difficulty Difficulty { get; }

        // IsProcedural
        public override bool IsProcedural => true;
    }
}