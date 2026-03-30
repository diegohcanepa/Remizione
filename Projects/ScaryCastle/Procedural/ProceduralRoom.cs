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

        private readonly CounterBank enemiesSpawnCounter = new();
        private int instanceCount;
        private readonly List<Placeholder> placeholders = [];
        private readonly CounterBank propsSpawnCounter = new();
        private readonly int randomSeed;
        private HashSet<Placeholder> usedPlaceholders = [];

        #endregion

        #region Constructor

        // Constructor
        protected ProceduralRoom(GameSession session, string name, RoomGraph roomGraph)
            : base(session, name)
        {
            this.RoomGraph = roomGraph;

            this.AllowGlobalLight = true;
            this.LightingSystem = true;
            this.UnloadMode = UnloadMode.Manual;

            int salt = roomGraph.Index;
            this.randomSeed = RandomHelper.GetSeed(Session.Seed, salt);
            this.Random = new Random(randomSeed);
            this.Placeholders = placeholders.AsReadOnly();
        }

        #endregion

        #region Private members

        // AdjustWeight
        // Modifica el peso de aparición combinando la dificultad base de la sala y la intensidad global de la partida.
        private static float AdjustWeight(float runIntensity, Difficulty roomDiff, Difficulty thingDiff, float baseWeight)
        {
            // 1. Lógica original: Respetamos la jerarquía de diseño de la sala
            int distance = (int)roomDiff - (int)thingDiff;

            float roomMultiplier = distance switch
            {
                1 => 0.15f,  // Ej: Sala Normal, Enemigo Easy (1 escalón)
                2 => 0.02f,  // Ej: Sala Hard, Enemigo Easy (2 escalones)
                _ => 1.0f
            };

            // 2. Lógica de Intensidad: Curvamos los pesos según el progreso del jugador
            float intensityMultiplier = thingDiff switch
            {
                // Easy: Arranca en x1.0 y decae hasta x0.2 en el último piso
                Difficulty.Easy => 1f - (runIntensity * 0.8f),

                // Normal: Arranca bajo (x0.2) y escala hasta x1.0
                Difficulty.Normal => 0.2f + (runIntensity * 0.8f),

                // Hard: Arranca en x0.0 (no sale) y escala exponencialmente hasta x1.0
                Difficulty.Hard => runIntensity * runIntensity,

                _ => 1.0f
            };

            return baseWeight * roomMultiplier * intensityMultiplier;
        }

        // GetCandidateDefinitions
        private List<TDefinition> GetCandidateDefinitions<TDefinition, TThing>(IList<TDefinition> definitions)
            where TDefinition : ThingDefinition where TThing : GameThing
        {
            var outList = new List<TDefinition>();
            if (Session.CurrentRun == null)
                return outList;

            foreach (var definition in definitions)
            {
                // 1. Filtro para cosas en el start room
                if (RoomGraph.RoomType == RoomType.Start && !definition.AllowStartRoomSpawn)
                    continue;

                // 2. Filtro de dificultad: No permitimos que aparezcan cosas más difíciles que el cuarto
                if (definition.Difficulty > RoomGraph.Definition.Difficulty)
                    continue;

                // 3. Filtro de topologia
                if (definition.RequiresDeadEnd && RoomGraph.ConnectionCount > 1)
                    continue;

                // 4. Validación de Existencia de instancia declarada en script
                var thing = Session.FindDeclaredThing(definition.Name) ?? throw new InvalidOperationException($"There is no declared thing named '{definition.Name}'. ");

                // Is expected type?
                if (thing is not TThing)
                    continue;

                // 5. Meta-progreso
                // Chequea si el enemigo está desbloqueado (MinRun)
                if (!definition.PassesRunConstraints(Session.RunCount))
                    continue;

                // 6. Historial de la Run
                // Chequea si el enemigo ya alcanzó su MaxPerRun global
                if (!definition.PassesMaxPerRunConstraint(Session.CurrentRun.Spawns))
                    continue;

                // 7. Reglas de Scope (Pools/Tags de la habitación)
                if (!TagScope.Test(RoomGraph.Definition.Scope, RoomGraph.Definition.Pools, RoomGraph.Definition.Tags))
                    continue;

                outList.Add(definition);
            }

            return outList;
        }

        // GetSpawnPoints (Refactorizado)
        // Ahora acepta ReadOnlyPolygon para validar la geometría real.
        private List<Vector2> GetSpawnPoints(ReadOnlyPolygon polygon, int count, int cellSize)
        {
            var cells = new List<Vector2>();

            // Usamos el rectángulo para limitar los bucles, pero el polígono para validar
            var area = polygon.BoundingRectangle;

            // Iteramos sobre la rejilla
            for (int y = area.Top; y < area.Bottom; y += cellSize)
            {
                for (int x = area.Left; x < area.Right; x += cellSize)
                {
                    // Centro teórico de la celda
                    float cx = x + (cellSize * 0.5f);
                    float cy = y + (cellSize * 0.5f);

                    // Jitter: Agregamos variación aleatoria (-25% a +25% del tamaño de celda)
                    // para que los enemigos no parezcan estar en una cuadrícula perfecta.
                    float offsetRange = cellSize * 0.25f;
                    cx += (float)((Random.NextDouble() * offsetRange * 2) - offsetRange);
                    cy += (float)((Random.NextDouble() * offsetRange * 2) - offsetRange);

                    var candidate = new Vector2(cx, cy);

                    // VALIDACIÓN CRÍTICA:
                    // polygon.Contains usa tu Ray Casting. Si el punto cae en una esquina
                    // vacía del bounding box pero fuera del cuarto, esto devolverá false.
                    if (polygon.Contains(candidate))
                    {
                        cells.Add(candidate);
                    }
                }
            }

            // Shuffle (Mezcla determinista usando la seed del cuarto)
            for (int i = cells.Count - 1; i > 0; i--)
            {
                int j = Random.Next(i + 1);
                (cells[i], cells[j]) = (cells[j], cells[i]);
            }

            // Devolvemos solo la cantidad solicitada (o menos si no hay espacio)
            if (cells.Count > count)
                cells.RemoveRange(count, cells.Count - count);

            return cells;
        }

        // Populate
        // Orquesta la instanciación respetando estrictamente la jerarquía física (Mampostería -> IA).
        private void Populate()
        {
            // CAPA 1: TOPOLOGÍA Y OBSTÁCULOS (Props)
            var propDefinitions = GetCandidateDefinitions<PropDefinition, Prop>(PropDefinition.Definitions.All);

            // 1.1 Props fijos en diseño
            SpawnInPlaceholders(PropDefinition.Definitions, propDefinitions, RoomGraph.Definition.MaxProps, propsSpawnCounter, PlaceholderTarget.Prop);

            // 1.2 Props aleatorios rellenando el espacio
            SpawnInWalkArea(PropDefinition.Definitions, propDefinitions, RoomGraph.Definition.MaxProps, propsSpawnCounter);

            // CAPA 2: ACTORES Y ENEMIGOS (IA)
            var actorDefinitions = GetCandidateDefinitions<ActorDefinition, Actor>(ActorDefinition.Definitions.All);

            // 2.1 Enemigos en puntos de emboscada/diseñados
            SpawnInPlaceholders(ActorDefinition.Definitions, actorDefinitions, RoomGraph.Definition.MaxEnemies, enemiesSpawnCounter, PlaceholderTarget.Enemy);

            // 2.2 Enemigos aleatorios patrullando
            SpawnInWalkArea(ActorDefinition.Definitions, actorDefinitions, RoomGraph.Definition.MaxEnemies, enemiesSpawnCounter);
        }

        // SpawnInPlaceholders
        private void SpawnInPlaceholders<T>(DataContainer<T> definitionContainer, IList<T> candidates, int maxInstances, CounterBank spawnCounter, PlaceholderTarget target)
            where T : ThingDefinition
        {
            if (Placeholders.Count == 0 || maxInstances == 0 || Session.CurrentRun == null)
                return;

            // 1) Shuffle placeholders
            var placeholders = new List<Placeholder>(Placeholders);
            placeholders.Shuffle(Random);

            // 2) Iterate placeholders
            foreach (var placeholder in placeholders)
            {
                // Already used
                if (usedPlaceholders.Contains(placeholder))
                    continue;

                // Functional filter (Prop vs Enemy)
                if (placeholder.Target != PlaceholderTarget.Any && placeholder.Target != target)
                    continue;

                // Roll fillChance
                if (!placeholder.FillChance.Roll(Random))
                    continue;

                // Collect candidates
                var selectedCandidates = new List<T>();
                foreach (var definition in candidates)
                {
                    // Is compatible with placeholder placement?
                    if (!definition.Placements.Contains(placeholder.Placement))
                        continue;

                    // Match tags?
                    if (placeholder.AllowTags.Count > 0 && !placeholder.AllowTags.Intersects(definition.Tags))
                        continue;

                    // MaxPerRoom (local)
                    if (!definition.PassesMaxPerRoomConstraint(spawnCounter.GetCount(definition.Name)))
                        continue;

                    // MaxPerRun (Global)
                    if (!definition.PassesMaxPerRunConstraint(Session.CurrentRun.Spawns))
                        continue;

                    selectedCandidates.Add(definition);
                }

                if (selectedCandidates.Count == 0)
                    continue;

                // Pick
                var chanceTable = new ChanceTable();
                foreach (var c in selectedCandidates)
                {
                    var finalWeight = AdjustWeight(Session.CurrentRun.Intensity, RoomGraph.Definition.Difficulty, c.Difficulty, c.SpawnWeight);
                    chanceTable.Add(c.Name, finalWeight);
                }

                if (chanceTable.GetValue() is not ChanceTableItem chanceTableItem)
                    continue;

                if (definitionContainer.Find(chanceTableItem.Name) is not T chosen)
                    continue;

                // Flag placeholder as used
                usedPlaceholders.Add(placeholder);

                var instance = CreateThingClone(chosen.Name);
                instance.Position = placeholder.Position;
                Children.Add(instance);

                // Log spawn in run
                Session.CurrentRun.Spawns.Increment(chosen.Name);

                int currentRoomCount = spawnCounter.Increment(chosen.Name);
                
                // Max per room
                if (maxInstances != -1 && currentRoomCount >= maxInstances)
                    return;
            }
        }

        // SpawnInWalkArea
        // Procesa la aparición de entidades en áreas caminables asegurando la sincronización de estado.
        private void SpawnInWalkArea<T>(DataContainer<T> definitionContainer, IList<T> candidates, int maxInstances, CounterBank spawnCounter)
            where T : ThingDefinition
        {
            if (WalkArea == null || maxInstances == 0 || Session.CurrentRun == null)
                return;

            // 1) Collect candidates
            var selectedCandidates = new List<T>();
            foreach (var definition in candidates)
            {
                if (!definition.Placements.Contains(PlacementType.WalkArea))
                    continue;

                if (!definition.PassesMaxPerRoomConstraint(spawnCounter.GetCount(definition.Name)))
                    continue;

                // Corrección: Inyectamos el contador global de la run
                if (!definition.PassesMaxPerRunConstraint(Session.CurrentRun.Spawns.GetCount(definition.Name)))
                    continue;

                selectedCandidates.Add(definition);
            }

            if (selectedCandidates.Count == 0)
                return;

            // 2) Build chance table
            var table = new ChanceTable();
            foreach (var c in selectedCandidates)
            {
                var finalWeight = AdjustWeight(Session.CurrentRun.Intensity, RoomGraph.Definition.Difficulty, c.Difficulty, c.SpawnWeight);
                table.Add(c.Name, finalWeight);
            }

            // Usamos una lista de definiciones concretas en lugar de solo strings
            var pendingSpawns = new List<T>();
            int remainingInstances = maxInstances;
            var continueChance = 1f;
            const float decay = 0.7f;
            int safety = (selectedCandidates.Count * 2) + 10;

            // 3) Pick groups (Solo intención, no alteramos estado global)
            while (table.Count > 0 && safety-- > 0)
            {
                if (maxInstances > 0 && remainingInstances <= 0)
                    break;

                if (maxInstances <= 0)
                {
                    if (Random.NextDouble() > continueChance)
                        break;

                    continueChance *= decay;
                }

                if (table.GetValue() is not ChanceTableItem item)
                    break;

                table.Remove(item.Name);

                // Corrección: Usamos 'T' directo gracias al contenedor tipado
                if (definitionContainer.Find(item.Name) is not T chosen)
                    continue;

                int min = Math.Max(1, chosen.MinSpawnAmount);
                int max = Math.Max(min, chosen.MaxSpawnAmount);
                int amount = Random.Next(min, max + 1);

                if (maxInstances > 0)
                    amount = Math.Min(amount, remainingInstances);

                // Agregamos a la lista de intención de spawn
                for (int i = 0; i < amount; i++)
                {
                    pendingSpawns.Add(chosen);

                    if (maxInstances > 0)
                        remainingInstances--;
                }
            }

            if (pendingSpawns.Count == 0)
                return;

            // 4) Spawn positions y Confirmación de Estado
            var safePoly = new Polygon(WalkArea.Polygon.Vertices, -45);
            var points = GetSpawnPoints(safePoly, pendingSpawns.Count, 45);

            // Iteramos solo hasta la cantidad de puntos físicos que conseguimos
            for (int i = 0; i < points.Count; i++)
            {
                var chosenDef = pendingSpawns[i];

                var instance = CreateThingClone(chosenDef.Name);
                instance.Position = points[i];
                Children.Add(instance);

                // CRÍTICO: Los contadores se incrementan SOLO cuando la instancia física existe
                spawnCounter.Increment(chosenDef.Name);
                Session.CurrentRun.Spawns.Increment(chosenDef.Name);
            }
        }

        #endregion

        #region Protected members

        // AddPlaceholder
        protected void AddPlaceholder(Placeholder placeholder)
        {
            placeholders.Add(placeholder);
        }

        // OnChildAdded
        protected override void OnChildAdded(Entity child)
        {
            base.OnChildAdded(child);

            if (child is Sack)
                SackCount++;
        }

        // OnChildRemoved
        protected override void OnChildRemoved(Entity child)
        {
            base.OnChildAdded(child);

            if (child is Sack)
                SackCount--;
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

        // CreateThingClone
        public GameThing CreateThingClone(string declaredName)
        {
            if (Session.CreateThingClone(declaredName, $"{declaredName}*{RoomGraph.Index}_{Name}_{instanceCount}") is not GameThing result)
                throw new InvalidOperationException($"Failed to create runtime clone from'{declaredName}'.");

            instanceCount++;

            return result;
        }

        // IsProcedural
        public override bool IsProcedural => true;

        // Placeholders
        public ReadOnlyCollection<Placeholder> Placeholders { get; }

        // RoomGraph
        public RoomGraph RoomGraph { get; }

        // SackCount
        public int SackCount { get; private set; }

        // ToString
        public override string ToString()
        {
            return RoomGraph.ToString();
        }
    }
}
