using Microsoft.Xna.Framework;
using System;

namespace Engendro
{
    /// <summary>
    /// SpriteFrame
    /// </summary>
    public sealed class SpriteFrame
    {
        // Constructor
        public SpriteFrame(SpriteAnimation animation, int index, string imageName, int duration, bool isEvent, string label, float speedFactor, string soundName, Rectangle subArea, bool footstep, string gotoLabel)
        {
            this.Animation = animation;
            this.ImageName = imageName;
            this.Duration = Math.Max(0, duration);
            this.IsEvent = isEvent;
            this.Label = label;
            this.Index = index;
            this.SpeedFactor = speedFactor;
            this.SoundName = soundName;
            this.Footstep = footstep;
            this.GotoLabel = gotoLabel;
            this.SubArea = subArea;

            InvalidateImage();
        }

        #region Internal members

        // InvalidateImage
        public void InvalidateImage()
        {
            Image = Animation.Sprite.Atlas?.GetImage(Animation.Sprite.ImagePath + ImageName);
        }

        #endregion

        // Animation
        public SpriteAnimation Animation { get; }

        // Duration
        public int Duration { get; }

        // Footstep
        public bool Footstep { get; }

        // GotoLabel
        public string GotoLabel { get; }

        // Image
        public AtlasImage? Image { get; private set; }

        // ImageName
        public string ImageName { get; }

        // Index
        public int Index { get; }

        // IsEvent
        public bool IsEvent { get; }

        // IsFirstFrame
        public bool IsFirstFrame => Index == 0;

        // IsLastFrame
        public bool IsLastFrame => Index == Animation.FrameCount - 1;

        // Label
        public string Label { get; }

        // SoundName
        public string SoundName { get; }

        // SubArea
        public Rectangle SubArea { get; }

        // SpeedFactor
        public float SpeedFactor { get; }
    }
}
