using Microsoft.Xna.Framework;
using System;

namespace Remizione
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

        private bool damageApplied;
        private int cooldown;
        private int cooldownInterval;
        private static readonly MetaItem metaItem = MetaItem.FindNotNull(nameof(SpearTrap));
        private int upCooldown;
        private SpearState state;

        // Constructor
        public SpearTrap(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Environment;
            CollisionDetection = false;
            DepthOffset = -5;
            GridMeasureType = GridMeasureType.Collider;
            IgnoreThrowables = true;

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
            cooldown = cooldownInterval;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            cooldownInterval = Random.Shared.Next(3500, 7000);
            Prepare();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            // Prepared
            if (state == SpearState.Prepared)
            {
                if (cooldown > 0)
                {
                    cooldown -= gameTime.ElapsedGameTime.Milliseconds;
                    if (cooldown <= 0)
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
                else if (!damageApplied && Session.Player != null && RuntimeHotspot.BoundingRectangleF.Intersects(Session.Player.RuntimeCollider.BoundingRectangleF))
                {
                    damageApplied = true;
                    metaItem.Effect.ApplyDamage(this, Session.Player);
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
