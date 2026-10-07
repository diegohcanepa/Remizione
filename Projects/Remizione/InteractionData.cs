using Adberration;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Remizione.InteractionCommands;
using System;

namespace Remizione
{
    /// <summary>
    /// InteractionData
    /// </summary>
    public sealed class InteractionData
    {
        #region Private fields

        private InteractionCommand? activeCommand;
        private readonly CombatCommand combatCommand = new();
        private readonly InteractionCommand[] commandChain;
        private readonly HoldCommand holdCommand = new();
        private readonly ItemCommand itemCommand = new();
        private readonly ScriptOutcomeCommand scriptCommand = new();
        private readonly ThrowHeldPropCommand throwCommand = new();

        #endregion

        // Constructor
        public InteractionData()
        {
            this.commandChain = [throwCommand, combatCommand, holdCommand, scriptCommand, itemCommand];
        }

        #region Private members

        // ApproachAndExecute
        private void ApproachAndExecute(Actor player, GameThing target)
        {
            AttackRange range = GetCurrentAttackRange(player);
            Vector2 destination;

            // Solo si requiere cálculo especial de tiro a distancia (Medium / Long)
            if (range is AttackRange.Medium or AttackRange.Long)
            {
                var (minX, maxX) = range.GetHorzRange();

                var rangedDestination = target.GetRangedApproachPosition(
                    player,
                    maxX,
                    minX,
                    GameSettings.YTolerance
                );

                if (!rangedDestination.HasValue)
                {
                    player.FaceTo(target);
                    Clear();
                    return;
                }

                destination = rangedDestination.Value;
            }
            // Para TODO lo demás (piñas, interacciones, props no arrojables, etc.)
            else
            {
                ApproachBehavior? behavior = this.IsAttack || player.HeldProp != null ? ApproachBehavior.ClosestSide : null;
                destination = target.GetApproachPosition(player, behavior);
            }

            var fastMove = this.IsAttack || Vector2.Distance(player.Position, destination) > GameSettings.WalkThreshold;
            var moveToResult = destination == Vector2.Zero ? MoveToResult.NoPath : player.MoveTo(destination, fastMove);

            if (destination != Vector2.Zero && moveToResult == MoveToResult.NoPath)
            {
                player.FaceTo(target);
                Clear();
            }
            else if (moveToResult == MoveToResult.LessThan1px || destination == Vector2.Zero)
            {
                // Medir cuánto se desplazó el target desde que se inició la orden
                float targetDisplacement = Vector2.Distance(target.Position, this.TargetPosition);

                // Si es un ataque de contacto y el target se movió de su posición original
                if (this.IsAttack && range == AttackRange.None && targetDisplacement > 5f)
                {
                    player.FaceTo(target);
                    Clear();
                    return;
                }

                ExecutePending(player);
            }
        }

        // GetCurrentAttackRange
        private AttackRange GetCurrentAttackRange(Actor player)
        {
            // 1. Props en mano: solo es Medium si es explícitamente arrojable
            if (player.HeldProp != null)
            {
                return player.HeldProp.Verb == Verb.Lift ? AttackRange.Medium : AttackRange.None;
            }

            // 2. Ítem equipado
            var heldItem = player.Session.InteractionContext.HeldItem;
            if (heldItem != null)
            {
                return heldItem.Definition.AttackRange;
            }

            // 3. Sin ítem ni prop (piña / interacción básica) -> Approch estándar
            return AttackRange.None;
        }

        // IsInThrowZone
        private bool IsInThrowZone(Actor player, GameThing target)
        {
            AttackRange range = GetCurrentAttackRange(player);

            if (range == AttackRange.None)
                return false;

            var (minX, maxX) = range.GetHorzRange();

            float deltaY = Math.Abs(player.Position.Y - target.Position.Y);
            float deltaX = Math.Abs(player.Position.X - target.Position.X);

            bool inYTolerance = deltaY <= GameSettings.YTolerance;
            bool inXRange = deltaX >= minX && deltaX <= maxX;

            if (!inYTolerance || !inXRange)
                return false;

            // Validación de línea de visión mediante tu método de Raycasting en el GameRoom
            return player.Room?.HasLineOfSight(player.Position, target.Position) == true;
        }

        // Prepare
        private void Prepare(Actor player, GameThing target, Verb verb)
        {
            Clear();

            if (player.HeldProp != null)
                verb = Verb.Attack;

            var heldItem = player.Session.InteractionContext.HeldItem;
            if (heldItem != null && !Utils.IsGoToVerb(verb))
            {
                if (player == target)
                {
                    if (heldItem.Definition.ActionKind is ActionKind.Projectile or ActionKind.Proximity)
                        return;
                }
                else if (heldItem.Definition.ActionKind == ActionKind.Self)
                {
                    return;
                }
            }

            this.Target = target;
            this.TargetPosition = target.Position;
            this.Verb = verb;

            for (int i = 0; i < commandChain.Length; i++)
            {
                if (commandChain[i].CanExecute(this, player, target, Verb))
                {
                    activeCommand = commandChain[i];
                    break;
                }
            }
        }

        #endregion

        // Clear
        public void Clear()
        {
            Target = null;
            TargetPosition = Vector2.Zero;
            Verb = Verb.None;
            activeCommand = null;
        }

        // ExecutePending
        public void ExecutePending(Actor player)
        {
            if (player.IsDead || Target == null || activeCommand == null)
                return;

            if (player.Session.State != GameSessionState.Idle)
                return;

            activeCommand.Execute(this, player, Target, Verb);

            Clear();
        }

        // IsAttack
        public bool IsAttack => activeCommand == combatCommand;

        // ProcessPrimaryAction
        public void ProcessPrimaryAction(Actor player)
        {
            if (player.IsDead)
                return;

            var context = player.Session.InteractionContext;

            // 1. Walk to (no target)
            if (context.Target == null || (context.Target is Actor actor && actor.HeldProp != null))
            {
                Clear();
                var destination = InputManager.DefaultPlayer.Mouse.WorldPosition(player.Session.Camera);
                var fastMove = Vector2.Distance(player.Position, destination) > GameSettings.WalkThreshold;
                player.MoveTo(destination, fastMove);
                return;
            }

            // 2. Prepare interaction
            Prepare(player, context.Target, context.Target.Verb);

            if (Target == null || activeCommand == null)
            {
                MouseCursor.Shake();
                return;
            }

            // Identificar si la acción actual involucra un ataque a distancia o lanzamiento
            AttackRange currentRange = GetCurrentAttackRange(player);
            bool isRangedAction = currentRange is AttackRange.Medium or AttackRange.Long;

            // In-place action?
            bool executeInPlace = (context.HeldItem == null && Target == player && player.HeldProp == null) ||
                                  (Verb == Verb.Examine && context.HeldItem == null && player.HeldProp == null) ||
                                  (context.HeldItem?.Definition.ActionKind == ActionKind.Self) ||
                                  (isRangedAction && IsInThrowZone(player, Target));

            if (executeInPlace)
                ExecutePending(player);
            else
                ApproachAndExecute(player, Target);
        }

        // ProcessSecondaryAction
        public void ProcessSecondaryAction(Actor player)
        {
            if (player.IsDead)
                return;

            var context = player.Session.InteractionContext;

            Clear();

            if (player.HeldProp != null)
            {
                player.ReleaseHeldProp();
            }
            else if (context.HeldItem == null)
            {
                context.HeldItem = player.Session.PlayerData.Inventory.Find(ItemNames.GadlingKnuckle);
            }
            else
            {
                player.StopMoving();
                context.HeldItem = null;
            }

            Sound.Play(SoundNames.Interact);
        }

        // Target
        public GameThing? Target { get; private set; }

        // TargetPosition
        public Vector2 TargetPosition { get; private set; }

        // Verb
        public Verb Verb { get; private set; }
    }
}