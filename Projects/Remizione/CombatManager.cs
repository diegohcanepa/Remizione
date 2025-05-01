using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    public class CombatManager
    {
        private readonly List<Actor> turnList = [];
        private int currentIndex = -1;
        private readonly GameSession session;

        // Constructor
        public CombatManager(GameSession session)
        {
            this.session = session;
            this.TurnList = new(turnList);
        }

        // Terminate
        private void Terminate()
        {
            IsActive = false;
            /*
            foreach (var actor in turnOrder)
            {
                actor.EndCombatTurn();
            }
            */

            turnList.Clear();
            currentIndex = -1;
        }

        // Add
        public void Add(Actor actor)
        {
            if (!turnList.Contains(actor))
            {
                turnList.Add(actor);

                if (!IsActive)
                {
                    IsActive = true;
                    AdvanceTurn();
                }
            }
        }

        // AdvanceTurn
        public void AdvanceTurn()
        {
            if (!IsActive)
                return;

            currentIndex++;
            if (currentIndex == turnList.Count)
                currentIndex = 0;

            if (CurrentActor is Actor currentActor && !currentActor.IsPlayer)
                currentActor.DoAttackTurn();
        }

        // CurrentActor
        public Actor? CurrentActor => currentIndex != -1 ? turnList[currentIndex] : null;

        // EndCurrentTurn
        public void EndCurrentTurn()
        {
            AdvanceTurn();
        }

        // IsActive
        public bool IsActive { get; private set; }

        // Remove
        public void Remove(Actor actor)
        {
            if (turnList.Contains(actor))
            {
                int removedIndex = turnList.IndexOf(actor);
                turnList.Remove(actor);

                if (removedIndex <= currentIndex && currentIndex > 0)
                    currentIndex--;

                if (turnList.Count == 0)
                    Terminate();
            }
        }

        // TurnList
        public ReadOnlyCollection<Actor> TurnList { get; }
    }
}
