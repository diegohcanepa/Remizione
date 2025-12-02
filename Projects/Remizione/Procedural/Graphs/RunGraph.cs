using Engendro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// RunGraph
    /// </summary>
    public class RunGraph
    {
        private readonly List<RoomConfig> availableRoomConfigs;
        private readonly GameSession session;

        // Constructor
        public RunGraph(GameSession session, IList<RoomGraph> entryRooms, Random random, Tags tags)
        {
            this.session = session;

            this.EntryRooms = new ReadOnlyCollection<RoomGraph>(entryRooms);

            if (PlaceCoin(random) is RoomGraph roomGraph)
                roomGraph.HasCoin = true;

            this.availableRoomConfigs = GetAvailableRooms(tags);

            for (var i = 0; i < entryRooms.Count; i++)
            {
                AssignRoomTypes(i);
            }
        }

        #region Private members

        // AssignRoomTypes
        private void AssignRoomTypes(int pathIndex)
        {
            var roomGraphs = GetRooms(pathIndex);
            var candidates = new List<RoomConfig>();

            // Room graphs in path index
            for (var i = 0; i < roomGraphs.Count; i++)
            {
                candidates.Clear();
                
                foreach (var roomConfig in availableRoomConfigs)
                {
                    if (!roomConfig.PassesMaxPerRunConstraint())
                        continue;
                    
                    candidates.Add(roomConfig);
                }

                // Pick
                var chanceTable = new ChanceTable();
                foreach (var candidate in candidates)
                {
                    chanceTable.Add(candidate.Name, candidate.Weight, 1, candidate);
                }

                if (chanceTable.GetValue() is ChanceTableItem chanceTableItem && chanceTableItem.Tag is RoomConfig chosenConfig)
                {
                    RunManager.SpawnCounter.Increment(chosenConfig.Name);
                    roomGraphs[i].Config = chosenConfig;
                }
            }
        }

        // GetAvailableRooms
        private List<RoomConfig> GetAvailableRooms(Tags tags)
        {
            var outList = new List<RoomConfig>();

            foreach (var roomConfig in RoomConfig.All)
            {
                if (!session.UnlockedPool.IsUnlocked(roomConfig.Name))
                    continue;

                // Run constraints
                if (!roomConfig.PassesRunConstraints(session))
                    continue;

                // Tags
                if (tags.Count > 0 && roomConfig.Tags.Count > 0)
                {
                    if (!Utils.Intersects(tags, roomConfig.Tags))
                        continue;
                }

                // Passed all checks
                outList.Add(roomConfig);
            }

            return outList;
        }

        // GetMainPathRooms
        private static List<RoomGraph> GetMainPathRooms(RoomGraph entryRoom)
        {
            var result = new List<RoomGraph>();
            var current = entryRoom;
            while (current != null)
            {
                result.Add(current);
                current = current.Up;
            }
            return result;
        }

        // PlaceCoin
        private RoomGraph? PlaceCoin(Random random)
        {
            if (EntryRooms.Count == 0)
                return null;

            // Choose random path
            var pathIndex = random.Next(EntryRooms.Count);
            var root = EntryRooms[pathIndex];

            var main = GetMainPathRooms(root);
            if (main.Count == 0)
                return null;

            // inicio de la "mitad superior" (incluye el punto medio)
            int startIndex = main.Count / 2;

            var candidates = new List<RoomGraph>();

            for (int i = startIndex; i < main.Count; i++)
            {
                var m = main[i];
                candidates.Add(m);

                if (m.Left != null)
                    candidates.Add(m.Left);

                if (m.Right != null)
                    candidates.Add(m.Right);
            }

            // Si por alguna razón no hay candidatos (muy raro), fallback al último main
            if (candidates.Count == 0)
                return main[main.Count - 1];

            return candidates[random.Next(candidates.Count)];
        }

        #endregion

        // EntryRooms
        public ReadOnlyCollection<RoomGraph> EntryRooms { get; }

        // GetRoom
        public RoomGraph? GetRoom(int roomId)
        {
            for (var i = 0; i < EntryRooms.Count; i++)
            {
                var pathRooms = GetRooms(i);

                foreach (var room in pathRooms)
                {
                    if (room.Id == roomId)
                        return room;
                }
            }

            return null;
        }

        // GetRooms
        public List<RoomGraph> GetRooms(int pathIndex)
        {
            var root = EntryRooms[pathIndex];
            var result = new List<RoomGraph>();
            var main = GetMainPathRooms(root);

            for (int i = 0; i < main.Count; i++)
            {
                var m = main[i];

                result.Add(m);

                if (m.Left != null)
                    result.Add(m.Left);

                if (m.Right != null)
                    result.Add(m.Right);
            }

            return result;
        }
    }
}
