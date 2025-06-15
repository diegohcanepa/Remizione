using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Throwable
    /// </summary>
    public abstract class Throwable : GameThing
    {
        #region Private fields

        private ThrowableBounceIntensity bounceIntensity;
        private int bounces;
        private int collectCooldown = -1;
        private readonly Int32Range[] collisionBounces = new Int32Range[2];
        private GameThing? ignoreThing;
        private bool instantBounce;
        private bool isFlipped;
        private Int32Range noCollisionBounce;
        private bool outOfBounce;
        private readonly Vector2Tween scaleTween = new();
        private bool targetHit;
        private readonly Polygon testPoly = new();
        private readonly FloatTween xBounceTween = new();
        private readonly FloatTween xTween = new();
        private readonly FloatTween yBounceTween = new();
        private readonly FloatTween yTween = new();

        #endregion

        #region Constructor

        // Constructor
        protected Throwable(GameSession session, int maxDistance, int duration, bool reusable)
            : base(session, string.Empty)
        {
            this.Atlas = Atlases.Environment;
            this.MaxDistance = maxDistance;
            this.Duration = duration;
            this.IgnoreCulling = true;
            this.Reusable = reusable;
            this.PivotOrigin = RectanglePoint.Middle;
            this.Shadow = new ImageSprite(session.Game)
            {
                Opacity = ColorPalette.ShadowOpacity,
                PivotOrigin = RectanglePoint.Middle,
            };

            this.BounceIntensity = ThrowableBounceIntensity.Medium;
        }

        #endregion

        #region Private members

        // Bounce
        private void Bounce()
        {
            bounces = 1;
            var offset = noCollisionBounce.Random();
            if (!isFlipped)
                offset *= -1;

            if (outOfBounce)
                offset *= -1;

            xBounceTween.Start(TweenStyle.CubicOut, X, X - offset, 200);
            Tweens.XTween = xBounceTween;

            yBounceTween.Start(TweenStyle.CubicOut, Y, Y - Randomizer.Next(1, 3), 100, 2);
            Tweens.YTween = yBounceTween;
        }

        // CheckCollision
        private GameThing? CheckCollision()
        {
            if (Room == null || Item == null)
                return null;

            for (var i = 0; i < Room.CulledThings.Count; i++)
            {
                if (Room.CulledThings[i] == this || Room.CulledThings[i] == Item.Owner || Room.CulledThings[i] == ignoreThing)
                    continue;

                if (Room.CulledThings[i] is GameThing target && !target.IgnoreThrowables && !target.IsDead)
                {
                    var hit = false;

                    if (target is IHoleArea holeArea)
                        hit = holeArea.Contains(Position);

                    if (!hit)
                        hit = target.HurtBox.Contains(Position);

                    if (hit)
                        return target;
                }
            }

            return null;
        }

        // CheckComplete
        private void CheckComplete()
        {
            // Impact
            if (CheckCollision() is GameThing target)
                Complete(target);

            // Out of bounds
            else if (Room?.WalkArea != null && !Room.WalkArea.IsInside(Position))
            {
                outOfBounce = true;
                Complete(null);
            }

            // Maximum distance
            else if (!xTween.IsRunning && !yTween.IsRunning)
                Complete(null);
        }

        // Complete
        private void Complete(GameThing? target)
        {
            IsActive = false;

            // Apply damage
            if (target != null && Item != null)
            {
                // TODO: Check ApplyDamage
                //?Item.ApplyDamage .EndUse(target);
                if (ImpactSound != null)
                    PlaySound(ImpactSound);
            }

            if (Reusable)
            {
                targetHit = target != null;
                if (target != null)
                    FirstBounce();
                else
                    Bounce();
            }
            else
                Unparent();

            OnComplete(target);
        }

        // EndBounce
        private void EndBounce()
        {
            if (Y != FloorPosition.Y)
            {
                yBounceTween.Start(TweenStyle.CubicOut, Y, FloorPosition.Y, 100);
                Tweens.YTween = yBounceTween;
            }

            Rotation = 0;
            RotationSpeed = 0;

            collectCooldown = 300;
        }

        // FirstBounce
        private void FirstBounce()
        {
            bounces = 2;
            RotationSpeed *= -.5f;
            var offset = collisionBounces[0].Random();
            if (isFlipped)
                offset *= -1;

            xBounceTween.Start(TweenStyle.Linear, X, X - offset, 200);
            Tweens.XTween = xBounceTween;
        }

        // ReturnToOwner
        private void ReturnToOwner()
        {
            if (HasParent && Item?.Owner is Actor player)
            {
                Unparent();
                Session.ObjectPools.ReturnThrowable(this);
                player.Inventory.Add(Item.Name, 1);
            }
        }

        // SecondBounce
        private void SecondBounce()
        {
            var offset = collisionBounces[1].Random();
            if (isFlipped)
                offset *= -1;

            xBounceTween.Start(TweenStyle.CubicOut, X, X - offset, 200);
            Tweens.XTween = xBounceTween;

            yBounceTween.Start(TweenStyle.CubicOut, Y, Y - Randomizer.Next(1, 3), 100, 2);
            Tweens.YTween = yBounceTween;
        }

        // UpdateCollectState
        private void UpdateCollectState(GameTime gameTime)
        {
            if (Item == null)
                return;

            if (collectCooldown > 0)
            {
                collectCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                if (collectCooldown < 0)
                    collectCooldown = 0;
            }
            else if (scaleTween.IsRunning || DistanceTo(Item.Owner) <= 5)
            {
                if (!scaleTween.IsRunning)
                {
                    scaleTween.Start(TweenStyle.Linear, Scale, Vector2.Zero, 100);
                }
                else
                {
                    scaleTween.Update(gameTime);

                    Scale = scaleTween.CurrentValue;

                    if (!scaleTween.IsRunning)
                    {
                        Item.Owner.PlaySound(SoundNames.ThrowablePickup);
                        ReturnToOwner();
                    }
                }
            }
        }

        #endregion

        #region Protected members

        // BounceIntensity
        protected ThrowableBounceIntensity BounceIntensity
        {
            get => bounceIntensity;
            set
            {
                if (value != bounceIntensity)
                {
                    bounceIntensity = value;

                    switch (bounceIntensity)
                    {
                        case ThrowableBounceIntensity.Low:
                            collisionBounces[0] = new Int32Range(4, 8);
                            collisionBounces[1] = new Int32Range(1, 3);
                            noCollisionBounce = new Int32Range(3, 7);
                            break;

                        case ThrowableBounceIntensity.Medium:
                            collisionBounces[0] = new Int32Range(8, 16);
                            collisionBounces[1] = new Int32Range(2, 4);
                            noCollisionBounce = new Int32Range(3, 11);
                            break;

                        case ThrowableBounceIntensity.High:
                            collisionBounces[0] = new Int32Range(12, 22);
                            collisionBounces[1] = new Int32Range(4, 8);
                            noCollisionBounce = new Int32Range(5, 14);
                            break;
                    }
                }
            }
        }

        // FloorPosition
        protected Vector2 FloorPosition { get; private set; }

        // LaunchPosition
        protected Vector2 LaunchPosition { get; private set; }

        // OnComplete
        protected virtual void OnComplete(GameThing? target)
        {
        }

        // OnDrawShadow
        protected override void OnDrawShadow(GameTime gameTime) => Shadow.Draw(gameTime);

        // OnUnload
        protected override void OnUnload()
        {
            base.OnUnload();
            if (scaleTween.IsRunning)
                ReturnToOwner();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (instantBounce)
                DepthOffset = (int)(FloorPosition.Y - Y);

            if (Shadow.Image != null)
            {
                Shadow.X = X;
                Shadow.Y = FloorPosition.Y + 2;
            }

            base.OnUpdate(gameTime);

            // Collect phase
            if (collectCooldown >= 0)
            {
                UpdateCollectState(gameTime);
                return;
            }

            if (IsActive)
                CheckComplete();

            if (bounces > 0)
            {
                if (!xBounceTween.IsRunning && !yBounceTween.IsRunning)
                {
                    if (targetHit)
                    {
                        bounces--;
                        if (bounces == 0)
                            EndBounce();
                        else
                            SecondBounce();
                    }
                    else
                    {
                        bounces--;
                        if (bounces == 0)
                            EndBounce();
                        else
                            Bounce();
                    }

                }
            }
        }

        // Shadow
        protected ImageSprite Shadow { get; }

        #endregion

        // Duration
        public int Duration { get; }

        // ImpactSound
        public Sound? ImpactSound { get; set; }

        // IsActive
        public bool IsActive { get; private set; }

        // Item
        public Item? Item { get; private set; }

        // Launch
        public void Launch(Item item)
        {
            if (item.Owner.Room == null)
                return;

            this.IsActive = true;
            this.Item = item;
            this.LaunchPosition = item.Owner.GetThrowableSpawnPosition();
            this.Position = LaunchPosition;
            this.Effects = item.Owner.Effects;
            this.FloorPosition = item.Owner.Position;
            this.Rotation = 0;
            this.RotationSpeed = 20;
            this.Shadow.Scale = Vector2.One;
            this.collectCooldown = -1;
            this.Scale = Vector2.One;
            this.isFlipped = IsFlippedHorizontally;
            this.outOfBounce = false;
            this.ignoreThing = null;

            if (isFlipped)
                RotationSpeed *= -1;

            item.Owner.Room.Children.Add(this);

            var offset = MaxDistance;
            if (isFlipped)
                offset *= -1;

            targetHit = false;
            bounces = 0;
            xBounceTween.Stop();
            yBounceTween.Stop();
            scaleTween.Stop();

            xTween.Start(TweenStyle.Linear, LaunchPosition.X, LaunchPosition.X + offset, Duration);
            Tweens.XTween = xTween;

            yTween.Start(TweenStyle.CubicIn, LaunchPosition.Y, FloorPosition.Y, Duration);
            Tweens.YTween = yTween;

            instantBounce = false;
            ignoreThing = CheckCollision();
            var y = float.MinValue;
            if (ignoreThing is IHoleArea holeArea)
            {
                testPoly.SetVertices(holeArea.Polygon.GetVertices(), -2);

                for (var i = 0; i < testPoly.Vertices.Count; i++)
                {
                    if (item.Owner.IsFlippedHorizontally)
                    {
                        if (testPoly.Vertices[i].X > item.Owner.X)
                            continue;
                    }
                    else if (testPoly.Vertices[i].X < item.Owner.X)
                        continue;

                    if (testPoly.Vertices[i].Y > y)
                        y = testPoly.Vertices[i].Y;
                }

                if (item.Owner.Y < y)
                {
                    ignoreThing = null;
                    instantBounce = true;
                }
            }

            item.Use();
        }

        // MaxDistance
        public float MaxDistance { get; }

        // Reusable
        public bool Reusable { get; }
    }
}
