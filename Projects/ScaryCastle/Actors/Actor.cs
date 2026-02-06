using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// Actor
    /// </summary>
    public class Actor : GameThing, IInputHandler
    {
        #region Private fields

        private readonly List<AtlasImage>? customGuts;
        private ParticlePopEffect? footstepEffect;
        private SpriteFrame? footstepLastUsedFrame;
        private readonly AnimatedSprite headSprite;
        private readonly FloatTween headTween = new();
        private readonly FloatTween moveBalancingTween = new();
        private readonly FloatTween moveVerticalTween = new();
        private GameThing? pendingInteractiveTarget;
        private Item? pendingInteractiveTargetItem;
        private readonly List<Vector2> pendingPathNodes = [];
        private readonly GameSession session;
        private SpeechBubble? speechBubble;
        private readonly ActorStandState standState;

        #endregion

        #region Constructor

        // Constructor
        public Actor(GameSession session, string name)
            : base(session, name)
        {
            this.session = session;
            this.Atlas = Atlases.Actors;
            this.ApproachBehavior = ApproachBehavior.FaceToFace;
            this.CombatBehavior = CombatBehavior.Find(DeclaredName);
            this.DisplayNameKey = $"Actor.{DeclaredName}";
            this.IgnoreWalkArea = false;

            headSprite = new AnimatedSprite(Game)
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

            this.standState = new ActorStandState(this);

            this.StateMachine = new ActorStateMachine(this, standState);
            this.StateMachine.RegisterState(new ActorAnimateState(this));
            this.StateMachine.RegisterState(new ActorDeathState(this));
            this.StateMachine.RegisterState(new ActorHurtState(this));
            this.StateMachine.RegisterState(new ActorMoveState(this));

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
        }

        #endregion

        #region Private members

        // HandlePendingInteraction
        private void HandlePendingInteraction()
        {
            if (!IsPlayer)
                return;

            if (pendingInteractiveTarget != null)
            {
                session.InteractionContext.HeldItem = null;
                FaceTo(pendingInteractiveTarget);
                Interact(pendingInteractiveTarget, pendingInteractiveTargetItem);
            }

            pendingInteractiveTarget = null;
            pendingInteractiveTargetItem = null;
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
        }

        // SyncHeadAnimation
        private void SyncHeadAnimation()
        {
            if (headSprite.Player.Animation?.Name != StateMachine.CurrentState.Name || !headSprite.Player.IsPlaying)
            {
                headSprite.Player.Stop();
                ResetHeadTween();
                if (headSprite.Animations.Find(StateMachine.CurrentState.Name) != null)
                    headSprite.Player.Play(StateMachine.CurrentState.Name);
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
                    footstepEffect ??= new(Game);
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

        // InputHandler
        protected InputHandler? InputHandler { get; set; }

        // OnCollisioning
        protected override void OnCollisioning(GameThing thing, out bool handled)
        {
            handled = thing is Coin;
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
                        ActorSize.Small => Vector2.One,
                        ActorSize.Medium => Vector2.One,
                        ActorSize.Large => Vector2.One,
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
                            ActorSize.Small => guts.PlaySound(SoundNames.GutsSmall),
                            ActorSize.Medium => guts.PlaySound(SoundNames.GutsMedium),
                            _ => guts.PlaySound(SoundNames.GutsLarge)
                        };
                    }

                    Unparent();
                }
            }
            else
            {
                StateMachine.ChangeState(ActorStateNames.Death);
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

            if (AnimationSettings.DetachedHead)
            {
                if (StateMachine.CurrentState == standState && headSprite.Player.IsPlaying)
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
            StateMachine.ChangeState(ActorStateNames.Move);

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

            if (!IsDead)
                Stand();
        }

        // OnTakeDamage
        protected override void OnTakeDamage(GameThing attacker, int amount, DamageType damageType, Vector2 knockback)
        {
            session.Camera.Shake(TweenStyle.Linear, Vector2.One, 40, 6);

            //FaceTo(attacker);

            if (HurtVoice != null)
                PlaySound(HurtVoice);

            if (Sprite.Animations.Contains(ActorStateNames.Hurt))
            {
                Stand();
                StateMachine.ChangeState(ActorStateNames.Hurt);
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
            StateMachine.Update(gameTime);
        }

        // OnWillpowerChanged
        protected virtual void OnWillpowerChanged()
        {
        }

        // StateMachine
        protected ActorStateMachine StateMachine { get; }

        #endregion

        // AnimationSettings
        public ActorAnimationSettings AnimationSettings { get; } = new();

        // Animate
        public SpriteAnimation? Animate(string animationName)
        {
            return Animate(animationName, false, AnimationDirection.Forward, false);
        }

        // Animate
        public SpriteAnimation? Animate(string animationName, bool loop, AnimationDirection direction, bool preserve)
        {
            var result = AnimationPlayer.Play(animationName, loop, direction);
            if (result != null && StateMachine.FindState(ActorStateNames.Animate) is ActorAnimateState animateState)
            {
                animateState.Preserve = preserve;
                StateMachine.ChangeState(animateState.Name, true);
            }

            return result;
        }

        // ApproachAndInteract
        public void ApproachAndInteract(GameThing target, Item? item)
        {
            if (!IsPlayer)
                return;

            var destination = target.GetApproachPosition(this);
            var result = target != this && MoveTo(destination);
            this.pendingInteractiveTarget = target;
            this.pendingInteractiveTargetItem = item;

            if (!result)
                HandlePendingInteraction();
        }

        // BodySize
        public ActorSize BodySize { get; set; } = ActorSize.Medium;

        // CanChangeState
        public bool CanChangeState
        {
            get
            {
                if (IsDead || session.IsAwaiting)
                    return false;

                return StateMachine.CurrentState is ActorStandState or ActorMoveState;
            }
        }

        // CombatBehavior
        public CombatBehavior? CombatBehavior { get; }

        // GetCombatIntent
        public CombatIntentDescriptor? GetCombatIntent()
        {
            return CombatBehavior == null ? null : Brain.Decide(CombatBehavior, HP, MaxHP);
        }

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
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (InputHandler == null || Session.IsAwaiting || !IsPlayer || !CanChangeState)
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

        // HitEffect
        public override HitEffect HitEffect => HitEffect.Blink;

        // HurtVoice
        [ScriptProperty]
        public Sound? HurtVoice { get; set; }

        // Interact
        public bool Interact(GameThing target, Item? item)
        {
            if (!IsInCurrentRoom)
                return false;

            // Session is busy
            if (Session.State != GameSessionState.Idle)
                return false;

            var script = item == null ? target.OutcomeScript : session.ScriptLibrary.FindOverload(target.DeclaredName, item.Name);

            if (script != null)
            {
                StopMoving();
                Session.BeginOutcome(script, target);
                return true;
            }
            else
            {
                return false;
            }
        }

        // IsFollowingPath
        public bool IsFollowingPath { get; private set; }

        // IsPlayer
        [ScriptProperty]
        public bool IsPlayer => Session.Player == this;

        // IsStandingOrMoving
        public bool IsStandingOrMoving => StateMachine.CurrentState is ActorStandState or ActorMoveState;

        // IsWalkAreaHole
        public override bool IsWalkAreaHole => false;

        // MoveTo
        public override bool MoveTo(Vector2 destination)
        {
            pendingInteractiveTarget = null;
            pendingInteractiveTargetItem = null;

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

            StateMachine.ChangeState(ActorStateNames.Move);

            return true;
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

        // Say
        public void Say(string text, bool awaitInput)
        {
            speechBubble ??= new SpeechBubble(this);
            speechBubble.Show(LocalizedDisplayName, text, awaitInput);
        }

        // SpeechBubbleSound
        [ScriptProperty(CodingContext.EntityDeclaration)]
        public Sound? SpeechBubbleSound { get; set; }

        // Stand
        [ScriptMethod()]
        public void Stand(bool forceRestart = false)
        {
            StateMachine.ChangeState(ActorStateNames.Stand, forceRestart);
        }

        // StartTalking
        public void StartTalking()
        {
            if (AnimationSettings.DetachedHead)
                headSprite.Player.Play(ActorStateNames.Talk, true);
            else
                Animate("Talk", true, AnimationDirection.Forward, false);
        }

        // StopTalking
        public void StopTalking()
        {
            if (AnimationSettings.DetachedHead)
                headSprite.Player.Play(StateMachine.CurrentState.Name, true);
            else
                Stand(true);
        }

        /// <summary>
        /// ActorStateMachine
        /// </summary>
        public sealed class ActorStateMachine(Actor owner, ActorState initialState)
            : StateMachine<Actor, ActorState>(owner, initialState)
        {
            // OnStateChanged
            protected override void OnStateChanged()
            {
                Owner.SyncHeadAnimation();
            }
        }

        // ActorAnimationSettings
        public sealed class ActorAnimationSettings
        {
            // DetachedHead
            public bool DetachedHead { get; set; } = true;

            // MoveBounce
            public bool MoveBounce { get; set; } = true;

            // MoveSway
            public bool MoveSway { get; set; } = true;

            // SupressAll
            public void SupressAll()
            {
                DetachedHead = false;
                MoveBounce = false;
                MoveSway = false;
            }
        }
    }
}
