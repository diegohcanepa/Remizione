using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Bonfire
    /// </summary>
    public class Bonfire : Prop
    {
        private static readonly Vector2 maxLightScale = new(18, 12);
        private static readonly Vector2 minLightScale = new(9, 6);
        private readonly AnimatedSprite flame;
        private const float flameOpacity = .8f;
        private const float flameScale = .6f;
        private readonly Sprite patch;
        private readonly FloatTween resetTween = new();
        private SoundInstance? soundInstance;

        #region Constructor

        // Constructor
        public Bonfire(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Props;
            DepthOffset = -2;

            // Flame
            this.flame = new()
            {
                Atlas = Atlases.Props,
                Opacity = .8f,
                PivotOrigin = RectanglePoint.Bottom,
                Scale = new(.6f)
            };

            // Patch
            this.patch = new(Atlas?.GetImage("BonfirePatch"))
            {
                PivotOrigin = RectanglePoint.Bottom
            };

            var animation = flame.AddAnimation("Default");
            animation.AddFrameSequence($"BonfireFlame", 60, 1, 8);

            this.AttachedLight = new("Light", LightKind.SulfurBonfire)
            {
                BlendMode = LightBlendMode.Additive,
                PivotOrigin = RectanglePoint.Center,
                Position = new(9),
                Scale = minLightScale
            };

            AttachedLight.Unlit(true);

            AttachedLightPosition = new(10, 4);
        }

        #endregion

        #region Private members

        // RefreshAttachedLightScale
        private void RefreshAttachedLightScale()
        {
            if (AttachedLight != null)
            {
                float progress = MathHelper.Clamp(RemainingTime / GameData.WorldSettings.BonfireDuration, 0f, 1f);
                AttachedLight.Scale = Vector2.Lerp(minLightScale, maxLightScale, progress);
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            if (IsLit || flame.Tweens.IsTweening)
            {
                flame.Draw(gameTime);
                patch.Draw(gameTime);
            }
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            RefreshAttachedLightScale();
        }

        // OnTransform
        protected override void OnTransform(TransformChange change)
        {
            base.OnTransform(change);

            if (change == TransformChange.Position)
            {
                flame.Position = Position + new Vector2(1, -8);
                patch.Position = Position;
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (IsLit && Session.IsGameplayActive)
            {
                if (resetTween.IsRunning)
                {
                    resetTween.Update(gameTime);
                    RemainingTime = resetTween.CurrentValue;
                }
                else if (RemainingTime > 0)
                {
                    float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    RemainingTime -= elapsed;

                    if (RemainingTime <= 0)
                    {
                        RemainingTime = 0;
                        IsLit = false;
                    }
                }

                // Ajusta la escala de la luz proporcionalmente al tiempo restante
                if (AttachedLight != null)
                {
                    float progress = MathHelper.Clamp(RemainingTime / GameData.WorldSettings.BonfireDuration, 0f, 1f);
                    AttachedLight.Scale = Vector2.Lerp(minLightScale, maxLightScale, progress);
                }
            }

            flame.Update(gameTime);
        }

        #endregion

        // IsLit
        [ScriptProperty]
        public bool IsLit
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;

                    if (field)
                    {
                        ResetRemainingTime();
                        flame.Player.Play("Default", true);
                        soundInstance = PlaySound(SoundNames.Bonfire, true);
                        AttachedLight?.Lit(Session.State == GameSessionState.Loading);

                        AttachedLight?.Scale = maxLightScale;

                        if (Session.State != GameSessionState.Loading)
                        {
                            flame.Tweens.OpacityTween = FloatTween.Create(TweenStyle.CubicIn, 0, flameOpacity, 500);
                            flame.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicIn, .2f, flameScale, 1000);
                        }
                    }
                    else
                    {
                        RemainingTime = 0;
                        AttachedLight?.Unlit(Session.State == GameSessionState.Loading);

                        AttachedLight?.Scale = minLightScale;

                        if (Session.State != GameSessionState.Loading)
                        {
                            soundInstance?.Stop(2000);
                            flame.Tweens.OpacityTween = FloatTween.Create(TweenStyle.CubicIn, flame.Opacity, 0, 2500);
                            flame.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicIn, flame.Scale.X, .2f, 2000);
                        }
                    }
                }
            }
        }

        // RemainingTime
        public float RemainingTime { get; set; }

        // ResetRemainingTime
        [ScriptMethod]
        public void ResetRemainingTime()
        {
            resetTween.Start(TweenStyle.Linear, RemainingTime, GameData.WorldSettings.BonfireDuration, 2000);
        }

        // SafeToRest
        [ScriptProperty]
        public bool SafeToRest
        {
            get
            {
                if (Session.Room == null || Session.Player == null)
                    return true;

                var safeArea = RectangleF.ScaleCentered(Session.Camera.VisibleBox, .7f);

                for (var i = 0; i < Session.Room.CulledThings.Count; i++)
                {
                    if (Session.Room.CulledThings[i] is Actor actor)
                    {
                        if (actor.IsPlayer)
                            continue;

                        if (FactionMatrix.IsHostile(actor, Session.Player))
                        {
                            if (safeArea.Intersects(actor.RuntimeHotspot.BoundingRectangleF))
                                return false;
                        }
                    }
                }

                return true;
            }
        }

        // SetAsDefault
        [ScriptMethod]
        public void SetAsDefault()
        {
            Session.LastBonfire = this;
        }
    }
}