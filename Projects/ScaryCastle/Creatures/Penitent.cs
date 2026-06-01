using Engendro.Audio;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// Penitent
    /// </summary>
    public sealed class Penitent : Actor
    {
        private int cooldown;
        private SoundInstance? activeSound;

        // Constructor
        public Penitent(GameSession session, string name)
            : base(session, name)
        {
        }

        // ResetCooldown
        private void ResetCooldown()
        {
            cooldown = Random.Shared.Next(5000, 10000);
        }

        // OnActivate
        protected override void OnActivate()
        {
            base.OnActivate();
            ResetCooldown();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (activeSound == null)
            {
                if (cooldown >= 0)
                {
                    cooldown -= gameTime.ElapsedGameTime.Milliseconds;
                    if (cooldown <= 0)
                    {
                        PlaySound(SoundNames.Penitent);
                        ResetCooldown();
                    }
                }
            }
            else if (!activeSound.IsPlaying)
            {
                activeSound = null;
            }
        }
    }
}
