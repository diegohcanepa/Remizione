namespace Remizione
{
    /// <summary>
    /// Enemy
    /// </summary>
    public class Enemy : Actor
    {
        // Constructor
        public Enemy(GameSession session, string name)
            : base(session, name)
        {
            IsEnemy = true;
        }

        #region Protected members

        // OnLoad
        protected override void OnLoad()
        {
            IsHostile = true;
            base.OnLoad();
        }

        #endregion

        // IsHostile
        public bool IsHostile { get; set; }
    }
}
