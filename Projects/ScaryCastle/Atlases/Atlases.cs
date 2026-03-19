using Engendro;

namespace ScaryCastle
{
    /// <summary>
    /// Atlases
    /// </summary>
    internal static class Atlases
    {
        private static bool isLoaded;

        #region Private members

        // Load
        private static void Load()
        {
            isLoaded = true;
            Actors = new Atlas(EngendroGame.Instance.Content, nameof(Actors), ContentManagerExtension.EncodePath(ContentFolder.Atlases, nameof(Actors)), true);
            Environment = new EnvironmentAtlas();
            Menu = new MenuAtlas();
            Props = new Atlas(EngendroGame.Instance.Content, nameof(Props), ContentManagerExtension.EncodePath(ContentFolder.Atlases, nameof(Props)), false);
            UI = new UIAtlas();
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

        // Props
        internal static Atlas Props
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
