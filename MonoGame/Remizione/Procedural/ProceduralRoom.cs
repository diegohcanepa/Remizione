using Adberration;
using Adberration.Scripting;
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

        private readonly List<Vector2> occupiedPositions = [];
        private readonly CounterBank spawnCounter = new();

        #endregion

        #region Constructor

        // Constructor
        public ProceduralRoom(GameSession session, string name)
            : base(session, name)
        {
            this.Definition = GameData.Rooms.Get(name);

            this.LightingSystem = true;
            this.UnloadMode = UnloadMode.Manual;
            this.Zoom = 1.1f;
        }

        #endregion

        #region Private members

        // CleanupAnnonymousThings
        private void CleanupAnnonymousThings()
        {
            var annonymousList = new List<Entity>();

            for (var i = 0; i < Children.Count; i++)
            {
                if (Children[i].InstanceKind == EntityInstanceKind.Anonymous)
                    annonymousList.Add(Children[i]);
            }

            for (var i = 0; i < annonymousList.Count; i++)
            {
                annonymousList[i].Unparent();
            }
        }

        // GetCandidateDefinitions
        private List<TDefinition> GetCandidateDefinitions<TDefinition>(IList<TDefinition> definitions)
            where TDefinition : ThingDefinition
        {
            var outList = new List<TDefinition>();

            foreach (var definition in definitions)
            {
                if (definition.SpawnWeight == 0)
                    continue;

                if (!Session.IsUnlocked(definition.Name))
                    continue;

                if (Session.GetProceduralThing(definition.Name) == null)
                    continue;

                if (!TagScope.Test(Definition.Scope, Definition.Pools, definition.Tags))
                    continue;

                outList.Add(definition);
            }

            return outList;
        }

        // GetVisibleBox
        //private RectangleF GetVisibleBox() => RectangleF.Inflate(Session.Camera.VisibleBox, 15, 15);

        public static RectangleF GetVisibleBox(Vector2 playerPosition, float cameraZoom)
        {
            float safeZoom = Math.Max(cameraZoom, 0.1f);
            float width = 240f / safeZoom;
            float height = 135f / safeZoom;

            return new RectangleF(
                playerPosition.X - width * 0.5f,
                playerPosition.Y - height * 0.5f,
                width,
                height);
        }

        // GetWalkAreaCandidates
        private List<T> GetWalkAreaCandidates<T>(List<T> list) where T : ThingDefinition
        {
            var result = new List<T>();

            for (int i = 0; i < list.Count; i++)
            {
                var def = list[i];
                if (!def.RequiresPlaceholder && def.PassesMaxPerRoomConstraint(spawnCounter.GetCount(def.Name)))
                {
                    result.Add(def);
                }
            }

            return result;
        }

        // GetSpawnPoints
        private static List<Vector2> GetSpawnPoints(Polygon polygon, int count, int cellSize, Random rng)
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
                    cx += (rng.NextSingle() * offsetRange * 2) - offsetRange;
                    cy += (rng.NextSingle() * offsetRange * 2) - offsetRange;

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

        // IsPositionOccupied
        private bool IsPositionOccupied(Vector2 position, float minDistance = 20)
        {
            // 1. Validar si cae dentro del viewport
            var rect = Session.Camera.VisibleBox;
            rect.Inflate(15, 15);
            if (rect.Contains(position))
                return true;

            // 2. Validar ocupación normal de otros Actores/Props
            float minDistanceSq = minDistance * minDistance;
            for (int i = 0; i < occupiedPositions.Count; i++)
            {
                if (Vector2.DistanceSquared(position, occupiedPositions[i]) < minDistanceSq)
                    return true;
            }

            return false;
        }

        // PopulatePlaceholders
        private int PopulatePlaceholders<TDefinition>(List<Placeholder> placeholders, IList<TDefinition> candidates, RectangleF? safeArea)
            where TDefinition : ThingDefinition
        {
            if (placeholders.Count == 0 || candidates.Count == 0)
                return 0;

            int spawnedCount = 0;
            var shuffled = new List<Placeholder>(placeholders);
            shuffled.Shuffle(Session.Random);

            for (int i = 0; i < shuffled.Count; i++)
            {
                var ph = shuffled[i];

                if (safeArea?.Contains(ph.Position) == true)
                    continue;

                // 1. PRESENCIA: El Placeholder decide si vive o muere este slot
                if (!ph.FillChance.Roll(Session.Random))
                    continue;

                // 2. SELECCIÓN: Se arma la tabla de candidatos válidos
                var phTable = new ChanceTable();

                for (int j = 0; j < candidates.Count; j++)
                {
                    var def = candidates[j];

                    if (!def.RequiresPlaceholder)
                        continue;

                    if (ph.AllowTags.Count > 0 && !ph.AllowTags.Intersects(def.Tags))
                        continue;

                    if (!def.PassesMaxPerRoomConstraint(spawnCounter.GetCount(def.Name)))
                        continue;

                    // El SpawnWeight entra directo como peso relativo para competir
                    if (def.SpawnWeight > 0)
                        phTable.Add(def.Name, def.SpawnWeight, def);
                }

                if (phTable.Count == 0)
                    continue;

                // 3. INSTANCIACIÓN: Sale el ganador de la competencia de pesos
                if (phTable.GetItem(Session.Random)?.Context is TDefinition chosen)
                {
                    SpawnThing<GameThing>(chosen.Name, ph.Position);
                    spawnedCount++;
                }
            }

            return spawnedCount;
        }

        // SpawnActors
        private void SpawnActors(RectangleF? safeArea)
        {
            var candidates = GetCandidateDefinitions(GameData.Actors);
            if (candidates.Count == 0)
                return;

            // 1. Placeholders
            var actorPlaceholders = Definition.GetPlaceholders(PlaceholderContentType.Actor);
            var spawnedCount = PopulatePlaceholders(actorPlaceholders, candidates, safeArea);

            // 2. WalkArea
            if (WalkArea == null)
                return;

            int targetTotal = Session.Random.Next(Definition.MinEnemies, Definition.MaxEnemies + 1);
            int remainingToSpawn = targetTotal - spawnedCount;

            if (remainingToSpawn <= 0)
                return;

            var walkAreaCandidates = GetWalkAreaCandidates(candidates);
            if (walkAreaCandidates.Count == 0)
                return;

            // Pedimos más puntos a la grilla para tener margen si IsPositionOccupied o el SpawnWeight descartan posiciones
            var safePoly = new Polygon(WalkArea.Polygon.Vertices, -45);
            int requestedPoints = remainingToSpawn * 4;
            var points = GetSpawnPoints(safePoly, requestedPoints, 45, Session.Random);

            int pointIndex = 0;

            while (remainingToSpawn > 0 && pointIndex < points.Count && walkAreaCandidates.Count > 0)
            {
                Vector2 candidatePoint = points[pointIndex];
                pointIndex++;

                // Descarta si cae cerca de un Placeholder, Prop o Actor ya instanciado
                if (safeArea?.Contains(candidatePoint) == true || IsPositionOccupied(candidatePoint))
                    continue;

                var chosen = walkAreaCandidates.GetRandomItem(Session.Random);
                if (chosen == null)
                    continue;

                // Tirada de peso para selección
                if (Session.Random.NextSingle() > MathHelper.Clamp(chosen.SpawnWeight, 0f, 1f))
                    continue;

                // Instanciación exitosa: se descuenta de la meta de enemigos restantes
                SpawnThing<Actor>(chosen.Name, candidatePoint);
                remainingToSpawn--;

                if (!chosen.PassesMaxPerRoomConstraint(spawnCounter.GetCount(chosen.Name)))
                {
                    walkAreaCandidates.Remove(chosen);
                }
            }
        }

        // SpawnProps
        private void SpawnProps(RectangleF? safeArea)
        {
            var candidates = GetCandidateDefinitions(GameData.Props);
            if (candidates.Count == 0)
                return;

            // 1. Placeholders: Se procesan siempre
            var propPlaceholders = Definition.GetPlaceholders(PlaceholderContentType.Prop);
            PopulatePlaceholders(propPlaceholders, candidates, safeArea);

            // 2. WalkArea: Solo si la sala admite props en piso libre
            if (WalkArea == null || Definition.MaxProps <= 0)
                return;

            var walkAreaCandidates = GetWalkAreaCandidates(candidates);
            if (walkAreaCandidates.Count == 0)
                return;

            // Determina la cuota objetivo a colocar en esta corrida
            int targetCount = Session.Random.Next(Definition.MinProps, Definition.MaxProps + 1);
            int safetyAttempts = targetCount * 5; // Margen para reintentar si cae en zona ocupada

            while (targetCount > 0 && safetyAttempts > 0 && walkAreaCandidates.Count > 0)
            {
                safetyAttempts--;

                var chosen = walkAreaCandidates.GetRandomItem(Session.Random);
                if (chosen == null)
                    continue;

                // Tirada de peso para selección de candidato
                if (Session.Random.NextSingle() > MathHelper.Clamp(chosen.SpawnWeight, 0f, 1f))
                    continue;

                Vector2 spawnPosition = WalkArea.RandomWalkablePoint(Session.Random);
                if (spawnPosition == Vector2.Zero || safeArea?.Contains(spawnPosition) == true || IsPositionOccupied(spawnPosition))
                    continue;

                // Instanciación exitosa: se descuenta de la meta objetivo
                SpawnThing<Prop>(chosen.Name, spawnPosition);
                targetCount--;

                if (!chosen.PassesMaxPerRoomConstraint(spawnCounter.GetCount(chosen.Name)))
                    walkAreaCandidates.Remove(chosen);
            }
        }

        // SpawnThing
        private T SpawnThing<T>(string name, Vector2 position)
            where T : GameThing
        {
            var instance = Session.CreateThingClone<T>(name);
            instance.HomePosition = position;
            instance.Position = position;
            Children.Add(instance);

            spawnCounter.Increment(name);
            occupiedPositions.Add(position);

            return instance;
        }

        #endregion

        #region Protected members

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            Spawn();
        }

        #endregion

        // Definition
        public RoomDefinition Definition { get; }

        // Spawn
        public void Spawn()
        {
            spawnCounter.Clear();
            occupiedPositions.Clear();
            Session.RenewSeed();
            Session.UnlockManager.Refresh();
            CleanupAnnonymousThings();

            var visibleBox = Session.Player == null ? RectangleF.Empty : GetVisibleBox(Session.Player.Position, Zoom);

            var cleanupList = new List<Entity>();
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].InstanceKind == EntityInstanceKind.RuntimeClone)
                {
                    if (visibleBox.Intersects(Children[i].BoundingBox))
                        continue;

                    if (Children[i] is not ItemOrb itemOrb || itemOrb.AllowCleanup)
                        cleanupList.Add(Children[i]);
                }
            }

            Session.CleanUpRuntimeClones(cleanupList);

            SpawnProps(visibleBox);
            SpawnActors(visibleBox);
        }
    }
}