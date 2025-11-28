namespace Engendro
{
    /// <summary>
    /// ImageSprite
    /// </summary>
    public class ImageSprite : Sprite
    {

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
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    InternalImage = value;
                }
            }
        }
    }
}
