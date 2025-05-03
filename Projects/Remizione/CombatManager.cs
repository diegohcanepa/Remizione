using Engendro.Audio;
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

        // Add
        public void Add(Actor actor)
        {
            if (!turnList.Contains(actor))
            {
                turnList.Add(actor);

                if (turnList.Count > 0 && AudioManager.Music.CurrentTag != "Anger")
                    AudioManager.Music.PlayTag("Anger", 1000);
            }
        }

        // AdvanceTurn
        public void AdvanceTurn()
        {
            if (!IsActive || turnList.Count == 0)
                return;

            currentIndex++;
            if (currentIndex == turnList.Count)
                currentIndex = 0;

            if (CurrentActor != null)
            {
                if (CurrentActor.IsPlayer && turnList.Count > 1)
                    session.HUD.NarrationText = "Make your move";
                else
                    session.HUD.NarrationText = string.Empty;

                CurrentActor.StartTurn();
            }
            else
                session.HUD.NarrationText = string.Empty;
        }

        // CurrentActor
        public Actor? CurrentActor => currentIndex != -1 ? turnList[currentIndex] : null;

        // EndCurrentTurn
        public void EndCurrentTurn() => AdvanceTurn();

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

        // Start
        public void Start(params Actor[] actors)
        {
            if (IsActive)
                return;

            IsActive = true;
            turnList.Clear();
            turnList.AddRange(actors);
            AdvanceTurn();
        }

        // Terminate
        public void Terminate()
        {
            IsActive = false;
            turnList.Clear();
            currentIndex = -1;

            foreach (var actor in turnList)
            {
                actor.EndTurn();
            }

            AudioManager.Music.CurrentTag = string.Empty;
            AudioManager.Music.Stop(5000);
        }

        // TurnList
        public ReadOnlyCollection<Actor> TurnList { get; }
    }
}
