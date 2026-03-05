namespace ScaryCastle
{
    /// <summary>
    /// HellGoat
    /// </summary>
    public sealed class HellGoat : Actor
    {
        // Constructor
        public HellGoat(GameSession session, string name)
            : base(session, name)
        {
            this.Guts = 8;
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
        }
    }
}
