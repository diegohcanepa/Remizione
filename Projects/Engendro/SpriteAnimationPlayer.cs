using Engendro.Audio;
using Microsoft.Xna.Framework;
using System;

namespace Engendro
{
    /// <summary>
    /// SpriteAnimationPlayer
    /// </summary>
    public sealed class SpriteAnimationPlayer
    {
        #region Private fields

        private int frameCooldown;
        private int frameIndex = -1;
        private readonly AnimatedSprite sprite;

        #endregion

        #region Constructor

        // Constructor
        public SpriteAnimationPlayer(AnimatedSprite sprite)
        {
            this.sprite = sprite;
        }

        #endregion

        // Animation
        public SpriteAnimation? Animation
        {
            get;
            set
            {
                if (value != null)
                {
                    field = value;
                    Stop();
                    GoTo(FramePosition.First);
                }
            }
        }

        #region Private members

        // NextFrame
        private void NextFrame()
        {
            if (Animation == null || Animation.FrameCount == 0)
                return;

            if (frameIndex == -1)
            {
                if (Direction == AnimationDirection.Forward)
                    GoTo(FramePosition.First);
                else
                    GoTo(FramePosition.Last);

                return;
            }

            else if (Frame?.GotoLabel is string gotoLabel && gotoLabel.Length > 0)
            {
                GoTo(gotoLabel);
            }

            else if (Direction == AnimationDirection.Forward)
            {
                if (AtLastFrame)
                {
                    if (Loop)
                        GoTo(FramePosition.First);
                    else
                        IsPlaying = false;
                }
                else
                {
                    GoTo(FramePosition.Next);
                }
            }
            else
            {
                if (AtFirstFrame)
                {
                    if (Loop)
                        GoTo(FramePosition.Last);
                    else
                        IsPlaying = false;
                }
                else
                {
                    GoTo(FramePosition.Previous);
                }
            }
        }

        #endregion

        #region Internal members

        // Update
        internal void Update(GameTime gameTime)
        {
            if (Animation == null || !IsPlaying)
                return;

            if (frameCooldown > 0)
            {
                if (sprite.TimeScale == 1)
                    frameCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                else
                    frameCooldown -= (int)(gameTime.ElapsedGameTime.Milliseconds * sprite.TimeScale);
            }
            else
            {
                if (Animation.FrameCount > 1)
                    NextFrame();
                else
                    IsPlaying = false;
            }
        }

        #endregion

        // AtFirstFrame
        public bool AtFirstFrame => frameIndex == 0;

        // AtLastFrame
        public bool AtLastFrame => Animation != null && frameIndex == Animation.FrameCount - 1;

        // Direction
        public AnimationDirection Direction { get; private set; }

        // Frame
        public SpriteFrame? Frame => frameIndex < 0 || Animation == null ? null : Animation.GetFrame(frameIndex);

        // GoTo
        public bool GoTo(int frameIndex)
        {
            CodeContract.EqualOrGreaterThanZero(frameIndex, nameof(frameIndex));

            if (Animation == null || Animation.FrameCount == 0 || frameIndex >= Animation.FrameCount)
            {
                return false;
            }

            this.frameIndex = frameIndex;

            return true;
        }

        // GoTo
        public bool GoTo(FramePosition framePosition)
        {
            if (Animation == null || Animation.FrameCount == 0)
                return false;

            switch (framePosition)
            {
                // RandomFrame
                case FramePosition.Random:
                    frameIndex = Random.Shared.Next(0, Animation.FrameCount);
                    break;

                // FirstFrame
                case FramePosition.First:
                    frameIndex = 0;
                    break;

                // LastFrame
                case FramePosition.Last:
                    frameIndex = Animation.FrameCount - 1;
                    break;

                // NextFrame
                case FramePosition.Next:
                    if (!AtLastFrame)
                        frameIndex++;
                    break;

                // PreviousFrame
                case FramePosition.Previous:
                    if (!AtFirstFrame)
                        frameIndex--;
                    break;
            }

            frameCooldown = Frame == null ? 0 : Frame.Duration;

            sprite.InvalidateInternalImage();

            if (IsPlaying && Frame != null && !string.IsNullOrWhiteSpace(Frame.SoundName))
            {
                if (Sound.Find(Frame.SoundName)?.PopInstance() is SoundInstance soundInstance)
                {
                    soundInstance.Emitter = sprite.SoundEmitter;
                    soundInstance.Play();
                }
            }

            return true;
        }

        // GoTo
        public bool GoTo(string label)
        {
            if (Animation?.GetFrame(label) is SpriteFrame frame)
            {
                GoTo(frame.Index);
                return true;
            }
            else
                return false;
        }

        // IsPlaying
        public bool IsPlaying { get; private set; }

        // Loop
        public bool Loop { get; set; }

        // Play
        public SpriteAnimation? Play(string name)
        {
            return Play(name, true, AnimationDirection.Forward, false);
        }

        // Play
        public SpriteAnimation? Play(string name, bool loop)
        {
            return Play(name, loop, AnimationDirection.Forward, false);
        }

        // Play
        public SpriteAnimation? Play(string name, bool loop, AnimationDirection direction)
        {
            return Play(name, loop, direction, false);
        }

        // Play
        public SpriteAnimation? Play(string name, bool loop, AnimationDirection direction, bool randomFrame)
        {
            CodeContract.ValidName(name, nameof(name));

            if (sprite.Animations.Find(name) is SpriteAnimation animation)
            {
                this.Animation = animation;
                Play(loop, direction, randomFrame);
                return animation;
            }
            else
            {
                this.Animation = null;
                return null;
            }
        }

        // Play
        public bool Play(bool loop)
        {
            return Play(loop, AnimationDirection.Forward, false);
        }

        // Play
        public bool Play(bool loop, AnimationDirection direction, bool randomFrame)
        {
            if (Animation == null || Animation.FrameCount == 0)
                return false;

            Stop();

            Animation.InvalidateFrames();

            this.Loop = loop;
            this.Direction = direction;
            this.IsPlaying = true;
            this.frameIndex = -1;

            NextFrame();

            if (randomFrame)
            {
                GoTo(FramePosition.Random);
            }
            else
            {
                GoTo(Direction == AnimationDirection.Forward ? FramePosition.First : FramePosition.Last);
            }

            return true;
        }

        // Stop
        public void Stop()
        {
            IsPlaying = false;
            if (Animation == null)
            {
                frameIndex = -1;
                return;
            }

            frameIndex = Animation.FrameCount > 0 ? 0 : -1;
            frameCooldown = 0;
        }
    }
}