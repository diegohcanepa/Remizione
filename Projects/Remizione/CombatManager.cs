using Microsoft.Xna.Framework;
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

            do
            {
                currentIndex++;
                if (currentIndex >= actors.Count)
                    currentIndex = 0;

                // Si dimos toda la vuelta sin encontrar un actor vivo
                if (currentIndex == startingIndex)
                {
                    Terminate();
                    return;
                }

            } while (CurrentActor != null && CurrentActor.IsDead);

            if (CurrentActor != null)
            {
                if (CurrentActor.IsPlayer)
                {
                    session.HUD.NarrationText = "Take your action";
                }
                else
                {
                    CurrentActor.PlayCombatTurn();
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

        // Remove
        public void Remove(Actor actor)
        {
            if (!actors.Contains(actor))
                return;

            int actorIndex = actors.IndexOf(actor);
            actors.RemoveAt(actorIndex);

            if (actors.Count <= 1)
            {
                Terminate();
                return;
            }

            if (actorIndex <= currentIndex && currentIndex > 0)
                currentIndex--;

            currentIndex %= actors.Count; // Por si acaso ajustar
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
            actors.Clear();
            currentIndex = -1;
        }

        // Update
        public void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;

            if (CurrentActor == null)
                return;

            if (!CurrentActor.IsPlayer)
                CurrentActor.PerformAICombatAction();
        }
    }
}
