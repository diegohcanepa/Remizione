using Engendro;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// RideRoom
    /// </summary>
    public sealed class RideRoom : ProceduralRoom
    {
        private static readonly Dictionary<RideRoomKind, string> walkAreasByType = [];

        // Static constructor
        static RideRoom()
        {
            walkAreasByType[RideRoomKind.Default] = "207,46;237,111;5,111;36,46;83,46;88,44;164,44;170,46";
        }

        // Constructor
        public RideRoom(GameSession session, RoomDescriptor descriptor)
            : base(session, string.Empty, descriptor)
        {
            AllowGlobalLight = true;
            AtlasName = $"RideRoom{descriptor.RoomKind}";
            DefaultImageName = AtlasName;
        }

        #region Protected members

        // OnSetupWalkArea
        protected override void OnSetupWalkArea()
        {
            if (walkAreasByType.TryGetValue(Descriptor.RoomKind, out string? vertices))
                AddWalkArea("Default", ReadOnlyPolygon.GetVertices(vertices));
        }

        #endregion
    }
}
