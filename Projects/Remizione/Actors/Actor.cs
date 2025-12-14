using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// Actor
    /// </summary>
    public class Actor : GameThing, IInputHandler
    {
        #region Private fields

        private readonly ActorCloseAttackState closeAttackState;
        private readonly ActorConsumeState consumeState;
        private readonly ActorContactDamageState contactDamageState;
        private readonly List<AtlasImage>? customGuts;
        private ParticlePopEffect? footstepEffect;
        private SpriteFrame? footstepLastUsedFrame;
        private readonly AnimatedSprite headSprite;
        private readonly FloatTween headTween = new();
        private readonly FloatTween moveBalancingTween = new();
        private readonly FloatTween moveVerticalTween = new();
        private readonly List<Vector2> pendingPathNodes = [];
        private readonly GameSession session;
        private readonly ShadowSpot shadowSpot;
        private SpeechBubble? speechBubble;
        private readonly ActorStandState standState;
        private int suspendInteractionCooldown;
        private readonly ActorThrowItemState throwItemState;

        #endregion

        #region Constructor

        // Constructor
        public Actor(GameSession session, string name)
            : base(session, name)
        {
            this.session = session;

            this.Atlas = Atlases.Actors;
            this.DisplayNameKey = $"Actor.{StaticName}";
            this.HitEffect = HitEffect.Blink;
            this.IgnoreWalkArea = false;
            this.shadowSpot = new ShadowSpot(this);
            this.PerceptionSensor = new PerceptionSensor(this);

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
            this.closeAttackState = new ActorCloseAttackState(this);

            this.StateMachine = new ActorStateMachine(this, standState);
            this.StateMachine.RegisterState(new ActorAnimateState(this));
            this.StateMachine.RegisterState(new ActorDeathState(this));
            this.StateMachine.RegisterState(new ActorHurtState(this));
            this.StateMachine.RegisterState(new ActorMoveState(this));
            this.StateMachine.RegisterState(closeAttackState);

            throwItemState = new ActorThrowItemState(this);
            this.StateMachine.RegisterState(throwItemState);

            consumeState = new ActorConsumeState(this);
            this.StateMachine.RegisterState(consumeState);

            contactDamageState = new ActorContactDamageState(this);
            this.StateMachine.RegisterState(contactDamageState);

            this.AIStateMachine = new(this);

            if (Atlas?.GetImage(Sprite.ImagePath + "Gut0") != null)
            {
                var index = 0;
                customGuts = [];

                while (true)
                {
                    if (Atlas.GetImage(Sprite.ImagePath + $"Gut{index}") is AtlasImage image)
                        customGuts.Add(image);
                    else
                        break;

                    index++;
                }
            }
        }

        #endregion

        #region Private members

        // ApplyContactDamage
        private void ApplyContactDamage(GameThing attacker)
        {
            if (session.IsAwaiting || !CanChangeState || attacker.ContactDamageType == DamageType.None)
                return;

            if (!CanTakeDamage(attacker))
                return;

            if (attacker.ContactDamageEffect == null)
                return;

            Stand();

            attacker.ContactDamageEffect.ApplyDamage(attacker, this);

            StateMachine.ChangeState(contactDamageState.Name);
        }

        // InflictContactDamage
        private bool InflictContactDamage()
        {
            if (session.IsAwaiting)
                return false;

            if (ContactDamageType == DamageType.None)
                return false;

            if (Room == null)
                return false;

            for (int i = 0; i < Room.CulledThings.Count; i++)
            {
                // Skip if it is the same thing
                if (Room.CulledThings[i] == this)
                    continue;

                if (Room.CulledThings[i] is Actor actor)
                {
                    if (actor.IsDead)
                        continue;

                    if (!IsEnemy(actor))
                        continue;

                    if (ContactDamagePolygon == TestPolygon.Collider)
                    {
                        if (RuntimeCollider.Contains(actor.Position))
                        {
                            actor.ApplyContactDamage(this);
                            return true;
                        }
                    }
                    else if (RuntimeHotspot.ContainsVertex(actor.RuntimeHotspot))
                    {
                        actor.ApplyContactDamage(this);
                        return true;
                    }
                }
            }

            return false;
        }

        // FindInteractiveTarget
        private GameThing? FindInteractiveTarget()
        {
            if (session.IsAwaiting || SpeechBubble.ModalInstance != null || !session.IsCurrentScene || Room == null)
                return null;

            for (int i = Room.CulledThings.Count - 1; i >= 0; i--)
            {
                if (Room.CulledThings[i] == this)
                    continue;

                if (Room.CulledThings[i] is GameThing target && target.CanInteract(this))
                    return target;
            }

            return null;
        }

        // MoveToNextPathNode
        private void MoveToNextPathNode()
        {
            base.MoveTo(pendingPathNodes[0]);
            pendingPathNodes.RemoveAt(0);
        }

        // ConsumeItem
        private bool ConsumeItem(Item item)
        {
            Stand();
            consumeState.Item = item;
            StateMachine.ChangeState(consumeState.Name);

            return true;
        }

        // PlaceItem
        private void PlaceItem(Item item)
        {
            if (Session.ObjectPools.GetPlacedItem(item.Name) is PlacedItem placedItem)
                placedItem.Place(this, item, Position);
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

        // ThrowItem
        private void ThrowItem(Item item)
        {
            //Stand();
            throwItemState.Item = item;
            StateMachine.ChangeState(throwItemState.Name);
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

        // AIStateMachine
        protected AIStateMachine AIStateMachine { get; }

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

        // OnDrawShadow
        protected override void OnDrawShadow(GameTime gameTime)
        {
            shadowSpot.Draw(gameTime);
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
        protected override void OnTakeDamage(GameThing attacker, int damage, DamageType damageType, Vector2 knockback)
        {
            if (IsPlayer)
            {
                Game.SceneManager.PopUntil(Session);

                var fullHearts = damage / 2;
                var hasHalfHeart = damage % 2 == 1;

                for (var i = 0; i < fullHearts; i++)
                {
                    Session.ObjectPools.FloatingHearts.Get()?.Show(GetFloatingTextPosition(knockback), false);
                }

                if (hasHalfHeart)
                    Session.ObjectPools.FloatingHearts.Get()?.Show(GetFloatingTextPosition(knockback), true);
            }

            LastKnownAttacker = attacker;
            FaceTo(attacker);

            /*
            if (Sprite.Animations.Contains(ActorStateNames.Hurt))
            {
                Stand();
                StateMachine.ChangeState(ActorStateNames.Hurt);
            }
            */
        }

        // OnUnload
        protected override void OnUnload()
        {
            InteractiveTarget = null;
            LastKnownAttacker = null;
            base.OnUnload();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (suspendInteractionCooldown > 0 && !session.IsAwaiting)
                suspendInteractionCooldown -= gameTime.ElapsedGameTime.Milliseconds;

            this.InteractiveTarget = null;
            if (IsPlayer && suspendInteractionCooldown <= 0 && !session.IsAwaiting)
                this.InteractiveTarget = FindInteractiveTarget();

            headTween.Update(gameTime);

            InflictContactDamage();

            if (AnimationSettings.DetachedHead)
                headSprite.Update(gameTime);

            shadowSpot.Update(gameTime);
            speechBubble?.Update(gameTime);
            moveVerticalTween.Update(gameTime);
            moveBalancingTween.Update(gameTime);
            UpdateDirection();
            UpdateFootstep();
            footstepEffect?.Update(gameTime);

            if (AIStateMachine.CurrentState != null)
                PerceptionSensor.Update(gameTime);

            AIStateMachine.Update(gameTime);
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
            if (result != null && StateMachine.GetState(ActorStateNames.Animate) is ActorAnimateState animateState)
            {
                animateState.Preserve = preserve;
                StateMachine.ChangeState(animateState.Name, true);
            }

            return result;
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

        // HotspotDetectorArea
        [ScriptProperty]
        public Rectangle HotspotDetectorArea { get; set; }

        // Interact
        public bool Interact(GameThing? target = null)
        {
            target ??= InteractiveTarget;

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

        // IsFollowingPath
        public bool IsFollowingPath { get; private set; }

        // IsPlayer
        [ScriptProperty]
        public bool IsPlayer => Session.Player == this;

        // IsStandingOrMoving
        public bool IsStandingOrMoving => StateMachine.CurrentState is ActorStandState or ActorMoveState;

        // IsWalkAreaHole
        public override bool IsWalkAreaHole => false;

        // LastKnownAttacker
        public GameThing? LastKnownAttacker { get; set; }

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

            StateMachine.ChangeState(ActorStateNames.Move);

            return true;
        }

        // PerceptionSensor
        public PerceptionSensor PerceptionSensor { get; }

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

        // ShadowOffset
        [ScriptProperty]
        public Vector2 ShadowOffset
        {
            get => shadowSpot.Offset;
            set => shadowSpot.Offset = value;
        }

        // ShadowSpotSize
        [ScriptProperty]
        public int ShadowSpotSize
        {
            get => shadowSpot.Size;
            set => shadowSpot.Size = value;
        }

        // ShowFloatingText
        public void ShowFloatingText(string text, Color color, int duration = 1000)
        {
            if (Session.ObjectPools.FloatingTexts.Get() is FloatingText floatingText)
                floatingText.Show(GetOverheadPosition(), text, color, duration);
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

        // SuspendInteraction
        public void SuspendInteraction(int duration)
        {
            CodeContract.GreaterThanZero(duration, nameof(duration));
            suspendInteractionCooldown = duration;
            InteractiveTarget = null;
        }

        // UseEquippedItem
        public void UseEquippedItem(ItemCategory category)
        {
            if (session.Inventory.GetEquippedItem(category) is not Item item)
                return;

            if (item.Count <= 0)
                return;

            if (!CanChangeState)
                return;

            // Place
            if (item.MetaItem.Action == ItemAction.Place)
                PlaceItem(item);

            // Throwable
            else if (item.MetaItem.Action == ItemAction.Throw)
                ThrowItem(item);
        }

        // WhooshSound
        [ScriptProperty]
        public Sound? WhooshSound { get; set; }

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
