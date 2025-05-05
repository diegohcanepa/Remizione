using Engendro.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    public class CombatManager
    {
        private int currentIndex = -1;
        private readonly GameSession session;
        private readonly List<Actor> turnList = [];

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
                if (TurnList.Count < 2)
                    session.HUD.MessageText = "Unleash your anger";
                else if (CurrentActor.IsPlayer)
                    session.HUD.MessageText = "Make your move";
                else
                    session.HUD.MessageText = "Wait the grace of God";

                CurrentActor.StartTurn();
            }
            else
                session.HUD.MessageText = string.Empty;
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

            session.HUD.MessageText = string.Empty;
        }

        // TurnList
        public ReadOnlyCollection<Actor> TurnList { get; }
    }
}
