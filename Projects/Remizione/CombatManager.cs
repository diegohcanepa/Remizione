using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// CombatManager
    /// </summary>
    public sealed class CombatManager
    {
        private readonly List<Actor> actors = [];
        private int currentIndex = -1;
        private readonly GameSession session;

        // Constructor
        public CombatManager(GameSession session)
        {
            this.session = session;
            IsActive = false;
        }

        // Add
        public void Add(Actor actor)
        {
            if (!actors.Contains(actor))
            {
                if (actors.Count == 0)
                {
                    Start(actor);
                    return;
                }
                else
                {
                    actors.Add(actor);
                    if (currentIndex == -1)
                        currentIndex = 0;
                }
            }
        }

        // AdvanceTurn
        public void AdvanceTurn()
        {
            int startingIndex = currentIndex;
            IsTurnInProgress = false;

            if (actors.Count < 2)
            {
                Terminate();
                return;
            }

            currentIndex++;
            if (currentIndex >= actors.Count)
                currentIndex = 0;

            if (CurrentActor != null)
            {
                if (CurrentActor.IsPlayer)
                    session.HUD.NarrationText = "Take your action";
                else
                {
                    CurrentActor.CombatTurnDone = false;
                    session.HUD.NarrationText = string.Empty;
                }
            }
        }

        // CurrentActor
        public Actor? CurrentActor => IsActive ? actors[currentIndex] : null;

        // IsActive
        public bool IsActive { get; private set; }

        // Contains
        public bool Contains(Actor actor) => actors.Contains(actor);

        // IsTurnInProgress
        public bool IsTurnInProgress { get; set; }

        // Remove
        public void Remove(Actor actor)
        {
            var index = actors.IndexOf(actor);
            if (index == -1)
                return;

            actors.RemoveAt(index);

            if (actors.Count <= 1)
            {
                Terminate();
                return;
            }

            currentIndex--;
            if (currentIndex < 0)
                currentIndex = actors.Count - 1;
        }

        // Start
        public void Start(params Actor[] members)
        {
            if (members.Length == 0)
                return;

            actors.Clear();
            actors.AddRange(members);
            currentIndex = 0;
            IsActive = true;
        }

        // Terminate
        public void Terminate()
        {
            session.HUD.NarrationText = string.Empty;
            IsActive = false;
            IsTurnInProgress = false;
            actors.Clear();
            currentIndex = -1;
        }
    }
}
