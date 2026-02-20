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
        private readonly List<Vector2> pendingPathNodes = [];
        private SpeechBubble? speechBubble;

        #endregion

        #region Constructor

        // Constructor
        public Actor(GameSession session, string name)
            : base(session, name)
        {
            this.Atlas = Atlases.Actors;
            this.ApproachBehavior = ApproachBehavior.FaceToFace;
            this.DisplayNameKey = $"Actor.{DeclaredName}";
            this.IgnoreWalkArea = false;
            this.SuppressImpactWordOnDeath = true;

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

        #region Private members

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

        // ResetHeadTween
        private void ResetHeadTween()
        {
            headTween.Start(TweenStyle.CubicInOut, 0, .25f, 400, -1);
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

        // BodyMachine
        protected BodyStateMachine BodyMachine { get; }

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

            if (AnimationSettings.DetachedHead)
            {
                if (BodyMachine.CurrentState is BodyStandState && headSprite.Player.IsPlaying)
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
        protected override void OnTakeDamage(GameThing attacker, int amount, DamageType damageType, Vector2 knockback)
        {
            if (!IsDead)
                FaceTo(attacker);

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

            if (IsPlayer && !Session.IsAwaiting && !IsMoving && CanHandleInput)
            {
                var mousePos = InputManager.DefaultPlayer.Mouse.WorldPosition(Session.Camera);
                FaceTo(mousePos);
            }
        }

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
            if (result != null)
            {
                var state = BodyMachine.FindOrCreateState<BodyAnimateState>();
                state.Preserve = preserve;
                BodyMachine.ChangeState(state.GetType());
            }

            return result;
        }

        // ApproachAndPlace
        public bool ApproachAndPlace(Vector2 destination, Item item)
        {
            if (!IsPlayer)
                return false;

            if (item.Definition.UsageMode != ItemUsageMode.Place)
                return false;

            var result = Position == destination;
            if (!result)
                result = MoveTo(destination);

            Session.InteractionData.SetPlaceOutcome(item);

            if (!result)
                HandlePendingInteraction();

            return true;
        }

        // ApproachAndInteract
        public bool ApproachAndInteract(GameThing target, Item? item)
        {
            if (!IsPlayer)
                return false;

            if (item != null && item.Definition.UsageMode != ItemUsageMode.Default)
                return false;

            if (item == null)
                Session.InteractionData.SetOutcome(target);
            else
                Session.InteractionData.SetUseWithOutcome(target, item);

            if (Session.InteractionData.Script == null)
                return false;

            var destination = target.GetApproachPosition(this);
            var result = target != this && MoveTo(destination);

            if (!result)
                HandlePendingInteraction();

            return true;
        }

        // Attack
        public void Attack(CombatIntent intent, GameThing? target)
        {
            StopMoving();
            if (target != null)
                FaceTo(target);

            if (intent.RangeAttack)
            {
            }
            else
            {
                var state = BodyMachine.FindOrCreateState<BodyCloseAttackState>();
                state.Intent = intent;
                state.Target = target;
                BodyMachine.ChangeState(state.GetType());
            }
        }

        // BodySize
        public ActorSize BodySize { get; set; } = ActorSize.Medium;

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
            if (IsPlayer && Session.AwaitingScript?.Interruptible == false)
                return false;

            return base.CanTakeDamage();
        }

        // Cast
        public bool Cast(Vector2 castPosition, Item item)
        {
            if (!IsPlayer)
                return false;

            if (item.Definition.UsageMode != ItemUsageMode.Cast)
                return false;

            if (Room?.WalkArea != null)
                castPosition = Room.WalkArea.ClampInside(castPosition);

            Session.InteractionData.SetCastOutcome(item, castPosition);

            HandlePendingInteraction();

            return true;
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
            if (InputHandler == null || Session.IsAwaiting || !IsPlayer || !CanHandleInput)
                return HandleInputResult.Unhandled;

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

        // IsAttacking
        public bool IsAttacking => BodyMachine.CurrentState is BodyAttackState;

        // IsFollowingPath
        public bool IsFollowingPath { get; private set; }

        // IsPlayer
        [ScriptProperty]
        public bool IsPlayer => Session.Player == this;

        // IsSneaking
        public bool IsSneaking { get; set; }

        // IsStandingOrMoving
        public bool IsStandingOrMoving => BodyMachine.CurrentState is BodyStandState or BodyMoveState;

        // IsWalkAreaHole
        public override bool IsWalkAreaHole => false;

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
            //StateMachine.ChangeState(ActorStateNames.Stand, forceRestart);
            BodyMachine.ChangeState<BodyStandState>();
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
                headSprite.Player.Play(AnimationNames.Stand, true);
            else
                Stand(true);
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
