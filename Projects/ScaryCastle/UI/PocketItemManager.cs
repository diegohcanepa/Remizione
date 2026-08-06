using System;

namespace ScaryCastle
{
    /// <summary>
    /// PocketItemManager
    /// </summary>
    public sealed class PocketItemManager
    {
        // Constructor
        public PocketItemManager(GameSession session)
        {
            this.Session = session;
        }

        // BronzeKeys
        public int BronzeKeys
        {
            get;
            set
            {
                if (value != field)
                {
                    field = Math.Max(0, value);
                    Session.HUD.PocketItems.Refresh();
                }
            }
        }

        // Coins
        public int Coins
        {
            get;
            set
            {
                if (value != field)
                {
                    field = Math.Max(0, value);
                    Session.HUD.PocketItems.Refresh();
                }
            }
        }

        // GetCount
        public int GetCount(PocketItemType pocketItemType)
        {
            return pocketItemType switch
            {
                PocketItemType.BronzeKey => Session.PocketItemManager.BronzeKeys,
                PocketItemType.Coin => Session.PocketItemManager.Coins,
                PocketItemType.GoldenKey => Session.PocketItemManager.GoldenKeys,
                _ => throw new NotImplementedException(),
            };
        }

        // GoldenKeys
        public int GoldenKeys
        {
            get;
            set
            {
                if (value != field)
                {
                    field = Math.Max(0, value);
                    Session.HUD.PocketItems.Refresh();
                }
            }
        }

        // Reset
        public void Reset()
        {
            BronzeKeys = 0;
            Coins = 0;
            GoldenKeys = 0;
        }

        // Session
        public GameSession Session { get; }
    }
}
