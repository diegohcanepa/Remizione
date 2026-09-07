using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// UIMiniMap
    /// </summary>
    public sealed class UIMiniMap : GameObject, IDisposable
    {
        #region Private fields

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
            container = new Sprite(Atlases.UI.GetImage("MiniMapContainer"))
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = new(236, 3)
            };

            // ContainerBorder
            containerBorder = new Sprite(Atlases.UI.GetImage("MiniMapContainerBorder"))
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = container.Position
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
            roomImages = new Sprite[Atlases.UI.MiniMapNodes.Count];
            for (var i = 0; i < roomImages.Length; i++)
            {
                roomImages[i] = new(Atlases.UI.MiniMapNodes[i])
                {
                    PivotOrigin = RectanglePoint.Center,
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
            {
                image = roomImages[(int)MapNodeState.Current];
            }

            // End
            else if (roomNode.Category == RoomCategory.End)
            {
                image = roomImages[(int)MapNodeState.End];
            }

            // Start
            else if (roomNode.Category == RoomCategory.Start)
            {
                image = roomImages[(int)MapNodeState.Start];
            }

            // Visited
            else if (roomNode.Visited)
            {
                image = roomImages[(int)MapNodeState.Visited];
            }

            // Not visited
            else
            {
                image = roomImages[(int)MapNodeState.NotVisited];
            }

            image.Opacity = roomNode == CurrentRoom ? opacityTween.CurrentValue : 1;
            image.Position = position;

            if (!image.BoundingBox.Intersects(container.BoundingBox))
                return;

            image.Draw(gameTime);

            drawnRooms.Add(roomNode);

            if (roomNode.Down != null && (roomNode == CurrentRoom || roomNode.Visited) && !drawnRooms.Contains(roomNode.Down))
                DrawRoom(gameTime, roomNode.Down, position + new Vector2(0, image.BoundingBox.Height - 1));

            if (roomNode.Left != null && (roomNode == CurrentRoom || roomNode.Visited) && !drawnRooms.Contains(roomNode.Left))
                DrawRoom(gameTime, roomNode.Left, position - new Vector2(image.BoundingBox.Width - 1, 0));

            if (roomNode.Right != null && (roomNode == CurrentRoom || roomNode.Visited) && !drawnRooms.Contains(roomNode.Right))
                DrawRoom(gameTime, roomNode.Right, position + new Vector2(image.BoundingBox.Width - 1, 0));

            if (roomNode.Up != null && (roomNode == CurrentRoom || roomNode.Visited) && !drawnRooms.Contains(roomNode.Up))
                DrawRoom(gameTime, roomNode.Up, position - new Vector2(0, image.BoundingBox.Height - 1));
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
