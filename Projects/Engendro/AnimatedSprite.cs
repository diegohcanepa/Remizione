using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Engendro
{
    /// <summary>
    /// AnimatedSprite
    /// </summary>
    public class AnimatedSprite : Sprite
    {
        #region Private fields

        private readonly NamedObjectCollection<SpriteAnimation> animations = [];
        private Atlas? atlas;
        private string defaultImageName = string.Empty;
        private string imagePath = string.Empty;

        #endregion

        #region Constructors

        // Constructor
        public AnimatedSprite(EngendroGame game)
            : base(game)
        {
            this.Animations = new NamedObjectReadOnlyCollection<SpriteAnimation>(animations);
            this.Player = new SpriteAnimationPlayer(this);
        }

        #endregion

        #region Private members

        // AddAnimationCore
        private SpriteAnimation AddAnimationCore(string name, string? imageName)
        {
            CodeContract.ValidName(name, nameof(name));

            if (animations.Contains(name))
            {
                CodeContract.ThrowDuplicatedNameException(nameof(name));
            }

            SpriteAnimation result = new(this, name);
            animations.Add(result);

            if (!string.IsNullOrWhiteSpace(imageName))
            {
                result.AddFrame(imageName, 10000);
                InvalidateInternalImage();
            }

            if (animations.Count == 1)
            {
                Player.Animation = result;
            }

            return result;
        }

        // InvalidateAnimationFrames
        private void InvalidateAnimationFrames()
        {
            for (var i = 0; i < animations.Count; i++)
            {
                animations[i].InvalidateFrames();
            }

            InvalidateInternalImage();
        }

        #endregion

        #region Protected members

        // GetSpeedFactor
        protected override float GetSpeedFactor()
        {
            return Player.Frame == null ? base.GetSpeedFactor() : Player.Frame.SpeedFactor;
        }

        // OnAtlasChanged
        protected virtual void OnAtlasChanged()
        {
        }

        // OnImagePathChanged
        protected virtual void OnImagePathChanged()
        {
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            Player.Update(gameTime);
        }

        #endregion

        #region Internal members

        // InvalidateInternalImage
        internal void InvalidateInternalImage()
        {
            if (Player.Frame == null)
            {
                InternalImage = string.IsNullOrWhiteSpace(DefaultImageName) ? null : Atlas?.GetImage(ImagePath + DefaultImageName);
            }
            else
            {
                InternalImage = Atlas?.GetImage(ImagePath + Player.Frame.ImageName);
            }
        }

        #endregion

        // AddAnimation
        public SpriteAnimation AddAnimation(string name)
        {
            return AddAnimationCore(name, null);
        }

        // AddAnimation
        public SpriteAnimation AddAnimation(string name, IList<AtlasImage> images, int duration)
        {
            var result = AddAnimationCore(name, null);

            for (var i = 0; i < images.Count; i++)
            {
                result.AddFrame(images[i].Name, duration);
            }

            return result;
        }

        // Animations
        public NamedObjectReadOnlyCollection<SpriteAnimation> Animations { get; }

        // Atlas
        public Atlas? Atlas
        {
            get => atlas;
            set
            {
                if (value != atlas)
                {
                    atlas = value;
                    InvalidateAnimationFrames();
                    OnAtlasChanged();
                }
            }
        }

        // ClearAnimations
        public void ClearAnimations()
        {
            animations.Clear();
            Player.Animation = null;
            InvalidateInternalImage();
        }

        // DefaultImageName
        public string DefaultImageName
        {
            get => defaultImageName;
            set
            {
                if (value != defaultImageName)
                {
                    this.defaultImageName = value;
                    InvalidateInternalImage();
                }
            }
        }

        // Direction
        public AnimationDirection Direction { get; private set; }

        // ImagePath
        public string ImagePath
        {
            get => imagePath;
            set
            {
                if (value != imagePath)
                {
                    imagePath = value;
                    InvalidateAnimationFrames();
                    OnImagePathChanged();
                }
            }
        }

        // Player
        public SpriteAnimationPlayer Player { get; }
    }
}
