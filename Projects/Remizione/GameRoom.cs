using Engendro;
using EngendroAdventure;
using EngendroAdventure.Scripting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// GameRoom
    /// </summary>
    public class GameRoom : Room
    {
        #region Private fields

        private int currentDrawIndex;
        private static DustEmitter dustEmitter = null!;
        private static FireflyEmitter fireflyEmitter = null!;
        private readonly Light lightning;
        private RenderTarget2D? lightMapTarget;
        private readonly List<Light> lights = [];
        private readonly List<ILightSource> lightSources = [];
        private readonly List<Light> renderedLights = [];
        private readonly List<TriggerArea> triggerAreas = [];
        private readonly List<WalkArea> walkAreas = [];

        #endregion

        #region Constructor

        // Constructor
        public GameRoom(GameSession session, string name)
            : base(session, name)
        {
            this.Session = session;
            this.lightning = new Light(Game, "<Lightning>")
            { 
                ImageName = nameof(Atlases.Environment.LightningLight),
                LightKind = LightKind.Lightning,
                Scale = new Vector2(6)
            };

            this.Lights = new NamedObjectReadOnlyCollection<Light>(lights);
            this.TriggerAreas = new RoomAreaReadOnlyCollection<TriggerArea>(triggerAreas);
            this.WalkAreas = new RoomAreaReadOnlyCollection<WalkArea>(walkAreas);

            dustEmitter ??= new DustEmitter(session, 6, 1000, 35);
            fireflyEmitter ??= new FireflyEmitter(session, 1, 500, 20);
        }

        #endregion

        #region Private members

        // ApplyLightMap
        private void ApplyLightMap(RenderTarget2D target, RenderTarget2D? lightMap)
        {
            if (Game.SpriteBatch == null || lightMap == null)
                return;

            Game.SpriteBatch.Begin(transformMatrix: Game.Camera.GetTransformationMatrix(), samplerState: SamplerState.LinearClamp, blendState: BlendState.AlphaBlend, effect: RemizioneGame.Effects.Lighting.Effect);
            RemizioneGame.Effects.Lighting.LightMask.SetValue(lightMap);
            RemizioneGame.Effects.Lighting.Effect.CurrentTechnique.Passes[0].Apply();
            Game.SpriteBatch.Draw(target, Screen.Area, Color.White);
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

        // DrawFloatingTexts
        private void DrawFloatingTexts(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Session.Camera, SamplerState.LinearClamp);
            for (var i = Session.ObjectPools.FloatingTexts.InUse.Count - 1; i >= 0; i--)
            {
                Session.ObjectPools.FloatingTexts.InUse[i].Draw(gameTime);
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

        // DrawThingUIElements
        private void DrawThingUIElements(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Session.Camera);
            for (int i = 0; i < CulledThings.Count; i++)
            {
                if (CulledThings[i] is GameThing thing)
                    thing.DrawDamagerMeter(gameTime);
            }
            Game.SpriteBatch.End();
        }

        // DrawThings
        private void DrawThings(GameTime gameTime, RenderLayer renderLayer, GameThing? interactiveTarget)
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
                    ShaderEffect? effect = null;

                    if (interactiveTarget == thing)
                    {
                        RemizioneGame.Effects.ColorSaturation.SetColor(.8f, .8f, .8f, 0);
                        effect = RemizioneGame.Effects.ColorSaturation;
                    }

                    Game.SpriteBatch.Begin(Session.Camera, RoomSampler == RoomSampler.PointClamp ? SamplerState.PointClamp : SamplerState.LinearClamp, BlendState.AlphaBlend, effect?.Effect);
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

            renderedLights.Clear();

            Game.GraphicsDevice.SetRenderTarget(renderTarget);

            Game.GraphicsDevice.Clear(LightMapColor);
            Game.SpriteBatch.Begin(Session.Camera, SamplerState.LinearClamp, BlendState.Additive, null);

            // Owned lights
            for (int i = 0; i < lights.Count; i++)
            {
                if (lights[i].IsEmitting)
                {
                    if (lights[i].BoundingBox.Intersects(Session.Camera.CullingBox))
                    {
                        lights[i].Draw(gameTime);
                        renderedLights.Add(lights[i]);
                    }
                }
            }

            if (lightning.IsFlashing)
            {
                lightning.Position = Session.Camera.VisibleBox.Center;
                lightning.Draw(gameTime);
                renderedLights.Add(lightning);
            }

            // Light sources
            for (int i = 0; i < CulledThings.Count; i++)
            {
                if (CulledThings[i] is GameThing thing && thing.IsEmittingLight && CulledThings[i].IsInCullingBox)
                    thing.DrawLights(gameTime, renderedLights);
            }

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

        // GetAtlasPath
        protected override string GetAtlasPath()
        {
            return ContentHelper.EncodePath(ContentFolder.Atlases, AtlasName);
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

            // Find outlined target
            var interactiveTarget = Session.Player?.InteractiveTarget;

            // BehindBackground (layer)
            DrawThings(gameTime, RenderLayer.BehindBackground, interactiveTarget);

            // Room Background
            Game.SpriteBatch.Begin(Session.Camera, SamplerState.PointClamp, BlendState.AlphaBlend, null);
            base.OnDraw(gameTime);
            Game.SpriteBatch.End();

            // Background (layer)
            DrawThings(gameTime, RenderLayer.Background, interactiveTarget);

            // Shadows
            DrawShadows(gameTime);

            // Rain drop impacts
            if (Session.Environment.Weather.IsRaining)
                Session.Environment.Weather.DrawRainDropImpacts(gameTime);

            // Move destination mark
            Session.HUD.DestinationMark.Draw(gameTime);

            // Doors (layer)
            DrawThings(gameTime, RenderLayer.Doors, interactiveTarget);

            // Default (layer)
            DrawThings(gameTime, RenderLayer.Default, interactiveTarget);

            // Environment particles
            DrawEnvironmentParticles(gameTime);

            // Foreround (layer)
            DrawThings(gameTime, RenderLayer.Foreground, interactiveTarget);

            // Rain
            if (Session.Environment.Weather.IsRaining)
                Session.Environment.Weather.DrawRain(gameTime);

            // Apply light map
            if (CanUseLightingSystem && Game.RenderTargets != null)
            {
                Game.RenderTargets.Swap();
                ApplyLightMap(Game.RenderTargets.PreviousTarget, lightMapTarget);
            }

            // Foreround (layer)
            DrawThings(gameTime, RenderLayer.ForegroundNoLight, interactiveTarget);

            // DrawThingUIElements
            DrawThingUIElements(gameTime);

            // Draw texts (hit numbers, etc)
            DrawFloatingTexts(gameTime);

            // Draw speech bubbles
            SpeechBubble.DrawSpeechBubbles(gameTime);

#if DEBUG
            DrawDebugBoxes();
#endif
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            if (Session.Player != null)
                return Session.Player.HandleInput(gameTime);
            else
                return base.OnHandleInput(gameTime);
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();

            if (DustParticleKind != DustParticleKind.None)
                dustEmitter?.Activate();

            if (AllowFireflyParticles)
                fireflyEmitter?.Activate();

            lightMapTarget = Game.RenderTargets.AuxiliaryTargets[0];
            lightSources.Clear();

            // ??
            //if (Width <= Screen.NativeWidth)
            //    Session.RoomCamera.Position += new Vector2(0, 5);

            // Prepare lights
            if (Atlas != null)
            {
                for (int i = 0; i < Lights.Count; i++)
                {
                    Lights[i].Prepare(Atlas);
                }
            }

            // Follow player
            if (Session.Player != null && Session.Player.InCurrentRoom)
                Session.Camera.FollowTarget(Session.Player, true);
        }

        // OnUnload
        protected override void OnUnload()
        {
            base.OnUnload();
            dustEmitter?.Deactivate();
            fireflyEmitter?.Deactivate();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            TestTriggerAreas();

            // Lights
            for (int i = 0; i < lights.Count; i++)
            {
                lights[i].Update(gameTime);
            }

            if (lightning.IsFlashing)
                lightning.Update(gameTime);

            // Dust particles
            if (DustParticleKind != DustParticleKind.None)
                dustEmitter?.Update(gameTime);

            // Firefly particles
            if (AllowFireflyParticles)
                fireflyEmitter?.Update(gameTime);

            // Floating texts
            for (var i = 0; i < Session.ObjectPools.FloatingTexts.InUse.Count; i++)
            {
                Session.ObjectPools.FloatingTexts.InUse[i].Update(gameTime);
            }
        }

        // RequiresPersistence
        protected override bool RequiresPersistence => true;

        #endregion

        // AddLight
        public Light AddLight(string name)
        {
            CodeContract.NotDuplicate(Lights, name, nameof(name));
            var result = new Light(Game, name);
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

        // CanUseLightingSystem
        public bool CanUseLightingSystem => Session.LightingSystem && LightingSystem;

        // DustParticleKind
        [ScriptProperty]
        public DustParticleKind DustParticleKind { get; set; } = DustParticleKind.Ash;

        // IsOutdoor
        [ScriptProperty]
        public bool IsOutdoor { get; set; } = true;

        // IsWalkable
        public virtual bool IsWalkable => true;

        // LightingSystem
        [ScriptProperty]
        public bool LightingSystem { get; set; }

        // LightMapColor
        [ScriptProperty]
        public Color LightMapColor { get; set; } = new Color(10, 10, 25);

        // Lights
        public NamedObjectReadOnlyCollection<Light> Lights { get; }

        // RemoveWalkArea
        public bool RemoveWalkArea(string name)
        {
            if (WalkAreas.Find(name) is WalkArea walkArea)
            {
                walkAreas.Remove(walkArea);

                if (WalkArea == walkArea)
                    WalkArea = null;

                return true;
            }

            return false;
        }

        // RoomSampler
        [ScriptProperty(CodingContext.EntityDeclaration)]
        public RoomSampler RoomSampler { get; set; } = RoomSampler.PointClamp;

        // SelectWalkArea
        public void SelectWalkArea(string name)
        {
            WalkArea = WalkAreas.Find(name) ?? throw new InvalidOperationException($"Walk area '{name}' not found.");
        }

        // Session
        public new GameSession Session { get; }

        // ShowLightning
        public void ShowLightning(bool extendedDuration)
        {
            var interval = extendedDuration ? new Int32Range(100, 150) : new Int32Range(50, 80);
            int count = extendedDuration ? 18 : 6;
            //var lightCount = 0;

            for (int i = 0; i < Lights.Count; i++)
            {
                /*
                if (Lights[i].Environmental)
                {
                    Lights[i].Flash(interval, count);
                    lightCount++;
                }
                */
            }

            if (IsOutdoor)
                lightning.Flash(interval, count);
        }

        // TriggerAreas
        public RoomAreaReadOnlyCollection<TriggerArea> TriggerAreas { get; }

        // WalkArea
        public WalkArea? WalkArea { get; private set; }

        // WalkAreas
        public RoomAreaReadOnlyCollection<WalkArea> WalkAreas { get; }
    }
}
