using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// RoomEditor
    /// </summary>
    public sealed class RoomEditor : GameObject
    {
        #region Private fields

        private enum EditMode { Things }
        private EditMode editMode;
        private readonly TextSprite editorInfoText;
        private bool leftPanelVisible = true;
        private readonly List<Thing> selectedThings = [];
        private readonly GameSession session;
        private readonly TextSprite text;

        #endregion

        #region Constructor

        // Constructor
        internal RoomEditor(GameSession session)
        {
            this.session = session;
            editorInfoText = new TextSprite(Fonts.Common) { Color = Color.White, PivotOrigin = RectanglePoint.RightTop, Scale = ScaleInfo.Text.Tiny };
            text = new TextSprite(Fonts.Common) { Color = Color.White, PivotOrigin = RectanglePoint.LeftTop, Scale = ScaleInfo.Text.Small };
        }

        #endregion

        #region Private members

        // DrawGeneralInfo
        private void DrawGeneralInfo(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);

            editorInfoText.Position = Screen.Area.GetPoint(RectanglePoint.RightTop, -4, 0);
            DrawText(gameTime, editorInfoText, Utils.GetVersion(), Colors.Palette.Text);
            DrawText(gameTime, editorInfoText, $"Entities: {session.Entities.Count}", Colors.Palette.Text);
            DrawText(gameTime, editorInfoText, $"Play time: {session.PlayTime:hh\\:mm\\:ss}", Colors.Palette.Text);
            DrawText(gameTime, editorInfoText, $"FPS: {EngendroGame.FPS}", Colors.Palette.Text);

            NewLine(editorInfoText);
            DrawText(gameTime, editorInfoText, $"Editor Mode (F3): {editMode}", Colors.Palette.Text);
            DrawText(gameTime, editorInfoText, "Save Game (F10)", Colors.Palette.Text);

            Game.SpriteBatch.End();
        }

        // DrawCameraData
        private void DrawCameraData(GameTime gameTime)
        {
            // Camera Position
            NewLine(text);
            DrawText(gameTime, text, "CAMERA", Colors.Palette.Text);
            DrawText(gameTime, text, $"Offset: {session.Camera.Offset}");
            DrawText(gameTime, text, $"Position: {session.Camera.Position}");

            var target = session.Camera.Target?.ToString();
            DrawText(gameTime, text, $"Target: {target}");
            DrawText(gameTime, text, $"Zoom / Rotation: {session.Camera.Zoom} / {session.Camera.Rotation}");
            DrawText(gameTime, text, $"BoundingBox: {session.Camera.VisibleBox}");
        }

        // DrawMouseData
        private void DrawMouseData(GameTime gameTime)
        {
            // Mouse Position
            NewLine(text);
            DrawText(gameTime, text, "MOUSE", Colors.Palette.Text);
            DrawText(gameTime, text, $"Position: {InputManager.DefaultPlayer.Mouse.Position}");
            DrawText(gameTime, text, $"Virtual position: {InputManager.DefaultPlayer.Mouse.VirtualPosition}");
            DrawText(gameTime, text, $"World position: {InputManager.DefaultPlayer.Mouse.WorldPosition(session.Camera)}");
        }

        // DrawRoomData
        private void DrawRoomData(GameTime gameTime, GameRoom room)
        {
            NewLine(text);
            DrawText(gameTime, text, $"ROOM ({room})", Colors.Palette.Text);
            DrawText(gameTime, text, $"Culled things: {room.CulledThings.Count} ({room.Children.Count})");
            DrawText(gameTime, text, $"Custom size: {room.CustomWidth}x{room.CustomHeight}");
        }

        // DrawSessionData
        private void DrawSessionData(GameTime gameTime)
        {
            NewLine(text);
            DrawText(gameTime, text, "SESSION", Colors.Palette.Text);

            string awaitingScript;
            if (session.AwaitingScript != null)
            {
                awaitingScript = session.AwaitingScript.Name;
                if (session.AwaitingScript?.CurrentStatement is Statement statement)
                    awaitingScript += $"({statement.Name})";
            }
            else
            {
                awaitingScript = "(None)";
            }

            DrawText(gameTime, text, $"InteractionData.Target: {session.InteractionData.Target}");
            DrawText(gameTime, text, $"Awaiting script: {awaitingScript}");
            DrawText(gameTime, text, $"Registered entities: {session.Entities.Count}");
        }

        // DrawSoundData
        private void DrawSoundData(GameTime gameTime)
        {
            NewLine(text);
            DrawText(gameTime, text, "AUDIO MANAGER", Colors.Palette.Text);
            DrawText(gameTime, text, $"Music: {AudioManager.Music.CurrentSoundName}");
            DrawText(gameTime, text, $"Music tag: {AudioManager.Music.CurrentTag}");
            DrawText(gameTime, text, $"Sound instances: {SoundInstance.RunningInstances.Count}");
        }

        // DrawText
        private static void DrawText(GameTime gameTime, TextSprite text, string value)
        {
            DrawText(gameTime, text, value, Colors.Palette.Text);
        }

        // DrawText
        private static void DrawText(GameTime gameTime, TextSprite text, string value, Color color)
        {
            DrawText(gameTime, text, value, color, ScaleInfo.Text.Tiny);
        }

        // DrawText
        private static void DrawText(GameTime gameTime, TextSprite text, string value, Color color, Vector2 scale)
        {
            NewLine(text);
            text.Color = color;
            text.Scale = scale;
            text.Text = value;
            text.Draw(gameTime);
        }

        // DrawThingsMode
        private void DrawThingsMode(GameTime gameTime, GameRoom room)
        {
            if (SelectedThing == null)
                return;

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);

            // Name
            var value = SelectedThing.Name + " (" + SelectedThing.GetType().Name + ")" + (SelectedThing.Persistent ? " [persistent]" : string.Empty);
            DrawText(gameTime, text, value, Colors.Palette.Text, ScaleInfo.Text.Tiny);

            // Animation Name
            var animationName = SelectedThing.AnimationPlayer.Animation?.Name ?? string.Empty;
            var imageName = SelectedThing.AnimationPlayer.Frame?.ImageName ?? string.Empty;
            DrawText(gameTime, text, $"Current Animation: {animationName} ({imageName})");

            // RenderLayer
            DrawText(gameTime, text, $"Render layer: {SelectedThing.RenderLayer}");

            // Position
            DrawText(gameTime, text, $"Position: {SelectedThing.X},{SelectedThing.Y}");

            // PivotOrigin
            DrawText(gameTime, text, $"Pivot Origin: {SelectedThing.PivotOrigin}");

            // Altitude
            DrawText(gameTime, text, $"Altitude: {SelectedThing.Altitude}");

            // BoundingBox
            DrawText(gameTime, text, $"Bounding box: {SelectedThing.BoundingBox}");

            // Depth Offset
            DrawText(gameTime, text, $"Depth Offset: {SelectedThing.DepthOffset}");

            // WalkArea
            DrawText(gameTime, text, $"Walk Area Name: {SelectedThing.WalkAreaName}");

            // Opacity
            DrawText(gameTime, text, $"Opacity: {SelectedThing.Opacity}");

            // ShadowOffset
            if (SelectedThing is Prop prop)
                DrawText(gameTime, text, $"Shadow Offset: {prop.ShadowOffset}");

            DrawRoomData(gameTime, room);
            DrawCameraData(gameTime);
            DrawMouseData(gameTime);
            DrawSoundData(gameTime);
            DrawSessionData(gameTime);

            Game.SpriteBatch.End();
        }

        // HandleThingInput
        private bool HandleThingInput()
        {
            var handled = false;

            // Mouse
            if (session.Room != null && InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
            {
                var clickPos = InputManager.DefaultPlayer.Mouse.WorldPosition(session.Camera);

                var found = false;
                for (var i = session.Room.CulledThings.Count - 1; i >= 0; i--)
                {
                    if (selectedThings.Contains(session.Room.CulledThings[i]))
                        continue;

                    if (session.Room.CulledThings[i].BoundingBox.Contains(clickPos))
                    {
                        SelectedThing = session.Room.CulledThings[i] as GameThing;

                        if (SelectedThing != null)
                        {
                            session.Camera.Follow(SelectedThing);
                        }

                        selectedThings.Add(session.Room.CulledThings[i]);
                        found = true;
                        break;
                    }
                }

                if (!found && selectedThings.Count > 0)
                {
                    SelectedThing = null;
                    selectedThings.Clear();
                }

                return true;
            }

            // Zoom
            if (InputManager.DefaultPlayer.Keyboard.IsKeyPressed(Keys.OemMinus))
            {
                session.Camera.Zoom -= .1f;
                handled = true;
            }

            if (InputManager.DefaultPlayer.Keyboard.IsKeyPressed(Keys.OemPlus))
            {
                session.Camera.Zoom += .1f;
                handled = true;
            }

            if (InputManager.DefaultPlayer.Keyboard.IsKeyPressed(Keys.K))
            {
                session.KillEnemies();
                handled = true;
            }

            else if (SelectedThing != null)
            {
                var control = InputManager.DefaultPlayer.Keyboard.IsKeyDown(Keys.LeftControl);
                var alt = InputManager.DefaultPlayer.Keyboard.IsKeyDown(Keys.LeftAlt);

                // Up
                if (InputManager.DefaultPlayer.Keyboard.IsKeyPressed(Keys.Up) || (control && InputManager.DefaultPlayer.Keyboard.IsKeyDown(Keys.Up)))
                {
                    if (alt && SelectedThing is Prop prop)
                        prop.ShadowOffset = new(prop.ShadowOffset.X, prop.ShadowOffset.Y - 1);
                    else
                        SelectedThing.Y--;
                    handled = true;
                }

                // Down
                else if (InputManager.DefaultPlayer.Keyboard.IsKeyPressed(Keys.Down) || (control && InputManager.DefaultPlayer.Keyboard.IsKeyDown(Keys.Down)))
                {
                    if (alt && SelectedThing is Prop prop)
                        prop.ShadowOffset = new(prop.ShadowOffset.X, prop.ShadowOffset.Y + 1);
                    else
                        SelectedThing.Y++;
                    handled = true;
                }

                // Left
                else if (InputManager.DefaultPlayer.Keyboard.IsKeyPressed(Keys.Left) || (control && InputManager.DefaultPlayer.Keyboard.IsKeyDown(Keys.Left)))
                {
                    if (alt && SelectedThing is Prop prop)
                        prop.ShadowOffset = new(prop.ShadowOffset.X - 1, prop.ShadowOffset.Y);
                    else
                        SelectedThing.X--;

                    handled = true;
                }

                // Right
                else if (InputManager.DefaultPlayer.Keyboard.IsKeyPressed(Keys.Right) || (control && InputManager.DefaultPlayer.Keyboard.IsKeyDown(Keys.Right)))
                {
                    if (alt && SelectedThing is Prop prop)
                        prop.ShadowOffset = new(prop.ShadowOffset.X + 1, prop.ShadowOffset.Y);
                    else
                        SelectedThing.X++;

                    handled = true;
                }

                // Increment Depth offset
                else if (InputManager.DefaultPlayer.Keyboard.IsKeyPressed(Keys.PageUp) || (control && InputManager.DefaultPlayer.Keyboard.IsKeyDown(Keys.PageUp)))
                {
                    SelectedThing.DepthOffset++;
                    handled = true;
                }

                // Decrement Depth offset
                else if (InputManager.DefaultPlayer.Keyboard.IsKeyPressed(Keys.PageDown) || (control && InputManager.DefaultPlayer.Keyboard.IsKeyDown(Keys.PageDown)))
                {
                    SelectedThing.DepthOffset--;
                    handled = true;
                }

                // Next thing
                else if (session.Room?.Children.Count > 0 && InputManager.DefaultPlayer.Keyboard.IsKeyPressed(Keys.Enter))
                {
                    var index = session.Room.Children.IndexOf(SelectedThing);
                    if (index >= 0)
                        index++;

                    if (index == session.Room.Children.Count)
                        index = 0;

                    SelectedThing = session.Room.Children[index] as GameThing;

                    handled = true;
                }
            }

            return handled;
        }

        // Invalidate
        private static void Invalidate()
        {
            if (SelectedThing?.Parent == null)
                SelectedThing = null;
        }

        // NewLine
        private static void NewLine(TextSprite text)
        {
            text.Y += text.BoundingBox.Height - 1;
        }

        // SelectFirstThing
        private void SelectFirstThing()
        {
            if (session.Room is Room room && room.Children.Count > 0)
                SelectedThing = room.Children[0] as GameThing;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsActive || !leftPanelVisible)
                return;

            if (session.Room is not GameRoom room)
                return;

            DrawGeneralInfo(gameTime);

            text.Color = Colors.Palette.Text;
            text.PivotOrigin = RectanglePoint.LeftTop;
            text.Position = new Vector2(5, 0);

            if (editMode == EditMode.Things)
                DrawThingsMode(gameTime, room);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (!IsActive)
                return;

            base.OnUpdate(gameTime);
            Invalidate();
        }

        #endregion

        // HandleInput
        internal HandleInputResult HandleInput()
        {
            var handled = false;

            if (InputManager.DefaultPlayer.Keyboard.IsKeyPressed(Keys.F2))
            {
                IsActive = !IsActive;
                return HandleInputResult.Handled;
            }

            if (!IsActive)
                return HandleInputResult.Unhandled;

            if (InputManager.DefaultPlayer.Keyboard.IsKeyPressed(Keys.V))
            {
                leftPanelVisible = !leftPanelVisible;
                return HandleInputResult.Handled;
            }

            if (InputManager.DefaultPlayer.Keyboard.IsKeyPressed(Keys.F10))
            {
                session.Save();
                handled = true;
            }

            else if (InputManager.DefaultPlayer.Keyboard.IsKeyPressed(Keys.F3))
            {
                editMode = EditMode.Things;
            }

            else if (InputManager.DefaultPlayer.Keyboard.IsKeyPressed(Keys.B))
            {
                GameThing.ShowBoundingBoxes = !GameThing.ShowBoundingBoxes;
                handled = true;
            }

            else if (InputManager.DefaultPlayer.Keyboard.IsKeyPressed(Keys.C))
            {
                GameThing.ShowColliders = !GameThing.ShowColliders;
                handled = true;
            }

            else if (InputManager.DefaultPlayer.Keyboard.IsKeyPressed(Keys.H))
            {
                GameThing.ShowHotspots = !GameThing.ShowHotspots;
                handled = true;
            }

            else
            {
                handled = HandleThingInput();
            }

            return handled ? HandleInputResult.Handled : HandleInputResult.Unhandled;
        }

        // IsActive
        public bool IsActive
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;

                    if (field)
                    {
                        if (session.Player != null && session.Player.IsInCurrentRoom)
                        {
                            SelectedThing = session.Player;
                        }
                        else
                        {
                            SelectFirstThing();
                            if (SelectedThing != null)
                            {
                                session.Camera.Follow(SelectedThing);
                                session.Camera.FocusTarget();
                            }
                        }
                    }
                    else if (session.Player is Actor player)
                    {
                        session.Camera.Follow(player);
                        SelectedThing = null;
                    }
                }
            }
        }

        // SelectedThing
        internal static GameThing? SelectedThing { get; private set; }
    }
}
