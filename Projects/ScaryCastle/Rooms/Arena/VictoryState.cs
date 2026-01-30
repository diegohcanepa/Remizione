namespace ScaryCastle
{
    /// <summary>
    /// VictoryState
    /// </summary>
    public sealed class VictoryState(Arena arena) : ArenaState(arena)
    {
        public override void Enter()
        {
            // Lógica de victoria (volver al mapa, loot)
            // Context.Session.GoBackToPreviousRoom();
        }
    }
}
