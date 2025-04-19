using Engendro;

namespace Remizione
{
    /// <summary>
    /// Atlases
    /// </summary>
    internal static class Atlases
    {
        #region Private fields

        private static Atlas actors = null!;
        private static EnvironmentAtlas environment = null!;
        private static bool isLoaded;
        private static MenuAtlas menu = null!;
        private static UIAtlas ui = null!;

        #endregion

        #region Private members

        // Load
        private static void Load()
        {
            isLoaded = true;
            actors = new Atlas(EngendroGame.Instance.Content, nameof(Actors), ContentHelper.EncodePath(ContentFolder.Atlases, nameof(Actors)), true);
            environment = new EnvironmentAtlas(EngendroGame.Instance);
            menu = new MenuAtlas(EngendroGame.Instance);
            ui = new UIAtlas(EngendroGame.Instance);
        }

        #endregion

        // Actors
        internal static Atlas Actors
        {
            get
            {
                if (!isLoaded)
                    Load();
                return actors;
            }
        }

        // Environment
        internal static EnvironmentAtlas Environment
        {
            get
            {
                if (!isLoaded)
                    Load();
                return environment;
            }
        }

        // Menu
        internal static MenuAtlas Menu
        {
            get
            {
                if (!isLoaded)
                    Load();
                return menu;
            }
        }

        // UI
        internal static UIAtlas UI
        {
            get
            {
                if (!isLoaded)
                    Load();
                return ui;
            }
        }
    }
}
