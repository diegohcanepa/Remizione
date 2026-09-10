using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Bonfire
    /// </summary>
    public class Bonfire : Prop
    {
        private readonly AnimatedSprite flame;
        private readonly Sprite patch;

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

            this.AttachedLight = new("Light")
            {
                PivotOrigin = RectanglePoint.Center,
                LightKind = LightKind.Default,
                Color = new(163, 168, 70),
                Position = new(9),
                Scale = new(8, 5)
            };

            AttachedLight.TurnOff(true);

            AttachedLightPosition = new(15, 4);
        }

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
                        flame.Player.Play("Default", true);
                        PlaySound(SoundNames.Bonfire, true);
                        AttachedLight?.TurnOn();
                    }
                }
            }
        }
    }
}
