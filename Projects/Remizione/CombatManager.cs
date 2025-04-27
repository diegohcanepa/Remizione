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
        private int playerTurnCooldown;
        private readonly GameSession session;

        // Constructor
        public CombatManager(GameSession session)
        {
            this.session = session;
            IsActive = false;
        }

        #region Private members

        // AdvanceTurn
        private void AdvanceTurn()
        {
            currentIndex++;

            if (currentIndex >= actors.Count)
                currentIndex = 0;

            if (CurrentActor != null)
            {
                if (CurrentActor.IsPlayer)
                    playerTurnCooldown = 5000;
                else
                    session.HUD.NarrationText = $"Wait the grace of God...";
            }
        }

        // CleanUp
        private void CleanUp()
        {
            for (int i = actors.Count - 1; i >= 0; i--)
            {
                if (actors[i].IsDead || !
                    actors[i].InCurrentRoom)
                    Remove(actors[i]);
            }
        }

        #endregion

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

        // BeginTurn
        public void BeginTurn()
        {
            if (CurrentActor == null)
                return;

            if (IsTurnInProgress)
                throw new InvalidOperationException("Turn already in progress.");

            IsTurnInProgress = true;
        }

        // CurrentActor
        public Actor? CurrentActor => IsActive ? actors[currentIndex] : null;

        // EndTurn
        public void EndTurn()
        {
            if (!IsTurnInProgress)
                throw new InvalidOperationException("No turn in progress.");
         
            IsTurnInProgress = false;
            
            AdvanceTurn();
        }

        // IsActive
        public bool IsActive { get; private set; }

        // IsTurnInProgress
        public bool IsTurnInProgress { get; private set; }

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
            IsActive = false;
            actors.Clear();
            currentIndex = -1;
            IsTurnInProgress = false;
        }

        // Update
        public void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;

            CleanUp();

            if (CurrentActor == null)
                return;

            if (CurrentActor.IsPlayer)
            {
                playerTurnCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                if (playerTurnCooldown <= 0)
                    AdvanceTurn();
                else
                    session.HUD.NarrationText = $"Your turn: {playerTurnCooldown / 1000}s";
            }
            else
                CurrentActor.PerformAICombatAction();
        }
    }
}
