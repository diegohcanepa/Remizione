using Engendro;

namespace Remizione
{
    /// <summary>
    /// ObjectPools
    /// </summary>
    public sealed class ObjectPools(GameSession session)
    {
        // Chilis
        public ObjectPool<ChiliThrowable> Chilis { get; } = new ObjectPool<ChiliThrowable>(() => new ChiliThrowable(session), 50);

        // Ducks
        public ObjectPool<DuckThrowable> Ducks { get; } = new ObjectPool<DuckThrowable>(() => new DuckThrowable(session), 50);

        // FloatingHeart
        public ObjectPool<FloatingHeart> FloatingHearts { get; } = new ObjectPool<FloatingHeart>(() => new FloatingHeart(session), 30);

        // FloatingText
        public ObjectPool<FloatingText> FloatingTexts { get; } = new ObjectPool<FloatingText>(() => new FloatingText(session), 30);

        // GetThrowable
        public Throwable? GetThrowable(string itemName)
        {
            if (itemName == "Chili")
                return Chilis.Get();

            if (itemName == "Duck")
                return Ducks.Get();

            return null;
        }

        // ReturnThrowable
        public void ReturnThrowable(Throwable throwable)
        {
            if (throwable is DuckThrowable duck)
                Ducks.Return(duck);

            if (throwable is ChiliThrowable chili)
                Chilis.Return(chili);
        }
    }
}
