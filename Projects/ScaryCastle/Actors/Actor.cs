using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// Actor
    /// </summary>
    public class Actor : GameThing, IInputHandler, IThingDefinition
    {
        #region Private fields

        private Sprite? activeThrowableSprite;
        private readonly List<AtlasImage>? customGuts;
        private ParticlePopEffect? footstepEffect;
        private SpriteFrame? footstepLastUsedFrame;
        private readonly AnimatedSprite headSprite;
        private readonly FloatTween headTween = new();
        private readonly FloatTween moveBalancingTween = new();
        private readonly FloatTween moveVerticalTween = new();
        private readonly List<Vector2> pendingPathNodes = [];
        private SpeechText? speechText;

        #endregion

        #region Constructor

        // Constructor
        public Actor(GameSession session, string name)
            : base(session, name)
        {
            this.Definition = ActorDefinition.Definitions.Find(DeclaredName);
            this.Atlas = Atlases.Actors;
            this.ApproachBehavior = ApproachBehavior.FaceToFace;
            this.DeathWord = ComicTextKind.PlopRed;
            this.DisplayNameKey = $"Actor.{DeclaredName}";
            this.IgnoreWalkArea = false;
            this.Verb = Verb.Talk;
            this.Faction = Definition == null ? Faction.Good : Definition.Faction;
            this.CombatBehavior = CombatBehavior.Behaviors.Find(DeclaredName);

            headSprite = new AnimatedSprite()
            {
                Atlas = Atlas,
                ImagePath = ImagePath,
                PivotOrigin = PivotOrigin,
            };

            var anim = headSprite.AddAnimation("Stand");
            anim.AddFrame("StandHead01", 1500);
            anim.AddFrame("StandHead02", 100);

            anim = headSprite.AddAnimation("Talk");
            anim.AddFrame("TalkHead01", 100);
            anim.AddFrame("TalkHead02", 100);

            ResetHeadTween();
            headTween.RandomizeTime();

            this.BodyMachine = new BodyStateMachine(this, new BodyStandState());
            this.BodyMachine.AddState(new BodyMoveState());

            if (Atlas?.FindImage(Sprite.ImagePath + "Gut0") != null)
            {
                var index = 0;
                customGuts = [];

                while (true)
                {
                    if (Atlas.FindImage(Sprite.ImagePath + $"Gut{index}") is AtlasImage image)
                        customGuts.Add(image);
                    else
                        break;

                    index++;
                }
            }

            ShadowSpotSize = 6;

            ResetRemainingTurns();
        }

        #endregion

        #region IThingDefinition

        ThingDefinition? IThingDefinition.Definition => this.Definition;

        #endregion

        #region Private members

        // FaceToMouseCursor
        private void FaceToMouseCursor()
        {
            var mousePos = InputManager.DefaultPlayer.Mouse.WorldPosition(Session.Camera);
            if (mousePos.X <= RuntimeHotspot.BoundingRectangleF.Left ||
                mousePos.X >= RuntimeHotspot.BoundingRectangleF.Right)
            {
                FaceTo(mousePos);
            }
        }

        // HandlePendingInteraction
        private void HandlePendingInteraction()
        {
            if (!IsPlayer || !IsInCurrentRoom || IsDead)
                return;

            // Session is busy
            if (Session.State != GameSessionState.Idle)
                return;

            Session.InteractionData.Execute(Session);

            Session.InteractionContext.Reset();
        }

        // Hurt
        private void Hurt()
        {
            if (HurtVoice != null)
                PlaySound(HurtVoice);

            if (Sprite.Animations.Contains(ActorStateNames.Hurt))
            {
                Stand();
                var state = BodyMachine.FindOrCreateState<BodyHurtState>();
                BodyMachine.ChangeState(state.GetType());
            }
        }

        // MoveToNextPathNode
        private void MoveToNextPathNode()
        {
            base.MoveTo(pendingPathNodes[0]);
            pendingPathNodes.RemoveAt(0);
        }

        // ResetHeadTween
        private void ResetHeadTween()
        {
            headTween.Start(TweenStyle.CubicInOut, 0, .25f, 400, -1);
            headTween.RandomizeTime();
        }

        // ResetRemainingTurns
        [ScriptMethod]
        private void ResetRemainingTurns(bool randomize = false)
        {
            if (CombatBehavior == null)
            {
                RemainingTurns = 0;
            }
            else
            {
                if (randomize && CombatBehavior.TurnInterval > 1)
                    RemainingTurns = Random.Shared.Next(1, CombatBehavior.TurnInterval + 1);
                else
                    RemainingTurns = CombatBehavior.TurnInterval;
            }
        }

        // SyncHeadAnimation
        private void SyncHeadAnimation()
        {
            if (!headSprite.Player.IsPlaying)
            {
                headSprite.Player.Stop();
                ResetHeadTween();
                if (headSprite.Animations.Find(AnimationNames.Stand) != null)
                    headSprite.Player.Play(AnimationNames.Stand);
            }
        }

        // UpdateCondition
        private void UpdateCondition(GameTime gameTime)
        {
            if (IsPlayer)
            {
                if (HasSpeechText)
                    return;

                if (Session.IsAwaiting && Session.ActiveNPC == null)
                    return;
            }

            if (Condition == ConditionType.None)
                return;

            if (ConditionTimer > 0)
            {
                ConditionTimer -= gameTime.ElapsedGameTime.Milliseconds;

                if (ConditionTimer <= 0)
                {
                    ConditionTimer = 0;

                    if (ConditionAmount > 0)
                    {
                        ConditionAmount -= 1;
                        HP -= 1;
                        Sound.Play(SoundNames.StatusEffectDamage);
                        ShowComicText(ComicTextKind.AghGreen);

                        if (ConditionAmount > 0)
                            ConditionTimer = GameSettings.ConditionCooldown;
                        else
                            Condition = ConditionType.None;
                    }
                }
            }
        }

        /*
        // UpdateContactIntent
        private bool UpdateContactIntent(GameTime gameTime)
        {
            if (contactTimer > 0)
            {
                contactTimer -= gameTime.ElapsedGameTime.Milliseconds;
            }
            else if (Session.Player != null && IsMoving)
            {
                if (RuntimeCollider.Contains(Session.Player.Position))
                {
                    TryInflictContactDamage(Session.Player);
                    contactTimer = 500;
                    return true;
                }
            }

            return false;
        }
        */

        // UpdateDirection
        private void UpdateDirection()
        {
            if (Sprite.Velocity.X != 0)
            {
                if (Sprite.Velocity.X < 0)
                    Sprite.FlipLeft();
                else
                    Sprite.FlipRight();
            }
        }

        // UpdateFootstep
        private void UpdateFootstep()
        {
            if (Room == null)
                return;

            if (Sprite.Player.Frame == null || Sprite.Player.Frame == footstepLastUsedFrame || !Sprite.Player.Frame.IsTrigger)
                return;

            for (var i = 0; i < Room.CulledThings.Count; i++)
            {
                if (Room.CulledThings[i] is GameThing thing && thing.TerrainSound != null && thing.GetFootstepSound(Position) is Sound sound)
                {
                    footstepEffect ??= new();
                    if (!footstepEffect.IsActive)
                        footstepEffect.Spawn(Position, thing.TerrainParticleColor);

                    PlaySound(sound);
                    footstepLastUsedFrame = Sprite.Player.Frame;
                    return;
                }
            }

            PlaySound(SoundNames.FootstepA);
            footstepLastUsedFrame = Sprite.Player.Frame;
        }

        /*
        // UpdateReactionCounter
        private void UpdateReactionCounter(GameTime gameTime)
        {
            // Can use brain?
            if (IsPlayer || CombatBehavior == null || Faction == Faction.Good || IsDead)
                return;

            if (CombatBehavior.Archetype.CooldownUnit == CombatArchetypeCooldownUnit.Milliseconds)
                ReactionCounter -= gameTime.ElapsedGameTime.Milliseconds;

            // Can update counter?
            if (IsMoving || IsPerformingAction || IsKnockbackInProgress)
                return;

            if (Session.IsAwaiting && Session.AwaitingScript != null && !Session.AwaitingScript.Interruptible)
                return;

            if (ReactionCounter <= 0)
            {
                React();
                ReactionCounter = CombatBehavior.Archetype.GetNextCooldown();
            }
        }
        */

        #endregion

        #region Protected members

        // BodyMachine
        protected BodyStateMachine BodyMachine { get; }

        // CalculateSpeed
        protected override float CalculateSpeed()
        {
            var result = base.CalculateSpeed();

            if (FastMove)
                result *= FastMoveFactor;

            if (ActiveThrowable != null)
                result *= .7f;

            return result;
        }

        // CanCheckCollisions
        protected override bool CanCheckCollisions()
        {
            return !IsFollowingPath && base.CanCheckCollisions();
        }

        // GetKnockbackMultiplier
        protected override float GetKnockbackMultiplier(GameThing target)
        {
            if (target is Prop)
                return 0;

            if (target is Actor targetActor)
            {
                // Esto permite asimetría y evita números mágicos en fórmulas
                return (BodySize, targetActor.BodySize) switch
                {
                    (BodySize.Small, BodySize.Large) => 0.2f, // Penalización severa
                    (BodySize.Small, BodySize.Medium) => 0.8f,

                    (BodySize.Medium, BodySize.Small) => 1.5f,
                    (BodySize.Medium, BodySize.Large) => 0.7f,

                    (BodySize.Large, BodySize.Small) => 3.0f, // Bonus masivo (Smash)
                    (BodySize.Large, BodySize.Medium) => 1.5f,

                    _ => 1.0f // Tamaños iguales o casos no cubiertos
                };
            }

            return 1;
        }

        // InputHandler
        protected InputHandler? InputHandler { get; set; }

        // OnCollisioning
        protected override void OnCollisioning(GameThing thing, out bool handled)
        {
            handled = thing.IsMoving;
        }

        // OnDeath
        protected override void OnDeath()
        {
            speechText?.Hide();

            if (Guts > 0 || customGuts?.Count > 0)
            {
                if (Room != null)
                {
                    var gutScale = BodySize switch
                    {
                        BodySize.Small => Vector2.One,
                        BodySize.Medium => Vector2.One,
                        BodySize.Large => Vector2.One,
                        _ => Vector2.One * 1.25f
                    };

                    var guts = new Guts(Session, Guts > 0, Guts, gutScale, customGuts)
                    {
                        Position = Position,
                    };

                    Room.Children.Add(guts);

                    if (Guts > 0)
                    {
                        _ = BodySize switch
                        {
                            BodySize.Small => guts.PlaySound(SoundNames.GutsSmall),
                            BodySize.Medium => guts.PlaySound(SoundNames.GutsMedium),
                            _ => guts.PlaySound(SoundNames.GutsLarge)
                        };
                    }

                    Unparent();
                }
            }
            else
            {
                var deathState = BodyMachine.FindOrCreateState<BodyDeathState>();
                BodyMachine.ChangeState(deathState.GetType());
            }

            if (Condition == ConditionType.None)
                ShowComicText(ComicTextKind.PlopRed);

            ClearCondition();
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (moveVerticalTween.IsRunning)
                Y -= moveVerticalTween.CurrentValue;

            if (moveBalancingTween.IsRunning)
                Rotation += moveBalancingTween.CurrentValue;

            base.OnDraw(gameTime);

            if (activeThrowableSprite?.RenderImage != null)
            {
                activeThrowableSprite.Position = RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.Top, 0, 3);
                activeThrowableSprite.Draw(gameTime);
            }

            if (AnimationSettings.DetachedHead)
            {
                if (AnimationPlayer.Animation?.Headless == true && headSprite.Player.IsPlaying)
                {
                    headSprite.Opacity = Opacity;
                    headSprite.OpacityFactor = OpacityFactor;
                    headSprite.MatchTransform(Sprite);
                    headSprite.Y += headTween.CurrentValue - Altitude;
                    headSprite.Draw(gameTime);
                }
            }

            if (moveVerticalTween.IsRunning)
                Y += moveVerticalTween.CurrentValue;

            if (moveBalancingTween.IsRunning)
                Rotation -= moveBalancingTween.CurrentValue;

            footstepEffect?.Draw(gameTime);
        }

        // OnEnergyChanged
        protected virtual void OnEnergyChanged(int previousValue)
        {
        }

        // OnFactionChanged
        protected override void OnFactionChanged()
        {
            base.OnFactionChanged();
            if (Faction == Faction.Evil)
                Verb = Verb.Attack;
        }

        // OnInitialize
        protected override void OnInitialize()
        {
            base.OnInitialize();
            BodyMachine.Start();
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            IsAlert = true;
            ResetRemainingTurns(true);
            OpacityFactor = 1;
            Stand();
        }

        // OnMoveToCompleted
        protected override void OnMoveToCompleted()
        {
            if (pendingPathNodes.Count > 0)
            {
                MoveToNextPathNode();
            }
            else
            {
                StopMoving();
                IsFollowingPath = false;
                HandlePendingInteraction();
            }
        }

        // OnStartMoving
        protected override void OnStartMoving()
        {
            BodyMachine.ChangeState<BodyMoveState>();

            if (AnimationSettings.MoveBounce)
                moveVerticalTween.Start(TweenStyle.QuadraticInOut, 0, .8f, 100, -1);

            if (AnimationSettings.MoveSway)
                moveBalancingTween.Start(TweenStyle.QuadraticInOut, 0, .04f, FastMove ? 100 : 200, -1);
        }

        // OnStopMoving
        protected override void OnStopMoving()
        {
            base.OnStopMoving();
            FastMove = false;
            moveVerticalTween.Stop();
            moveBalancingTween.Stop();

            if (EnforceTurn)
            {
                EnforceTurn = false;
                Session.ProcessTurn(PixelsMoved > 80 ? -1 : 0);
            }
        }

        // OnTakeDamage
        protected override void OnTakeDamage(GameThing attacker, int amount, DamageType damageType)
        {
            DiscardActiveThrowable();

            if (!IsDead)
                FaceTo(attacker);

            if (IsPlayer)
            {
                if (Session.ActiveNPC == null && Session.InterruptAwaitingScript())
                {
                    this.Game.SceneManager.PopUntil(Session);
                    StopTalking();
                    speechText?.Hide();
                }
            }
            else if (!IsDead)
            {
                if (Definition?.DropTrigger == LootDropTrigger.OnImpact)
                    DropLoot();

                IsHostile = true;
                RemainingTurns = 0;
            }

            Session.Camera.Shake(TweenStyle.Linear, Vector2.One, 40, 6);

            Hurt();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            UpdateCondition(gameTime);

            headTween.Update(gameTime);

            if (AnimationSettings.DetachedHead)
                headSprite.Update(gameTime);

            speechText?.Update(gameTime);
            moveVerticalTween.Update(gameTime);
            moveBalancingTween.Update(gameTime);
            UpdateDirection();
            UpdateFootstep();
            footstepEffect?.Update(gameTime);
            BodyMachine.Update(gameTime);

            if (!Session.IsAwaiting && !IsMoving)
            {
                if (IsAlert && IsHostile && Session.Player != null)
                    FaceTo(Session.Player);
            }
        }

        #endregion

        // ActiveThrowable
        [ScriptProperty]
        public Prop? ActiveThrowable
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;

                    if (field != null)
                    {
                        activeThrowableSprite ??= new Sprite() { PivotOrigin = RectanglePoint.Bottom };
                        activeThrowableSprite.RenderImage = Atlases.Environment.FindImage(field.GetThrowableImageName());
                        field.Unparent();
                        Stand();
                    }
                    else
                    {
                        activeThrowableSprite?.RenderImage = null;
                        Stand(true);
                    }
                }
            }
        }

        // Animate
        public SpriteAnimation? Animate(string animationName)
        {
            return Animate(animationName, false, AnimationDirection.Forward, false);
        }

        // Animate
        public SpriteAnimation? Animate(string animationName, bool loop, AnimationDirection direction, bool preserve)
        {
            StopMoving();
            var result = AnimationPlayer.Play(animationName, loop, direction);
            if (result != null)
            {
                var state = BodyMachine.FindOrCreateState<BodyAnimateState>();
                state.Preserve = preserve;
                BodyMachine.ChangeState(state.GetType());
            }

            return result;
        }

        // AnimationSettings
        public ActorAnimationSettings AnimationSettings { get; } = new();

        // ApplyCondition
        public void ApplyCondition(ConditionType condition, int amount, ComicTextKind comicTextKind)
        {
            // 1. Clear
            if (condition == ConditionType.None)
            {
                ClearCondition();
                return;
            }

            // 2. Chromatic aberration
            if (condition == ConditionType.ChromaticAberration)
            {
                Session.PerformChromaticAberration();
                return;
            }

            // 3. Coin loss
            if (condition == ConditionType.CoinLoss)
            {
                if (Session.Player == this)
                {
                    if (Session.PlayerInventory.Find(nameof(Coin)) is Item coin)
                    {
                        coin.Amount--;
                        Sound.Play(SoundNames.CoinLoss);
                        Session.TextHUD.Log.Show(LogVerb.Lost, coin.Definition, true);
                    }
                }

                return;
            }

            // 4. Si el efecto entrante es Maldición: PISA el veneno o SE SUMA a una maldición previa.
            if (condition == ConditionType.Curse)
            {
                if (Condition != ConditionType.Curse)
                {
                    Condition = ConditionType.Curse;
                    ConditionAmount = amount;
                }
                else
                {
                    ConditionAmount += amount; // Ya estaba maldito, se acumula.
                    if (ConditionAmount > HP)
                        HP -= 1;
                }

                ConditionTimer = GameSettings.ConditionCooldown;

                if (IsPlayer)
                    Session.ObjectPools.FloatingTexts.Get()?.ShowAmount(this, ColorPalette.Condition.Curse, amount);
            }

            // 5. Si el efecto entrante es Veneno: Solo importa si no estás maldito.
            if (condition == ConditionType.Poison && Condition != ConditionType.Curse)
            {
                if (Condition != ConditionType.Poison)
                {
                    Condition = ConditionType.Poison;
                    ConditionAmount = amount;
                }
                else
                {
                    ConditionAmount += amount; // Ya estaba envenenado, se acumula.
                    if (ConditionAmount > HP)
                        HP -= 1;
                }

                ConditionTimer = GameSettings.ConditionCooldown;

                if (IsPlayer)
                    Session.ObjectPools.FloatingTexts.Get()?.ShowAmount(this, ColorPalette.Condition.Poison, amount);
            }

            // ComicText si hubo daño real
            if (comicTextKind != ComicTextKind.None)
            {
                if (!IsDead || DeathWord == ComicTextKind.None)
                    ShowComicText(comicTextKind);
            }

            OnApplyCondition(condition, amount);
        }

        // BeginTurn
        public Script? BeginTurn()
        {
            if (CombatBehavior == null || IsPlayer || Session.Player == null || !IsHostile || Session.IsAwaiting)
                return null;

            CombatDecision = Brain.Decide(this, Session.Player);

            if (OutcomeScript != null && CombatDecision != null)
            {
                ResetRemainingTurns();
                return OutcomeScript;
            }
            else
            {
                CombatDecision = null;
            }

            return null;
        }

        // BodySize
        public BodySize BodySize { get; set; } = BodySize.Medium;

        // CanHandleInput
        public bool CanHandleInput
        {
            get
            {
                if (IsDead || Session.IsAwaiting)
                    return false;

                return BodyMachine.CurrentState is BodyStandState or BodyMoveState;
            }
        }

        // CanTakeDamage
        public override bool CanTakeDamage()
        {
            if (IsPlayer && Session.AwaitingScript != null && Session.ActiveNPC == null)
            {
                return Session.AwaitingScript.Interruptible && base.CanTakeDamage();
            }
            else
            {
                return base.CanTakeDamage();
            }
        }

        // Charge
        public virtual void Charge(Vector2 destination)
        {
            var state = BodyMachine.FindOrCreateState<BodyChargeState>();
            state.Destination = destination;
            BodyMachine.ChangeState(state.GetType());
        }

        // ClearCondition
        public void ClearCondition()
        {
            this.Condition = ConditionType.None;
            this.ConditionAmount = 0;
        }

        // CombatBehavior
        public CombatBehavior? CombatBehavior { get; }

        // CombatDecision
        public CombatDecision? CombatDecision { get; set; }

        // CombatDecisionType
        [ScriptProperty]
        public CombatDecisionType CombatDecisionType => CombatDecision?.Type ?? CombatDecisionType.None;

        // Condition
        public ConditionType Condition { get; private set; }

        // ConditionAmount
        [ScriptProperty]
        public int ConditionAmount
        {
            get;
            set
            {
                if (value != field)
                {
                    field = Math.Max(0, value);
                    if (field == 0)
                        ClearCondition();
                }
            }
        }

        // ConditionTimer
        public int ConditionTimer { get; private set; }

        // DiscardActiveThrowable
        [ScriptMethod]
        public void DiscardActiveThrowable()
        {
            if (ActiveThrowable != null)
            {
                var thrownObject = new ThrownProp(this, ActiveThrowable);
                thrownObject.Drop();
                ActiveThrowable = null;
            }
        }

        // Definition
        public ActorDefinition? Definition { get; }

        // Energy
        [ScriptProperty]
        public int Energy
        {
            get;
            set
            {
                if (value != field)
                {
                    var previousValue = field;
                    field = int.Clamp(value, 0, MaxEnergy);
                    OnEnergyChanged(previousValue);
                }
            }
        }

        // EnforceTurn
        public bool EnforceTurn { get; set; }

        // ExecuteAction
        public bool ExecuteAction(IAction action, GameThing? target)
        {
            if (IsDead)
                return false;

            StopMoving();

            if (target != null)
            {
                FaceTo(target);

                if (action.ActionKind is ActionKind.Projectile or ActionKind.Proximity)
                {
                    if (!IsInAttackLane(target))
                        return false;
                }
            }

            var state = BodyMachine.FindOrCreateState<BodyExecuteActionState>();
            state.Action = action;
            state.Target = target;
            BodyMachine.ChangeState(state.GetType());

            return true;
        }

        // FastMove
        public bool FastMove { get; set; }

        // FastMoveFactor
        [ScriptProperty(CodingContext.EntityDeclaration)]
        public float FastMoveFactor { get; set; } = 1;

        // Flee
        public virtual void Flee()
        {
            MoveRandomly();
        }

        // FootstepSound
        [ScriptProperty]
        public Sound? FootstepSound { get; set; }

        // GetActiveThrowablePosition
        public Vector2? GetActiveThrowablePosition()
        {
            return activeThrowableSprite?.Position;
        }

        // Guts
        [ScriptProperty]
        public int Guts { get; set; } = 8;

        // HandleInput
        public HandleInputResult HandleInput()
        {
            if (InputHandler == null || Session.IsAwaiting || !IsPlayer || !CanHandleInput)
                return HandleInputResult.Unhandled;

            if (InputHandler != null && Session.IsCurrentScene)
                return InputHandler.HandleInput();

            return HandleInputResult.Unhandled;
        }

        // HasThrowable
        [ScriptProperty]
        public bool HasThrowable => ActiveThrowable != null;

        // HasSpeechText
        public bool HasSpeechText => speechText != null && speechText.State != SpeechTextState.Hidden;

        // HurtVoice
        [ScriptProperty]
        public Sound? HurtVoice { get; set; }

        // IsAlert
        public bool IsAlert { get; set; }

        // IsInAttackLane
        public bool IsInAttackLane(GameThing target, int attackLaneThickness = 3)
        {
            float dy = Math.Abs(Position.Y - target.Y);
            return dy <= attackLaneThickness;
        }

        // IsFollowingPath
        public bool IsFollowingPath { get; private set; }

        // IsReacting
        [ScriptProperty]
        public bool IsReacting => Session.ActiveNPC == this;

        // IsPerformingAction
        public bool IsPerformingAction => BodyMachine.CurrentState is BodyExecuteActionState;

        // IsPlayer
        [ScriptProperty]
        public bool IsPlayer => Session.Player == this;

        // IsStanding
        public bool IsStanding => BodyMachine.CurrentState is BodyStandState;

        // IsStandingOrMoving
        public bool IsStandingOrMoving => BodyMachine.CurrentState is BodyStandState or BodyMoveState;

        // Lift
        public void Lift(Prop prop)
        {
            if (IsDead)
                return;

            StopMoving();

            if (prop != null)
                FaceTo(prop);

            var state = BodyMachine.FindOrCreateState<BodyLiftState>();
            state.Target = prop;
            BodyMachine.ChangeState(state.GetType());

            if (IsPlayer)
                Session.InteractionContext.HeldItem = null;
        }

        // MaxEnergy
        [ScriptProperty]
        public int MaxEnergy
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    if (HP > field)
                        HP = field;
                }
            }
        }

        // MoveNearby
        public virtual void MoveNearby(GameThing target)
        {
            var arch = CombatBehavior?.Archetype;
            if (arch == null)
                return;

            if (WalkArea == null)
                return;

            // 1. Calculamos el vector dirección hacia el jugador
            Vector2 direction = target.Position - Position;
            float currentDistance = direction.Length();

            if (currentDistance <= 0.1f)
                return; // Ya está encima, no hay nada que mover

            direction.Normalize();

            // 2. El MeleeAttackRange del arquetipo define el paso máximo de persecución de este turno
            float maxStepThisTurn = arch.MeleeRange;

            // 3. El punto ideal es la posición del jugador menos un pequeño margen (ej. 12px) 
            // para que el sprite del bicho quede perfectamente enfrente y no encima del centro del player
            float idealStopDistance = Math.Max(0f, currentDistance - 12f);

            // No caminamos más de nuestro rango de ataque por turno, ni nos pasamos del punto ideal
            float actualMoveDistance = Math.Min(maxStepThisTurn, idealStopDistance);

            // 4. Proyectamos el punto de destino en línea recta hacia el objetivo
            Vector2 potentialTarget = Position + (direction * actualMoveDistance);

            // 5. Lo blindamos pasándolo por el polígono transitable del cuarto
            Vector2 bestPoint = WalkArea.Polygon.Clamp(potentialTarget);

            // Si el punto es válido y nos saca de la inercia (evita vibraciones contra muros)
            if (Vector2.Distance(Position, bestPoint) > 4f)
                MoveTo(bestPoint);
        }

        // MoveRandomly
        [ScriptMethod]
        public void MoveRandomly()
        {
            if (Room?.WalkArea is WalkArea walkArea)
            {
                Vector2 destination;

                for (var i = 0; i < 10; i++)
                {
                    destination = walkArea.RandomWalkablePoint();
                    if (DistanceTo(destination) > 20)
                    {
                        MoveTo(destination);
                        return;
                    }
                }
            }
        }

        // MoveTo
        public override bool MoveTo(Vector2 destination)
        {
            // No path needed
            if (WalkArea == null || IgnoreWalkArea)
                return base.MoveTo(destination);

            if (!CanMove || destination == Position)
                return false;

            var distance = DistanceTo(destination);
            if (distance <= 1)
                return false;

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

            BodyMachine.ChangeState<BodyMoveState>();

            return true;
        }

        // MoveToDestination
        public Vector2? MoveToDestination => pendingPathNodes.Count == 0 ? null : pendingPathNodes[^1];

        // PixelsMoved
        public float PixelsMoved { get; set; }

        // PlayerNumber
        [ScriptProperty]
        public PlayerNumber PlayerNumber
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    if (value == PlayerNumber.None)
                        InputHandler = null;
                    else
                        InputHandler = new PlayerInputHandler<Actor>(this, (PlayerIndex)value);
                }
            }
        } = PlayerNumber.None;

        // RemainingTurns
        public int RemainingTurns
        {
            get;
            set
            {
                if (value != field)
                {
                    field = Math.Max(0, value);
                }
            }
        }

        // ResolveInteraction
        public bool ResolveInteraction(GameThing target, Item? item)
        {
            if (!IsPlayer || IsDead)
                return false;

            Session.InteractionData.Prepare(Session.InteractionContext);
            if (!Session.InteractionData.CanExecute)
            {
                MouseCursor.Shake();
                return false;
            }

            if ((item == null && target == this) || (item?.Definition.ActionKind is ActionKind.InPlace or ActionKind.Self))
            {
                HandlePendingInteraction();
            }
            else
            {
                if (target is RideDoor door && door.IsBlocked(this))
                {
                    FaceTo(target);
                    Session.AwaitRoutine(RoutineNames.WayBlockedHandler);
                    return false;
                }

                var destination = target.GetApproachPosition(this, Session.InteractionData.IsAttack || ActiveThrowable != null ? ApproachBehavior.ClosestSide : null);

                if (ActiveThrowable != null)
                {
                    if (target.X < X)
                        destination.X += 14;
                    else
                        destination.X -= 14;
                }
                else if (item?.Definition.ActionKind == ActionKind.Projectile)
                {
                    destination.X = X;
                }

                // Because destination is based on the hotspot it is possible that the current destination
                // is within the collider polygon so we need to move it out; otherwise MoveTo() will fail.
                if (!target.RuntimeCollider.IsEmpty)
                    destination = target.RuntimeCollider.GetClosestPointOnEdge(destination);

                if (!MoveTo(destination))
                    HandlePendingInteraction();
            }

            return true;
        }

        // Say
        public void Say(string text, bool awaitInput)
        {
            speechText ??= new SpeechText(this);

            var color = SpeechColor;
            if (color == Color.Transparent)
            {
                if (IsPlayer)
                {
                    color = ColorPalette.Text.Sentence;
                }
                else
                {
                    color = Faction == Faction.Evil ? ColorPalette.Text.Yellow : ColorPalette.Text.Default;
                }
            }

            speechText.Show(text, color, awaitInput);
        }

        // SpeechColor
        public Color SpeechColor { get; set; } = Color.Transparent;

        // SpeechSound
        [ScriptProperty(CodingContext.EntityDeclaration)]
        public Sound? SpeechSound { get; set; }

        // Stand
        [ScriptMethod()]
        public void Stand()
        {
            Stand(true);
        }

        // Stand
        public void Stand(bool enforce = false)
        {
            BodyMachine.ChangeState<BodyStandState>(enforce);
        }

        // StartTalking
        public void StartTalking()
        {
            if (AnimationSettings.DetachedHead)
                headSprite.Player.Play(ActorStateNames.Talk, true);
            else
                Animate(AnimationNames.Talk, true, AnimationDirection.Forward, false);
        }

        // StopTalking
        public void StopTalking()
        {
            if (AnimationSettings.DetachedHead)
                headSprite.Player.Play(AnimationNames.Stand, true);
            else
                Stand();
        }

        // ThrowActiveTrowable
        public void ThrowActiveTrowable(GameThing target)
        {
            if (ActiveThrowable is null)
                return;

            FaceTo(target);
            var state = BodyMachine.FindOrCreateState<ActorThrowObjectState>();
            state.Target = target;
            state.Prop = ActiveThrowable;
            ActiveThrowable = null;
            BodyMachine.ChangeState(state.GetType());
        }

        // ProcessTurn
        public void ProcessTurn()
        {
            if (CombatBehavior == null || IsPlayer || !IsHostile || RemainingTurns == 0)
                return;

            if (!IsAlert)
            {
                if (Session.Player != null)
                {
                    if (!IsFacingTarget(Session.Player))
                        return;

                    //if (Room?.WalkArea?.InLineOfSight(Session.Player.Position, Position, this) == true)
                    IsAlert = true;
                }

                return;
            }

            if (Session.Player != null && RemainingTurns > 0)
            {
                if (DistanceToTarget(Session.Player) <= CombatBehavior.Archetype.MeleeRange)
                {
                    RemainingTurns = 0;
                    return;
                }
            }

            RemainingTurns -= 1;
        }

        /// <summary>
        /// BodyStateMachine
        /// </summary>
        public sealed class BodyStateMachine(Actor owner, BodyState initialState)
            : StateMachine<Actor>(owner, initialState)
        {
            // OnStateChanged
            protected override void OnStateChanged()
            {
                Owner.SyncHeadAnimation();
            }
        }
    }
}
