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

        private const float attackLaneThickness = 4;
        private Sprite? carriedPropSprite;
        private readonly List<AtlasImage>? customGuts;
        private ParticlePopEffect? footstepEffect;
        private SpriteFrame? footstepLastUsedFrame;
        private readonly AnimatedSprite headSprite;
        private readonly FloatTween headTween = new();
        private readonly FloatTween moveBalancingTween = new();
        private readonly FloatTween moveVerticalTween = new();
        private readonly List<Vector2> pendingPathNodes = [];
        private int reactionTimer;
        private SpeechBubble? speechBubble;

        #endregion

        #region Constructor

        // Constructor
        public Actor(GameSession session, string name)
            : base(session, name)
        {
            this.Definition = ActorDefinition.Definitions.Find(DeclaredName);
            this.Atlas = Atlases.Actors;
            this.ApproachBehavior = ApproachBehavior.FaceToFace;
            this.DeathWord = ImpactWordName.PlopRed;
            this.DisplayNameKey = $"Actor.{DeclaredName}";
            this.HitEffect = HitEffect.Blink;
            this.IgnoreWalkArea = false;
            this.Faction = Definition == null || Definition.Role == ActorRole.Interactive ? Faction.Good : Faction.Evil;

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
            if (!IsPlayer || !IsInCurrentRoom)
                return;

            // Session is busy
            if (Session.State != GameSessionState.Idle)
                return;

            Session.InteractionData.Execute(Session);
        }

        // MoveToNextPathNode
        private void MoveToNextPathNode()
        {
            base.MoveTo(pendingPathNodes[0]);
            pendingPathNodes.RemoveAt(0);
        }

        // React
        private void React()
        {
            if (Brain.Decide(this, Session.Player) is CombatDecision decision && decision.Target is { } target)
            {
                if (decision.Type == CombatDecisionType.Charge)
                {
                    PerformChargeReaction(target);
                }
                else if (decision.Type == CombatDecisionType.Flee)
                {
                    PerformFleeReaction(target);
                }
                else if (decision.Type == CombatDecisionType.Move)
                {
                    PerformMoveReaction(target);
                }
                else if (decision.Type == CombatDecisionType.Attack && decision.Intent != null)
                {
                    PerformAttack(decision.Intent, target);
                }
            }
        }

        // ResetHeadTween
        private void ResetHeadTween()
        {
            headTween.Start(TweenStyle.CubicInOut, 0, .25f, 400, -1);
            headTween.RandomizeTime();
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

        // UpdateReactionTimer
        private void UpdateReactionTimer(GameTime gameTime)
        {
            // Can use brain?
            if (IsPlayer || CombatBehavior == null || !IsAngry)
                return;

            // Can update timer?
            if (IsMoving || Session.IsAwaiting || IsAttacking)
                return;

            reactionTimer -= gameTime.ElapsedGameTime.Milliseconds;
            if (reactionTimer <= 0)
            {
                React();
                reactionTimer = CombatBehavior.Archetype.GetNextCooldown();
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

            if (Sprite.Player.Frame == null || Sprite.Player.Frame == footstepLastUsedFrame || !Sprite.Player.Frame.IsEvent)
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

        #endregion

        #region Protected members

        // BodyMachine
        protected BodyStateMachine BodyMachine { get; }

        // CalculateSpeed
        protected override float CalculateSpeed()
        {
            return base.CalculateSpeed() * (FastMove ? FastMoveFactor : 1);
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

        // OnActivate
        protected override void OnActivate()
        {
            base.OnActivate();

            if (reactionTimer <= 0)
                reactionTimer = CombatBehavior?.Archetype.GetNextCooldown() ?? 2000;
        }

        // OnCollisioning
        protected override void OnCollisioning(GameThing thing, out bool handled)
        {
            handled = thing.IsMoving;
        }

        // OnDie
        protected override void OnDie()
        {
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

            ShowImpactWord(ImpactWordName.PlopRed);
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (moveVerticalTween.IsRunning)
                Y -= moveVerticalTween.CurrentValue;

            if (moveBalancingTween.IsRunning)
                Rotation += moveBalancingTween.CurrentValue;

            base.OnDraw(gameTime);

            if (carriedPropSprite?.RenderImage != null)
            {
                carriedPropSprite.Position = RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.Top, 0, 1);
                carriedPropSprite.Draw(gameTime);
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
        }

        // OnTakeDamage
        protected override void OnTakeDamage(GameThing attacker, int amount, DamageType damageType)
        {
            if (!IsDead)
                FaceTo(attacker);

            if (IsPlayer)
            {
                Session.InterruptAwaitingScript();
            }
            else if (IsHostile(attacker) && !IsDead)
            {
                IsAngry = true;
            }

            Session.ObjectPools.FloatingTexts.Get()?.ShowHPAmount(this, amount, true);

            Session.Camera.Shake(TweenStyle.Linear, Vector2.One, 40, 6);

            if (HurtVoice != null)
                PlaySound(HurtVoice);

            if (Sprite.Animations.Contains(ActorStateNames.Hurt))
            {
                Stand();
                var state = BodyMachine.FindOrCreateState<BodyHurtState>();
                BodyMachine.ChangeState(state.GetType());
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            headTween.Update(gameTime);

            if (AnimationSettings.DetachedHead)
                headSprite.Update(gameTime);

            speechBubble?.Update(gameTime);
            moveVerticalTween.Update(gameTime);
            moveBalancingTween.Update(gameTime);
            UpdateDirection();
            UpdateFootstep();
            footstepEffect?.Update(gameTime);
            BodyMachine.Update(gameTime);

            UpdateReactionTimer(gameTime);

            if (IsPlayer && CarriedProp != null && Session.IsCurrentScene && !Session.IsAwaiting && !IsMoving && CanHandleInput)
                FaceToMouseCursor();
        }

        // PerformChargeReaction
        protected virtual void PerformChargeReaction(GameThing target)
        {
            MoveTo(target.Position);
        }

        // PerformFleeReaction
        protected virtual void PerformFleeReaction(GameThing target)
        {
            if (Direction == FacingDirection.Left)
                MoveTo(new(X + 30, Y));
            else
                MoveTo(new(X - 30, Y));
        }

        // PerformMoveReaction
        protected virtual void PerformMoveReaction(GameThing target)
        {
            var arch = CombatBehavior?.Archetype;
            if (arch == null)
                return;

            // 1. Dirección: ¿Dónde está el bicho respecto al jugador?
            Vector2 direction = Position - target.Position;
            float currentDistance = direction.Length();

            // Evitamos división por cero si están exactamente en el mismo pixel
            direction = currentDistance > 0 ? direction / currentDistance : new Vector2(1, 0);

            // 2. Distancia Ideal: El centro de su "zona de confort" definida en el arquetipo.
            float idealDistance = (arch.MinComfortDistance + arch.MaxComfortDistance) / 2f;

            // 3. El punto destino: Es la posición del jugador más el vector de dirección 
            // por la distancia que al bicho le gusta mantener.
            Vector2 finalTarget = target.Position + (direction * idealDistance);

            MoveTo(finalTarget);
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

        // ApproachAndInteract
        public bool ApproachAndInteract(GameThing target, Item? item)
        {
            if (!IsPlayer)
                return false;

            if (!MouseCursor.IsEnabled)
            {
                Session.HUD.Message.Show(MessageKind.HandsFull);
                return false;
            }

            if (item == null)
            {
                if (Session.InteractionContext.HeadbuttMode)
                    Session.InteractionData.SetHeadbuttOutcome(target);
                else
                    Session.InteractionData.SetDefaultOutcome(target);
            }
            else
            {
                Session.InteractionData.SetUseWithOutcome(target, item);
            }

            if (Session.InteractionData.Script == null)
            {
                MouseCursor.Shake();
                return false;
            }

            var destination = target.GetApproachPosition(this, Session.InteractionData.InteractionType == InteractionType.Headbutt ? ApproachBehavior.ClosestSide : null);
            var result = target != this && MoveTo(destination);

            if (result && !Session.InteractionContext.HeadbuttMode && target is Actor actor && actor.IsAngry)
                result = false;

            if (!result)
                HandlePendingInteraction();

            return true;
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
            if (IsPlayer && Session.AwaitingScript != null)
            {
                if (Session.AwaitingScript.Interruptible)
                {
                    return base.CanTakeDamage();
                }
                //else if (Session.OutcomeTarget is Actor actor && actor.IsAttacking)
                //{
                //    return base.CanTakeDamage();
                //}

                return false;
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
                        carriedPropSprite.RenderImage = Atlases.Environment.FindImage(field.DeclaredName);
                        field.Unparent();
                        Stand();
                    }
                }
            }
        }

        // CarriedPropPosition
        public Vector2? CarriedPropPosition => carriedPropSprite?.Position;

        // Cast
        public bool Cast(GameThing target, Item item)
        {
            if (!IsPlayer)
                return false;

            if (item.Definition.FaithCost == 0)
                return false;

            Session.InteractionData.SetCastOutcome(target, item);

            HandlePendingInteraction();

            return true;
        }

        // Definition
        public ActorDefinition? Definition { get; }

        // Faction
        [ScriptProperty]
        public Faction Faction { get; set; }

        // FastMove
        public bool FastMove { get; set; }

        // FastMoveFactor
        [ScriptProperty(CodingContext.EntityDeclaration)]
        public float FastMoveFactor { get; set; } = 1;

        // FootstepSound
        [ScriptProperty]
        public Sound? FootstepSound { get; set; }

        // Guts
        [ScriptProperty]
        public int Guts { get; set; } = 3;

        // HandleInput
        public HandleInputResult HandleInput()
        {
            if (InputHandler == null || Session.IsAwaiting || !IsPlayer || !CanHandleInput)
                return HandleInputResult.Unhandled;

            if (InputHandler != null)
                return InputHandler.HandleInput();

            return HandleInputResult.Unhandled;
        }

        // HasSpeechBubble
        public bool HasSpeechBubble => speechBubble != null && speechBubble.State != SpeechBubbleState.Hidden;

        // HurtVoice
        [ScriptProperty]
        public Sound? HurtVoice { get; set; }

        // IsAngry
        [ScriptProperty]
        public bool IsAngry
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    
                    if (value)
                    {
                        Faction = Faction.Evil;
                        reactionTimer = 1;

                        if (Session.Guard == this)
                            Session.HUD.GuardMeter.Target = this;
                    }
                }
            }
        }

        // IsAttacking
        public bool IsAttacking => BodyMachine.CurrentState is BodyCloseAttackState;

        // IsCornered
        public bool IsCornered(GameThing target)
        {
            if (Room?.WalkArea == null)
                return false;

            var destX = Direction == FacingDirection.Left ? int.MaxValue : int.MinValue;
            var destination = Room.WalkArea.ClampInside(new(destX, Y));

            var distanceToTarget = float.MaxValue;
            if (target != null)
            {
                if (target.X < X)
                {
                    distanceToTarget = Vector2.Distance(target.RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.RightBottom), RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.LeftBottom));
                }
                else
                {
                    distanceToTarget = Vector2.Distance(target.RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.LeftBottom), RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.RightBottom));
                }
            }

            float distanceToWall;
            if (Direction == FacingDirection.Left)
                distanceToWall = Vector2.Distance(RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.RightBottom), destination);
            else
                distanceToWall = Vector2.Distance(RuntimeHotspot.BoundingRectangleF.GetPoint(RectanglePoint.LeftBottom), destination);

            return distanceToWall < 40 && distanceToTarget < 20;
        }

        // IsInAttackLane
        public bool IsInAttackLane(GameThing target)
        {
            // CONDICIÓN Y: Debe estar en mi misma línea de profundidad
            float dy = Math.Abs(Position.Y - target.Y);
            return dy <= attackLaneThickness;
        }

        // IsFollowingPath
        public bool IsFollowingPath { get; private set; }

        // IsHostile
        public virtual bool IsHostile(GameThing other)
        {
            return this.Faction == Faction.Evil && other == Session.Player;
        }

        // IsPlayer
        [ScriptProperty]
        public bool IsPlayer => Session.Player == this;

        // IsStandingOrMoving
        public bool IsStandingOrMoving => BodyMachine.CurrentState is BodyStandState or BodyMoveState;

        // MoveRandomly
        [ScriptMethod]
        public void MoveRandomly()
        {
            if (Room?.WalkArea is WalkArea walkArea)
            {
                var margin = 0;// Math.Abs(Position.X - RuntimeHotspot.BoundingRectangleF.Right);
                MoveTo(walkArea.RandomWalkablePoint(margin));
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

        // PerformAttack
        public void PerformAttack(CombatIntent intent, GameThing? target)
        {
            StopMoving();
            if (target != null)
                FaceTo(target);

            var state = BodyMachine.FindOrCreateState<BodyCloseAttackState>();
            state.Intent = intent;
            state.Target = target;
            BodyMachine.ChangeState(state.GetType());
        }

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

        // RandomMoveAggressiveness
        [ScriptProperty]
        public float RandomMoveAggressiveness
        {
            get;
            set
            {
                if (value != field)
                    field = float.Clamp(value, 0, 1);
            }
        }

        // Say
        public void Say(string text, bool awaitInput)
        {
            speechBubble ??= new SpeechBubble(this);
            speechBubble.Show(DisplayName, text, awaitInput);
        }

        // SpeechBubbleSound
        [ScriptProperty(CodingContext.EntityDeclaration)]
        public Sound? SpeechBubbleSound { get; set; }

        // Stand
        [ScriptMethod()]
        public void Stand()
        {
            BodyMachine.ChangeState<BodyStandState>();
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

        // ThrowCarriedProp
        public void ThrowCarriedProp()
        {
            if (CarriedProp is null)
                return;

            var state = BodyMachine.FindOrCreateState<ActorThrowObjectState>();
            state.Prop = CarriedProp;
            BodyMachine.ChangeState(state.GetType());

            carriedPropSprite?.RenderImage = null;
            CarriedProp = null;
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
