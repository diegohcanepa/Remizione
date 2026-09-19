using Adberration;
using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Bonfire
    /// </summary>
    public class Bonfire : Prop, ISafeZone
    {
        private readonly AnimatedSprite flame;
        private readonly FloatTween globalOpacityTween = new();
        private readonly Sprite patch;

        #region Constructor

        // Constructor
        public Bonfire(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Props;

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
                IgnoreGlobalOpacity = true,
                PivotOrigin = RectanglePoint.Center,
                Position = new(9),
                Scale = new(9, 6)
            };

            AttachedLight.Unlit(true);

            AttachedLightPosition = new(15, 4);
        }

        #endregion

        #region ISafeZone interface

        // Center
        Vector2 ISafeZone.Center => BoundingBox.Center;

        // IsEnabled
        bool ISafeZone.IsEnabled => IsLit;

        // Radius
        float ISafeZone.Radius => 30;

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
            if (IsLit)
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
                flame.Update(gameTime);

            if (globalOpacityTween.IsRunning)
            {
                globalOpacityTween.Update(gameTime);
                Light.GlobalOpacity = globalOpacityTween.CurrentValue;
            }
        }

        #endregion

        // Activate
        [ScriptMethod]
        public void Activate()
        {
            Session.Environment.GlobalLight.Unlit();
            globalOpacityTween.Start(TweenStyle.CubicIn, 1, 0, 2000, Session.CleanUpRuntimeEntities);
        }

        // Deactivate
        [ScriptMethod]
        public void Deactivate()
        {
            Session.Environment.GlobalLight.Lit();
            globalOpacityTween.Start(TweenStyle.CubicIn, 0, 1, 2000);
        }

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
                        flame.Player.Play("Default", true);
                        PlaySound(SoundNames.Bonfire, true);
                        AttachedLight?.Lit(Session.State == GameSessionState.Loading);

                        if (Session.State != GameSessionState.Loading)
                        {
                            flame.Tweens.OpacityTween = FloatTween.Create(TweenStyle.CubicIn, 0, flame.Opacity, 500);
                            flame.Tweens.ScaleTween = Vector2Tween.Create(TweenStyle.CubicIn, .2f, flame.Scale.X, 1000);
                        }
                    }
                }
            }
        }
    }
}
