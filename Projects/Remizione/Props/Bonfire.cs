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
                IgnoreGlobalOpacity = true,
                PivotOrigin = RectanglePoint.Center,
                Position = new(9),
                Scale = new(9, 6)
            };

            AttachedLight.Unlit(true);

            AttachedLightPosition = new(10, 4);
        }

        #endregion

        #region ISafeZone interface

        // Center
        Vector2 ISafeZone.Center => BoundingBox.Center;

        // IsEnabled
        bool ISafeZone.IsEnabled => IsLit;

        // Radius
        float ISafeZone.Radius => 50;

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

        // Enter
        [ScriptMethod]
        public void Enter()
        {
            Enter(false);
        }

        // Enter
        public void Enter(bool immediate)
        {
            if (Session.BonfireMode || Session.Room == null)
                return;

            if (immediate)
            {
                Light.GlobalOpacity = 0;
            }
            else
            {
                Sound.Play(SoundNames.BonfireMenu);
                globalOpacityTween.Start(TweenStyle.CubicIn, 1, 0, 2000);
                Session.Camera.StopFollowing();
                Session.Camera.FlyTo(TweenStyle.CubicInOut, new Vector2(X - 50, Y - 10), 1.2f, 2000);
                Session.Save(true);
            }

            Session.BonfireMode = true;
        }

        // Exit
        [ScriptMethod]
        public void Exit()
        {
            Sound.Play(SoundNames.BonfireMenu);
            Session.Camera.ZoomTo(TweenStyle.CubicInOut, GameSettings.DefaultZoom, 3000);
            if (Session.Player != null)
                Session.Camera.Follow(Session.Player);
            globalOpacityTween.Start(TweenStyle.CubicIn, 0, 1, 2000);
            Session.BonfireMode = false;
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

        // IsSafe
        [ScriptProperty]
        public bool IsSafe
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
    }
}
