namespace Engendro
{
    /// <summary>
    /// ImageSprite
    /// </summary>
    public class ImageSprite : Sprite
    {
        private AtlasImage? image;

        // Constructor
        public ImageSprite(EngendroGame game)
            : this(game, null)
        {
        }

        // Constructor
        public ImageSprite(EngendroGame game, AtlasImage? image)
            : base(game)
        {
            this.Image = image;
        }

        // Image
        public AtlasImage? Image
        {
            get => image;
            set
            {
                if (value != image)
                {
                    this.image = value;
                    InternalImage = value;
                }
            }
        }
    }
}
