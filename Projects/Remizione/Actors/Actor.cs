using Engendro;
using Engendro.Audio;
using Engendro.Input;
using EngendroAdventure;
using EngendroAdventure.Scripting;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Remizione
{
    /// <summary>
    /// Actor
    /// </summary>
    public class Actor : GameThing, IInputHandler
    {
        #region Private fields

        private readonly FloatTween accelerationFactorTween = new();
        private BloodSplash? bloodSplash;
        private readonly CombatStateMachine combatStateMachine;
        private int faith;
        private int faithRecoveryCooldown;
        private FloatingText? floatingMessage;
        private SoundInstance? footstepSoundInstance;
        private readonly FloatTween headTween = new();
        private int level = 1;
        private int maxFaith;
        private readonly FloatTween moveTween = new();
        private GameThing? pendingInteractiveTarget;
        private readonly List<Vector2> pendingPathNodes = [];
        private PlayerNumber playerNumber = PlayerNumber.None;
        private readonly GameSession session;
        private SpeechBubble? speechBubble;
        private readonly ActorStandState standState;
        private int suspendInteractionCooldown;
        private readonly ActorThrowObjectState throwObjectState;
        private float tinyMoveSpeedFactor = 1;
        private Item? weapon;
        private string weaponName = string.Empty;

        #endregion

        #region Constructor

        // Constructor
        public Actor(GameSession session, string name)
            : base(session, name)
        {
            this.session = session;
            this.Stats = new Stats(this);

            this.Atlas = Atlases.Actors;
            this.IgnoreWalkArea = false;
            this.ShadowSpot = new ShadowSpot(this);

            this.Gifts = new ItemContainer(this, ItemContainerCategory.Gifts, Localization.GetValue(InGameMenuOptionName.Gifts));
            this.Prayers = new ItemContainer(this, ItemContainerCategory.Prayers, Localization.GetValue(InGameMenuOptionName.Prayers));
            this.standState = new ActorStandState(this);

            this.StateMachine = new ActorStateMachine(this, standState);
            this.StateMachine.RegisterState(new ActorCreateState(this));
            this.StateMachine.RegisterState(new ActorUseItemState(this));
            this.StateMachine.RegisterState(new ActorDeathState(this));
            this.StateMachine.RegisterState(new ActorHurtState(this));
            this.StateMachine.RegisterState(new ActorFatigueState(this));
            this.StateMachine.RegisterState(new ActorMoveState(this));
            this.StateMachine.RegisterState(new ActorMoveFastState(this));
            this.StateMachine.RegisterState(new ActorCloseAttackState(this));

            throwObjectState = new ActorThrowObjectState(this);
            this.StateMachine.RegisterState(throwObjectState);

            this.combatStateMachine = new(this);
        }

        #endregion

        #region Private members

        // FindGamePadTarget
        private GameThing? FindGamePadTarget()
        {
            if (Room != null)
            {
                for (int i = Room.CulledThings.Count - 1; i >= 0; i--)
                {
                    if (Room.CulledThings[i] == this)
                        continue;
                    else if (Room.CulledThings[i] is GameThing target && target.CanInteract(this))
                        return target;
                }
            }

            return null;
        }

        // FindInteractiveTarget
        private GameThing? FindInteractiveTarget()
        {
            if (session.IsAwaiting || SpeechBubble.ModalInstance != null || !session.IsCurrentScene)
                return null;

            return InputManager.DefaultPlayer.LastInputMethod == InputMethod.GamePad ? FindGamePadTarget() : FindMouseCursorTarget();
        }

        // FindMouseCursorTarget
        private GameThing? FindMouseCursorTarget()
        {
            if (Room != null)
            {
                var mousePos = InputManager.DefaultPlayer.Mouse.WorldPosition(Session.Camera);

                for (var i = Room.CulledThings.Count - 1; i >= 0; i--)
                {
                    if (Room.CulledThings[i] == Session.Player)
                        continue;

                    if (Room.CulledThings[i] is GameThing thing && thing.HotspotBox.Contains(mousePos))
                    {
                        if (Session.TargetMode)
                        {
                            if (thing.CanBeTargeted)
                                return thing;
                        }
                        else
                            return thing;
                    }
                }
            }

            return null;
        }

        // HandlePendingInteraction
        private void HandlePendingInteraction()
        {
            if (!IsPlayer)
                return;

            if (pendingInteractiveTarget != null)
            {
                FaceTo(pendingInteractiveTarget);

                if (pendingInteractiveTarget.GetVerbs() == null)
                    Interact(pendingInteractiveTarget);
                else
                    session.ShowContextMenu(pendingInteractiveTarget);
            }

            pendingInteractiveTarget = null;
        }

        // MoveToNextPathNode
        private void MoveToNextPathNode()
        {
            base.MoveTo(pendingPathNodes[0]);
            pendingPathNodes.RemoveAt(0);
        }

        // UpdateDirection
        private void UpdateDirection()
        {
            if (Velocity.X != 0)
            {
                if (Velocity.X < 0)
                    Sprite.FlipLeft();
                else
                    Sprite.FlipRight();
            }
        }

        // UpdateFaithRecovery
        private void UpdateFaithRecovery(GameTime gameTime)
        {
            if (faithRecoveryCooldown > 0)
            {
                faithRecoveryCooldown -= gameTime.ElapsedGameTime.Milliseconds;
            }
            else
            {
                faithRecoveryCooldown = Stats.GetFaithRecoveryInterval();
                Faith++;
            }
        }

        // UpdateFloatingMessage
        private void UpdateFloatingMessage(GameTime gameTime)
        {
            if (floatingMessage != null)
            {
                if (floatingMessage.IsVisible)
                    floatingMessage.Update(gameTime);
                else
                {
                    session.ObjectPools.FloatingTexts.Return(floatingMessage);
                    floatingMessage = null;
                }
            }
        }

        // UpdateFootstep
        private void UpdateFootstep()
        {
            if (Sprite.Player.Frame == null || !Sprite.Player.Frame.Footstep)
                return;

            if (Room is not ProceduralRoom room)
                return;

            if (footstepSoundInstance != null && footstepSoundInstance.IsPlaying)
                return;

            if (room.WorldManager.GetBlockFromScreen(Position) is WorldBlock worldBlock)
            {
                for (var i = 0; i < worldBlock.ProceduralThings.Count; i++)
                {
                    if (worldBlock.ProceduralThings[i] is Prop prop && prop.GetFootstepSound(Position) is Sound sound)
                    {
                        footstepSoundInstance = PlaySound(sound);
                        return;
                    }
                }
            }

            if (room.TerrainSound != null)
                footstepSoundInstance = PlaySound(room.TerrainSound);
        }

        #endregion

        #region Protected members

        // CalculateSpeed
        protected override float CalculateSpeed() => base.CalculateSpeed() * (FastMove ? FastMoveFactor : 1) * tinyMoveSpeedFactor * (accelerationFactorTween.IsRunning ? accelerationFactorTween.CurrentValue : 1);

        // InputHandler
        protected InputHandler? InputHandler { get; set; }

        // OnDamageReaction
        protected override void OnDamageReaction(GameThing attacker)
        {
            StopMoving();
            FaceTo(attacker);
        }

        // OnDeath
        protected override void OnDeath()
        {
            if (IsPlayer)
                session.CombatManager.Terminate();
            else
                session.CombatManager.Remove(this);

            StateMachine.ChangeState(ActorStateNames.Death);
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (moveTween.IsRunning)
                Y -= moveTween.CurrentValue;

            base.OnDraw(gameTime);

            bloodSplash?.Draw(gameTime);

            if (moveTween.IsRunning)
                Y += moveTween.CurrentValue;
        }

        // OnDrawShadow
        protected override void OnDrawShadow(GameTime gameTime)
        {
            base.OnDrawShadow(gameTime);
            ShadowSpot.Draw(gameTime);
        }

        // OnFaithChanged
        protected virtual void OnFaithChanged()
        {
        }

        // OnHurt
        protected override void OnHurt(GameThing attacker)
        {
            IsAlert = true;

            StateMachine.ChangeState(ActorStateNames.Hurt);

            if (BloodSplashOrigin != Vector2.Zero)
            {
                bloodSplash ??= new BloodSplash(Game, this);
                bloodSplash.Show(BodySize, GetBloodSplashPosition());
            }

            Session.CombatManager.Add(this);
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            OpacityFactor = 1;
            Stand(true);
        }

        // OnMoveToCompleted
        protected override void OnMoveToCompleted()
        {
            if (pendingPathNodes.Count > 0)
                MoveToNextPathNode();
            else
            {
                StopMoving();
                IsFollowingPath = false;
                HandlePendingInteraction();
            }
        }

        // OnRead
        protected override void OnRead(XmlAttributeCollection attributes)
        {
            base.OnRead(attributes);

            // Devotion
            if (attributes[nameof(Stats.Devotion)]?.Value is string devotion)
                Stats.Devotion = XmlConvert.ToInt32(devotion);

            // Dexterity
            if (attributes[nameof(Stats.Dexterity)]?.Value is string dexterity)
                Stats.Dexterity = XmlConvert.ToInt32(dexterity);

            // Fortitude
            if (attributes[nameof(Stats.Fortitude)]?.Value is string fortitude)
                Stats.Fortitude = XmlConvert.ToInt32(fortitude);

            // Mind
            if (attributes[nameof(Stats.Mind)]?.Value is string mind)
                Stats.Mind = XmlConvert.ToInt32(mind);

            // Charisma
            if (attributes[nameof(Stats.Charisma)]?.Value is string charisma)
                Stats.Charisma = XmlConvert.ToInt32(charisma);

            // Strength
            if (attributes[nameof(Stats.Strength)]?.Value is string strength)
                Stats.Strength = XmlConvert.ToInt32(strength);
        }

        // OnSelectTarget
        protected virtual GameThing? OnSelectTarget() => session.Player;

        // OnStartMoving
        protected override void OnStartMoving()
        {
            if (FastMove)
            {
                StateMachine.ChangeState(ActorStateNames.MoveFast);
                moveTween.Start(TweenStyle.QuadraticInOut, 0, .8f, 100, -1);
            }
            else
            {
                StateMachine.ChangeState(ActorStateNames.Move);
                moveTween.Start(TweenStyle.QuadraticInOut, 0, .8f, 200, -1);
            }

            accelerationFactorTween.Start(TweenStyle.Linear, .4f, 1, 150);
        }

        // OnStopMoving
        protected override void OnStopMoving()
        {
            base.OnStopMoving();

            if (IsPlayer)
                session.HUD.DestinationMark.Position = null;

            FastMove = false;
            accelerationFactorTween.Stop();
            moveTween.Stop();
            tinyMoveSpeedFactor = 1;

            if (!IsDead)
                Stand();
        }

        // OnUnload
        protected override void OnUnload()
        {
            IsAlert = false;

            InteractiveTarget = null;

            if (Session.CombatManager.IsActive)
                Session.CombatManager.Remove(this);

            base.OnUnload();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            combatStateMachine.Update(gameTime);

            UpdateFaithRecovery(gameTime);
            UpdateFloatingMessage(gameTime);
            StateMachine.Update(gameTime);

            base.OnUpdate(gameTime);

            bloodSplash?.Update(gameTime);

            if (suspendInteractionCooldown > 0 && !session.IsAwaiting)
                suspendInteractionCooldown -= gameTime.ElapsedGameTime.Milliseconds;

            this.InteractiveTarget = null;
            if (IsPlayer && suspendInteractionCooldown <= 0 && !session.IsAwaiting)
                this.InteractiveTarget = FindInteractiveTarget();

            accelerationFactorTween.Update(gameTime);
            headTween.Update(gameTime);
            ShadowSpot.Update(gameTime);
            speechBubble?.Update(gameTime);
            moveTween.Update(gameTime);
            UpdateDirection();
            UpdateFootstep();
        }

        // OnWrite
        protected override void OnWrite(XmlWriter output)
        {
            base.OnWrite(output);

            output.WriteAttributeString(nameof(Stats.Devotion), XmlConvert.ToString(Stats.Devotion));
            output.WriteAttributeString(nameof(Stats.Dexterity), XmlConvert.ToString(Stats.Dexterity));
            output.WriteAttributeString(nameof(Stats.Fortitude), XmlConvert.ToString(Stats.Fortitude));
            output.WriteAttributeString(nameof(Stats.Mind), XmlConvert.ToString(Stats.Mind));
            output.WriteAttributeString(nameof(Stats.Charisma), XmlConvert.ToString(Stats.Charisma));
            output.WriteAttributeString(nameof(Stats.Strength), XmlConvert.ToString(Stats.Strength));
        }

        // StateMachine
        protected ActorStateMachine StateMachine { get; }

        #endregion

        // Affinity
        [ScriptProperty]
        public Affinity Affinity { get; set; } = Affinity.Neutral;

        // ApplyStats
        [ScriptMethod]
        public void ApplyStats() => Stats.Apply();

        // ApproachAndInteract
        public bool ApproachAndInteract(GameThing target)
        {
            if (!IsPlayer)
                return false;

            var destination = target.GetApproachPosition(this, true);

            var canApproach = target is not Actor actor || !actor.IsCombating;

            var result = canApproach && MoveTo(destination);
            this.pendingInteractiveTarget = target;

            if (target is PickupItem)
                Session.HUD.DestinationMark.Position = null;

            if (!result)
                HandlePendingInteraction();

            return result;
        }

        // BloodSplashOrigin
        [ScriptProperty]
        public Vector2 BloodSplashOrigin { get; set; }

        // BodySize
        public ActorSize BodySize { get; set; } = ActorSize.Medium;

        // CanChangeState
        public bool CanChangeState
        {
            get
            {
                if (IsDead)
                    return false;

                return StateMachine.CurrentState is ActorStandState ||
                       StateMachine.CurrentState is ActorMoveState ||
                       StateMachine.CurrentState is ActorMoveFastState ||
                       StateMachine.CurrentState is ActorFatigueState;
            }
        }

        // CanSeeTarget
        public bool CanSeeTarget()
        {
            if (Target == null)
                return false;

            Vector2 toTarget = Target.Position - Position;

            if (ViewDistance > 0 && toTarget.Length() > ViewDistance)
                return false;

            Vector2 directionToTarget = Vector2.Normalize(toTarget);
            Vector2 forward = Direction == FacingDirection.Right ? Vector2.UnitX : -Vector2.UnitX;

            float dot = Vector2.Dot(forward, directionToTarget);
            float angleThreshold = MathF.Cos(MathHelper.ToRadians(ViewAngle / 2f));

            return dot >= angleThreshold;
        }

        // CloseAttack
        public void CloseAttack() => StateMachine.ChangeState(ActorStateNames.CloseAttack);

        // CreateItem
        public void CreateItem(Item item)
        {
            if (item.MetaItem.Action != ItemAction.Create)
                return;

            if (StateMachine.GetState(ActorStateNames.CreateItem) is ActorCreateState state)
            {
                state.Item = item;
                StateMachine.ChangeState(state.Name);
            }
        }

        // DoAttackTurn
        public void DoAttackTurn()
        {
            if (GetAttackItem() is null)
                return;

            TurnState = CombatTurnState.Busy;

            this.Target = InteractiveTarget;

            combatStateMachine.ExecuteAction(CombatStateSignal.Attack);
        }

        // DoDecideTurn (for NPCs)
        public void DoDecideTurn()
        {
            if (!IsPlayer)
                combatStateMachine.ExecuteAction(CombatStateSignal.Decide);
        }

        // DoMoveTurn
        public void DoMoveTurn(Vector2 destination)
        {
            TurnState = CombatTurnState.Busy;
            combatStateMachine.ExecuteAction(CombatStateSignal.Move, destination);
        }

        // EndTurn
        public void EndTurn()
        {
            if (session.CombatManager.CurrentActor != this)
                return;

            TurnState = IsCombating ? CombatTurnState.Waiting : CombatTurnState.None;
            session.CombatManager.EndCurrentTurn();
        }

        // FaceToTarget
        public void FaceToTarget()
        {
            if (Target != null)
                FaceTo(Target);
        }

        // Faith
        [ScriptProperty]
        public int Faith
        {
            get => faith;
            set
            {
                if (value != faith)
                {
                    faith = Math.Min(value, MaxFaith);
                    if (faith < 0)
                        faith = 0;

                    OnFaithChanged();
                }
            }
        }

        // FastMove
        public bool FastMove { get; set; }

        // FastMoveFactor
        [ScriptProperty(CodingContext.EntityDeclaration)]
        public float FastMoveFactor { get; set; } = 1;

        // Fatigue
        public void Fatigue()
        {
            StopMoving();
            IsAlert = false;
            StateMachine.ChangeState(ActorStateNames.Fatigue);
        }

        // GetBloodSplashPosition
        public Vector2 GetBloodSplashPosition()
        {
            // Origin
            if (BloodSplashOrigin == Vector2.Zero)
                return Vector2.Zero;
            else
                return this.GetAbsolutePoint(BloodSplashOrigin);
        }

        // GetAttackItem
        public Item? GetAttackItem()
        {
            var result = Inventory.GetItem(WeaponName);
            result ??= Gifts.GetItem("UnarmedAttack");
            return result;
        }

        // GetItemContainer
        public ItemContainer GetItemContainer(ItemContainerCategory category)
        {
            return category switch
            {
                ItemContainerCategory.Inventory => Inventory,
                ItemContainerCategory.Gifts => Gifts,
                ItemContainerCategory.Prayers => Prayers,
                _ => throw new ArgumentOutOfRangeException(nameof(category), category, null),
            };
        }

        // GetItemContainerSize
        public override int GetItemContainerSize(ItemContainerCategory category)
        {
            return Stats.GetItemContainerSize(category);
        }

        // Gifts
        public ItemContainer Gifts { get; }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (InputHandler == null || Session.IsAwaiting || !IsPlayer || !CanChangeState)
                return HandleInputResult.Unhandled;

            if (Session.CombatManager.TurnList.Count >= 2 && TurnState != CombatTurnState.WaitingInput)
                return HandleInputResult.Unhandled;

            if (StateMachine.CurrentState.HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            if (InputHandler != null)
                return InputHandler.HandleInput(gameTime);

            return HandleInputResult.Unhandled;
        }

        // HasSpeechBubble
        public bool HasSpeechBubble
        {
            get
            {
                if (speechBubble == null)
                    return false;
                else
                    return speechBubble.State != SpeechBubbleState.Hidden;
            }
        }

        // HotspotDetectorPosition
        [ScriptProperty]
        public Vector2 HotspotDetectorPosition { get; set; }

        // Interact
        public bool Interact(GameThing? target = null)
        {
            if (target == null)
                target = InteractiveTarget;

            if (target == null || !InCurrentRoom || suspendInteractionCooldown > 0)
                return false;

            // Session is busy
            if (Session.State != GameSessionState.Idle)
                return false;

            var script = target.OutcomeScript;

            if (script != null)
            {
                StopMoving();
                Session.BeginOutcome(script, target);
                return true;
            }
            else
                return false;
        }

        // InteractiveTarget
        public GameThing? InteractiveTarget { get; private set; }

        // IsAlert
        public bool IsAlert { get; set; }

        // IsAttacking
        public bool IsAttacking => StateMachine.CurrentState is ActorCloseAttackState;

        // IsCombating
        public bool IsCombating => session.CombatManager.TurnList.Count > 1 && session.CombatManager.TurnList.Contains(this);

        // IsFollowingPath
        public bool IsFollowingPath { get; private set; }

        // IsInteractiveTarget
        public bool IsInteractiveTarget => session.Player?.InteractiveTarget == this;

        // IsPlayer
        public bool IsPlayer => Session.Player == this;

        // IsTargetInAttackRange
        public bool IsTargetInAttackRange()
        {
            return false;

            /*
            if (Target == null || CloseAttackItem == null)
                return false;

            Vector2 targetPos = Target.Position;
            Vector2 toTarget = targetPos - Position;

            // Out of range
            float distance = toTarget.Length();
            if (distance > CloseAttackItem.Range)
                return false;

            // Ensure player is not behind
            //if ((Direction == FacingDirection.Right && toTarget.X < 0) ||
              //  (Direction == FacingDirection.Left && toTarget.X > 0))
            //{
                return false;
            //}

            if (Math.Abs(Target.Y-Y) > 8)
                return false;

            return true;
            */
        }

        // IsTired
        public bool IsTired => StateMachine.CurrentState is ActorFatigueState;

        // IsWalkAreaHole
        public override bool IsWalkAreaHole => false;

        // IsBroken
        [ScriptProperty]
        public bool IsBroken => !IsDead && (float)HP / MaxHP < .3f;

        // Level
        [ScriptProperty]
        public int Level
        {
            get => level;
            set
            {
                if (value != level)
                {
                    if (value < 0)
                        value = 1;

                    level = value;
                    Stats.Apply();
                }
            }
        }

        // MaxFaith
        [ScriptProperty]
        public int MaxFaith
        {
            get => maxFaith;
            set
            {
                if (value != maxFaith)
                {
                    maxFaith = value;
                    Faith = value;
                }
            }
        }

        // MoveTo
        public override bool MoveTo(Vector2 destination)
        {
            if (IsPlayer)
                pendingInteractiveTarget = null;

            // No path needed
            if (WalkArea == null || IgnoreWalkArea)
                return base.MoveTo(destination);

            if (!CanMove || destination == Position)
                return false;

            var distance = DistanceTo(destination);
            if (distance <= 1)
                return false;

            tinyMoveSpeedFactor = 1;
            if (distance <= 15)
            {
                if (distance <= 3)
                    tinyMoveSpeedFactor = .25f;
                else
                    tinyMoveSpeedFactor = .5f;
            }

            var path = WalkArea.FindPath(this, destination);

            // No path
            if (path == null || path.Length == 0)
            {
                FastMove = false;
                return false;
            }

            // Only one path node equals to starting position
            if (path.Length == 1 && path[0] == Position)
            {
                FastMove = false;
                return false;
            }

            pendingPathNodes.Clear();
            pendingPathNodes.AddRange(path);
            MoveToNextPathNode();

            IsFollowingPath = true;

            if (IsPlayer)
                Session.HUD.DestinationMark.Position = path[^1];

            if (FastMove && StateMachine.CurrentState is ActorMoveState)
                StateMachine.ChangeState(ActorStateNames.MoveFast);
            else if (!FastMove && StateMachine.CurrentState is ActorMoveFastState)
                StateMachine.ChangeState(ActorStateNames.Move);

            return true;
        }

        // PlayerNumber
        [ScriptProperty]
        public PlayerNumber PlayerNumber
        {
            get => playerNumber;
            set
            {
                if (value != playerNumber)
                {
                    this.playerNumber = value;
                    if (value == PlayerNumber.None)
                        InputHandler = null;
                    else
                        InputHandler = new PlayerInputHandler<Actor>(this, (PlayerIndex)value);
                }
            }
        }

        // Prayers
        public ItemContainer Prayers { get; }

        // Replenish
        [ScriptMethod]
        public override void Replenish()
        {
            base.Replenish();
            Faith = MaxFaith;
        }

        // Say
        public void Say(string text, bool awaitInput)
        {
            speechBubble ??= new SpeechBubble(this);
            speechBubble.Show(text, awaitInput);
        }

        // SelectTarget
        public void SelectTarget()
        {
            if (!IsPlayer)
                Target = OnSelectTarget();
        }

        // ShadowOffset
        [ScriptProperty]
        public Vector2 ShadowOffset
        {
            get => ShadowSpot.Offset;
            set => ShadowSpot.Offset = value;
        }

        // ShadowSpot
        public ShadowSpot ShadowSpot { get; }

        // ShowMessage
        public void ShowMessage(Message message, int duration = 1000)
        {
            floatingMessage ??= session.ObjectPools.FloatingTexts.Get();
            floatingMessage.Show(GetOverheadPosition(), Localization.GetValue(message), ColorPalette.TextDepracated.Dark, duration);
        }

        // SpeechBubbleSound
        [ScriptProperty(CodingContext.EntityDeclaration)]
        public Sound? SpeechBubbleSound { get; set; }

        // Stand
        [ScriptMethod(CodingContext.Any)]
        public void Stand(bool forceRestart = false) => StateMachine.ChangeState(ActorStateNames.Stand, forceRestart);

        // StartTurn
        public void StartTurn()
        {
            if (HasSpeechBubble)
                speechBubble?.Hide();

            Stand();

            if (IsPlayer)
            {
                TurnState = CombatTurnState.WaitingInput;
            }
            else
            {
                TurnState = CombatTurnState.Busy;
                combatStateMachine.ExecuteAction(CombatStateSignal.Decide);
            }
        }

        // Stats
        public Stats Stats { get; }

        // SuspendInteraction
        public void SuspendInteraction(int duration)
        {
            CodeContract.GreaterThanZero(duration, nameof(duration));
            suspendInteractionCooldown = duration;
            InteractiveTarget = null;
        }

        // Target
        public GameThing? Target { get; private set; }

        // ThrowObject
        public virtual bool ThrowObject()
        {
            Stand();
            /*
            if (StateMachine.ChangeState(ActorStateNames.ThrowObject))
            {
                if (Throwables.SelectedItem != null && Throwables.SelectedItem.Consume())
                    throwObjectState.Item = Throwables.SelectedItem;

                return true;
            }
            */

            return false;
        }

        // TurnState
        public CombatTurnState TurnState { get; private set; }

        // UseItem
        public void UseItem(Item item)
        {
            if (StateMachine.GetState(ActorStateNames.UseItem) is ActorUseItemState state)
            {
                state.Item = item;
                StateMachine.ChangeState(state.Name);
            }
        }

        // WeaponName
        [ScriptProperty]
        public string WeaponName
        {
            get => weaponName;
            set
            {
                if (value != weaponName)
                    weaponName = value;
            }
        }

        /// <summary>
        /// ActorStateMachine
        /// </summary>
        public sealed class ActorStateMachine(Actor owner, ActorState initialState)
            : StateMachine<Actor, ActorState>(owner, initialState)
        {
        }
    }
}
