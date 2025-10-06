using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// RunInfo
    /// </summary>
    internal sealed class RunInfo
    {
        private readonly List<RideRoom> rideRooms = [];

        // Constructor
        internal RunInfo(GameSession session, int length)
        {
            for (var i = 0; i < length; i++)
            {
                var room = new RideRoom(session, string.Empty, i, i == length - 1);
                rideRooms.Add(room);
            }
        }
    }
}
