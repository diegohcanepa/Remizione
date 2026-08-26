using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Engendro.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ScaryCastle
{
    /// <summary>
    /// GameRoom
    /// </summary>
    public class GameRoom : Room
    {
        #region Private fields

        private Color brightnessColor;
        private int currentDrawIndex;
        private readonly Color defaultPlayerLightColor = new(190, 190, 190);
        private readonly Vector2 defaultPlayerLightScale = new(5);
        private static DustEmitter dustEmitter = null!;
        private static FireflyEmitter fireflyEmitter = null!;
        private RenderTarget2D? lightMapTarget;
        private readonly List<Light> lights = [];
        private readonly List<ILightSource> lightSources = [];
        private string lastKnownMusicTag = string.Empty;
        private FacingDirection lastKnownPlayerDirection;
        private Vector2? lastKnownPlayerPosition;
        private static readonly Light playerLight = new("PlayerLight")
        {
            LightKind = LightKind.Player,
            PivotOrigin = RectanglePoint.Center,
            Position = Screen.Center,
        };
        private readonly List<TriggerArea> triggerAreas = [];
        private readonly List<WalkArea> walkAreas = [];
        private readonly List<IReadOnlyPolygon> walls = [];

        #endregion

        #region Constructor

        // Constructor
        public GameRoom(GameSession session, string name)
            : base(session, name)
        {
            this.Session = session;
            this.Lights = new NamedReadOnlyCollection<Light>(lights);
            this.TriggerAreas = new RoomAreaReadOnlyCollection<TriggerArea>(triggerAreas);
            this.WalkAreas = new RoomAreaReadOnlyCollection<WalkArea>(walkAreas);
            this.Walls = new(walls);

            dustEmitter ??= new DustEmitter(session, 6, 1000, 35);
            fireflyEmitter ??= new FireflyEmitter(session, 1, 500, 20);
            playerLight.TurnOff(true);
        }

        #endregion

        #region Private members

        // ApplyLightMap
        private void ApplyLightMap(RenderTarget2D target, RenderTarget2D? lightMap)
        {
            if (Game.SpriteBatch == null || lightMap == null)
                return;

            Game.SpriteBatch.Begin(transformMatrix: Game.Camera.GetTransformationMatrix(), samplerState: SamplerState.LinearClamp, blendState: BlendState.AlphaBlend, effect: ScaryCastleGame.Effects.Lighting.Effect);
            ScaryCastleGame.Effects.Lighting.LightMask.SetValue(lightMap);
            ScaryCastleGame.Effects.Lighting.Effect.CurrentTechnique.Passes[0].Apply();
            Game.SpriteBatch.Draw(target, Screen.Area, Color.White);
            Game.SpriteBatch.End();
        }

        // DrawComicTexts
        private void DrawComicTexts(GameTime gameTime)
        {
            if (Session.ComicTextPool.InUse.Count == 0)
                return;

            Game.SpriteBatch.Begin(Session.Camera, SamplerState.PointClamp, BlendState.AlphaBlend, null);
            for (int i = 0; i < Session.ComicTextPool.InUse.Count; i++)
            {
                Session.ComicTextPool.InUse[i].Draw(gameTime);
            }
            Game.SpriteBatch.End();
        }

#if DEBUG

        // DrawDebugBoxes
        private void DrawDebugBoxes()
        {
            Game.SpriteBatch.Begin(Session.Camera, SamplerState.PointClamp, BlendState.AlphaBlend, null);
            for (int i = 0; i < CulledThings.Count; i++)
            {
                if (CulledThings[i] is GameThing thing)
                    thing.DrawDebugBoxes();
            }
            Game.SpriteBatch.End();
        }

#endif

        // DrawDustParticles
        private void DrawDustParticles(GameTime gameTime)
        {
            if (dustEmitter != null)
            {
                Game.SpriteBatch.Begin(Session.Camera, SamplerState.PointClamp, BlendState.AlphaBlend, null);
                dustEmitter.Draw(gameTime);
                Game.SpriteBatch.End();
            }
        }

        // DrawEnvironmentParticles
        private void DrawEnvironmentParticles(GameTime gameTime)
        {
            if (DustParticleKind != DustParticleKind.None)
                DrawDustParticles(gameTime);

            if (AllowFireflyParticles)
                DrawFireflyParticles(gameTime);
        }

        // DrawFireflyParticles
        private void DrawFireflyParticles(GameTime gameTime)
        {
            if (fireflyEmitter != null)
            {
                Game.SpriteBatch.Begin(Session.Camera, SamplerState.PointClamp, BlendState.AlphaBlend, null);
                fireflyEmitter.Draw(gameTime);
                Game.SpriteBatch.End();
            }
        }

        // DrawFlyOffs
        private void DrawFlyOffs(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Session.Camera, SamplerState.PointClamp);
            for (var i = Session.ObjectPools.FlyOffs.InUse.Count - 1; i >= 0; i--)
            {
                Session.ObjectPools.FlyOffs.InUse[i].Draw(gameTime);
            }
            Game.SpriteBatch.End();
        }

        // DrawMeters
        private void DrawMeters(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Session.Camera, SamplerState.PointClamp, BlendState.AlphaBlend, null);
            for (int i = 0; i < CulledThings.Count; i++)
            {
                if (CulledThings[i] is GameThing thing)
                    thing.DrawMeter(gameTime);
            }
            Game.SpriteBatch.End();
        }

        // DrawShadows
        private void DrawShadows(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Session.Camera, SamplerState.PointClamp, BlendState.AlphaBlend, null);
            for (int i = 0; i < CulledThings.Count; i++)
            {
                if (CulledThings[i] is GameThing thing)
                    thing.DrawShadow(gameTime);
            }
            Game.SpriteBatch.End();
        }

        // DrawThings
        private void DrawThings(GameTime gameTime, RenderLayer renderLayer)
        {
            var depth = (int)renderLayer;

            for (var i = currentDrawIndex; i < CulledThings.Count; i++)
            {
                if (CulledThings[i].RenderLayerDepth != depth)
                {
                    break;
                }
                else if (CulledThings[i] is GameThing thing)
                {
                    var matrix = Session.Camera.GetViewMatrix(thing.ParallaxFactor);
                    Game.SpriteBatch.Begin(matrix, SamplerState.PointClamp, BlendState.AlphaBlend, null);
                    thing.Draw(gameTime);
                    Game.SpriteBatch.End();
                    currentDrawIndex++;
                }
            }
        }

        // PrepareLightMap
        private void PrepareLightMap(GameTime gameTime, RenderTarget2D? renderTarget)
        {
            if (renderTarget == null)
                return;

            // Save current render target
            var previousRenderTarget = Game.RenderTargets.CurrentTarget;

            Game.GraphicsDevice.SetRenderTarget(renderTarget);

            Game.GraphicsDevice.Clear(AmbientLights ? LightMapColor : Color.Black);

            Game.SpriteBatch.Begin(Session.Camera, SamplerState.LinearClamp, BlendState.Additive, null);

            // Owned lights
            for (int i = 0; i < lights.Count; i++)
            {
                if (lights[i].IsEmitting)
                {
                    if (lights[i].BoundingBox.Intersects(Session.Camera.CullingBox))
                    {
                        lights[i].Draw(gameTime);
                    }
                }
            }

            // Light sources
            for (int i = 0; i < CulledThings.Count; i++)
            {
                if (CulledThings[i] is GameThing thing && thing.IsEmittingLight && CulledThings[i].IsInCullingBox)
                    thing.DrawLights(gameTime);
            }

            if (Session.Player != null)
            {
                if (!AmbientLights)
                {
                    // TODO: Check old stat
                    playerLight.Color = Session.CurrentRun?.PlayerInventory.AmbientLightColor ?? defaultPlayerLightColor;
                    playerLight.Scale = defaultPlayerLightScale;// * Session.PlayerStats.AmbientLight.Value;
                    playerLight.Position = Session.Player.GetAnchoredPosition(15, 15);
                    playerLight.Draw(gameTime);
                }
            }

            if (BrightnessModifier > 0)
                Game.Shapes.DrawRectangle(Session.Viewport.ToRectangle(), brightnessColor);

            Game.SpriteBatch.End();

            if (AllowFireflyParticles)
                DrawFireflyParticles(gameTime);

            // Restore previous render target
            Game.GraphicsDevice.SetRenderTarget(previousRenderTarget);
        }

        // TestTriggerAreas
        private void TestTriggerAreas()
        {
            if (triggerAreas.Count > 0 && !Session.IsAwaiting && Session.Player != null)
            {
                for (int i = 0; i < TriggerAreas.Count; i++)
                {
                    TriggerAreas[i].Trigger(Session.Player);
                    if (TriggerAreas[i].Await && Session.IsAwaiting)
                        break;
                }
            }
        }

        #endregion

        #region Protected members

        // AddWall
        protected void AddWall(string vertices)
        {
            walls.Add(new Polygon(vertices));
        }

        // ClearWalkAreas
        protected void ClearWalkAreas()
        {
            walkAreas.Clear();
            WalkArea = null;
        }

        // GetAtlasPath
        protected override string GetAtlasPath()
        {
            return ContentManagerExtension.EncodePath(ContentFolder.Atlases, AtlasName);
        }

        // OnActivate
        protected override void OnActivate()
        {
            ScaryCastleGame.Effects.CRT.MonitorStyle = MonitorStyle;

            // Follow player
            if (Session.Player != null && Session.Player.IsInCurrentRoom && FollowPlayer)
                Session.Camera.Follow(Session.Player, true);

            if (DustParticleKind != DustParticleKind.None)
                dustEmitter?.Activate();

            if (AllowFireflyParticles)
                fireflyEmitter?.Activate();

            RefreshAmbientLightSources();
        }

        // OnDeactivate
        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            dustEmitter?.Deactivate();
            fireflyEmitter?.Deactivate();
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            // Prepare light map
            if (CanUseLightingSystem)
            {
                if (lightMapTarget != null && lightMapTarget.IsDisposed)
                    lightMapTarget = Game.RenderTargets.AuxiliaryTargets[0];

                PrepareLightMap(gameTime, lightMapTarget);
            }

            currentDrawIndex = 0;

            // BehindBackground (layer)
            DrawThings(gameTime, RenderLayer.BehindBackground);

            // Background
            Game.SpriteBatch.Begin(Session.Camera, SamplerState.PointClamp, BlendState.AlphaBlend, null);
            base.OnDraw(gameTime);
            Game.SpriteBatch.End();

            // Background (layer)
            DrawThings(gameTime, RenderLayer.Background);

            // OverBackground (layer)
            DrawThings(gameTime, RenderLayer.OverBackground);

            // Shadows
            DrawShadows(gameTime);

            // Default (layer)
            DrawThings(gameTime, RenderLayer.Default);

            // Environment particles
            DrawEnvironmentParticles(gameTime);

            // Foreround (layer)
            DrawThings(gameTime, RenderLayer.Foreground);

            // Apply light map
            if (CanUseLightingSystem && Game.RenderTargets != null)
            {
                Game.RenderTargets.Swap();
                ApplyLightMap(Game.RenderTargets.PreviousTarget, lightMapTarget);
            }

            // Draw meters
            DrawMeters(gameTime);

            Game.SpriteBatch.Begin(Session.Camera, SamplerState.PointClamp);
            Session.Environment.DevilHand.Draw(gameTime);
            Game.SpriteBatch.End();

            // Foreround (layer)
            DrawThings(gameTime, RenderLayer.ForegroundNoLight);

            // Draw texts (hit numbers, etc)
            DrawFlyOffs(gameTime);

            // Impact words
            DrawComicTexts(gameTime);

#if DEBUG
            DrawDebugBoxes();
#endif
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput()
        {
            if (Session.Player != null)
                return Session.Player.HandleInput();
            else
                return base.OnHandleInput();
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();

            // Follow player
            if (FollowPlayer && Session.Player?.Room == this)
                Session.Camera.Follow(Session.Player, true);

            lightMapTarget = Game.RenderTargets.AuxiliaryTargets[0];
            lightSources.Clear();
        }

        // OnRefreshAmbientLightSources
        protected virtual void OnRefreshAmbientLightSources()
        {
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            Session.Environment.DevilHand.Update(gameTime);

            for (int i = 0; i < Session.ComicTextPool.InUse.Count; i++)
            {
                Session.ComicTextPool.InUse[i].Update(gameTime);
            }

            TestTriggerAreas();

            // Lights
            for (int i = 0; i < lights.Count; i++)
            {
                lights[i].Update(gameTime);
            }

            if (!AmbientLights && Session.Player != null)
                playerLight.Update(gameTime);

            // Dust particles
            if (DustParticleKind != DustParticleKind.None)
                dustEmitter?.Update(gameTime);

            // Firefly particles
            if (AllowFireflyParticles)
                fireflyEmitter?.Update(gameTime);

            // FlOffs
            for (var i = 0; i < Session.ObjectPools.FlyOffs.InUse.Count; i++)
            {
                Session.ObjectPools.FlyOffs.InUse[i].Update(gameTime);
            }
        }

        #endregion

        // AddLight
        public Light AddLight(string name)
        {
            CodeContract.NotDuplicate(Lights, name, nameof(name));
            var result = new Light(name);
            lights.Add(result);
            return result;
        }

        // AddTriggerArea
        public TriggerArea AddTriggerArea(string name, Script routine, Script? exitRoutine, bool await, bool stopActor, bool once, FlagCondition? condition, params Vector2[] vertices)
        {
            CodeContract.NotDuplicate(TriggerAreas, name, nameof(name));
            var result = new TriggerArea(this, name, routine, exitRoutine, await, stopActor, once, condition, vertices);
            triggerAreas.Add(result);

            return result;
        }

        // AddWalkArea
        public WalkArea AddWalkArea(string name, string vertices)
        {
            return AddWalkArea(name, Polygon.GetVertices(vertices));
        }

        // AddWalkArea
        public WalkArea AddWalkArea(string name, params Vector2[] vertices)
        {
            CodeContract.NotDuplicate(WalkAreas, name, nameof(name));

            WalkArea result = new(this, name, null, vertices);
            walkAreas.Add(result);

            if (walkAreas.Count == 1)
                WalkArea = WalkAreas[0];

            return result;
        }

        // AllowFireflyParticles
        [ScriptProperty]
        public bool AllowFireflyParticles { get; set; }

        // AllowPauseMenu
        [ScriptProperty]
        public bool AllowPauseMenu { get; set; } = true;

        // AmbientLights
        public bool AmbientLights { get; private set; }

        // BrightnessModifier
        [ScriptProperty]
        public float BrightnessModifier
        {
            get;
            set
            {
                if (value != field)
                {
                    field = Math.Clamp(value, 0, 1);
                    brightnessColor = Color.White * field;
                }
            }
        }

        // CanUseLightingSystem
        public bool CanUseLightingSystem => Session.LightingSystem && LightingSystem;

        // ControlMouseCursor
        public virtual bool ControlMouseCursor => false;

        // DustParticleKind
        [ScriptProperty]
        public DustParticleKind DustParticleKind { get; set; } = DustParticleKind.Ash;

        // FollowPlayer
        public bool FollowPlayer { get; set; } = true;

        // IsProcedural
        [ScriptProperty]
        public virtual bool IsProcedural => false;

        // IsWalkable
        public virtual bool IsWalkable => true;

        // LightingSystem
        [ScriptProperty]
        public bool LightingSystem { get; set; }

        // LightMapColor
        [ScriptProperty]
        public Color LightMapColor { get; set; } = new Color(20, 20, 25);

        // Lights
        public NamedReadOnlyCollection<Light> Lights { get; }

        // MonitorStyle
        public bool MonitorStyle { get; init; }

        // PlayerLightBounds
        public static RectangleF PlayerLightBounds => playerLight.BoundingBox;

        // PreserveBeforeGateway
        [ScriptMethod]
        public void PreserveBeforeGateway()
        {
            if (Session.Player != null)
            {
                lastKnownPlayerDirection = Session.Player.Direction;
                lastKnownPlayerPosition = Session.Player.Position;
            }

            lastKnownMusicTag = AudioManager.Music.CurrentTag;
        }

        // RefreshAmbientLightSources
        public void RefreshAmbientLightSources()
        {
            var hasAmbientLights = !IsProcedural;

            for (var i = 0; i < Lights.Count; i++)
            {
                if (Lights[i].Ambient && Lights[i].IsEmitting)
                {
                    hasAmbientLights = true;
                    break;
                }
            }

            if (!hasAmbientLights)
            {
                for (var i = 0; i < Children.Count; i++)
                {
                    if (Children[i] is Prop prop && prop.IsAmbientLight && prop.IsEmittingLight)
                    {
                        hasAmbientLights = true;
                        break;
                    }
                }
            }

            if (hasAmbientLights)
                playerLight.TurnOff();
            else
                playerLight.TurnOn();

            AmbientLights = hasAmbientLights;

            OnRefreshAmbientLightSources();
        }

        // RestoreAfterGateway
        [ScriptMethod]
        public void RestoreAfterGateway()
        {
            if (lastKnownPlayerPosition.HasValue && Session.Player != null)
            {
                Session.Player.Position = lastKnownPlayerPosition.Value;
                Session.Player.Direction = lastKnownPlayerDirection;
                Children.Add(Session.Player);
                Session.Player.FlipHorizontally();
                Session.Camera.Follow(Session.Player, true);
            }

            if (!string.IsNullOrWhiteSpace(lastKnownMusicTag))
                AudioManager.Music.PlayTag(lastKnownMusicTag);
        }

        // SelectWalkArea
        public void SelectWalkArea(string name)
        {
            WalkArea = WalkAreas.Find(name) ?? throw new InvalidOperationException($"Walk area '{name}' not found.");
        }

        // Session
        public new GameSession Session { get; }

        // TurnOffAmbientLights
        public void TurnOffAmbientLights()
        {
            for (var i = 0; i < Children.Count; i++)
            {
                if (Children[i] is Prop prop && prop.IsAmbientLight)
                    prop.IgnoreAttachedLight = true;
            }

            RefreshAmbientLightSources();
        }

        // TriggerAreas
        public RoomAreaReadOnlyCollection<TriggerArea> TriggerAreas { get; }

        // Walls
        public ReadOnlyCollection<IReadOnlyPolygon> Walls { get; }

        // WalkArea
        public WalkArea? WalkArea { get; private set; }

        // WalkAreas
        public RoomAreaReadOnlyCollection<WalkArea> WalkAreas { get; }
    }
}
