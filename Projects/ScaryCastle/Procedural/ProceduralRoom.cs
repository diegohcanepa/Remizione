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

        private readonly CounterBank actorsSpawnCounter = new();
        private int instanceCount;
        private readonly List<Placeholder> placeholders = [];
        private readonly CounterBank propsSpawnCounter = new();
        private readonly int randomSeed;

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
        private List<Vector2> GetSpawnPoints(IReadOnlyPolygon polygon, int count, int cellSize)
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
            SpawnActors();
        }

        // SpawnActors
        private void SpawnActors()
        {
            if (WalkArea == null || Session.CurrentRun == null)
                return;

            var candidates = GetCandidateDefinitions<ActorDefinition, Actor>(ActorDefinition.Definitions.All);

            if (candidates.Count == 0)
                return;

            var table = new ChanceTable();
            foreach (var c in candidates)
            {
                if (!RoomNode.Definition.AllowEnemies && c.Faction == Faction.Evil)
                    continue;

                var finalWeight = AdjustWeight(RoomNode.TopographicDifficulty, c.Difficulty, c.SpawnWeight, c.Rank);
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

                    int currentInRoom = actorsSpawnCounter.GetCount(chosen.Name) + pendingCount;

                    if (!chosen.PassesMaxPerRoomConstraint(currentInRoom))
                        break;

                    pendingSpawns.Add(chosen);
                    successfulGroupSpawns++;
                    remainingInstances--;
                }

                if (successfulGroupSpawns == 0)
                    table.Remove(item.Name);
            }

            if (pendingSpawns.Count == 0)
                return;

            var safePoly = new Polygon(WalkArea.Polygon.Vertices, -45);
            var points = GetSpawnPoints(safePoly, pendingSpawns.Count, 45);

            for (int i = 0; i < points.Count; i++)
            {
                SpawnThing<Actor>(Session.CurrentRun, pendingSpawns[i].Name, points[i], actorsSpawnCounter);
            }
        }

        // SpawnProps
        private void SpawnProps()
        {
            if (Session.CurrentRun == null)
                return;

            var candidates = GetCandidateDefinitions<PropDefinition, Prop>(PropDefinition.Definitions.All);

            if (candidates.Count == 0)
                return;

            #region Placeholders

            if (placeholders.Count > 0)
            {
                var shuffledPlaceholders = new List<Placeholder>(Placeholders);
                shuffledPlaceholders.Shuffle(Random);

                foreach (var ph in shuffledPlaceholders)
                {
                    var phState = RoomNode.GetPlaceholderState(ph);

                    if (phState == PlaceholderState.Used)
                        continue;

                    if (phState == PlaceholderState.GateLever)
                    {
                        SpawnThing<Prop>(Session.CurrentRun, nameof(PlaceholderState.GateLever), ph.Position, propsSpawnCounter);
                        RoomNode.SetPlaceholderState(ph, PlaceholderState.Used);
                        continue;
                    }

                    if (!ph.FillChance.Roll(Random))
                        continue;

                    var table = new ChanceTable();

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

                        if (!def.PassesMaxPerRunConstraint(Session.CurrentRun.Spawns.GetCount(def.Name)))
                            continue;

                        var finalWeight = AdjustWeight(RoomNode.TopographicDifficulty, def.Difficulty, def.SpawnWeight, null);
                        table.Add(def.Name, finalWeight);
                    }

                    if (table.GetValue() is not ChanceTableItem item)
                        continue;

                    if (PropDefinition.Definitions.Find(item.Name) is not PropDefinition chosen)
                        continue;

                    SpawnThing<Prop>(Session.CurrentRun, chosen.Name, ph.Position, propsSpawnCounter);

                    RoomNode.SetPlaceholderState(ph, PlaceholderState.Used);
                }
            }

            #endregion

            if (WalkArea == null)
                return;

            var freeTable = new ChanceTable();

            foreach (var def in candidates)
            {
                // Si la definición dice que SÍ necesita un placeholder, la salteamos (ya se procesó arriba)
                if (def.RequiresPlaceholder)
                    continue;

                // Control estricto de topes (Si pusiste MaxPerRoom = 1 en el JSON para la baba, acá se frena)
                if (!def.PassesMaxPerRoomConstraint(propsSpawnCounter.GetCount(def.Name)))
                    continue;

                if (!def.PassesMaxPerRunConstraint(Session.CurrentRun.Spawns.GetCount(def.Name)))
                    continue;

                // CRUCE CON LA DIFICULTAD TOPOGRÁFICA DE LA RUN:
                // Si el cuarto es Easy y la baba es Hard, el peso se desploma (ej: de 1.0f a 0.02f)
                var finalWeight = AdjustWeight(RoomNode.TopographicDifficulty, def.Difficulty, def.SpawnWeight, null);

                // Si el peso es 0.1f, tiene un 90% de chances de quedar afuera de entrada.
                // Esto rompe el monopolio de la ruleta cuando hay un solo elemento en el JSON.
                if (Random.NextDouble() > finalWeight)
                    continue;

                freeTable.Add(def.Name, finalWeight);
            }

            if (freeTable.Count == 0)
                return;

            var occupiedPositions = new List<Vector2>();
            int maxAttemptsInRoom = Random.Next(1, 3);

            // 3. Hacemos girar la ruleta hasta agotar los intentos o vaciar las opciones legales
            while (maxAttemptsInRoom > 0 && freeTable.Count > 0)
            {
                maxAttemptsInRoom--;

                if (freeTable.GetValue() is not ChanceTableItem item)
                    break;

                if (PropDefinition.Definitions.Find(item.Name) is not PropDefinition chosen)
                    continue;

                // Pedimos el punto al WalkArea
                Vector2 spawnPosition = WalkArea.RandomWalkablePoint(Random);

                if (spawnPosition == Vector2.Zero || occupiedPositions.Contains(spawnPosition))
                    continue;

                occupiedPositions.Add(spawnPosition);

                // Clonación e inyección directa en MonoGame
                var instance = CreateThingClone<Prop>(chosen.Name);
                instance.Position = spawnPosition;
                Children.Add(instance);

                Session.CurrentRun.Spawns.Increment(chosen.Name);
                propsSpawnCounter.Increment(chosen.Name);

                // EXCLUSIÓN POR REGISTRO (Tu regla del MaxPerRoom)
                // Si la baba tenía MaxPerRoom = 1 y ya spawneó, la borramos de la ruleta para el siguiente tiro
                if (!chosen.PassesMaxPerRoomConstraint(propsSpawnCounter.GetCount(chosen.Name)))
                    freeTable.Remove(chosen.Name);
            }
        }

        // SpawnThing
        private T SpawnThing<T>(Run run, string name, Vector2 position, CounterBank counterBank)
            where T : GameThing
        {
            var instance = CreateThingClone<T>(name);
            instance.Position = position;
            Children.Add(instance);
            run.Spawns.Increment(name);
            counterBank.Increment(name);

            return instance;
        }

        #endregion

        #region Protected members

        // AddPlaceholder
        protected void AddPlaceholder(Placeholder placeholder)
        {
            placeholders.Add(placeholder);
        }

        // AdjustWeight
        protected static float AdjustWeight(Difficulty roomDiff, Difficulty thingDiff, float baseWeight, ActorRank? rank)
        {
            int distance = (int)roomDiff - (int)thingDiff;

            // 1. Modificador por choque de dificultades (Topografía vs Definición)
            float roomMultiplier = distance switch
            {
                2 => 0.05f,   // Sala Hard, Enemigo Easy -> Desalentamos masillas en el clímax
                1 => 0.25f,   // Sala Normal, Enemigo Easy
                0 => 1.0f,    // Calce ideal
                -1 => 0.10f,  // Out of Depth leve: Sala Easy, Enemigo Normal -> 10% de chances base
                -2 => 0.02f,  // Out of Depth severo: Sala Easy, Enemigo Hard -> 2% de chances base
                _ => 1.0f
            };

            // 2. Modificador por jerarquía de combate (El filtro salvaje)
            float rankMultiplier = 1.0f;
            if (rank.HasValue)
            {
                rankMultiplier = rank.Value switch
                {
                    // El Boss real tiene peso plano porque ya está blindado por su filtro de sala dedicado
                    ActorRank.Boss => 1.0f,

                    // Si es un MiniBoss y está queriendo irrumpir en una zona que no es Hard, 
                    // le pegamos un hachazo drástico a su peso para que sea una rareza absoluta.
                    ActorRank.MiniBoss => roomDiff switch
                    {
                        Difficulty.Easy => 0.10f,   // Hachazo del 90%. Combinado con el -2 de arriba, da un 0.002% real. Épico si sale.
                        Difficulty.Normal => 0.30f, // Hachazo del 70%. Aparece a mitad de camino de forma muy esporádica.
                        Difficulty.Hard => 1.0f,   // Peso completo: es su hábitat natural.
                        _ => 1.0f
                    },

                    _ => 1.0f
                };
            }

            return baseWeight * roomMultiplier * rankMultiplier;
        }

        // GetCandidateDefinitions
        protected List<TDefinition> GetCandidateDefinitions<TDefinition, TThing>(IList<TDefinition> definitions)
            where TDefinition : ThingDefinition where TThing : GameThing
        {
            var outList = new List<TDefinition>();
            if (Session.CurrentRun == null)
                return outList;

            foreach (var definition in definitions)
            {
                if (definition is ActorDefinition actorDefinition)
                {
                    // Si es un Boss real, SOLO puede aparecer en la habitación etiquetada como Boss
                    if (actorDefinition.Rank == ActorRank.Boss && RoomNode.Category != RoomCategory.Boss)
                        continue;

                    // Y viceversa: en la sala del Boss no queremos que spawneen murciélagos comunes como plato principal
                    if (RoomNode.Category == RoomCategory.Boss && actorDefinition.Rank != ActorRank.Boss)
                        continue;
                }

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

                if (!TagScope.Test(RoomNode.Definition.Scope, RoomNode.Definition.Pools, definition.Tags))
                    continue;

                outList.Add(definition);
            }

            return outList;
        }

        // OnChildAdded
        protected override void OnChildAdded(Entity child)
        {
            base.OnChildAdded(child);

            if (child is PickableLoot)
            {
                if (child is Coin)
                    RoomNode.CoinCount++;
                else
                    RoomNode.LootCount++;
            }
        }

        // OnChildRemoved
        protected override void OnChildRemoved(Entity child)
        {
            base.OnChildAdded(child);

            if (child is PickableLoot)
            {
                if (child is Coin)
                    RoomNode.CoinCount--;
                else
                    RoomNode.LootCount--;
            }
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

        // Random
        public Random Random { get; }

        // RoomNode
        public RoomNode RoomNode { get; }

        // ToString
        public override string ToString()
        {
            return RoomNode.ToString();
        }
    }
}