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
                    Session.RunHUD?.PocketItems.Refresh();
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
                    Session.RunHUD?.PocketItems.Refresh();
                }
            }
        }

        // GetCount
        public int GetCount(PocketItemType pocketItemType)
        {
            if (Session.CurrentRun == null)
                return 0;

            return pocketItemType switch
            {
                PocketItemType.BronzeKey => Session.CurrentRun.PocketItems.BronzeKeys,
                PocketItemType.Coin => Session.CurrentRun.PocketItems.Coins,
                PocketItemType.GoldenKey => Session.CurrentRun.PocketItems.GoldenKeys,
                PocketItemType.TrapdoorKey => Session.CurrentRun.PocketItems.TrapdoorKeys,
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
                    Session.RunHUD?.PocketItems.Refresh();
                }
            }
        }

        // Reset
        public void Reset()
        {
            BronzeKeys = 0;
            Coins = 0;
            GoldenKeys = 0;
            TrapdoorKeys = 0;
        }

        // Session
        public GameSession Session { get; }

        // TrapdoorKeys
        public int TrapdoorKeys
        {
            get;
            set
            {
                if (value != field)
                {
                    field = Math.Max(0, value);
                    Session.RunHUD?.PocketItems.Refresh();
                }
            }
        }
    }
}
