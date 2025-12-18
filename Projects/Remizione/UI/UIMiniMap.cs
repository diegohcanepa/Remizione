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
        #region Private fields

        private enum RoomImage { Current, Visited, NotVisited };
        private readonly ImageSprite container;
        private readonly Vector2 containerCenter;
        private readonly HashSet<RoomGraph> drawnRooms = [];
        private readonly ImageSprite looMarker;
        private readonly FloatTween opacityTween = new();
        private readonly ImageSprite[] roomImages;
        private readonly ImageSprite startMarker;
        private readonly RectangleF visibleArea;

        #endregion

        #region Constructor

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

            containerCenter = container.BoundingBox.Center - (Vector2.UnitY * .5f);
            visibleArea = container.BoundingBox;
            visibleArea.Inflate(-1, -1);

            // Loot marker
            looMarker = new ImageSprite(game, Atlases.UI.GetImageNotNull("UIMiniMapMarker"))
            {
                PivotOrigin = RectanglePoint.Center,
                Scale = ScaleInfo.UIElement.Tiny
            };

            // Start marker
            startMarker = new ImageSprite(game, Atlases.UI.GetImageNotNull("UIMiniMapStartMarker"))
            {
                PivotOrigin = RectanglePoint.Center,
                Scale = ScaleInfo.UIElement.Tiny
            };

            // Room images
            roomImages = new ImageSprite[3];
            for (var i = 0; i < roomImages.Length; i++)
            {
                roomImages[i] = new(game, Atlases.UI.GetImageNotNull($"UIMiniMapRoom{i}"))
                {
                    PivotOrigin = RectanglePoint.Center,
                    Scale = new(.4f)
                };
            }

            opacityTween.Start(TweenStyle.CubicInOut, 1, .6f, 300, -1);
        }

        #endregion

        #region Private members

        // DrawRoom
        private void DrawRoom(GameTime gameTime, RoomGraph roomGraph, Vector2 position)
        {
            if (roomGraph.RoomType == RoomType.Entrance)
                return;

            ImageSprite image;
         
            // Current
            if (roomGraph == CurrentRoom)
                image = roomImages[(int)RoomImage.Current];

            // Visited
            else if (roomGraph.Visited)
                image = roomImages[(int)RoomImage.Visited];

            // Not visited
            else
                image = roomImages[(int)RoomImage.NotVisited];

            image.Opacity = roomGraph == CurrentRoom ? opacityTween.CurrentValue : .8f;
            image.Position = position;

            if (RectangleF.Intersects(image.BoundingBox, visibleArea) != image.BoundingBox)
                return;

            image.Draw(gameTime);

            /*
            if (roomGraph.SackCount > 0 || roomGraph.HasCoin)
            {
                looMarker.Position = image.BoundingBox.Center;

                if (roomGraph.IsSide)
                {
                    if (roomGraph.Right != null)
                        looMarker.X -= .25f;

                    else if (roomGraph.Left != null)
                        looMarker.X += .25f;
                }

                looMarker.Draw(gameTime);
            }
            */

            // Draw start marker
            if (roomGraph.RoomType == RoomType.Start)
            {
                startMarker.Position = image.BoundingBox.GetPoint(RectanglePoint.Bottom, -.25f, 0);
                startMarker.Draw(gameTime);
            }

            drawnRooms.Add(roomGraph);

            if (roomGraph.Down != null && (roomGraph == CurrentRoom || roomGraph.Visited) && !drawnRooms.Contains(roomGraph.Down))
                DrawRoom(gameTime, roomGraph.Down, position + new Vector2(0, image.BoundingBox.Height));

            if (roomGraph.Left != null && (roomGraph == CurrentRoom || roomGraph.Visited) && !drawnRooms.Contains(roomGraph.Left))
                DrawRoom(gameTime, roomGraph.Left, position - new Vector2(image.BoundingBox.Width, 0));

            if (roomGraph.Right != null && (roomGraph == CurrentRoom || roomGraph.Visited) && !drawnRooms.Contains(roomGraph.Right))
                DrawRoom(gameTime, roomGraph.Right, position + new Vector2(image.BoundingBox.Width, 0));

            if (roomGraph.Up != null && (roomGraph == CurrentRoom || roomGraph.Visited) && !drawnRooms.Contains(roomGraph.Up))
                DrawRoom(gameTime, roomGraph.Up, position - new Vector2(0, image.BoundingBox.Height));
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (CurrentRoom == null)
                return;

            drawnRooms.Clear();
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);
            container.Draw(gameTime);
            var pos = containerCenter;
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
