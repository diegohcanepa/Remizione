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
    public class ProceduralRoom : GameRoom
    {
        #region Private fields

        private readonly CounterBank actorsSpawnCounter = new();
        private readonly List<Door> doors = [];
        private readonly Prop foreground;
        private int instanceCount;
        private readonly List<Placeholder> placeholders = [];
        private readonly CounterBank propsSpawnCounter = new();
        private readonly int randomSeed;

        #endregion

        #region Constructor

        // Constructor
        public ProceduralRoom(GameSession session, RoomNode roomNode, int seed)
            : base(session, string.Empty)
        {
            this.RoomNode = roomNode;
            this.LightingSystem = true;
            this.UnloadMode = UnloadMode.Manual;

            // MAGIA PURA Y LIMPIA:
            this.randomSeed = seed;
            this.Random = new Random(this.randomSeed);

            this.MonitorStyle = true;

            // Foreground
            this.foreground = new(Session, string.Empty)
            {
                PivotOrigin = RectanglePoint.LeftTop,
                RenderLayer = RenderLayer.Foreground
            };

            this.Doors = doors.AsReadOnly();

            Zoom = 1.15f;
            AtlasName = roomNode.Definition.Name ?? string.Empty;
            DefaultImageName = AtlasName;
            DustParticleKind = DustParticleKind.Ash;
            LightMapColor = roomNode.Definition.LightMapColor;
            LightingSystem = true;

            AddWalkArea("WalkArea", roomNode.Definition.WalkArea);

            // Add lights
            var index = 0;
            foreach (var lightDescriptor in roomNode.Definition.Lights)
            {
                var light = AddLight($"Light{index}__");
                light.Ambient = true;
                light.Color = lightDescriptor.Color;
                light.Position = lightDescriptor.Position;
                light.Scale = lightDescriptor.Scale;
                index++;
            }

            // Add placeholders
            foreach (var placeholder in roomNode.Definition.Placeholders)
            {
                AddPlaceholder(placeholder);
            }

            // Add walls
            foreach (var wall in roomNode.Definition.Walls)
            {
                AddWall(wall);
            }
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

        // DistributeBronzeKeys
        private void DistributeBronzeKeys()
        {
            if (RoomNode.BronzeKeys <= 0)
                return;

            // Collect actors
            var actors = new List<Actor>();
            foreach (var actor in Children.OfType<Actor>())
            {
                if (actor.IsDead || actor.IsPlayer || actor.Faction != Faction.Evil)
                    continue;

                if (actor.Definition?.DropTrigger == LootDropTrigger.OnImpact)
                    continue;

                actors.Add(actor);
            }
            actors.Shuffle(this.Random);

            // Collect pottery
            var potteryList = new List<Pottery>();
            foreach (var pottery in Children.OfType<Pottery>())
            {
                if (pottery.CanHideLoot)
                    potteryList.Add(pottery);
            }
            potteryList.Shuffle(this.Random);

            var pendingKeys = RoomNode.BronzeKeys;

            // Randomly hide a bronze key under a pot (Chance = 20%)
            if (potteryList.Count > 0 && DiceExpression.Dice10.Roll(this.Random) <= 2)
            {
                if (potteryList.GetRandomItem(this.Random) is Pottery pot)
                {
                    DropBronzeKey(pot.Position - new Vector2(0, 2));
                    pendingKeys--;
                    if (pendingKeys <= 0)
                        return;
                }
            }

            // Randomly drop a bronze key in the room (Chance = 20%)
            if (DiceExpression.Dice10.Roll(this.Random) <= 2)
            {
                DropBronzeKey();
                pendingKeys--;
                if (pendingKeys <= 0)
                    return;
            }

            // Distribute bronze keys among actors
            while (pendingKeys > 0 && actors.Count > 0)
            {
                if (actors.Count > 0)
                {
                    actors[0].ItemReward = ItemDefinition.Container.Get(ItemNames.BronzeKey);
                    actors.RemoveAt(0);
                    pendingKeys--;
                    if (pendingKeys <= 0)
                        return;
                }
            }

            // If there are still pending keys, drop them in the room
            while (pendingKeys > 0)
            {
                DropBronzeKey();
                pendingKeys--;
            }
        }

        // DropBronzeKey
        private void DropBronzeKey(Vector2? position = null)
        {
            var key = CreateThingClone<Prop>(ItemNames.BronzeKey);
            Children.Add(key);

            if (position.HasValue)
            {
                key.Position = position.Value;
            }
            else if (WalkArea != null)
            {
                key.Position = WalkArea.RandomWalkablePoint(Random, 30);
            }
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

        // LockDoorsAccordingly
        private void LockDoorsAccordingly()
        {
            foreach (var door in doors)
            {
                if (RoomNode.LockedDoors.TryGetValue(door.DoorDirection, out LockType lockType))
                    door.LockType = lockType;
            }
        }

        // PrepareLights
        private void PrepareLights()
        {
            foreach (var door in Doors)
            {
                var doorLight = AddLight(door.Name);
                //doorLight.Ambient = true;
                doorLight.Color = new Color(240, 181, 65) * .7f;
                doorLight.Position = door.BoundingBox.Center;
                doorLight.Scale = new(2, 7);
            }
        }

        // PrepareView
        private void PrepareView()
        {
            if (Atlas == null)
                return;

            var index = 0;
            while (true)
            {
                if (Atlas.FindImage($"View{index + 1}") == null)
                    break;
                else
                    index++;
            }

            if (index > 0)
            {
                var animation = AddAnimation("View");
                var viewName = $"View{this.Random.Next(1, index + 1)}";
                animation.AddFrame(viewName, 10000);

                var foregroundImageName = viewName + "Foreground";
                var hasForeground = Atlas.Contains(foregroundImageName);
                if (!hasForeground)
                    foregroundImageName = "Foreground";
                hasForeground = Atlas.Contains(foregroundImageName);

                if (hasForeground)
                {
                    foreground.Atlas = Atlas;
                    foreground.DefaultImageName = foregroundImageName;
                    foreground.ParallaxFactor = new(1.1f, 1);
                    //foreground.Position = new(0, 15);
                    Children.Add(foreground);
                }
            }
        }

        // RefreshRunModifiers
        private void RefreshRunModifiers()
        {
            var hasDarknessModifier = Session.RunModifiers.IsActive(RunModifierNames.Darkness);

            if (AmbientLights && hasDarknessModifier)
            {
                Session.RunModifiers.Deactivate(RunModifierNames.Darkness);
            }
            else if (!AmbientLights && !hasDarknessModifier)
            {
                Session.RunModifiers.Activate(RunModifierNames.Darkness);
            }
        }

        // SpawnActors
        private void SpawnActors()
        {
            if (WalkArea == null || Session.CurrentRun == null)
                return;

            var candidates = GetCandidateDefinitions<ActorDefinition, Actor>(ActorDefinition.Container.All);

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

                if (ActorDefinition.Container.Find(item.Name) is not ActorDefinition chosen)
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

        // SpawnDoors
        private void SpawnDoors()
        {
            var def = this.RoomNode.Definition;

            // Up
            if (RoomNode.Up != null && def.DoorUp != null && CreateThingClone<Door>("DoorUp") is Door upDoor)
            {
                doors.Add(upDoor);
                Children.Add(upDoor);
                upDoor.Position = def.DoorUp.Value;
                upDoor.TargetRoom = RoomNode.Up.Room;
            }

            // Left
            if (RoomNode.Left != null && def.DoorLeft != null && CreateThingClone<Door>("DoorLeft") is Door leftDoor)
            {
                doors.Add(leftDoor);
                Children.Add(leftDoor);
                leftDoor.Position = def.DoorLeft.Value;
                leftDoor.TargetRoom = RoomNode.Left.Room;
            }

            // Right
            if (RoomNode.Right != null && def.DoorRight != null && CreateThingClone<Door>("DoorRight") is Door rightDoor)
            {
                doors.Add(rightDoor);
                Children.Add(rightDoor);
                rightDoor.Position = def.DoorRight.Value;
                rightDoor.TargetRoom = RoomNode.Right.Room;
            }

            // Down
            if (RoomNode.Down != null)
            {
                if (def.DoorDown != null && CreateThingClone<Door>("DoorDown") is Door downDoor)
                {
                    doors.Add(downDoor);
                    Children.Add(downDoor);
                    downDoor.Position = def.DoorDown.Value;

                    if (RoomNode.Down != null)
                        downDoor.TargetRoom = RoomNode.Down.Room;
                }
            }
        }

        // SpawnProps
        private void SpawnProps()
        {
            if (Session.CurrentRun == null)
                return;

            var candidates = GetCandidateDefinitions<PropDefinition, Prop>(PropDefinition.Container.All);

            if (candidates.Count == 0)
                return;

            #region Placeholders

            if (placeholders.Count > 0)
            {
                var shuffledPlaceholders = new List<Placeholder>(placeholders);
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

                    if (PropDefinition.Container.Find(item.Name) is not PropDefinition chosen)
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

                if (PropDefinition.Container.Find(item.Name) is not PropDefinition chosen)
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

            if (instance is ISpawnNotification spawnNotification)
                spawnNotification.OnSpawned(this);

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

        // OnActivate
        protected override void OnActivate()
        {
            base.OnActivate();

            if (!RoomNode.Visited)
                RoomNode.Visited = true;

            foreach (var name in RoomNode.Definition.RunModifiers)
            {
                Session.RunModifiers.Activate(name);
            }

            Session.StatusHUD.MiniMap.CurrentRoom = RoomNode;
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

        // OnDeactivate
        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            Session.RunModifiers.Clear(RunModifierScope.Room);
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();

            CustomWidth = (int)BoundingBox.Width;
            CustomHeight = (int)BoundingBox.Height;

            SpawnDoors();
            SpawnProps();
            SpawnActors();

            // Prepare doors
            var doors = new List<Door>(Children.OfType<Door>());
            for (var i = 0; i < doors.Count; i++)
            {
                doors[i].Prepare();
            }

            LockDoorsAccordingly();
            PrepareView();
            PrepareLights();
            DistributeBronzeKeys();
        }

        // OnRefreshAmbientLightSources
        protected override void OnRefreshAmbientLightSources()
        {
            base.OnRefreshAmbientLightSources();
            RefreshRunModifiers();
        }

        #endregion

        // CreateInstance
        public static ProceduralRoom CreateInstance(GameSession session, RoomNode roomNode, int roomSeed)
        {
            // Get type from AOT registry
            if (Activator.CreateInstance(typeof(ProceduralRoom), session, roomNode, roomSeed) is not ProceduralRoom result)
                throw new InvalidOperationException($"Cannot create instance [{roomNode.Definition.Name}]");

            return result;
        }

        // CreateThingClone
        public T CreateThingClone<T>(string declaredName) where T : GameThing
        {
            if (Session.CreateThingClone(declaredName, $"{declaredName}*{RoomNode.Index}_{Name}_{instanceCount}") is not T result)
                throw new InvalidOperationException($"Failed to create runtime clone from'{declaredName}'.");

            instanceCount++;

            return result;
        }

        // Doors
        public ReadOnlyCollection<Door> Doors { get; }

        // GetPlayerPosition
        public Vector2 GetPlayerPosition(int previousRoomIndex, out Door? targetDoor)
        {
            targetDoor = null;

            foreach (var door in Children.OfType<Door>())
            {
                if ((previousRoomIndex == -1 && door.DoorDirection == DoorDirection.Down) ||
                     door.TargetRoom?.RoomNode.Index == previousRoomIndex)
                {
                    targetDoor = door;
                    return door.GetAnchoredPosition(door.ApproachPosition);
                }
            }

            return Vector2.Zero;
        }

        // IsProcedural
        public override bool IsProcedural => true;

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