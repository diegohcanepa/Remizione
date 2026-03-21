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

        private readonly MultiCounter enemiesSpawnCounter = new();
        private int instanceCount;
        private readonly List<Placeholder> placeholders = [];
        private readonly MultiCounter propsSpawnCounter = new();
        private readonly int randomSeed;

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

        // AdjustWeightByDifficulty
        // Aplica un multiplicador al peso original basado en la disparidad de dificultad.
        private static float AdjustWeightByDifficulty(Difficulty roomDiff, Difficulty thingDiff, float baseWeight)
        {
            // Si coinciden, es el peso ideal.
            if (roomDiff == thingDiff)
                return baseWeight;

            // Calculamos la distancia (Easy=0, Medium=1, Hard=2)
            int distance = (int)roomDiff - (int)thingDiff;

            float multiplier = distance switch
            {
                1 => 0.15f,  // Ej: Sala Medium, Enemigo Easy (1 escalón de diferencia)
                2 => 0.02f,  // Ej: Sala Hard, Enemigo Easy (2 escalones de diferencia)
                _ => 1.0f    // Por seguridad, aunque el techo ya filtra los negativos
            };

            return baseWeight * multiplier;
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
                // 1. Filtro de dificultad: No permitimos que aparezcan cosas más difíciles que el cuarto
                if (definition.Difficulty > RoomGraph.Definition.Difficulty)
                    continue;

                // 2. Filtro de topologia
                if (definition.RequiresDeadEnd && RoomGraph.ConnectionCount > 1)
                    continue;

                // 3. Validación de Existencia de instancia declarada en script
                var thing = Session.FindDeclaredThing(definition.Name) ?? throw new InvalidOperationException($"There is no declared thing named '{definition.Name}'. ");

                // Is expected type?
                if (thing is not TThing)
                    continue;

                // 4. Meta-progreso
                // Chequea si el enemigo está desbloqueado (MinRun)
                if (!definition.PassesRunConstraints(Session.RunCount))
                    continue;

                // 5. Historial de la Run
                // Chequea si el enemigo ya alcanzó su MaxPerRun global
                if (!definition.PassesMaxPerRunConstraint(Session.CurrentRun.RunSpawns))
                    continue;

                // 6. Reglas de Scope (Pools/Tags de la habitación)
                if (!definition.PassesScope(RoomGraph.Definition.Scope))
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
        private void Populate()
        {
            PopulateProps();
            PopulateNPCs();
        }

        // PopulateNPCs
        private void PopulateNPCs()
        {
            var definitions = GetCandidateDefinitions<ActorDefinition, Actor>(ActorDefinition.Definitions.All);
            SpawnInPlaceholders(ActorDefinition.Definitions, definitions, RoomGraph.Definition.MaxEnemies, enemiesSpawnCounter, PlaceholderTarget.Enemy);
            SpawnInWalkArea(ActorDefinition.Definitions, definitions, RoomGraph.Definition.MaxEnemies, enemiesSpawnCounter);
        }

        // PopulateProps
        private void PopulateProps()
        {
            var definitions = GetCandidateDefinitions<PropDefinition, Prop>(PropDefinition.Definitions.All);
            SpawnInPlaceholders(PropDefinition.Definitions, definitions, RoomGraph.Definition.MaxProps, propsSpawnCounter, PlaceholderTarget.Prop);
            SpawnInWalkArea(PropDefinition.Definitions, definitions, RoomGraph.Definition.MaxProps, propsSpawnCounter);
        }

        // SpawnInPlaceholders
        private void SpawnInPlaceholders<T>(DataContainer<T> definitionContainer, IList<T> candidates, int maxInstances, MultiCounter spawnCounter, PlaceholderTarget target)
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
                if (placeholder.Used)
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
                    // Is compatible with placehokder placement?
                    if (!definition.Placements.Contains(placeholder.Placement))
                        continue;

                    // MaxPerRoom (local)
                    if (!definition.PassesMaxPerRoomConstraint(spawnCounter.GetCount(definition.Name)))
                        continue;

                    // MaxPerRun (Global)
                    if (!definition.PassesMaxPerRunConstraint(Session.CurrentRun.RunSpawns))
                        continue;

                    selectedCandidates.Add(definition);
                }

                if (selectedCandidates.Count == 0)
                    continue;

                // Pick
                var chanceTable = new ChanceTable();
                foreach (var c in selectedCandidates)
                {
                    var finalWeight = AdjustWeightByDifficulty(RoomGraph.Definition.Difficulty, c.Difficulty, c.SpawnWeight);
                    chanceTable.Add(c.Name, finalWeight);
                }

                if (chanceTable.GetValue() is not ChanceTableItem chanceTableItem)
                    continue;

                if (definitionContainer.Find(chanceTableItem.Name) is not T chosen)
                    continue;

                // Flag placeholder as used
                placeholder.Used = true;

                var instance = CreateThingClone(chosen.Name);
                instance.Position = placeholder.Position;
                Children.Add(instance);

                // Log spawn in run
                Session.CurrentRun.RunSpawns.Increment(chosen.Name);

                int currentRoomCount = spawnCounter.Increment(chosen.Name);
                
                // Max per room
                if (maxInstances != -1 && currentRoomCount >= maxInstances)
                    return;
            }
        }

        // SpawnInWalkArea
        // Procesa la aparición de entidades en áreas caminables asegurando la sincronización de estado.
        private void SpawnInWalkArea<T>(DataContainer<T> definitionContainer, IList<T> candidates, int maxInstances, MultiCounter spawnCounter)
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
                if (!definition.PassesMaxPerRunConstraint(Session.CurrentRun.RunSpawns.GetCount(definition.Name)))
                    continue;

                selectedCandidates.Add(definition);
            }

            if (selectedCandidates.Count == 0)
                return;

            // 2) Build chance table
            var table = new ChanceTable();
            foreach (var c in selectedCandidates)
            {
                var finalWeight = AdjustWeightByDifficulty(RoomGraph.Definition.Difficulty, c.Difficulty, c.SpawnWeight);
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
            var safePoly = new Polygon(WalkArea.Polygon.Vertices, -30);
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
                Session.CurrentRun.RunSpawns.Increment(chosenDef.Name);
            }
        }

        #endregion

        #region Protected members

        // AddPlaceholder
        protected void AddPlaceholder(Placeholder placeholder)
        {
            placeholders.Add(placeholder);
        }

        // DropLoot
        protected virtual void DropLoot()
        {
        }

        // GetDropLootPosition
        protected Vector2 GetDropLootPosition()
        {
            return WalkArea != null ? WalkArea.Polygon.BoundingRectangleF.Center : BoundingBox.Center;
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
