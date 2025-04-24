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
        private readonly ActorActState actState;
        private BloodSplash? bloodSplash;
        private ItemName closeAttackItemName;
        private readonly ActorCloseAttackState closeAttackState;
        private Meter? damageMeter;
        private int damageMeterCooldown;
        private readonly ActorDeathState deathState;
        private readonly FloatTween headTween = new();
        private readonly ActorHurtState hurtState;
        private int level;
        private GameThing? moveToTarget;
        private readonly FloatTween moveTween = new();
        private FloatingText? staminaMessage;
        private PlayerNumber playerNumber = PlayerNumber.None;
        private readonly List<Vector2> pendingPathNodes = [];
        private readonly GameSession session;
        private SpeechBubble? speechBubble;
        private readonly ActorStandState standState;
        private int suspendInteractionCooldown;
        private readonly ActorThrowObjectState throwObjectState;
        private float tinyMoveSpeedFactor = 1;
        private readonly Blinker<float> vanishBlinker = new(1, 0) { StartDelay = 500 };

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

            this.Equipment = new ItemStorage(this);
            this.Skills = new ItemStorage(this);

            actState = new ActorActState(this);
            deathState = new ActorDeathState(this);
            hurtState = new ActorHurtState(this);

            this.standState = new ActorStandState(this);

            this.StateMachine = new ActorStateMachine(this, standState);
            this.StateMachine.RegisterState(actState);
            this.StateMachine.RegisterState(deathState);
            this.StateMachine.RegisterState(hurtState);
            this.StateMachine.RegisterState(new ActorMoveState(this));
            this.StateMachine.RegisterState(new ActorMoveFastState(this));

            closeAttackState = new ActorCloseAttackState(this);
            this.StateMachine.RegisterState(closeAttackState);

            throwObjectState = new ActorThrowObjectState(this);
            this.StateMachine.RegisterState(throwObjectState);
        }

        #endregion

        #region Private members

        // FindInteractiveTarget
        private GameThing? FindInteractiveTarget()
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

        // HandleMoveToTarget
        private void HandleMoveToTarget()
        {
            if (moveToTarget != null)
            {
                FaceTo(moveToTarget);

                if (moveToTarget.CanBeTargeted)
                {
                    if (Direction == FacingDirection.Right)
                    {
                        if (Vector2.Distance(HurtBox.GetPoint(RectanglePoint.RightBottom), moveToTarget.HurtBox.GetPoint(RectanglePoint.LeftBottom)) > 10)
                            moveToTarget = null;
                    }
                    else
                    {
                        if (Vector2.Distance(HurtBox.GetPoint(RectanglePoint.LeftBottom), moveToTarget.HurtBox.GetPoint(RectanglePoint.RightBottom)) > 10)
                            moveToTarget = null;
                    }

                    CloseAttack(moveToTarget);
                }
                else
                    Interact(moveToTarget);

                moveToTarget = null;
            }
        }

        // InvalidateDamageMeter
        private void InvalidateDamageMeter()
        {
            if (damageMeter != null)
                damageMeter.Value = HP * 100 / MaxHP / damageMeter.MaximumValue;
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

        #endregion

        #region Protected members

        // AIStateMachine
        protected AIStateMachine? AIStateMachine { get; set; }

        // CalculateSpeed
        protected override float CalculateSpeed() => base.CalculateSpeed() * (FastMove ? FastMoveFactor : 1) * tinyMoveSpeedFactor * (accelerationFactorTween.IsRunning ? accelerationFactorTween.CurrentValue : 1);

        // CanCheckCollisions
        protected override bool CanCheckCollisions()
        {
            if (Session.IsAwaiting)
                return false;

            return base.CanCheckCollisions();
        }

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
            damageMeterCooldown = 0;
            StateMachine.ChangeState(ActorStateNames.Death);
            Vanish();
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

            if (damageMeterCooldown > 0 && damageMeter != null)
            {
                damageMeter.Position = GetOverheadPosition(-5, -3);
                damageMeter.Draw(gameTime);
            }
        }

        // OnDrawShadow
        protected override void OnDrawShadow(GameTime gameTime)
        {
            base.OnDrawShadow(gameTime);
            ShadowSpot.Draw(gameTime);
        }

        // OnHPChanged
        protected override void OnHPChanged()
        {
            base.OnHPChanged();
            InvalidateDamageMeter();
        }

        // OnHurt
        protected override void OnHurt()
        {
            StateMachine.ChangeState(ActorStateNames.Hurt);
            damageMeterCooldown = 2500;

            if (damageMeter == null)
            {
                damageMeter = new(Game, ColorPalette.HPMeter.Back, ColorPalette.HPMeter.Fore) { MaximumValue = 10 };
                InvalidateDamageMeter();
            }

            if (BloodSplashOrigin != Vector2.Zero)
            {
                bloodSplash ??= new BloodSplash(Game, this);
                bloodSplash.Show(BodySize, GetBloodSplashPosition());
            }
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
                HandleMoveToTarget();
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

            // Empathy
            if (attributes[nameof(Stats.Empathy)]?.Value is string empathy)
                Stats.Empathy = XmlConvert.ToInt32(empathy);

            // Endurance
            if (attributes[nameof(Stats.Endurance)]?.Value is string endurance)
                Stats.Endurance = XmlConvert.ToInt32(endurance);

            // GP
            if (attributes[nameof(Stats.GP)]?.Value is string gp)
                Stats.GP = XmlConvert.ToInt32(gp);

            // Mind
            if (attributes[nameof(Stats.Mind)]?.Value is string mind)
                Stats.Mind = XmlConvert.ToInt32(mind);

            // Strength
            if (attributes[nameof(Stats.Strength)]?.Value is string strength)
                Stats.Strength = XmlConvert.ToInt32(strength);

            // Vigor
            if (attributes[nameof(Stats.Vigor)]?.Value is string vigor)
                Stats.Vigor = XmlConvert.ToInt32(vigor);
        }

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
            InteractionTarget = null;
            base.OnUnload();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            bloodSplash?.Update(gameTime);

            if (damageMeterCooldown > 0)
            {
                damageMeterCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                damageMeter?.Update(gameTime);
            }

            if (vanishBlinker.IsRunning)
            {
                vanishBlinker.Update(gameTime);
                OpacityFactor = vanishBlinker.CurrentValue;
                if (!vanishBlinker.IsRunning)
                {
                    Unparent();
                    return;
                }
            }

            if (suspendInteractionCooldown > 0 && !session.IsAwaiting)
                suspendInteractionCooldown -= gameTime.ElapsedGameTime.Milliseconds;

            this.InteractionTarget = null;
            if (IsPlayer && suspendInteractionCooldown <= 0 && !session.IsAwaiting)
                this.InteractionTarget = FindInteractiveTarget();

            accelerationFactorTween.Update(gameTime);
            headTween.Update(gameTime);
            ShadowSpot.Update(gameTime);
            speechBubble?.Update(gameTime);
            moveTween.Update(gameTime);
            UpdateDirection();
            StateMachine.Update(gameTime);

            if (AI)
                AIStateMachine?.Update(gameTime);
        }

        // OnWrite
        protected override void OnWrite(XmlWriter output)
        {
            base.OnWrite(output);

            output.WriteAttributeString(nameof(Stats.Devotion), XmlConvert.ToString(Stats.Devotion));
            output.WriteAttributeString(nameof(Stats.Dexterity), XmlConvert.ToString(Stats.Dexterity));
            output.WriteAttributeString(nameof(Stats.Empathy), XmlConvert.ToString(Stats.Empathy));
            output.WriteAttributeString(nameof(Stats.Endurance), XmlConvert.ToString(Stats.Endurance));
            output.WriteAttributeString(nameof(Stats.GP), XmlConvert.ToString(Stats.GP));
            output.WriteAttributeString(nameof(Stats.Mind), XmlConvert.ToString(Stats.Mind));
            output.WriteAttributeString(nameof(Stats.Strength), XmlConvert.ToString(Stats.Strength));
            output.WriteAttributeString(nameof(Stats.Vigor), XmlConvert.ToString(Stats.Vigor));
        }

        // StateMachine
        protected ActorStateMachine StateMachine { get; }

        #endregion

        // Act
        public SpriteAnimation? Act(string animationName, bool loop, AnimationDirection direction, bool preserve)
        {
            var result = AnimationPlayer.Play(animationName, loop, direction);

            if (result != null)
            {
                actState.Preserve = preserve;
                StateMachine.ChangeState(ActorStateNames.Act, true);
            }

            return result;
        }

        // Affinity
        public Affinity Affinity { get; set; } = Affinity.Neutral;  

        // AI
        [ScriptProperty]
        public bool AI { get; set; }

        // ApplyStats
        [ScriptMethod]
        public void ApplyStats() => Stats.Apply();

        // BloodSplashOrigin
        [ScriptProperty]
        public Vector2 BloodSplashOrigin { get; set; }

        // BodySize
        public ActorSize BodySize { get; set; } = ActorSize.Medium;

        // CanBeTargeted
        public override bool CanBeTargeted => base.CanBeTargeted && IsHostile;

        // CanHandleInput
        public bool CanHandleInput => InputHandler != null && !Session.IsAwaiting;

        // CloseAttack
        public virtual bool CloseAttack(GameThing? target)
        {
            if (CloseAttackItem != null)
            {
                Stand();

                var usageResult = CloseAttackItem.BeginUse();

                if (usageResult == ItemUsageResult.Succeeded)
                {
                    closeAttackState.AnimationName = CloseAttackItem.Name.ToString();
                    closeAttackState.Target = target;
                    StateMachine.ChangeState(ActorStateNames.CloseAttack);
                    return true;
                }
                else if (usageResult == ItemUsageResult.NotEnoughStamina)
                {
                    if (IsPlayer)
                    {
                        if (staminaMessage == null || !staminaMessage.IsVisible)
                        {
                            staminaMessage = session.ObjectPools.FloatingTexts.Get();
                            staminaMessage.Show(GetOverheadPosition(-3, -1), Utils.EncodeMessageKey(MessageKey.NoStamina), ColorPalette.StaminaMeter.Fore);
                        }
                    }
                }
            }

            return false;
        }

        // CloseAttackItem
        public Item? CloseAttackItem { get; private set; }

        // CloseAttackItemName
        [ScriptProperty]
        public ItemName CloseAttackItemName
        {
            get => closeAttackItemName;
            set
            {
                if (value != closeAttackItemName)
                {
                    closeAttackItemName = value;
                    var item = value == ItemName.None ? null : Skills.GetItem(value);

                    if (item?.Storage != Skills)
                        throw new InvalidOperationException("Item must be a skill.");
                    else
                        this.CloseAttackItem = item;
                }
            }
        }

        // Equipment
        public ItemStorage Equipment { get; }

        // FastMove
        public bool FastMove { get; set; }

        // FastMoveFactor
        [ScriptProperty(CodingContext.EntityDeclaration)]
        public float FastMoveFactor { get; set; } = 1;

        // FollowingPathDestination
        public Vector2? FollowingPathDestination { get; private set; }

        // GetBloodSplashPosition
        public Vector2 GetBloodSplashPosition()
        {
            // Origin
            if (BloodSplashOrigin == Vector2.Zero)
                return Vector2.Zero;
            else
                return this.GetAbsolutePoint(BloodSplashOrigin);
        }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (!CanHandleInput)
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
        public virtual bool Interact(GameThing? target)
        {
            if (target == null)
                target = InteractionTarget;

            if (target == null || !IsPresent || suspendInteractionCooldown > 0)
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

        // InteractionTarget
        public GameThing? InteractionTarget { get; private set; }

        // IsFollowingPath
        public bool IsFollowingPath { get; private set; }

        // IsHostile
        public bool IsHostile { get; set; }

        // IsPlayer
        public bool IsPlayer => Session.Player == this;

        // IsWalkAreaHole
        public override bool IsWalkAreaHole => false;

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

        // MoveTo
        public override bool MoveTo(Vector2 destination)
        {
            moveToTarget = null;
            FollowingPathDestination = null;

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
                return false;

            // Only one path node equals to starting position
            if (path.Length == 1 && path[0] == Position)
                return false;

            pendingPathNodes.Clear();
            pendingPathNodes.AddRange(path);
            MoveToNextPathNode();

            FollowingPathDestination = path[^1];
            IsFollowingPath = true;

            if (FastMove && StateMachine.CurrentState is ActorMoveState)
                StateMachine.ChangeState(ActorStateNames.MoveFast);
            else if (!FastMove && StateMachine.CurrentState is ActorMoveFastState)
                StateMachine.ChangeState(ActorStateNames.Move);

            return true;
        }

        // MoveTo
        public bool MoveTo(Vector2 destination, GameThing? target)
        {
            var result = MoveTo(destination);
            this.moveToTarget = target;

            if (!result)
                HandleMoveToTarget();

            return result;
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

        // Say
        public void Say(string text, bool awaitInput)
        {
            speechBubble ??= new SpeechBubble(this);
            speechBubble.Show(GetLocalizedDisplayName(), text, awaitInput);
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

        // Skills
        public ItemStorage Skills { get; }

        // SpeechBubbleSound
        [ScriptProperty(CodingContext.EntityDeclaration)]
        public Sound? SpeechBubbleSound { get; set; }

        // Stand
        [ScriptMethod(CodingContext.Any)]
        public void Stand(bool forceRestart = false) => StateMachine.ChangeState(ActorStateNames.Stand, forceRestart);

        // Stats
        public Stats Stats { get; }

        // SuspendInteraction
        public void SuspendInteraction(int duration)
        {
            CodeContract.GreaterThanZero(duration, nameof(duration));
            suspendInteractionCooldown = duration;
            InteractionTarget = null;
        }

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

        // Vanish
        public void Vanish()
        {
            vanishBlinker.Start(70, 8);
            var tween = new ColorTween() { StartDelay = vanishBlinker.StartDelay };
            tween.Start(TweenStyle.CubicIn, Color, Color.Black, 200);
            Tweens.ColorTween = tween;
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
