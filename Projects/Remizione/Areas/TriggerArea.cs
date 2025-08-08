using Adberration;
using Adberration.Scripting;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// TriggerArea
    /// </summary>
    public sealed class TriggerArea : Room.Area
    {
        private bool triggered;

        // Constructor
        public TriggerArea(Room room, string name, Script routine, Script? exitRoutine, bool await, bool stopPlayer, bool once, FlagCondition? condition, params Vector2[] vertices)
            : base(room, name, condition, vertices)
        {
            this.Routine = routine;
            this.ExitRoutine = exitRoutine;
            this.Await = await;
            this.StopPlayer = stopPlayer;
            this.Once = once;

            if (routine.ScriptType != ScriptType.Routine)
                throw new ArgumentException("The supplied script is not a routine.", nameof(routine));
        }

        #region Protected members

        // OnReset
        protected override void OnReset() => triggered = false;

        #endregion

        // Await
        public bool Await { get; }

        // ExitRoutine
        public Script? ExitRoutine { get; }

        // IsActive
        public bool IsActive { get; private set; }

        // Once
        public bool Once { get; }

        // Routine
        public Script Routine { get; }

        // StopPlayer
        public bool StopPlayer { get; }

        // Trigger
        public void Trigger(Actor actor)
        {
            if (Once && triggered)
                return;

            if (!Test())
                return;

            if (IsActive)
            {
                if (!Polygon.Contains(actor.Position))
                {
                    IsActive = false;

                    if (ExitRoutine != null)
                    {
                        if (Await)
                        {
                            if (StopPlayer)
                                actor.StopMoving();
                            Room.Session.AwaitScript(ExitRoutine);
                        }
                        else
                            Room.Session.ScriptProcessor.StartScript(ExitRoutine);
                    }
                }
            }
            else
            {
                if (Polygon.Contains(actor.Position))
                {
                    if (Await)
                    {
                        if (StopPlayer)
                            actor.StopMoving();
                        Room.Session.AwaitScript(Routine);
                    }
                    else
                        Room.Session.ScriptProcessor.StartScript(Routine);

                    if (Once)
                        triggered = true;

                    IsActive = true;
                }
            }
        }
    }
}