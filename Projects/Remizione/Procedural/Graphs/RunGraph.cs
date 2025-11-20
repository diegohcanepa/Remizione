using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// RunGraph
    /// </summary>
    public class RunGraph
    {
        // Constructor
        public RunGraph(IList<RoomGraph> entryRooms)
        {
            this.EntryRooms = new ReadOnlyCollection<RoomGraph>(entryRooms);
        }

        #region Private members

        // GetMainPathRooms
        private static List<RoomGraph> GetMainPathRooms(RoomGraph entryRoom)
        {
            var list = new List<RoomGraph>();
            var cur = entryRoom;
            while (cur != null)
            {
                list.Add(cur);
                cur = cur.Up;
            }
            return list;
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
