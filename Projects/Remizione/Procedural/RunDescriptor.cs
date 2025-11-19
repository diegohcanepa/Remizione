using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// RunDescriptor
    /// </summary>
    public class RunDescriptor
    {
        // Constructor
        public RunDescriptor(int pathCount)
        {
            Paths = new RoomDescriptor[pathCount];
        }

        #region Private members

        // GetMainPathRooms
        private static List<RoomDescriptor> GetMainPathRooms(RoomDescriptor root)
        {
            var list = new List<RoomDescriptor>();
            var cur = root;
            while (cur != null)
            {
                list.Add(cur);
                cur = cur.Up;
            }
            return list;
        }

        #endregion

        // GetRoomDescriptors
        public List<RoomDescriptor> GetRoomDescriptors(int pathIndex)
        {
            var root = Paths[pathIndex];
            var result = new List<RoomDescriptor>();
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

        // GetRoomDescriptor
        public RoomDescriptor? GetRoomDescriptor(int roomIndex)
        {
            for (var i = 0; i < Paths.Length; i++)
            {
                var pathRooms = GetRoomDescriptors(i);

                foreach (var room in pathRooms)
                {
                    if (room.Id == roomIndex)
                        return room;
                }
            }

            return null;
        }

        // Paths
        public RoomDescriptor[] Paths { get; }
    }
}
