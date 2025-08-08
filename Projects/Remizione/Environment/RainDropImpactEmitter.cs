using Engendro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// RainDropImpactEmitter
    /// </summary>
    public sealed class RainDropImpactEmitter : GameObject
    {
        private int cooldown;
        private const int frameDuration = 120;
        private readonly List<AnimatedSprite> sprites = new();
        private readonly GameSession session;

        // Constructor
        public RainDropImpactEmitter(GameSession session, RainDropImpactKind kind)
            : base(session.Game)
        {
            this.session = session;
            this.Kind = kind;

            if (kind == RainDropImpactKind.Ground)
            {
                for (int i = 0; i < 20; i++)
                {
                    sprites.Add(CreateGroundImpactA());
                    sprites.Add(CreateGroundImpactB());
                }
            }
            else
            {
                for (int i = 0; i < 12; i++)
                {
                    sprites.Add(CreateGroundImpactC());
                }
            }
        }

        #region Private members

        // CreateAnimatedSprite
        private static AnimatedSprite CreateAnimatedSprite(EngendroGame game)
        {
            return new(game)
            {
                Atlas = Atlases.Environment,
                Color = Color.White * .2f,
                PivotOrigin = RectanglePoint.Center,
                Scale = new(.5f)
            };
        }

        // ResetCooldown
        private void ResetCooldown() => cooldown = Kind == RainDropImpactKind.Ground ? 500 : 250;

        // Spawn
        private void Spawn()
        {
            var room = session.Room;
            if (room == null)
                return;

            var spawnCount = Kind == RainDropImpactKind.Ground ? 10 : 1;

            for (int i = 0; i < sprites.Count; i++)
            {
                if (!sprites[i].Player.IsPlaying)
                {
                    Vector2 position;

                    if (Polygon != null)
                        position = Polygon.RandomPoint();

                    else if (session.Player?.WalkArea != null && session.Player.InCurrentRoom)
                        position = session.Player.WalkArea.GetWalkablePoint(session.Player.Position.Random(50));

                    else if (room.WalkArea != null)
                        position = room.WalkArea.Polygon.RandomPoint();

                    else
                        position = session.Camera.VisibleBox.GetRandomPoint();

                    sprites[i].Position = position;
                    sprites[i].Player.Play(false);
                    spawnCount--;

                    if (spawnCount == 0)
                        break;
                }
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                if (sprites[i].Player.IsPlaying)
                    sprites[i].Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (session.Environment.Rain.IsRaining)
            {
                cooldown -= gameTime.ElapsedGameTime.Milliseconds;
                if (cooldown <= 0)
                {
                    ResetCooldown();
                    Spawn();
                }
            }

            for (int i = 0; i < sprites.Count; i++)
            {
                sprites[i].Update(gameTime);
            }
        }

        #endregion

        #region Internal members

        // CreateGroundImpactA
        internal AnimatedSprite CreateGroundImpactA()
        {
            var result = CreateAnimatedSprite(Game);
            var animation = result.AddAnimation("A");
            animation.AddFrameSequence("RainDropImpactA", frameDuration, 1, 3);
            return result;
        }

        // CreateGroundImpactB
        internal AnimatedSprite CreateGroundImpactB()
        {
            var result = CreateAnimatedSprite(Game);
            var animation = result.AddAnimation("B");
            animation.AddFrameSequence("RainDropImpactB", frameDuration, 1, 4);
            return result;
        }

        // CreateGroundImpactC
        internal AnimatedSprite CreateGroundImpactC()
        {
            var result = CreateAnimatedSprite(Game);
            result.Scale = new Vector2(.3f);
            var animation = result.AddAnimation("C");
            animation.AddFrameSequence("RainDropImpactC", frameDuration, 1, 4);
            return result;
        }

        #endregion

        // Polygon
        public Polygon? Polygon { get; set; }

        // Kind
        public RainDropImpactKind Kind { get; }
    }
}
