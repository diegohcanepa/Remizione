using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// RideDoor
    /// </summary>
    public class RideDoor : Prop
    {
        private readonly ImageSprite lockImage;
        private readonly Vector2Tween scaleTween = new();
        private readonly FloatTween xTween = new();

        // Constructor
        public RideDoor(GameSession session, string name)
            : base(session, name)
        {
            if (name.StartsWith("RideDoorUp", StringComparison.OrdinalIgnoreCase))
                DoorDirection = Adberration.Direction.Up;

            else if (name.StartsWith("RideDoorDown", StringComparison.OrdinalIgnoreCase))
                DoorDirection = Adberration.Direction.Down;

            else if (name.StartsWith("RideDoorLeft", StringComparison.OrdinalIgnoreCase))
                DoorDirection = Adberration.Direction.Left;

            else if (name.StartsWith("RideDoorRight", StringComparison.OrdinalIgnoreCase))
                DoorDirection = Adberration.Direction.Right;

            else
                throw new InvalidOperationException("Cannot infere door direction from entity name.");

            Atlas = Atlases.Environment;
            CollisionDetection = false;
            DisplayNameKey = "Verb.Enter";
            CloseSound = Sound.Find("DoorClose");
            OpenSound = Sound.Find("DoorOpen");

            this.lockImage = new(Game, Atlas.GetImage($"{StaticName}Lock"));
        }

        #region Private members

        // BackToHub
        private void BackToHub(RideDoor hubDoor)
        {
            if (Session.GetEntity<Hub>("Hub") is not Hub hubRoom)
                return;

            ConnectCore(hubRoom, hubDoor.BoundingBox.GetPoint(RectanglePoint.Bottom));
        }

        // ConnectCore
        private void ConnectCore(GameRoom targetRoom, Vector2 targetPosition)
        {
            if (Session.Player != null)
            {
                Session.Player.Unparent();
                targetRoom.Children.Add(Session.Player);
                Session.Player.Position = targetPosition;
                Session.Camera.FollowTarget(Session.Player, true);
            }

            Session.EnterRoom(targetRoom);
        }

        // SyncAnimation
        private void SyncAnimation()
        {
            if (IsOpen)
                Sprite.Player.Play("Open");
            else
                Sprite.Player.Play("Closed");
        }

        #endregion

        #region Protected members

        // CanInteractCore
        protected override bool CanInteractCore(Actor requester)
        {
            if (SwitchStateCooldown > 0)
                return false;
            else
                return base.CanInteractCore(requester);
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
            if (PropState == PropState.Locked)
                lockImage.Draw(gameTime);
        }

        // OnPropStateChanged
        protected override void OnPropStateChanged(PropState previousState)
        {
            base.OnPropStateChanged(previousState);
            if (previousState == PropState.Locked && PropState == PropState.Unlocked)
                PlaySound(SoundNames.LockOpen);
        }

        // OnTransform
        protected override void OnTransform(TransformChange change)
        {
            base.OnTransform(change);
            lockImage?.MatchTransform(this.Sprite);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (SwitchStateCooldown > 0)
            {
                SwitchStateCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                if (SwitchStateCooldown <= 0)
                {
                    SwitchStateCooldown = 0;
                    if (IsOpen)
                        Close();
                    else
                        Open();
                }
            }
        }

        #endregion

        // Close
        [ScriptMethod]
        public void Close()
        {
            if (CloseSound != null)
                PlaySound(CloseSound);

            SyncAnimation();

            scaleTween.Start(TweenStyle.QuadraticInOut, Scale, new Vector2(1f, .96f), 100, 2);
            xTween.Start(TweenStyle.QuadraticInOut, X, X - 1, 50, 4);

            Tweens.ScaleTween = scaleTween;
            Tweens.XTween = xTween;

            IsOpen = false;
        }

        // CloseSound
        [ScriptProperty]
        public Sound? CloseSound { get; set; }

        // Connect
        [ScriptMethod(CodingContext.Execution)]
        public void Connect()
        {
            if (TargetRoom != null)
            {
                var pos = Vector2.Zero;
                int roomId = 0;
                if (Room is RideRoom rideRoom)
                    roomId = rideRoom.RoomGraph.Id;

                pos = TargetRoom.GetPlayerPosition(roomId, out RideDoor? door);
                if (door != null)
                    door.IsOpen = true;

                ConnectCore(TargetRoom, pos);
            }
            else if (Room is RideRoom rideRoom && rideRoom.HubDoor != null)
            {
                BackToHub(rideRoom.HubDoor);
            }
        }

        // Direction
        public Direction DoorDirection { get; }

        // IsOpen
        [ScriptProperty]
        public bool IsOpen
        {
            get;
            set
            {
                if (value != field)
                {
                    if (value && PropState == PropState.Locked)
                        return;

                    field = value;
                    SyncAnimation();
                }
            }
        }

        // Open
        [ScriptMethod]
        public void Open()
        {
            if (PropState == PropState.Locked)
                return;

            if (OpenSound != null)
                PlaySound(OpenSound);

            SyncAnimation();

            scaleTween.Start(TweenStyle.QuadraticInOut, Scale, new Vector2(1f, .96f), 100, 2);
            xTween.Start(TweenStyle.QuadraticInOut, X, X - 1, 50, 4);

            Tweens.ScaleTween = scaleTween;
            Tweens.XTween = xTween;

            IsOpen = true;
        }

        // OpenSound
        [ScriptProperty]
        public Sound? OpenSound { get; set; }

        // Prepare
        [ScriptMethod]
        public void Prepare()
        {
            Sprite.ClearAnimations();

            string prefix;

            // Hub, uses common room style
            if (Room is not RideRoom rideRoom || TargetRoom?.Config is not RoomConfig config)
                prefix = $"{nameof(CommonRoom)}";

            // Side rooms uses its own style
            else if (rideRoom.RoomGraph.IsSide)
                prefix = $"{rideRoom.Config.Name}";

            // Root rooms uses target room style
            else
                prefix = $"{config.Name}";

            prefix = $"{prefix}Door{DoorDirection}";

            var animation = AddAnimation("Closed");
            animation.AddFrame(prefix + animation.Name, 1000);

            animation = AddAnimation("Open");
            animation.AddFrame(prefix + animation.Name, 1000);

            SyncAnimation();
        }

        // SwitchStateCooldown
        public int SwitchStateCooldown { get; set; }

        // TargetRoom
        public RideRoom? TargetRoom { get; set; }

        // TargetRoomPosition
        public Vector2 TargetRoomPosition { get; set; }
    }
}
