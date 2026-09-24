using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// Actor
    /// </summary>
    public class Actor : GameThing, IInputHandler
    {
        #region Private fields

        private BloodSplash? bloodSplash;
        private Sprite? carriedPropSprite;
        private ParticlePopEffect? footstepEffect;
        private SpriteFrame? footstepLastUsedFrame;
        private const float InitialSpeedMultiplier = .15f;
        private Vector2? lastKnownLiftPosition;
        private readonly FloatTween moveVerticalTween = new();
        private readonly List<Vector2> pendingPathNodes = [];
        private List<AtlasImage>? remainsPieces;
        private SpeechText? speechText;
        private readonly ColorTween tintTween = new();
        private float turnTimer;
        private float moveAccelerationMultiplier = .75f;

        #endregion

        #region Constructor

        // Constructor
        public Actor(GameSession session, string name)
            : base(session, name)
        {
            this.Definition = GameData.Actors.Find(DeclaredName);
            this.Atlas = Atlases.Actors;
            this.ApproachBehavior = ApproachBehavior.FaceToFace;
            this.LabelKey = $"Actor.{DeclaredName}";
            this.IgnoreWalkArea = false;
            this.DefaultVerb = Verb.Talk;
            this.Faction = Definition == null ? Faction.Good : Definition.Faction;
            this.CombatBehavior = GameData.CombatBehaviors.Find(DeclaredName);
            this.BodyMachine = new StateMachine<Actor>(this, new BodyStandState());
            this.BodyMachine.AddState(new BodyMoveState());

            ShadowSpotSize = 6;

            ResetRemainingTurns();
        }

        #endregion

        #region Private members

        // GetBloodSplashPosition
        private Vector2 GetBloodSplashPosition()
        {
            return BloodSplashOrigin == Vector2.Zero ? Vector2.Zero : this.GetAnchoredPosition(BloodSplashOrigin);
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

        // ResetRemainingTurns
        [ScriptMethod]
        private void ResetRemainingTurns(bool randomize = false)
        {
            if (CombatBehavior == null)
            {
                RemainingTurns = 0;
                turnTimer = 0;
            }
            else
            {
                turnTimer = Random.Shared.Next(CombatBehavior.TurnCooldown / 2, CombatBehavior.TurnCooldown + 1);

                if (randomize && CombatBehavior.TurnInterval > 1)
                {
                    RemainingTurns = RemainingTurns = Random.Shared.Next(1, CombatBehavior.TurnInterval + 1);
                }
                else
                {
                    RemainingTurns = CombatBehavior.TurnInterval;
                }
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

                remains = new Remains(Session, imageName, Vector2.One, guts, pieceCount, false, true, Definition?.EffectDescriptors);

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
                remains = new Remains(Session, imageName, Vector2.One, Atlases.Environment.Bones, pieceCount, true, false);
                remains.PlaySound(SoundNames.Bones);
            }

            // Custom
            else if (RemainsKind == RemainsKind.Custom)
            {
                if (remainsPieces != null)
                    remains = new Remains(Session, imageName, Vector2.One, remainsPieces, remainsPieces.Count, false, false);
            }

            if (remains != null)
            {
                remains.Position = Position;
                Room.Children.Add(remains);
            }

            Unparent();
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

            PlaySound(SoundNames.FootstepGrass);
            footstepLastUsedFrame = Sprite.Player.Frame;
        }

        #endregion

        #region Protected members

        // BodyMachine
        protected StateMachine<Actor> BodyMachine { get; }

        // CalculateSpeed
        protected override float CalculateSpeed()
        {
            var result = base.CalculateSpeed();

            if (FastMove)
                result *= FastMoveFactor;

            if (CarriedProp != null)
                result *= .9f;

            // Se aplica la curva de aceleración al resultado final
            result *= moveAccelerationMultiplier;

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

            if (tintTween.IsRunning)
                Color = tintTween.CurrentValue;

            base.OnDraw(gameTime);

            bloodSplash?.Draw(gameTime);

            if (carriedPropSprite?.RenderImage != null)
            {
                carriedPropSprite.Position = RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.Top, 0, 3);
                carriedPropSprite.Draw(gameTime);
            }

            if (tintTween.IsRunning)
                Color = Color.White;

            if (moveVerticalTween.IsRunning)
                Y += moveVerticalTween.CurrentValue;

            footstepEffect?.Draw(gameTime);
        }

        // OnEnergyChanged
        protected virtual void OnEnergyChanged(int previousValue)
        {
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

                if (IsPlayer && IsInCurrentRoom)
                    Session.InteractionData.ExecutePending(this);
            }
        }

        // OnKnockbackCompleted
        protected override void OnKnockbackCompleted()
        {
            base.OnKnockbackCompleted();

            if (BloodSplashOrigin != Vector2.Zero)
            {
                bloodSplash ??= new(this);
                bloodSplash.Show(GetBloodSplashPosition());
            }
        }

        // OnStartMoving
        protected override void OnStartMoving()
        {
            lastKnownLiftPosition = null;

            if (StartMovingSound != null)
                PlaySound(StartMovingSound);

            BodyMachine.ChangeState<BodyMoveState>();

            // Reiniciamos el multiplicador de velocidad para aplicar la aceleración
            moveAccelerationMultiplier = InitialSpeedMultiplier;

            if (!SuppressMoveBounceEffect)
                moveVerticalTween.Start(TweenStyle.QuadraticInOut, 0, .8f, 200, -1);
        }

        // OnStopMoving
        protected override void OnStopMoving()
        {
            base.OnStopMoving();

            if (IsPlayer)
                Session.HUD.DestinationMark.Position = null;

            FastMove = false;
            moveVerticalTween.Stop();

            if (EnforceTurn)
            {
                EnforceTurn = false;
                Session.ProcessTurn();
            }
        }

        // OnTakeDamage
        protected override void OnTakeDamage(GameThing attacker, int amount, DamageType damageType)
        {
            DiscardCarriedProp();

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
                IsHostile = true;
            }

            Session.Camera.Shake(TweenStyle.Linear, Vector2.One, 40, 6);

            Hurt();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            speechText?.Update(gameTime);
            moveVerticalTween.Update(gameTime);
            bloodSplash?.Update(gameTime);
            UpdateDirection();
            UpdateFootstep();
            footstepEffect?.Update(gameTime);
            BodyMachine.Update(gameTime);
            tintTween.Update(gameTime);

            // Lógica de aceleración
            if (IsMoving && moveAccelerationMultiplier < 1f)
            {
                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
                moveAccelerationMultiplier = float.Min(1f, moveAccelerationMultiplier + (dt / MoveAccelerationTime));
            }

            if (!Session.IsAwaiting && !IsMoving && !IsPlayer && Faction == Faction.Evil)
            {
                Brain.UpdatePerception(this, Session.Player);

                if (IsHostile && Session.Player != null)
                    FaceTo(Session.Player);
            }

            if (!Session.IsAwaiting && RemainingTurns > 0 && IsHostile)
            {
                if (CombatBehavior != null && turnTimer > 0)
                {
                    turnTimer -= gameTime.ElapsedGameTime.Milliseconds;
                    if (turnTimer <= 0)
                    {
                        RemainingTurns--;
                        turnTimer = CombatBehavior.TurnCooldown;
                    }
                }
            }
        }

        #endregion

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

        // ApplyAction
        public void ApplyAction(IAction action)
        {
            // HP penalty
            if (action.HPCost > 0)
            {
                HP -= action.HPCost;
                /*
                if (IsPlayer)
                    ShowFlyOff(Atlases.UI.DroolIcon);
                */
            }
        }

        // BeginTurn
        public Script? BeginTurn()
        {
            // Le quitamos el "!IsHostile" de acá arriba
            if (CombatBehavior == null || IsPlayer || Session.Player == null || Session.IsAwaiting)
                return null;

            turnTimer = CombatBehavior == null ? 0 : CombatBehavior.TurnInterval;

            // Ahora el Brain siempre corre. Si está lejos, devolverá Lurk/Random. 
            // Si te acercas, el Brain mismo pasará IsHostile a true y devolverá Attack/MoveNearby.
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

        // BloodSplashOrigin
        [ScriptProperty]
        public Vector2 BloodSplashOrigin { get; set; }

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

        // CanInteract
        public override bool CanInteract()
        {
            if (IsPlayer && Session.InteractionContext.HeldItem == null && CarriedProp == null)
                return false;

            return base.CanInteract();
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

        // CarriedProp
        [ScriptProperty]
        public Prop? CarriedProp
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;

                    if (field != null)
                    {
                        carriedPropSprite ??= new Sprite() { PivotOrigin = RectanglePoint.Bottom };
                        carriedPropSprite.RenderImage = Atlases.Environment.FindImage(field.GetCarriedPropImageName());
                        field.Unparent();
                        Stand();
                    }
                    else
                    {
                        carriedPropSprite?.RenderImage = null;
                        Stand(true);
                    }
                }
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
        public override ActorDefinition? Definition { get; }

        // DiscardCarriedProp
        [ScriptMethod]
        public void DiscardCarriedProp()
        {
            if (CarriedProp == null)
                return;

            if (CarriedProp.MaxHP > 0)
            {
                StopMoving();
                var thrownObject = new ThrownProp(this, CarriedProp);
                thrownObject.Drop();
                CarriedProp = null;
                lastKnownLiftPosition = null;
            }
            else
            {
                DropCarriedProp();
            }
        }

        // DropCarriedProp
        [ScriptMethod]
        public void DropCarriedProp()
        {
            if (CarriedProp == null || Room == null)
                return;

            StopMoving();

            Room.Children.Add(CarriedProp);

            if (lastKnownLiftPosition == null)
            {
                CarriedProp.Position = Position;
                CarriedProp.Y += CarriedProp.RuntimeCollider.BoundingRectangleF.Height;
            }
            else
            {
                CarriedProp.Position = lastKnownLiftPosition.Value;
                lastKnownLiftPosition = null;
            }

            CarriedProp = null;

            PlaySound(SoundNames.PropPlace);
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
                    if (target.Verb == Verb.Attack && !IsInAttackLane(target))
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

        // Fatigue
        public bool Fatigue()
        {
            if (Sprite.Animations.Contains(ActorStateNames.Fatigue))
            {
                StopMoving();
                DiscardCarriedProp();
                var state = BodyMachine.FindOrCreateState<BodyFatigueState>();
                BodyMachine.ChangeState(state.GetType());
                Session.ProcessTurn(10);
                return true;
            }

            return false;
        }

        // Flee
        public virtual void Flee()
        {
            MoveRandomly();
        }

        // FootstepSound
        [ScriptProperty]
        public Sound? FootstepSound { get; set; }

        // GetCarriedPropPosition
        public Vector2? GetCarriedPropPosition()
        {
            return carriedPropSprite?.Position;
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

        // HasSpeechText
        public bool HasSpeechText => speechText != null && speechText.State != SpeechTextState.Hidden;

        // HurtVoice
        [ScriptProperty]
        public Sound? HurtVoice { get; set; }

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

        // IsStanding
        public bool IsStanding => BodyMachine.CurrentState is BodyStandState;

        // IsStandingOrMoving
        public bool IsStandingOrMoving => BodyMachine.CurrentState is BodyStandState or BodyMoveState;

        // Label
        public override string Label
        {
            get
            {
                if (IsPlayer && CarriedProp != null)
                    return CarriedProp.Label;

                return base.Label;
            }
        }

        // Lift
        public void Lift(Prop prop)
        {
            if (IsDead)
                return;

            StopMoving();

            FaceTo(prop);

            var state = BodyMachine.FindOrCreateState<BodyLiftState>();
            state.Target = prop;
            lastKnownLiftPosition = prop.Position;
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
                    var prevValue = field;

                    field = value;

                    if (Energy == 0 && prevValue == 0)
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

        // MoveAccelerationTime
        [ScriptProperty]
        public float MoveAccelerationTime { get; set; }

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

        public virtual void MoveNearby(GameThing target)
        {
            var arch = CombatBehavior?.Archetype;
            if (arch == null || WalkArea == null)
                return;

            Vector2 direction = target.Position - Position;
            float currentDistance = direction.Length();

            if (currentDistance <= 0.1f)
                return;

            direction.Normalize();

            // AHORA SÍ: El paso de este turno está limitado por su velocidad de zancada, no por el rango de golpe
            float maxStepThisTurn = arch.MaxStepPerTurn;

            // No queremos que camine sobre el centro del player, frenamos a un margen razonable
            float idealStopDistance = Math.Max(0f, currentDistance - 12f);

            float actualMoveDistance = Math.Min(maxStepThisTurn, idealStopDistance);

            Vector2 potentialTarget = Position + (direction * actualMoveDistance);
            Vector2 bestPoint = WalkArea.Polygon.Clamp(potentialTarget);

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
        public MoveToResult MoveTo(Vector2 destination, float slowMoveThreshold = 0)
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

            if (IsPlayer)
                FastMove = CarriedProp == null && FastMoveFactor > 1 && Vector2.Distance(Position, path[^1]) > slowMoveThreshold;

            pendingPathNodes.Clear();
            pendingPathNodes.AddRange(path);
            MoveToNextPathNode();

            IsFollowingPath = true;

            if (IsPlayer)
                Session.HUD.DestinationMark.Position = path[^1];

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

        // ProcessTurn
        public void ProcessTurn()
        {
            if (CombatBehavior == null || IsPlayer || RemainingTurns == 0)
                return;

            // Si el Brain determinó que estamos en rango para pegar, cortamos los turnos de espera.
            if (Session.Player != null && IsHostile && RemainingTurns > 0)
            {
                if (DistanceToTarget(Session.Player) <= CombatBehavior.Archetype.MeleeRange)
                {
                    RemainingTurns = 0;
                    return;
                }
            }

            RemainingTurns -= 1;
        }

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

        // Say
        public void Say(string text, bool awaitInput, string? soundName)
        {
            speechText ??= new SpeechText(this);
            speechText.Show(text, awaitInput, soundName);
        }

        // ShowStatusReaction
        /*
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
        */

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

        // StartMovingSound
        [ScriptProperty]
        public Sound? StartMovingSound { get; set; }

        // StartTalking
        public void StartTalking()
        {
            Animate(AnimationNames.Talk, true, AnimationDirection.Forward, false);
        }

        // StopTalking
        public void StopTalking()
        {
            Stand();
        }

        // SuppressMoveBounceEffect
        [ScriptProperty]
        public bool SuppressMoveBounceEffect { get; set; }

        // ThrowCarriedProp
        public void ThrowCarriedProp(GameThing target)
        {
            if (CarriedProp == null)
                return;

            StopMoving();
            FaceTo(target);
            var state = BodyMachine.FindOrCreateState<ActorThrowObjectState>();
            state.Target = target;
            state.Prop = CarriedProp;
            CarriedProp = null;
            BodyMachine.ChangeState(state.GetType());
        }

        // Verb
        public override Verb Verb
        {
            get
            {
                if (IsPlayer && CarriedProp != null)
                    return Verb.Drop;

                if (Faction == Faction.Evil)
                    return Verb.Attack;

                return DefaultVerb;
            }
        }
    }
}