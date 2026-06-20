using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// UIMiniMap
    /// </summary>
    public sealed class UIMiniMap : GameObject, IDisposable
    {
        #region Private fields

        private enum RoomImage { Current, Visited, NotVisited };
        private readonly Sprite container;
        private readonly Sprite containerBorder;
        private readonly Vector2 containerCenter;
        private readonly HashSet<RoomNode> drawnRooms = [];
        private readonly FloatTween opacityTween = new();
        private readonly RasterizerState rasterizerState;
        private readonly Rectangle screenScissorRect;
        private readonly Sprite[] roomImages;

        #endregion

        #region Constructor

        // Constructor
        public UIMiniMap()
        {
            rasterizerState = new RasterizerState { ScissorTestEnable = true };

            // Container
            container = new Sprite(Atlases.UI.GetImage("UIMiniMapContainer"))
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = new(234, 2)
            };

            // ContainerBorder
            containerBorder = new Sprite(Atlases.UI.GetImage("UIMiniMapContainerBorder"))
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = new(234, 2)
            };

            containerCenter = container.BoundingBox.Center;

            float scaleX = (float)Game.GraphicsDevice.Viewport.Width / Screen.NativeWidth;
            float scaleY = (float)Game.GraphicsDevice.Viewport.Height / Screen.NativeHeight;

            // 1. Rectángulo en tu escala pequeña (240x135)
            Rectangle virtualMapRect = container.BoundingBox.ToRectangle();
            //virtualMapRect.Inflate(-1, -1);

            // 2. Convert to current screen resolution
            screenScissorRect = new(
                (int)(virtualMapRect.X * scaleX),
                (int)(virtualMapRect.Y * scaleY),
                (int)(virtualMapRect.Width * scaleX),
                (int)(virtualMapRect.Height * scaleY)
            );

            // Room images
            roomImages = new Sprite[3];
            for (var i = 0; i < roomImages.Length; i++)
            {
                roomImages[i] = new(Atlases.UI.GetImage($"UIMiniMapRoom{i}"))
                {
                    PivotOrigin = RectanglePoint.Center,
                    Scale = new Vector2(.8f)
                };
            }

            opacityTween.Start(TweenStyle.CubicInOut, .8f, .6f, 300, -1);
        }

        #endregion

        #region Private members

        // DrawRoom
        private void DrawRoom(GameTime gameTime, RoomNode roomNode, Vector2 position)
        {
            Sprite image;

            // Current
            if (roomNode == CurrentRoom)
                image = roomImages[(int)RoomImage.Current];

            // Visited
            else if (roomNode.Visited)
                image = roomImages[(int)RoomImage.Visited];

            // Not visited
            else
                image = roomImages[(int)RoomImage.NotVisited];

            image.Opacity = roomNode == CurrentRoom ? opacityTween.CurrentValue : 1;
            image.Position = position;

            image.Draw(gameTime);

            if (roomNode != CurrentRoom)
            {
                /*
                if (roomGraph.HeartCount > 0)
                {
                    heartMarker.Position = image.BoundingBox.GetPoint(RectanglePoint.Center, -.25f, -.25f);
                    heartMarker.Draw(gameTime);
                }
                */
            }

            drawnRooms.Add(roomNode);

            if (roomNode.Down != null && (roomNode == CurrentRoom || roomNode.Visited) && !drawnRooms.Contains(roomNode.Down))
                DrawRoom(gameTime, roomNode.Down, position + new Vector2(0, image.BoundingBox.Height));

            if (roomNode.Left != null && (roomNode == CurrentRoom || roomNode.Visited) && !drawnRooms.Contains(roomNode.Left))
                DrawRoom(gameTime, roomNode.Left, position - new Vector2(image.BoundingBox.Width, 0));

            if (roomNode.Right != null && (roomNode == CurrentRoom || roomNode.Visited) && !drawnRooms.Contains(roomNode.Right))
                DrawRoom(gameTime, roomNode.Right, position + new Vector2(image.BoundingBox.Width, 0));

            if (roomNode.Up != null && (roomNode == CurrentRoom || roomNode.Visited) && !drawnRooms.Contains(roomNode.Up))
                DrawRoom(gameTime, roomNode.Up, position - new Vector2(0, image.BoundingBox.Height));
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (CurrentRoom == null)
                return;

            var oldRect = Game.GraphicsDevice.ScissorRectangle;
            Game.GraphicsDevice.ScissorRectangle = screenScissorRect;

            drawnRooms.Clear();
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp, null, null, rasterizerState);
            container.Draw(gameTime);
            var pos = containerCenter;
            DrawRoom(gameTime, CurrentRoom, pos);
            containerBorder.Draw(gameTime);
            Game.SpriteBatch.End();
            Game.SpriteBatch.GraphicsDevice.ScissorRectangle = oldRect;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            opacityTween.Update(gameTime);
        }

        #endregion

        // CurrentRoom
        public RoomNode? CurrentRoom { get; set; }

        // Dispose
        public void Dispose()
        {
            rasterizerState.Dispose();
        }
    }
}
