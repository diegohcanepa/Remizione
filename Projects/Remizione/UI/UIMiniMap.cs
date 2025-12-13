using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Remizione.UI
{
    /// <summary>
    /// UIMiniMap
    /// </summary>
    public sealed class UIMiniMap : GameObject
    {
        private enum RoomImage { MiddleCurrent, MiddleVisited, MiddleNotVisited, LeftCurrent, LeftVisited, LeftNotVisited, RightCurrent, RightVisited, RightNotVisited };
        private readonly ImageSprite container;
        private int downLimit;
        private readonly HashSet<RoomGraph> drawnRooms = [];
        private readonly ImageSprite marker;
        private readonly FloatTween opacityTween = new();
        private readonly ImageSprite[] roomImages;
        private int upLimit;

        // Constructor
        public UIMiniMap(EngendroGame game)
            : base(game)
        {
            // Container
            container = new ImageSprite(game, Atlases.UI.GetImageNotNull("UIMiniMapContainer"))
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = new(234, 2)
            };

            // Marker
            marker = new ImageSprite(game, Atlases.UI.GetImageNotNull("UIMiniMapMarker"))
            {
                PivotOrigin = RectanglePoint.Center,
                Scale = ScaleInfo.UIElement.Tiny
            };

            // Room images
            roomImages = new ImageSprite[9];
            for (var i = 0; i < roomImages.Length; i++)
            {
                roomImages[i] = new(game, Atlases.UI.GetImageNotNull($"UIMiniMapRoom{i}"))
                {
                    PivotOrigin = RectanglePoint.Center,
                    Scale = ScaleInfo.UIElement.Tiny
                };
            }

            opacityTween.Start(TweenStyle.CubicInOut, 1, .6f, 300, -1);
       }

        #region Private members

        // DrawRoom
        private void DrawRoom(GameTime gameTime, RoomGraph roomGraph, Vector2 position)
        {
            ImageSprite image;

            // Current
            if (roomGraph == CurrentRoom)
            {
                if (roomGraph.IsSide)
                    image = roomGraph.Right != null ? roomImages[(int)RoomImage.LeftCurrent] : roomImages[(int)RoomImage.RightCurrent];
                else
                    image = roomImages[(int)RoomImage.MiddleCurrent];
            }

            // Visited
            else if (roomGraph.Visited)
            {
                if (roomGraph.IsSide)
                    image = roomGraph.Right != null ? roomImages[(int)RoomImage.LeftVisited] : roomImages[(int)RoomImage.RightVisited];
                else
                    image = roomImages[(int)RoomImage.MiddleVisited];
            }

            // Not visited
            else
            {
                if (roomGraph.IsSide)
                    image = roomGraph.Right != null ? roomImages[(int)RoomImage.LeftNotVisited] : roomImages[(int)RoomImage.RightNotVisited];
                else
                    image = roomImages[(int)RoomImage.MiddleNotVisited];
            }

            image.Opacity = roomGraph == CurrentRoom ? opacityTween.CurrentValue : .6f;
            image.Position = position;
            image.Draw(gameTime);

            if (roomGraph.SackCount > 0 || roomGraph.HasCoin)
            {
                marker.Position = image.BoundingBox.Center;
                marker.Draw(gameTime);
            }

            drawnRooms.Add(roomGraph);

            if (roomGraph.Down != null && (roomGraph == CurrentRoom || roomGraph.Visited) && !drawnRooms.Contains(roomGraph.Down))
            {
                if (downLimit < 2)
                {
                    downLimit++;
                    DrawRoom(gameTime, roomGraph.Down, position + new Vector2(0, image.BoundingBox.Height));
                }
            }

            if (roomGraph.Left != null && (roomGraph == CurrentRoom || roomGraph.Visited) && !drawnRooms.Contains(roomGraph.Left))
                DrawRoom(gameTime, roomGraph.Left, position - new Vector2(image.BoundingBox.Width, 0));

            if (roomGraph.Right != null && (roomGraph == CurrentRoom || roomGraph.Visited) && !drawnRooms.Contains(roomGraph.Right))
                DrawRoom(gameTime, roomGraph.Right, position + new Vector2(image.BoundingBox.Width, 0));

            if (roomGraph.Up != null && (roomGraph == CurrentRoom || roomGraph.Visited) && !drawnRooms.Contains(roomGraph.Up))
            {
                if (upLimit < 2)
                {
                    upLimit++;
                    DrawRoom(gameTime, roomGraph.Up, position - new Vector2(0, image.BoundingBox.Height));
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
                    pos.X -= roomImages[0].BoundingBox.Width;
                else
                    pos.X += roomImages[0].BoundingBox.Width;
            }

            DrawRoom(gameTime, CurrentRoom, pos);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            opacityTween.Update(gameTime);
        }

        #endregion

        // CurrentRoom
        public RoomGraph? CurrentRoom { get; set; }
    }
}
