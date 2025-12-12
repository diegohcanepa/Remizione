using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Remizione.UI
{
    /// <summary>
    /// UIMiniMap
    /// </summary>
    public sealed class UIMiniMap : GameObject
    {
        private readonly ImageSprite container;
        private readonly ImageSprite currentRoom;
        private readonly ImageSprite currentSideRoom;
        private int downLimit;
        private readonly HashSet<RoomGraph> drawnRooms = [];
        private readonly ImageSprite notVisitedRoom;
        private readonly ImageSprite notVisitedSideRoom;
        private int upLimit;
        private readonly ImageSprite visitedRoom;
        private readonly ImageSprite visitedSideRoom;

        // Constructor
        public UIMiniMap(EngendroGame game)
            : base(game)
        {
            // CreateRoomImage
            ImageSprite CreateRoomImage(AtlasImage image)
            {
                return new(game, image)
                {
                    Opacity = .9f,
                    PivotOrigin = RectanglePoint.Center,
                    Scale = ScaleInfo.UIElement.Tiny
                };
            }

            // Container
            container = new ImageSprite(game, Atlases.UI.UIMiniMapContainer)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = new(234, 4),
                Scale = ScaleInfo.UIElement.Small
            };

            // Current room
            currentRoom = CreateRoomImage(Atlases.UI.UIMiniMapCurrentRoom);

            // Not visited room
            notVisitedRoom = CreateRoomImage(Atlases.UI.UIMiniMapNotVisitedRoom);

            // Visited room
            visitedRoom = CreateRoomImage(Atlases.UI.UIMiniMapVisitedRoom);

            // Current side room
            currentSideRoom = CreateRoomImage(Atlases.UI.UIMiniMapCurrentSideRoom);

            // Not visited side room
            notVisitedSideRoom = CreateRoomImage(Atlases.UI.UIMiniMapNotVisitedSideRoom);

            // Visited side room
            visitedSideRoom = CreateRoomImage(Atlases.UI.UIMiniMapVisitedSideRoom);
       }

        #region Private members

        // DrawRoom
        private void DrawRoom(GameTime gameTime, RoomGraph room, Vector2 position)
        {
            ImageSprite image;

            if (room == CurrentRoom)
                image = room.IsSide ? currentSideRoom : currentRoom;
            else if (room.Visited)
                image = room.IsSide ? visitedSideRoom : visitedRoom;
            else
                image = room.IsSide ? notVisitedSideRoom : notVisitedRoom;

            image.Position = position;
            image.Draw(gameTime);

            drawnRooms.Add(room);

            if (room.Down != null && (room == CurrentRoom || room.Visited) && !drawnRooms.Contains(room.Down))
            {
                if (downLimit < 2)
                {
                    downLimit++;
                    DrawRoom(gameTime, room.Down, position + new Vector2(0, image.BoundingBox.Height));
                }
            }

            if (room.Left != null && (room == CurrentRoom || room.Visited) && !drawnRooms.Contains(room.Left))
                DrawRoom(gameTime, room.Left, position - new Vector2(image.BoundingBox.Width, 0));

            if (room.Right != null && (room == CurrentRoom || room.Visited) && !drawnRooms.Contains(room.Right))
                DrawRoom(gameTime, room.Right, position + new Vector2(image.BoundingBox.Width, 0));

            if (room.Up != null && (room == CurrentRoom || room.Visited) && !drawnRooms.Contains(room.Up))
            {
                if (upLimit < 2)
                {
                    upLimit++;
                    DrawRoom(gameTime, room.Up, position - new Vector2(0, image.BoundingBox.Height));
                }
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (CurrentRoom == null)
                return;

            downLimit = 0;
            upLimit = 0;
            drawnRooms.Clear();
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);
            container.Draw(gameTime);
            var pos = container.BoundingBox.Center;

            if (CurrentRoom.IsSide)
            {
                if (CurrentRoom.Right != null)
                    pos.X -= currentRoom.BoundingBox.Width;
                else
                    pos.X += currentRoom.BoundingBox.Width;
            }

            DrawRoom(gameTime, CurrentRoom, pos);
            Game.SpriteBatch.End();
        }

        #endregion

        // CurrentRoom
        public RoomGraph? CurrentRoom { get; set; }
    }
}
