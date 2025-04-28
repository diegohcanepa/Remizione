using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Engendro
{
    /// <summary>
    /// SpriteAnimation
    /// </summary>
    public sealed class SpriteAnimation : INamedObject
    {
        private readonly List<SpriteFrame> frameList = [];

        #region Constructor

        // Constructor
        internal SpriteAnimation(AnimatedSprite sprite, string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                CodeContract.ValidName(name, nameof(name));
            }

            this.Sprite = sprite;
            this.Name = name;
        }

        #endregion

        #region Private members

        // Invalidate
        private void Invalidate()
        {
            Duration = 0;

            for (var i = 0; i < frameList.Count; i++)
            {
                Duration += frameList[i].Duration;
            }
        }

        #endregion

        #region Internal members

        // InvalidateFrames
        internal void InvalidateFrames()
        {
            for (var i = 0; i < frameList.Count; i++)
            {
                frameList[i].InvalidateImage();
            }
        }

        #endregion

        // AddFrame
        public SpriteFrame AddFrame(string imageName, int duration)
        {
            return AddFrame(imageName, duration, string.Empty, 1, string.Empty);
        }

        // AddFrame
        public SpriteFrame AddFrame(string imageName, int duration, string label)
        {
            return AddFrame(imageName, duration, label, 1, string.Empty);
        }

        // AddFrame
        public SpriteFrame AddFrame(string imageName, int duration, string label, float speedFactor, string soundName)
        {
            SpriteFrame result = new(this, frameList.Count, imageName, duration, label, speedFactor, soundName);
            frameList.Add(result);
            Invalidate();

            if (Sprite.Player.Animation == this && frameList.Count == 1)
                Sprite.Player.GoTo(FramePosition.First);

            return result;
        }

        // AddFrameSequence
        public SpriteFrame[] AddFrameSequence(string imageName, int duration, int start, int end)
        {
            return AddFrameSequence(imageName, duration, start, end, "00");
        }

        // AddFrameSequence
        public SpriteFrame[] AddFrameSequence(string imageName, int duration, int start, int end, string format)
        {
            CodeContract.NotEmpty(imageName, nameof(imageName));
            CodeContract.EqualOrGreaterThanZero(duration, nameof(duration));

            ArgumentOutOfRangeException.ThrowIfGreaterThan(start, end);

            SpriteFrame[] result = new SpriteFrame[end - start + 1];

            for (var i = 0; i < result.Length; i++)
            {
                result[i] = AddFrame(imageName + (start + i).ToString(format, CultureInfo.InvariantCulture), duration);
            }

            Invalidate();

            return result;
        }

        // Duration
        public int Duration { get; private set; }

        // FrameCount
        public int FrameCount => frameList.Count;

        // GetFrame
        public SpriteFrame GetFrame(int index) => frameList[index];

        // GetFrames
        public SpriteFrame[] GetFrames() => frameList.ToArray();

        // Name
        public string Name { get; }

        // Sprite
        public AnimatedSprite Sprite { get; }

        // ToString
        public override string ToString()
        {
            return Name;
        }
    }
}
