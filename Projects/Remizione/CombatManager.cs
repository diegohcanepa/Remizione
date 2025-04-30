using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Remizione
{
    public class CombatManager
    {
        private readonly List<Actor> turnOrder = [];
        private int currentIndex = 0;
        private readonly GameSession session;

        // Constructor
        public CombatManager(GameSession session)
        {
            this.session = session;
        }

        // Add
        public void Add(Actor actor)
        {
            if (!turnOrder.Contains(actor))
                turnOrder.Add(actor);
        }

        // AdvanceTurn
        public void AdvanceTurn()
        {
            if (!IsActive)
                return;

            for (int i = 0; i < turnOrder.Count; i++)
            {
                currentIndex = (currentIndex + 1) % turnOrder.Count;
                var next = turnOrder[currentIndex];
                if (!next.IsDead)
                {
                    next.StartCombatTurn();
                    return;
                }
            }

            // Si ninguno está vivo
            Terminate();
        }

        // CurrentActor
        public Actor? CurrentActor => (turnOrder.Count > 0 && currentIndex < turnOrder.Count) ? turnOrder[currentIndex] : null;

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
            if (turnOrder.Contains(actor))
            {
                int removedIndex = turnOrder.IndexOf(actor);
                turnOrder.Remove(actor);

                if (removedIndex <= currentIndex && currentIndex > 0)
                    currentIndex--;

                if (turnOrder.Count == 0)
                    Terminate();
            }
        }

        // Start
        public void Start(params Actor[] participants)
        {
            turnOrder.Clear();
            turnOrder.AddRange(participants);
            currentIndex = 0;
            IsActive = true;

            AdvanceTurn();
        }

        // Terminate
        public void Terminate()
        {
            IsActive = false;
            foreach (var actor in turnOrder)
            {
                actor.EndCombatTurn();
            }

            turnOrder.Clear();
            currentIndex = 0;
        }
    }
}
