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
        private readonly FloatTween alertTween = FloatTween.Create(TweenStyle.Linear, 0, .5f, 50, -1);
        private ParticlePopEffect? footstepEffect;
        private SpriteFrame? footstepLastUsedFrame;
        private readonly AnimatedSprite headSprite;
        private readonly FloatTween headTween = new();
        private readonly FloatTween moveBalancingTween = new();
        private readonly FloatTween moveVerticalTween = new();
        private readonly List<Vector2> pendingPathNodes = [];
        private List<AtlasImage>? remainsPieces;
        private SpeechText? speechText;
        private readonly ColorTween tintTween = new();

        #endregion

        #region Constructor

        // Constructor
        public Actor(GameSession session, string name)
            : base(session, name)
        {
            this.Definition = ActorDefinition.Container.Find(DeclaredName);
            this.Atlas = Atlases.Actors;
            this.ApproachBehavior = ApproachBehavior.FaceToFace;
            this.DeathWord = ComicTextKind.PlopRed;
            this.DisplayNameKey = $"Actor.{DeclaredName}";
            this.IgnoreWalkArea = false;
            this.Verb = Verb.Talk;
            this.Faction = Definition == null ? Faction.Good : Definition.Faction;
            this.CombatBehavior = CombatBehavior.Container.Find(DeclaredName);
            this.StatusManager = new(this);

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

            ShadowSpotSize = 6;

            ResetRemainingTurns();
        }

        #endregion

        #region IThingDefinition

        ThingDefinition? IThingDefinition.Definition => this.Definition;

        #endregion

        #region Private members

        // Fatigue
        private void Fatigue()
        {
            if (Sprite.Animations.Contains(ActorStateNames.Fatigue))
            {
                var state = BodyMachine.FindOrCreateState<BodyFatigueState>();
                BodyMachine.ChangeState(state.GetType());
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

            Session.InteractionData.Execute();

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

        // SpawnRemains
        private void SpawnRemains()
        {
            if (Room == null)
                return;

            // Amount of pieces
            var pieceCount = BodySize switch
            {
                BodySize.Small => 4,
                BodySize.Medium => 7,
                BodySize.Large => 12,
                _ => throw new NotImplementedException(),
            };

            if (RemainsKind == RemainsKind.ToxicGuts)
                pieceCount = 0;

            Remains? remains = null;
            var imageName = string.Empty;

            // Guts
            if (RemainsKind is RemainsKind.Guts or RemainsKind.ToxicGuts)
            {
                var stains = RemainsKind switch
                {
                    RemainsKind.Guts => Atlases.Environment.GutStains,
                    RemainsKind.ToxicGuts => Atlases.Environment.ToxicGutStains,
                    _ => throw new NotImplementedException(),
                };

                var guts = RemainsKind switch
                {
                    RemainsKind.Guts => Atlases.Environment.Guts,
                    RemainsKind.ToxicGuts => Atlases.Environment.ToxicGuts,
                    _ => throw new NotImplementedException(),
                };

                if (stains.GetRandomItem() is AtlasImage atlasImage)
                    imageName = atlasImage.Name;

                remains = new Remains(Session, imageName, Vector2.One, guts, pieceCount, false, Definition?.EffectDescriptors);
                
                _ = BodySize switch
                {
                    BodySize.Small => remains.PlaySound(SoundNames.GutsSmall),
                    BodySize.Medium => remains.PlaySound(SoundNames.GutsMedium),
                    BodySize.Large => remains.PlaySound(SoundNames.GutsLarge),
                    _ => throw new NotImplementedException(),
                };
            }

            // Bones
            else if (RemainsKind == RemainsKind.Bones)
            {
                remains = new Remains(Session, imageName, Vector2.One, Atlases.Environment.Bones, pieceCount, true);
                remains.PlaySound(SoundNames.Bones);
            }

            // Custom
            else if (RemainsKind == RemainsKind.Custom)
            {
                if (remainsPieces != null)
                    remains = new Remains(Session, imageName, Vector2.One, remainsPieces, remainsPieces.Count, false);
            }

            if (remains != null)
            {
                remains.Position = Position;
                Room.Children.Add(remains);
            }

            Unparent();
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

        // OnAtlasChanged
        protected override void OnAtlasChanged()
        {
            base.OnAtlasChanged();

            remainsPieces?.Clear();

            if (Atlas == null)
                return;

            var index = 0;
            while (true)
            {
                if (Atlas.FindImage(Sprite.ImagePath + $"Remains{index}") is AtlasImage image)
                {
                    remainsPieces ??= [];
                    remainsPieces.Add(image);
                }
                else
                {
                    break;
                }

                index++;
            }
        }

        // OnCollisioning
        protected override void OnCollisioning(GameThing thing, out bool handled)
        {
            handled = thing.IsMoving;
        }

        // OnDeath
        protected override void OnDeath()
        {
            speechText?.Hide();

            if (RemainsKind != RemainsKind.None)
            {
                SpawnRemains();
            }
            else
            {
                var deathState = BodyMachine.FindOrCreateState<BodyDeathState>();
                BodyMachine.ChangeState(deathState.GetType());
            }
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (moveVerticalTween.IsRunning)
                Y -= moveVerticalTween.CurrentValue;

            if (moveBalancingTween.IsRunning)
                Rotation += moveBalancingTween.CurrentValue;

            var shake = !Session.IsAwaiting && RemainingTurns == 0 && IsHostile;
            if (shake)
                X += alertTween.CurrentValue;

            if (tintTween.IsRunning)
                Color = tintTween.CurrentValue;

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
                    headSprite.Color = Color;
                    headSprite.Opacity = Opacity;
                    headSprite.OpacityFactor = OpacityFactor;
                    headSprite.MatchTransform(Sprite);
                    headSprite.Y += headTween.CurrentValue - Altitude;
                    headSprite.Draw(gameTime);
                }
            }

            if (tintTween.IsRunning)
                Color = Color.White;

            if (moveVerticalTween.IsRunning)
                Y += moveVerticalTween.CurrentValue;

            if (shake)
                X -= alertTween.CurrentValue;

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
            alertTween.RandomizeTime();
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

        // OnStaminaChanged
        protected virtual void OnStaminaChanged(int previousValue)
        {
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
                if (ActiveThrowable == null)
                    Stamina++;
                else
                    Stamina--;

                Session.ProcessTurn(PixelsMoved > 80 ? 1 : 0);
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
            }

            Session.Camera.Shake(TweenStyle.Linear, Vector2.One, 40, 6);

            Hurt();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

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
            alertTween.Update(gameTime);
            tintTween.Update(gameTime);

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

        // CombatBehavior
        public CombatBehavior? CombatBehavior { get; }

        // CombatDecision
        public CombatDecision? CombatDecision { get; set; }

        // CombatDecisionType
        [ScriptProperty]
        public CombatDecisionType CombatDecisionType => CombatDecision?.Type ?? CombatDecisionType.None;

        // Definition
        public ActorDefinition? Definition { get; }

        // DiscardActiveThrowable
        [ScriptMethod]
        public void DiscardActiveThrowable()
        {
            if (ActiveThrowable != null)
            {
                if (ActiveThrowable.MaxHP > 0)
                {
                    var thrownObject = new ThrownProp(this, ActiveThrowable);
                    thrownObject.Drop();
                    ActiveThrowable = null;
                }
                else
                {
                    DropActiveThrowable();
                }
            }
        }

        // DropActiveThrowable
        [ScriptMethod]
        public void DropActiveThrowable()
        {
            if (ActiveThrowable != null && Room != null)
            {
                Room.Children.Add(ActiveThrowable);
                ActiveThrowable.Position = Position;
                ActiveThrowable.Y += 2;
                ActiveThrowable = null;
                PlaySound(SoundNames.PropPlace);
            }
        }

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
                    field = Math.Clamp(value, 0, MaxEnergy);
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
                    if (Energy == 0)
                    {
                        Energy = value;
                    }
                    else if (Energy > field)
                    {
                        Energy = field;
                    }
                }
            }
        }

        // MaxStamina
        [ScriptProperty]
        public int MaxStamina
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    if (Stamina == 0)
                    {
                        Stamina = value;
                    }
                    else if (Stamina > field)
                    {
                        Stamina = field;
                    }
                }
            }
        }

        // MoveLurk
        public virtual void MoveLurk(GameThing target)
        {
            if (WalkArea == null)
                return;

            if (CombatBehavior?.Archetype is not { } arch)
                return;

            // 1. Calculamos la métrica espacial con el jugador
            Vector2 toTarget = target.Position - Position;
            float currentDistance = toTarget.Length();

            if (currentDistance <= 0.1f)
                return;

            // 2. Definimos los parámetros de comportamiento basados en el Arquetipo
            float minLurkDistance = arch.MeleeRange * 1.4f;  // Justo afuera de su rango de ataque
            float maxLurkDistance = arch.MeleeRange * 3f;    // Distancia máxima de acecho proporcional
            float maxStepThisTurn = arch.MeleeRange * 0.75f; // El paso es una fracción de su rango para que sea corto

            Vector2 desiredDirection;

            // 3. Evaluamos la posición en base a su zona de confort
            if (currentDistance > maxLurkDistance)
            {
                // Muy lejos: Acorta distancia en línea recta (como MoveNearby pero a paso corto)
                desiredDirection = toTarget;
                desiredDirection.Normalize();
            }
            else if (currentDistance < minLurkDistance)
            {
                // Muy cerca: Da un paso de retirada para recuperar su distancia de acecho
                desiredDirection = -toTarget;
                desiredDirection.Normalize();
            }
            else
            {
                // En zona de confort: Orbita de costado de forma errática tendiendo a buscar los bordes
                toTarget.Normalize();

                // Calculamos los vectores perpendiculares (izquierda y derecha)
                var perpendicularLeft = new Vector2(-toTarget.Y, toTarget.X);
                var perpendicularRight = new Vector2(toTarget.Y, -toTarget.X);

                // Determinismo espacial para elegir el sentido de la órbita
                bool chooseLeft = (int)(Position.X + Position.Y) % 2 == 0;
                Vector2 orbitDirection = chooseLeft ? perpendicularLeft : perpendicularRight;

                // CRÍTICO: Mezclamos la dirección lateral con un empuje HACIA AFUERA (-toTarget)
                // Esto arrastra al bicho hacia los muros y rincones del WalkArea
                desiredDirection = (orbitDirection * 0.7f) - (toTarget * 0.3f);
                desiredDirection.Normalize();
            }

            // 4. Proyectamos el punto potencial de este turno
            Vector2 potentialTarget = Position + (desiredDirection * maxStepThisTurn);

            // 5. Blindaje geométrico contra los muros del cuarto
            Vector2 bestPoint = WalkArea.Polygon.Clamp(potentialTarget);

            // Evitamos vibraciones rústicas contra los colisionadores
            if (Vector2.Distance(Position, bestPoint) > 4f)
                MoveTo(bestPoint);
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
                    destination = walkArea.RandomWalkablePoint(Random.Shared);
                    if (DistanceTo(destination) > 20)
                    {
                        MoveTo(destination);
                        return;
                    }
                }
            }
        }

        // MoveTo
        public override MoveToResult MoveTo(Vector2 destination)
        {
            // No path needed
            if (WalkArea == null || IgnoreWalkArea)
                return base.MoveTo(destination);

            if (!CanMove)
                return MoveToResult.MoveNotAllowed;

            if (destination == Position || DistanceTo(destination) <= 1)
                return MoveToResult.LessThan1px;

            var path = WalkArea.FindPath(this, destination);

            // No path
            if (path == null || path.Length == 0)
            {
                FastMove = false;
                return MoveToResult.NoPath;
            }

            // Only one path node equals to starting position
            if (path.Length == 1 && path[0] == Position)
            {
                FastMove = false;
                return MoveToResult.LessThan1px;
            }

            pendingPathNodes.Clear();
            pendingPathNodes.AddRange(path);
            MoveToNextPathNode();

            IsFollowingPath = true;

            BodyMachine.ChangeState<BodyMoveState>();

            return MoveToResult.Success;
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

        // Recharge
        [ScriptMethod]
        public virtual void Recharge()
        {
            Energy = MaxEnergy;
        }

        // RemainingTurns
        public int RemainingTurns
        {
            get;
            set
            {
                if (value != field)
                    field = Math.Max(0, value);
            }
        }

        // RemainsKind
        [ScriptProperty]
        public RemainsKind RemainsKind { get; set; } = RemainsKind.Guts;

        // ResolveInteraction
        public bool ResolveInteraction(GameThing target, Item? item)
        {
            if (!IsPlayer || IsDead)
                return false;

            Session.InteractionData.Prepare();
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
                var destination = target.GetApproachPosition(this, Session.InteractionData.IsAttack || ActiveThrowable != null ? ApproachBehavior.ClosestSide : null);

                if (destination != Vector2.Zero)
                {
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
                }

                var moveToResult = destination == Vector2.Zero ? MoveToResult.NoPath : MoveTo(destination);
                if (destination != Vector2.Zero && moveToResult == MoveToResult.NoPath)
                {
                    FaceTo(target);
                    return false;
                }
                else if (moveToResult == MoveToResult.LessThan1px || destination == Vector2.Zero)
                {
                    HandlePendingInteraction();
                }
            }

            return true;
        }

        // Say
        public void Say(string text, bool awaitInput)
        {
            speechText ??= new SpeechText(this);
            speechText.Show(DisplayName, text, awaitInput);
        }

        // ShowStatusReaction
        public void ShowStatusReaction(Status status, bool showIcon)
        {
            switch (status.StatusType)
            {
                // Poison
                case StatusType.Poison:
                    if (showIcon)
                        ShowFlyOff(status.Definition.Image);
                    tintTween.Start(TweenStyle.QuadraticInOut, Color.White, Color.Green, 150, 2);
                    PlaySound(SoundNames.StatusPoison);
                    break;

                default:
                    break;
            }
        }

        // SpeechColor
        public Color SpeechColor { get; set; } = Color.Transparent;

        // SpeechSound
        [ScriptProperty(CodingContext.EntityDeclaration)]
        public Sound? SpeechSound { get; set; }

        // Stamina
        [ScriptProperty]
        public int Stamina
        {
            get;
            set
            {
                if (value != field)
                {
                    var previousValue = field;
                    field = Math.Clamp(value, 0, MaxStamina);
                    OnStaminaChanged(previousValue);
                    if (field == 0)
                    {
                        DiscardActiveThrowable();
                        Fatigue();
                    }
                }
            }
        }

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

        // StatusManager
        public StatusManager StatusManager { get; }

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
