using Engendro.Audio;

namespace ScaryCastle
{
    /// <summary>
    /// TrapDoor
    /// </summary>
    public sealed class TrapDoor : Openable, ISpawnNotification
    {
        #region Constructor

        // Constructor
        public TrapDoor(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Props;
            CollisionDetection = false;
            DisplayNameKey = "Prop.TrapDoor";
            Verb = Verb.Use;
        }

        #endregion

        #region ISpawnNotification interface

        // OnSpawned
        void ISpawnNotification.OnSpawned(ProceduralRoom room)
        {
            CloseSound = Sound.Find(SoundNames.DoorGenericClose);
            OpenSound = Sound.Find(SoundNames.DoorGenericOpen);

            var prefix = $"{nameof(TrapDoor)}_{room.RoomNode.Definition.Theme}_";

            var animation = AddAnimation(AnimationNames.Closed);
            animation.AddFrame(prefix + animation.Name, 1000);

            animation = AddAnimation(AnimationNames.Open);
            animation.AddFrame(prefix + animation.Name, 1000);

            SyncAnimation();
        }

        #endregion

        #region Protected members

        // OnClosureStatusChanged
        protected override void OnClosureStatusChanged(bool actionInProgress)
        {
            if (actionInProgress)
                Bounce();

            if (IsOpen)
                Verb = Verb.GoDown;
        }

        #endregion
    }
}
