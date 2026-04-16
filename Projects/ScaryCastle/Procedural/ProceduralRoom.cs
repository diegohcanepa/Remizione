using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Globalization;

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
        private readonly HashSet<Placeholder> usedPlaceholders = [];

        #endregion

        #region Constructor

        // Constructor
        protected ProceduralRoom(GameSession session, string name, RoomNode roomNode)
            : base(session, name)
        {
            this.RoomNode = roomNode;

            this.AllowGlobalLight = true;
            this.LightingSystem = true;
            this.UnloadMode = UnloadMode.Manual;

            int salt = roomNode.Index;
            this.randomSeed = RandomHelper.GetSeed(Session.Seed, salt);
            this.Random = new Random(randomSeed);
            this.Placeholders = placeholders.AsReadOnly();
        }

        #endregion

        #region Private members

        // CalculateEnemyBudget
        private int CalculateEnemyBudget(Run run)
        {
            // 1. Definimos rangos mínimos y máximos según la dificultad de la zona
            // Estos números son los "puntos de control". 
            // Los mantenemos bajos (máximo 6) para no saturar el espacio.
            var (min, max) = RoomNode.Definition.Difficulty switch
            {
                Difficulty.Easy => (1, 1), // Muy tranquilo
                Difficulty.Normal => (1, 2), // Reto estándar
                Difficulty.Hard => (1, 3), // Presión alta, pero navegable
                _ => (0, 0)
            };

            // 2. Usamos la Intensity (que ya tiene tu curva exponencial) para mover el presupuesto
            // Si intensity es 0, tiende al min. Si es 1, tiende al max.
            float baseBudget = min + (max - min) * run.Intensity;

            // 3. Variación aleatoria (+-1) para que no todas las salas de la misma zona sean iguales
            int finalBudget = (int)Math.Round(baseBudget) + Random.Next(-1, 2);

            // 4. Clamp final para respetar los límites físicos que decidimos
            return Math.Clamp(finalBudget, min, max);
        }

        // GetCandidateDefinitions
        private List<TDefinition> GetCandidateDefinitions<TDefinition, TThing>(IList<TDefinition> definitions, Func<TDefinition, bool>? predicate = null)
            where TDefinition : ThingDefinition where TThing : GameThing
        {
            var outList = new List<TDefinition>();
            if (Session.CurrentRun == null)
                return outList;

            foreach (var definition in definitions)
            {
                if (predicate != null && !predicate(definition))
                    continue;

                // 1. Filtro de dificultad: No permitimos que aparezcan cosas más difíciles que el cuarto
                if (definition.Difficulty > RoomNode.Definition.Difficulty)
                    continue;

                // 2. Filtro de topologia
                if (definition.RequiresDeadEnd && RoomNode.ConnectionCount > 1)
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
                if (!definition.PassesMaxPerRunConstraint(Session.CurrentRun.Spawns))
                    continue;

                // 6. Reglas de Scope (Pools/Tags de la habitación)
                if (!TagScope.Test(RoomNode.Definition.Scope, RoomNode.Definition.Pools, RoomNode.Definition.Tags))
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
            SpawnProps();
            if (RoomNode.Definition.AllowEnemies)
                SpawnEnemies();
        }

        // SpawnEnemies
        private void SpawnEnemies()
        {
            if (WalkArea == null || Session.CurrentRun == null)
                return;

            // 1) Collect candidates
            var candidates = GetCandidateDefinitions<ActorDefinition, Actor>(ActorDefinition.Definitions.All, def => def.Role == ActorRole.Ambient);
            var selectedCandidates = new List<ActorDefinition>();
            foreach (var definition in candidates)
            {
                if (!definition.PassesMaxPerRoomConstraint(enemiesSpawnCounter.GetCount(definition.Name)))
                    continue;

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
                var finalWeight = AdjustWeight(Session.CurrentRun.Intensity, RoomNode.Definition.Difficulty, c.Difficulty, c.SpawnWeight);
                table.Add(c.Name, finalWeight);
            }

            // Usamos una lista de definiciones concretas en lugar de solo strings
            var pendingSpawns = new List<ActorDefinition>();
            int remainingInstances = CalculateEnemyBudget(Session.CurrentRun);
            int safety = (selectedCandidates.Count * 2) + 10;

            // 3) Pick groups (Solo intención, no alteramos estado global)
            while (table.Count > 0 && safety-- > 0)
            {
                if (remainingInstances <= 0)
                    break;

                if (table.GetValue() is not ChanceTableItem item)
                    break;

                table.Remove(item.Name);

                if (ActorDefinition.Definitions.Find(item.Name) is not ActorDefinition chosen)
                    continue;

                pendingSpawns.Add(chosen);
                remainingInstances--;
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
                enemiesSpawnCounter.Increment(chosenDef.Name);
                Session.CurrentRun.Spawns.Increment(chosenDef.Name);
            }
        }

        // SpawnProps
        private void SpawnProps()
        {
            if (Session.CurrentRun == null || Placeholders.Count == 0)
                return;

            var candidates = GetCandidateDefinitions<PropDefinition, Prop>(PropDefinition.Definitions.All);

            // 1) Shuffle placeholders
            var placeholders = new List<Placeholder>(Placeholders);
            placeholders.Shuffle(Random);

            // 2) Iterate placeholders
            foreach (var placeholder in placeholders)
            {
                // Already used
                if (usedPlaceholders.Contains(placeholder))
                    continue;

                // Roll fillChance
                if (!placeholder.FillChance.Roll(Random))
                    continue;

                // Collect candidates
                var selectedCandidates = new List<PropDefinition>();
                foreach (var definition in candidates)
                {
                    // Is compatible with placeholder placement?
                    if (!definition.Placements.Contains(placeholder.Placement))
                        continue;

                    // Match tags?
                    if (placeholder.AllowTags.Count > 0 && !placeholder.AllowTags.Intersects(definition.Tags))
                        continue;

                    // MaxPerRoom (local)
                    if (!definition.PassesMaxPerRoomConstraint(propsSpawnCounter.GetCount(definition.Name)))
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
                    var finalWeight = AdjustWeight(Session.CurrentRun.Intensity, RoomNode.Definition.Difficulty, c.Difficulty, c.SpawnWeight);
                    chanceTable.Add(c.Name, finalWeight);
                }

                if (chanceTable.GetValue() is not ChanceTableItem chanceTableItem)
                    continue;

                if (PropDefinition.Definitions.Find(chanceTableItem.Name) is not PropDefinition chosen)
                    continue;

                // Flag placeholder as used
                usedPlaceholders.Add(placeholder);

                var instance = CreateThingClone(chosen.Name);
                instance.Position = placeholder.Position;
                Children.Add(instance);

                // Log spawn in run
                Session.CurrentRun.Spawns.Increment(chosen.Name);

                int currentRoomCount = propsSpawnCounter.Increment(chosen.Name);

                // Max per room
                if (chosen.MaxPerRun > 0 && currentRoomCount >= chosen.MaxPerRun)
                    return;
            }
        }

        #endregion

        #region Protected members

        // AddPlaceholder
        protected void AddPlaceholder(Placeholder placeholder)
        {
            placeholders.Add(placeholder);
        }

        // AdjustWeight
        // Modifica el peso de aparición combinando la dificultad base de la sala y la intensidad global de la partida.
        protected static float AdjustWeight(float runIntensity, Difficulty roomDiff, Difficulty thingDiff, float baseWeight)
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
            if (Session.CreateThingClone(declaredName, $"{declaredName}*{RoomNode.Index}_{Name}_{instanceCount}") is not GameThing result)
                throw new InvalidOperationException($"Failed to create runtime clone from'{declaredName}'.");

            instanceCount++;

            return result;
        }

        // IsProcedural
        public override bool IsProcedural => true;

        // Placeholders
        public ReadOnlyCollection<Placeholder> Placeholders { get; }

        // RoomNode
        public RoomNode RoomNode { get; }

        // SackCount
        public int SackCount { get; private set; }

        // ToString
        public override string ToString()
        {
            return RoomNode.ToString();
        }
    }
}
