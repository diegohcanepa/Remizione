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
        private static readonly Vector2 maxLightScale = new(14, 10);
        private static readonly Vector2 minLightScale = new(9, 6);
        private readonly AnimatedSprite flame;
        private const float flameOpacity = .8f;
        private const float flameScale = .6f;
        private readonly Sprite patch;
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

            if (IsLit)
            {
                if (ActiveTimeRemaining > 0f)
                {
                    float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    ActiveTimeRemaining -= elapsed;

                    if (ActiveTimeRemaining <= 0)
                    {
                        ActiveTimeRemaining = 0;
                        IsLit = false;
                    }
                }

                // Ajusta la escala de la luz proporcionalmente al tiempo restante
                if (AttachedLight != null)
                {
                    float progress = MathHelper.Clamp(ActiveTimeRemaining / GameData.WorldSettings.BonfireDuration, 0f, 1f);
                    AttachedLight.Scale = Vector2.Lerp(minLightScale, maxLightScale, progress);
                }
            }

            flame.Update(gameTime);
        }

        #endregion

        // ActiveTimeRemaining
        [ScriptProperty]
        public float ActiveTimeRemaining { get; set; }

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
                        ActiveTimeRemaining = GameData.WorldSettings.BonfireDuration;

                        flame.Player.Play("Default", true);
                        soundInstance = PlaySound(SoundNames.Bonfire, true);
                        AttachedLight?.Lit(Session.State == GameSessionState.Loading);

                        if (AttachedLight != null)
                            AttachedLight.Scale = maxLightScale;

                        if (Session.State != GameSessionState.Loading)
                        {
                            flame.Tweens.OpacityTween = FloatTween.Create(TweenStyle.CubicIn, 0, flameOpacity, 500);
                            flame.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicIn, .2f, flameScale, 1000);
                        }
                    }
                    else
                    {
                        ActiveTimeRemaining = 0;
                        AttachedLight?.Unlit(Session.State == GameSessionState.Loading);

                        if (AttachedLight != null)
                            AttachedLight.Scale = minLightScale;

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
    }
}