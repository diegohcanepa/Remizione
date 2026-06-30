using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// SpearTrap
    /// </summary>
    public class SpearTrap : Prop
    {
        private enum SpearState { Prepared, Reloading, Attacking, Up }

        private const string PreparedAnimationName = "Prepared";
        private const string ReloadAnimationName = "Reload";
        private const string AttackAnimationName = "Attack";

        private int attackCooldown;
        private int attackTimer;
        private bool damageApplied;
        private int upCooldown;
        private SpearState state;

        // Constructor
        public SpearTrap(GameSession session, string name)
            : base(session, name)
        {
            AffectsPathfinding = false;
            Atlas = Atlases.Props;
            CollisionDetection = false;
            IgnoreWalkArea = false;
            DepthOffset = -10;
            attackCooldown = Random.Shared.Next(2500, 4500);

            var animation = AddAnimation(PreparedAnimationName);
            animation.AddFrame("SpearTrap01", 1000);

            animation = AddAnimation(AttackAnimationName);
            animation.AddFrameSequence("SpearTrap", 20, 2, 6);

            animation = AddAnimation(ReloadAnimationName);
            animation.AddFrame("SpearTrap06", 20);
            animation.AddFrame("SpearTrap05", 20);
            animation.AddFrame("SpearTrap04", 20);
            animation.AddFrame("SpearTrap03", 20);
            animation.AddFrame("SpearTrap02", 20);
            animation.AddFrame("SpearTrap01", 20);
        }

        #region Private members

        // ApplyDamage
        private void ApplyDamage()
        {
            if (Room == null || Session.IsAwaiting || Definition == null)
                return;

            for (var i = 0; i < Room.Children.Count; i++)
            {
                if (Room.Children[i] == this)
                    continue;

                if (Room.Children[i] is GameThing target && !target.IsDead && target.MaxHP > 0)
                {
                    if (RuntimeCollider.BoundingRectangleF.Bottom >= target.Y && RuntimeCollider.BoundingRectangleF.Intersects(target.RuntimeHotspot.BoundingRectangleF))
                        EffectDescriptor.Apply(Definition.EffectDescriptors, this, target, EffectContext.Contact);
                }
            }
        }

        // Attack
        private void Attack()
        {
            state = SpearState.Attacking;
            AnimationPlayer.Play(AttackAnimationName, false);

            if (IsInViewport)
                PlaySound(SoundNames.SpearTrap);
        }

        // Prepare
        private void Prepare()
        {
            damageApplied = false;
            state = SpearState.Prepared;
            AnimationPlayer.Play(PreparedAnimationName, true);
            ResetCooldown();
        }

        // ResetCooldown
        private void ResetCooldown()
        {
            attackTimer = attackCooldown;
        }

        #endregion

        #region Protected members

        // OnActivate
        protected override void OnActivate()
        {
            base.OnActivate();
            attackCooldown = Random.Shared.Next(2500, 4500);
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            Prepare();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            // Prepared
            if (state == SpearState.Prepared)
            {
                if (!Session.IsAwaiting && attackTimer >= 0)
                {
                    attackTimer -= gameTime.ElapsedGameTime.Milliseconds;
                    if (attackTimer <= 0)
                        Attack();
                }
            }

            // Attacking
            else if (state == SpearState.Attacking)
            {
                if (!AnimationPlayer.IsPlaying)
                {
                    state = SpearState.Up;
                    upCooldown = 1000;
                }
                else if (!damageApplied)
                {
                    damageApplied = true;
                    ApplyDamage();
                }
            }

            // Up
            else if (state == SpearState.Up)
            {
                if (upCooldown > 0)
                {
                    upCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                    if (upCooldown <= 0)
                    {
                        state = SpearState.Reloading;
                        AnimationPlayer.Play(ReloadAnimationName, false);
                    }
                }
            }

            // Reloading
            else if (state == SpearState.Reloading)
            {
                if (!AnimationPlayer.IsPlaying)
                    Prepare();
            }

            base.OnUpdate(gameTime);
        }

        #endregion
    }
}
