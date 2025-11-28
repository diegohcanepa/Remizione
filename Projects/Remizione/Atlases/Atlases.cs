using Engendro;

namespace Remizione
{
    /// <summary>
    /// Atlases
    /// </summary>
    internal static class Atlases
    {
        #region Private fields

        private static bool isLoaded;

        #endregion

        #region Private members

        // Load
        private static void Load()
        {
            isLoaded = true;
            Actors = new Atlas(EngendroGame.Instance.Content, nameof(Actors), ContentHelper.EncodePath(ContentFolder.Atlases, nameof(Actors)), true);
            Environment = new EnvironmentAtlas(EngendroGame.Instance);
            Menu = new MenuAtlas(EngendroGame.Instance);
            UI = new UIAtlas(EngendroGame.Instance);
        }

        #endregion

        // Actors
        internal static Atlas Actors
        {
            get
            {
                if (!isLoaded)
                    Load();
                return field;
            }

            private set;
        } = null!;

        // Environment
        internal static EnvironmentAtlas Environment
        {
            get
            {
                if (!isLoaded)
                    Load();
                return field;
            }

            private set;
        } = null!;

        // Menu
        internal static MenuAtlas Menu
        {
            get
            {
                if (!isLoaded)
                    Load();
                return field;
            }

            private set;
        } = null!;

        // UI
        internal static UIAtlas UI
        {
            get
            {
                if (!isLoaded)
                    Load();
                return field;
            }

            private set;
        } = null!;
    }
}
