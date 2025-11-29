namespace Remizione
{
    /// <summary>
    /// Enemy
    /// </summary>
    public class Enemy : Actor
    {
        private readonly EnemyConfig? config;

        // Constructor
        public Enemy(GameSession session, string name)
            : base(session, name)
        {
            AllowInteraction = false;
            ContactDamageType = DamageType.Physical;
            Faction = Faction.Evil;
            PowerBonus = 1;

            config = EnemyConfig.GetConfig(StaticName);
        }

        #region Protected members

        // OnDie
        protected override void OnDie()
        {
            base.OnDie();
            
            if (config?.KillGoal > 0)
                Session.KillCounter.Increment(StaticName);
        }

        #endregion
    }
}
