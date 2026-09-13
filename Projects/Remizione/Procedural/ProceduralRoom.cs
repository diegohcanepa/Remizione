using Adberration;
using Engendro;
using Engendro.Collections;
using Microsoft.Xna.Framework;
using Remizione.Procedural;
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
            this.LightingSystem = true;

            // Add placeholders
            foreach (var placeholder in Definition.Placeholders)
            {
                placeholders.Add(placeholder);
            }

            // Add walls
            foreach (var wall in Definition.Walls)
            {
                AddWall(wall);
            }
        }

        #endregion

        #region Private members

        // SpawnActors
        private void SpawnActors()
        {
            if (WalkArea == null)
                return;

            var candidates = ProceduralUtils.GetCandidateDefinitions(this, GameData.Actors);
            if (candidates.Count == 0)
                return;

            // 2. Filtrar candidatos compatibles por dificultad de la sala
            var validCandidates = new List<ActorDefinition>();
            for (int i = 0; i < candidates.Count; i++)
            {
                var c = candidates[i];

                if (!Definition.AllowEnemies && c.Faction == Faction.Evil)
                    continue;

                if (!ProceduralUtils.IsValidActorForRoom(Definition.Difficulty, c.Difficulty))
                    continue;

                validCandidates.Add(c);
            }

            if (validCandidates.Count == 0)
                return;

            /*
            targetTypesCount = Math.Min(targetTypesCount, validCandidates.Count);

            var chosenTypes = new List<ActorDefinition>();

            // 4. Garantizar la amenaza principal
            var primaryCandidates = new List<ActorDefinition>();
            for (int i = 0; i < validCandidates.Count; i++)
            {
                if (validCandidates[i].Difficulty == RoomNode.TopographicDifficulty)
                {
                    primaryCandidates.Add(validCandidates[i]);
                }
            }

            if (primaryCandidates.GetRandomItem(Random) is { } primary)
            {
                chosenTypes.Add(primary);
                validCandidates.Remove(primary);
            }

            // 5. Completar los slots secundarios
            int remainingSlots = targetTypesCount - chosenTypes.Count;
            if (remainingSlots > 0 && validCandidates.Count > 0)
            {
                // Asumiendo que Shuffle es tu extensión existente
                validCandidates.Shuffle(Random);

                int limit = Math.Min(remainingSlots, validCandidates.Count);
                for (int i = 0; i < limit; i++)
                {
                    chosenTypes.Add(validCandidates[i]);
                }
            }

            // 6. Armar los packs aplicando MaxPerRoom de forma estricta
            var pendingSpawns = new List<ActorDefinition>();
            for (int i = 0; i < chosenTypes.Count; i++)
            {
                var chosen = chosenTypes[i];
                int packSize = chosen.RollPackSize(Random);

                for (int p = 0; p < packSize; p++)
                {
                    // Contar cuántos de este tipo ya metimos a mano
                    int currentPending = 0;
                    for (int k = 0; k < pendingSpawns.Count; k++)
                    {
                        if (pendingSpawns[k].Name == chosen.Name)
                        {
                            currentPending++;
                        }
                    }

                    int totalInRoom = actorsSpawnCounter.GetCount(chosen.Name) + currentPending;

                    // Si llegamos al tope de diseño para esta sala, cortamos la generación de este pack
                    if (!chosen.PassesMaxPerRoomConstraint(totalInRoom))
                        break;

                    pendingSpawns.Add(chosen);
                }
            }

            if (pendingSpawns.Count == 0)
                return;

            // 7. Inyección física segura
            var safePoly = new Polygon(WalkArea.Polygon.Vertices, -45);
            var points = ProceduralUtils.GetSpawnPoints(safePoly, pendingSpawns.Count, 45, Random);

            int spawnsToExecute = Math.Min(pendingSpawns.Count, points.Count);

            for (int i = 0; i < spawnsToExecute; i++)
            {
                SpawnThing<Actor>(run, pendingSpawns[i].Name, points[i], actorsSpawnCounter);
            }
            */
        }

        // SpawnProps
        private void SpawnProps()
        {
            var candidates = ProceduralUtils.GetCandidateDefinitions(this, GameData.Props);
            if (candidates.Count == 0)
                return;

            #region Placeholders

            if (placeholders.Count > 0)
            {
                var shuffledPlaceholders = new List<Placeholder>(placeholders);
                shuffledPlaceholders.Shuffle(Session.Random);

                foreach (var ph in shuffledPlaceholders)
                {
                    // 1. CONDICIÓN DE SLOT (Placeholder)
                    // Si la estrategia NO es ContentChanceOnly, el placeholder debe pasar su tirada de FillChance.
                    if (ph.SpawnRule != PlaceholderSpawnRule.ContentChanceOnly)
                    {
                        if (!ph.FillChance.Roll(Session.Random))
                            continue;
                    }

                    // 2. FILTRADO Y EVALUACIÓN DE CANDIDATOS
                    var phTable = new ChanceTable();

                    foreach (var def in candidates)
                    {
                        if (!def.RequiresPlaceholder)
                            continue;

                        if (!def.Placements.Contains(ph.Placement))
                            continue;

                        if (ph.AllowTags.Count > 0 && !ph.AllowTags.Intersects(def.Tags))
                            continue;

                        if (!def.PassesMaxPerRoomConstraint(propsSpawnCounter.GetCount(def.Name)))
                            continue;

                        if (!def.PassesMaxPerRunConstraint(Session.Spawns.GetCount(def.Name)))
                            continue;

                        float finalWeight = ProceduralUtils.AdjustPropWeight(Definition.Difficulty, def.Difficulty, def.SpawnWeight);

                        // 3. CONDICIÓN DE CONTENIDO (Prop)
                        // Si la estrategia NO es PlaceholderChanceOnly, el prop debe pasar su tirada individual de rareza.
                        if (ph.SpawnRule != PlaceholderSpawnRule.PlaceholderChanceOnly)
                        {
                            if (Session.Random.NextSingle() > MathHelper.Clamp(finalWeight, 0f, 1f))
                                continue;
                        }

                        if (finalWeight > 0f)
                        {
                            phTable.Add(def.Name, finalWeight);
                        }
                    }

                    if (phTable.Count == 0)
                        continue;

                    // 4. INSTANCIACIÓN FINAL
                    if (phTable.GetItem() is ChanceTableItem item &&
                        GameData.Props.Find(item.Name) is PropDefinition chosen)
                    {
                        SpawnThing<Prop>(chosen.Name, ph.Position, propsSpawnCounter);
                    }
                }
            }

            #endregion

            #region Props Libres en WalkArea

            if (WalkArea == null)
                return;

            // 1. Filtrar candidatos que NO requieran placeholder y cumplan las restricciones de conteo
            var freeCandidates = new List<PropDefinition>();
            foreach (var def in candidates)
            {
                if (def.RequiresPlaceholder)
                    continue;

                if (!def.PassesMaxPerRoomConstraint(propsSpawnCounter.GetCount(def.Name)))
                    continue;

                if (!def.PassesMaxPerRunConstraint(Session.Spawns.GetCount(def.Name)))
                    continue;

                freeCandidates.Add(def);
            }

            if (freeCandidates.Count == 0)
                return;

            var occupiedPositions = new List<Vector2>();
            int maxAttemptsInRoom = Session.Random.Next(1, 3); // 1 a 2 intentos de apariciones libres por sala

            // 2. Bucle de apariciones por Tirada Absoluta
            while (maxAttemptsInRoom > 0 && freeCandidates.Count > 0)
            {
                maxAttemptsInRoom--;

                // Elegimos un candidato al azar del pool de elegibles
                var chosen = freeCandidates.GetRandomItem(Session.Random);

                if (chosen == null)
                    continue;

                // Calculamos su probabilidad ajustada por la dificultad topográfica de la sala (0.0f a 1.0f)
                float finalChance = ProceduralUtils.AdjustPropWeight(Definition.Difficulty, chosen.Difficulty, chosen.SpawnWeight);

                // Tirada Absoluta: Si el dado no supera la probabilidad, este intento queda VACÍO de forma natural
                if (Session.Random.NextSingle() > MathHelper.Clamp(finalChance, 0f, 1f))
                    continue;

                // Si pasó la tirada de rareza, pedimos la posición al WalkArea
                Vector2 spawnPosition = WalkArea.RandomWalkablePoint(Session.Random);

                if (spawnPosition == Vector2.Zero || occupiedPositions.Contains(spawnPosition))
                    continue;

                occupiedPositions.Add(spawnPosition);

                // Instanciación directa
                var instance = CreateThingClone<Prop>(chosen.Name);
                instance.Position = spawnPosition;
                Children.Add(instance);

                Session.Spawns.Increment(chosen.Name);
                propsSpawnCounter.Increment(chosen.Name);

                // Si el prop alcanzó su límite por sala, lo removemos del pool de candidatos
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
            Session.Spawns.Increment(name);
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

        // IsProcedural
        public override bool IsProcedural => true;
    }
}