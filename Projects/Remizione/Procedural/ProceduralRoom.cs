using Adberration;
using Engendro;
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

            this.randomSeed = seed;
            this.Random = new Random(this.randomSeed);

            this.MonitorStyle = true;

            // Foreground
            this.foreground = new(Session, string.Empty)
            {
                PivotOrigin = RectanglePoint.LeftTop,
                RenderLayer = RenderLayer.Foreground
            };

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
                placeholders.Add(placeholder);
            }

            // Add walls
            foreach (var wall in roomNode.Definition.Walls)
            {
                AddWall(wall);
            }
        }

        #endregion

        #region Private members

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
                    actors[0].ItemReward = GameData.Items.Get(ItemNames.BronzeKey);
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

        // GetHardTypesCount
        private int GetHardTypesCount()
        {
            float roll = Random.NextSingle();
            
            if (roll < 0.20f)
                return 1;
            
            if (roll < 0.70f)
                return 2;
            
            return 3;
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
            foreach (var door in doors)
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
            if (Session.CurrentRun == null)
                return;

            var hasDarknessModifier = Session.CurrentRun.Modifiers.IsActive(RunModifierNames.Darkness);

            if (AmbientLights && hasDarknessModifier)
            {
                Session.CurrentRun.Modifiers.Deactivate(RunModifierNames.Darkness);
            }
            else if (!AmbientLights && !hasDarknessModifier)
            {
                Session.CurrentRun.Modifiers.Activate(RunModifierNames.Darkness);
            }
        }

        // SpawnActors
        private void SpawnActors()
        {
            if (WalkArea == null || Session.CurrentRun is not Run run)
                return;

            // 1. Pacing: ¿Esta sala debería tener combate?
            float combatChance = RoomNode.TopographicDifficulty switch
            {
                Difficulty.Easy => 0.5f,
                Difficulty.Normal => 0.7f,
                Difficulty.Hard => 0.85f,
                _ => 0.50f
            };

            if (Random.NextSingle() > combatChance)
                return;

            var candidates = ProceduralUtils.GetCandidateDefinitions(run, RoomNode, GameData.Actors);
            if (candidates.Count == 0)
                return;

            // 2. Filtrar candidatos compatibles por dificultad de la sala
            var validCandidates = new List<ActorDefinition>();
            for (int i = 0; i < candidates.Count; i++)
            {
                var c = candidates[i];

                if (!RoomNode.Definition.AllowEnemies && c.Faction == Faction.Evil)
                    continue;

                if (!ProceduralUtils.IsValidActorForRoom(RoomNode.TopographicDifficulty, c.Difficulty))
                    continue;

                validCandidates.Add(c);
            }

            if (validCandidates.Count == 0)
                return;

            // 3. Definir la cantidad de slots (tipos distintos)
            int targetTypesCount = RoomNode.TopographicDifficulty switch
            {
                Difficulty.Easy => Random.NextSingle() < 0.90f ? 1 : 2,
                Difficulty.Normal => Random.NextSingle() < 0.70f ? 1 : 2,
                Difficulty.Hard => GetHardTypesCount(),
                _ => 1
            };

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
            if (Session.CurrentRun is not Run run)
                return;

            var candidates = ProceduralUtils.GetCandidateDefinitions(run, RoomNode, GameData.Props);
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
                        SpawnThing<Prop>(run, nameof(PlaceholderState.GateLever), ph.Position, propsSpawnCounter);
                        RoomNode.SetPlaceholderState(ph, PlaceholderState.Used);
                        continue;
                    }

                    // 1. CONDICIÓN DE SLOT (Placeholder)
                    // Si la estrategia NO es ContentChanceOnly, el placeholder debe pasar su tirada de FillChance.
                    if (ph.SpawnRule != PlaceholderSpawnRule.ContentChanceOnly)
                    {
                        if (!ph.FillChance.Roll(Random))
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

                        if (!def.PassesMaxPerRunConstraint(run.Spawns.GetCount(def.Name)))
                            continue;

                        float finalWeight = ProceduralUtils.AdjustPropWeight(RoomNode.TopographicDifficulty, def.Difficulty, def.SpawnWeight);

                        // 3. CONDICIÓN DE CONTENIDO (Prop)
                        // Si la estrategia NO es PlaceholderChanceOnly, el prop debe pasar su tirada individual de rareza.
                        if (ph.SpawnRule != PlaceholderSpawnRule.PlaceholderChanceOnly)
                        {
                            if (Random.NextSingle() > MathHelper.Clamp(finalWeight, 0f, 1f))
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
                    if (phTable.GetValue() is ChanceTableItem item &&
                        GameData.Props.Find(item.Name) is PropDefinition chosen)
                    {
                        SpawnThing<Prop>(run, chosen.Name, ph.Position, propsSpawnCounter);
                        RoomNode.SetPlaceholderState(ph, PlaceholderState.Used);
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

                if (!def.PassesMaxPerRunConstraint(run.Spawns.GetCount(def.Name)))
                    continue;

                freeCandidates.Add(def);
            }

            if (freeCandidates.Count == 0)
                return;

            var occupiedPositions = new List<Vector2>();
            int maxAttemptsInRoom = Random.Next(1, 3); // 1 a 2 intentos de apariciones libres por sala

            // 2. Bucle de apariciones por Tirada Absoluta
            while (maxAttemptsInRoom > 0 && freeCandidates.Count > 0)
            {
                maxAttemptsInRoom--;

                // Elegimos un candidato al azar del pool de elegibles
                var chosen = freeCandidates.GetRandomItem(Random);

                if (chosen == null)
                    continue;

                // Calculamos su probabilidad ajustada por la dificultad topográfica de la sala (0.0f a 1.0f)
                float finalChance = ProceduralUtils.AdjustPropWeight(RoomNode.TopographicDifficulty, chosen.Difficulty, chosen.SpawnWeight);

                // Tirada Absoluta: Si el dado no supera la probabilidad, este intento queda VACÍO de forma natural
                if (Random.NextSingle() > MathHelper.Clamp(finalChance, 0f, 1f))
                    continue;

                // Si pasó la tirada de rareza, pedimos la posición al WalkArea
                Vector2 spawnPosition = WalkArea.RandomWalkablePoint(Random);

                if (spawnPosition == Vector2.Zero || occupiedPositions.Contains(spawnPosition))
                    continue;

                occupiedPositions.Add(spawnPosition);

                // Instanciación directa
                var instance = CreateThingClone<Prop>(chosen.Name);
                instance.Position = spawnPosition;
                Children.Add(instance);

                run.Spawns.Increment(chosen.Name);
                propsSpawnCounter.Increment(chosen.Name);

                // Si el prop alcanzó su límite por sala, lo removemos del pool de candidatos
                if (!chosen.PassesMaxPerRoomConstraint(propsSpawnCounter.GetCount(chosen.Name)))
                    freeCandidates.Remove(chosen);
            }

            #endregion
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

        // OnActivate
        protected override void OnActivate()
        {
            base.OnActivate();

            if (!RoomNode.Visited)
                RoomNode.Visited = true;

            if (Session.CurrentRun != null)
            {
                foreach (var name in RoomNode.Definition.RunModifiers)
                {
                    Session.CurrentRun.Modifiers.Activate(name);
                }
            }

            Session.HUD?.MiniMap.CurrentRoom = RoomNode;
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
            Session.CurrentRun?.Modifiers.Clear(RunModifierScope.Room);
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

            if (Session.CurrentRun?.FloorDescriptor?.Darkness == true)
                TurnOffAmbientLights();
        }

        // OnRefreshAmbientLightSources
        protected override void OnRefreshAmbientLightSources()
        {
            base.OnRefreshAmbientLightSources();
            RefreshRunModifiers();
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