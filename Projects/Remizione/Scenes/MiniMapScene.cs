using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// MiniMapScene
    /// </summary>
    public sealed class MiniMapScene : Scene, IInputHandler
    {
        #region Private fields

        private readonly Sprite map = new() { PivotOrigin = RectanglePoint.Center, Position = Screen.Area.GetPoint(RectanglePoint.Center), Scale = new(.75f) };
        private readonly List<Marker> markers = [];
        private readonly GameSession session;
        private readonly TextSprite titleText;

        #endregion

        #region Constructor

        // Constructor
        public MiniMapScene(GameSession session)
            : base()
        {
            this.session = session;
            this.PausePreviousScenes = false;

            // Title text
            titleText = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.MouseCursor.Tooltip,
                PivotOrigin = RectanglePoint.Bottom,
                Scale = ScaleInfo.Text.Huge
            };
        }

        #endregion

        #region Private members

        // GetCoordinates
        private static Vector2? GetCoordinates(GameThing thing)
        {
            if (thing.Room == null)
                return null;

            const float mapWidth = 128;
            const float mapHeight = 72;
            const float scale = .75f;

            // Tamaño real del mapa dibujado en pantalla (96x54)
            float scaledMapWidth = mapWidth * scale;
            float scaledMapHeight = mapHeight * scale;

            // Offset para centrar 96x54 en la pantalla de 240x135
            float offsetX = (Screen.NativeWidth - scaledMapWidth) / 2f;  // 72px
            float offsetY = (Screen.NativeHeight - scaledMapHeight) / 2f; // 40.5px

            // Normalización y proyección a la escala del mapa
            float normalizedX = thing.X / thing.Room.Width;
            float normalizedY = thing.Y / thing.Room.Height;

            float mapX = offsetX + (normalizedX * scaledMapWidth);
            float mapY = offsetY + (normalizedY * scaledMapHeight);

            return new(mapX, mapY);
        }

        // HandleMouseInput
        private bool HandleMouseInput()
        {
            if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed() ||
                InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                Game.SceneManager.Pop();
                return true;
            }

            return false;
        }

        #endregion

        #region Protected members

        // OnActivate
        protected override void OnActivate()
        {
            base.OnActivate();

            markers.Clear();

            map.RenderImage = session.Room?.MiniMapImage;
            titleText.Position = map.BoundingBox.GetPoint(RectanglePoint.Top, 0, -5);
            titleText.Text = session.Room?.Label;

            if (session.Room is not GameRoom room)
                return;

            // Markers
            for (var i = 0; i < room.Children.Count; i++)
            {
                if (room.Children[i] is GameThing thing)
                {
                    // Player
                    if (thing.IsPlayer)
                        markers.Add(new(thing, ColorPalette.MiniMap.Player, true));

                    else if (thing is Bonfire)
                        markers.Add(new(thing, ColorPalette.MiniMap.Bonfire, false));
                }
            }
        }

        // OnDeactivate
        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            markers.Clear();
            titleText.Clear();
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            Game.Shapes.DrawRectangle(Screen.Area, ColorPalette.SceneShade);
            titleText.Draw(gameTime);
            map.Draw(gameTime);

            for (var i = 0; i < markers.Count; i++)
            {
                markers[i].Draw(gameTime);
            }

            Game.SpriteBatch.End();
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput()
        {
            // Mouse input
            if (InputManager.DefaultPlayer.LastInputMethod == InputMethod.Mouse)
            {
                if (HandleMouseInput())
                    return HandleInputResult.Handled;
            }

            if (InputBindings.Map.IsPressed(0))
            {
                Game.SceneManager.Pop();
                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            for (var i = 0; i < markers.Count; i++)
            {
                markers[i].Update(gameTime);
            }
        }

        #endregion

        /// <summary>
        /// Marker
        /// </summary>
        private sealed class Marker : GameObject
        {
            private readonly Sprite image = new(Atlases.UI.MiniMapMarker)
            {
                PivotOrigin = RectanglePoint.Center,
                Scale = new(2)
            };

            // Constructor
            internal Marker(GameThing thing, Color color, bool blink)
            {
                if (GetCoordinates(thing) is Vector2 coord)
                {
                    image.Position = coord;
                    image.Color = color;

                    if (blink)
                        image.Tweens.OpacityTween = FloatTween.Create(TweenStyle.Linear, .7f, 1, 300, -1);
                }
            }

            // OnDraw
            protected override void OnDraw(GameTime gameTime)
            {
                if (image.Position != Vector2.Zero)
                    image.Draw(gameTime);
            }

            // OnUpdate
            protected override void OnUpdate(GameTime gameTime)
            {
                image.Update(gameTime);
            }
        }
    }
}