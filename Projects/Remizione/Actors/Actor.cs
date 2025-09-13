using Adberration;
using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Engendro.Input;
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
        private readonly ActorCloseAttackState closeAttackState;
        private readonly ActorConsumeState consumeState;
        private readonly List<AtlasImage>? customGuts;
        private ParticlePopEffect? footstepEffect;
        private SpriteFrame? footstepLastUsedFrame;
        private readonly AnimatedSprite headSprite;
        private readonly FloatTween headTween = new();
        private InventoryScene? inventoryScene;
        private readonly FloatTween moveBalancingTween = new();
        private readonly FloatTween moveVerticalTween = new();
        private readonly List<Vector2> pendingPathNodes = [];
        private PlayerNumber playerNumber = PlayerNumber.None;
        private readonly GameSession session;
        private readonly ShadowSpot shadowSpot;
        private readonly ActorShockZapState shockZapState;
        private SpeechBubble? speechBubble;
        private readonly ActorStandState standState;
        private int suspendInteractionCooldown;
        private readonly ActorThrowItemState throwItemState;
        private float tinyMoveSpeedFactor = 1;
        private UseFriendlyItemScene? useFriendlyItemScene;

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
            this.Inventory = new(this);
            this.shadowSpot = new ShadowSpot(this);

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
            this.StateMachine.RegisterState(new ActorPickUpState(this));
            this.StateMachine.RegisterState(closeAttackState);

            throwItemState = new ActorThrowItemState(this);
            this.StateMachine.RegisterState(throwItemState);

            consumeState = new ActorConsumeState(this);
            this.StateMachine.RegisterState(consumeState);

            shockZapState = new ActorShockZapState(this);
            this.StateMachine.RegisterState(shockZapState);

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

        // FindInteractiveTarget
        private GameThing? FindInteractiveTarget()
        {
            if (session.IsAwaiting || SpeechBubble.ModalInstance != null || !session.IsCurrentScene || Room == null)
                return null;

            for (int i = Room.CulledThings.Count - 1; i >= 0; i--)
            {
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

        // PerformConsumeAction
        private bool PerformConsumeAction()
        {
            /*
            if (!CanChangeState)
                return false;

            if (Inventory.SelectedItem == null || Inventory.SelectedItem.MetaItem.Category != MetaItemCategory.Consumable || Inventory.SelectedItem.Count <= 0)
                return false;

            Stand();

            var itemToConsume = Inventory.SelectedItem;
            var nextItem = Inventory.SelectNext(MetaItemCategory.Consumable);
            if (nextItem == null || nextItem == itemToConsume)
                nextItem = Inventory.SelectNext(MetaItemCategory.Equipment);

            itemToConsume.Use();

            if (Inventory.SelectedItem == null && nextItem != null)
                Inventory.Select(nextItem);

            consumeState.Item = itemToConsume;
            StateMachine.ChangeState(consumeState.Name);
            */

            return true;
        }

        // PerformShockZap
        private void PerformShockZap(GameThing attacker)
        {
            if (session.IsAwaiting || !CanChangeState)
                return;

            Stand();

            InputManager.DefaultPlayer.GamePad.Vibrate(200, 1, 1);

            if (MetaItem.Find("ShockZap") is MetaItem metaItem)
                metaItem.ApplyDamage(attacker, this);

            StateMachine.ChangeState(shockZapState.Name);
        }

        // PerformThrowAction
        private bool PerformThrowAction()
        {
            if (!CanChangeState)
                return false;

            if (Inventory.Junk.SelectedItem is not Item item)
                return false;

            if (item.MetaItem.Action != ItemAction.Throw || item.Count <= 0)
                return false;

            //Stand();
            item.Use();
            throwItemState.Item = item;
            StateMachine.ChangeState(throwItemState.Name);
            return true;
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

        // AIStateMachine
        protected AIStateMachine AIStateMachine { get; }

        // CalculateSpeed
        protected override float CalculateSpeed() => base.CalculateSpeed() * (FastMove ? FastMoveFactor : 1) * tinyMoveSpeedFactor * (accelerationFactorTween.IsRunning ? accelerationFactorTween.CurrentValue : 1);

        // CanCheckCollisions
        protected override bool CanCheckCollisions() => !IsFollowingPath && base.CanCheckCollisions();

        // InputHandler
        protected InputHandler? InputHandler { get; set; }

        // OnCollision
        protected override void OnCollision(GameThing thing)
        {
            if (thing.ContactDamageKind == DamageKind.Lightning)
                PerformShockZap(thing);
        }

        // OnDie
        protected override void OnDie()
        {
            if (Guts > 0)
            {
                if (Room != null)
                {
                    var gutScale = BodySize switch
                    {
                        ActorSize.Small => Vector2.One,
                        ActorSize.Medium => Vector2.One * 1.25f,
                        _ => Vector2.One * 1.5f
                    };

                    var guts = new Guts(Session, Guts, gutScale, customGuts)
                    {
                        Position = Position,
                    };

                    Room.Children.Add(guts);

                    _ = BodySize switch
                    {
                        ActorSize.Small => guts.PlaySound(SoundNames.GutsSmall),
                        ActorSize.Medium => guts.PlaySound(SoundNames.GutsMedium),
                        _ => guts.PlaySound(SoundNames.GutsLarge)
                    };

                    Unparent();

                    if (IsPlayer)
                        Session.AwaitRoutine(RoutineNames.GameOver);
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

        // OnFindEnemy
        protected virtual GameThing? OnFindEnemy() => null;

        // OnHurt
        protected override void OnHurt(GameThing attacker, int damage, DamageKind damageKind, Vector2 knockback)
        {
            if (IsPlayer)
            {
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

            if (Sprite.Animations.Contains(ActorStateNames.Hurt))
            {
                Blinker.Stop();
                Stand();
                StateMachine.ChangeState(ActorStateNames.Hurt);
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
            }
        }

        // OnRead
        protected override void OnRead(XmlAttributeCollection attributes)
        {
            base.OnRead(attributes);

            // Consumables
            if (attributes[nameof(Inventory.Consumables)]?.Value is string consumablesData)
                Inventory.Consumables.SetSerializationData(consumablesData);

            // Junk
            if (attributes[nameof(Inventory.Junk)]?.Value is string junkData)
                Inventory.Junk.SetSerializationData(junkData);

            // KeyItems
            if (attributes[nameof(Inventory.KeyItems)]?.Value is string keyItemsData)
                Inventory.KeyItems.SetSerializationData(keyItemsData);

            // Traits
            if (attributes[nameof(Inventory.Traits)]?.Value is string traitsData)
                Inventory.Traits.SetSerializationData(traitsData);

            // Trinkets
            if (attributes[nameof(Inventory.Trinkets)]?.Value is string trinketsData)
                Inventory.Trinkets.SetSerializationData(trinketsData);
        }

        // OnStartMoving
        protected override void OnStartMoving()
        {
            StateMachine.ChangeState(ActorStateNames.Move);

            if (AnimationSettings.MoveBounce)
                moveVerticalTween.Start(TweenStyle.QuadraticInOut, 0, .8f, 100, -1);

            if (AnimationSettings.MoveSway)
                moveBalancingTween.Start(TweenStyle.QuadraticInOut, 0, .03f, FastMove ? 100 : 200, -1);

            accelerationFactorTween.Start(TweenStyle.Linear, .4f, 1, 150);
        }

        // OnStopMoving
        protected override void OnStopMoving()
        {
            base.OnStopMoving();

            FastMove = false;
            accelerationFactorTween.Stop();
            moveVerticalTween.Stop();
            moveBalancingTween.Stop();
            tinyMoveSpeedFactor = 1;

            if (!IsDead)
                Stand();
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
            AIStateMachine.Update(gameTime);

            StateMachine.Update(gameTime);

            base.OnUpdate(gameTime);

            if (suspendInteractionCooldown > 0 && !session.IsAwaiting)
                suspendInteractionCooldown -= gameTime.ElapsedGameTime.Milliseconds;

            this.InteractiveTarget = null;
            if (IsPlayer && suspendInteractionCooldown <= 0 && !session.IsAwaiting)
                this.InteractiveTarget = FindInteractiveTarget();

            accelerationFactorTween.Update(gameTime);
            headTween.Update(gameTime);

            if (AnimationSettings.DetachedHead)
                headSprite.Update(gameTime);

            shadowSpot.Update(gameTime);
            speechBubble?.Update(gameTime);
            moveVerticalTween.Update(gameTime);
            moveBalancingTween.Update(gameTime);
            UpdateDirection();
            UpdateFootstep();
            footstepEffect?.Update(gameTime);

            Inventory.Trinkets.SelectedItem?.Update(gameTime);
        }

        // OnWillpowerChanged
        protected virtual void OnWillpowerChanged()
        {
        }

        // OnWrite
        protected override void OnWrite(XmlWriter output)
        {
            base.OnWrite(output);

            output.WriteAttributeString(nameof(Inventory.Consumables), Inventory.Consumables.GetSerializationData());
            output.WriteAttributeString(nameof(Inventory.Junk), Inventory.Junk.GetSerializationData());
            output.WriteAttributeString(nameof(Inventory.KeyItems), Inventory.KeyItems.GetSerializationData());
            output.WriteAttributeString(nameof(Inventory.Traits), Inventory.Traits.GetSerializationData());
            output.WriteAttributeString(nameof(Inventory.Trinkets), Inventory.Trinkets.GetSerializationData());
        }

        // StateMachine
        protected ActorStateMachine StateMachine { get; }

        #endregion

        // Affinity
        [ScriptProperty]
        public Affinity Affinity { get; set; }

        // AnimationSettings
        public ActorAnimationSettings AnimationSettings { get; } = new();

        // Animate
        public SpriteAnimation? Animate(string animationName) => Animate(animationName, false, AnimationDirection.Forward, false);

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

                return StateMachine.CurrentState is ActorStandState ||
                       StateMachine.CurrentState is ActorMoveState;
            }
        }

        // CanSeeTarget
        public bool CanSeeTarget(GameThing target)
        {
            Vector2 toTarget = target.Position - Position;

            if (ViewDistance > 0 && toTarget.Length() > ViewDistance)
                return false;

            Vector2 directionToTarget = Vector2.Normalize(toTarget);
            Vector2 forward = Direction == FacingDirection.Right ? Vector2.UnitX : -Vector2.UnitX;

            float dot = Vector2.Dot(forward, directionToTarget);
            float angleThreshold = MathF.Cos(MathHelper.ToRadians(ViewAngle / 2f));

            return dot >= angleThreshold;
        }

        // ChooseFriendlyItem
        public bool ChooseFriendlyItem(string text)
        {
            Stand();
            useFriendlyItemScene ??= new UseFriendlyItemScene(this);

            if (Session.OutcomeTarget is Prop prop)
            {
                Session.FriendlyItemTarget = prop;
                useFriendlyItemScene.Text = text;
                useFriendlyItemScene.SceneController.Push();
                return true;
            }

            return false;
        }

        // FastMove
        public bool FastMove { get; set; }

        // FastMoveFactor
        [ScriptProperty(CodingContext.EntityDeclaration)]
        public float FastMoveFactor { get; set; } = 1;

        // FindEnemy
        public GameThing? FindEnemy()
        {
            var result = OnFindEnemy();
            
            if (result?.IsDead == true)
                result = null;  

            return result;
        }

        // FootstepSound
        [ScriptProperty]
        public Sound? FootstepSound { get; set; }

        // GoldenTickets
        [ScriptProperty]
        public int GoldenTickets { get; set; }

        // Guts
        [ScriptProperty]
        public int Guts { get; set; }

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

        // Inventory
        public Inventory Inventory { get; }

        // InventorySelectedItemName
        public string InventorySelectedItemName { get; set; } = string.Empty;

        // IsFollowingPath
        public bool IsFollowingPath { get; private set; }

        // IsPickingUp
        public bool IsPickingUp => StateMachine.CurrentState is ActorPickUpState;

        // IsPlayer
        public bool IsPlayer => Session.Player == this;

        // IsStandingOrMoving
        public bool IsStandingOrMoving => StateMachine.CurrentState is ActorStandState || StateMachine.CurrentState is ActorMoveState;

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

            StateMachine.ChangeState(ActorStateNames.Move);

            return true;
        }

        // PickUp
        public void PickUp(Pickup pickup, MetaItem? metaItem)
        {
            if (StateMachine.GetState(ActorStateNames.PickUp) is ActorPickUpState state)
            {
                state.Prepare(pickup, metaItem);
                StateMachine.ChangeState(state.Name);
            }
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

        // RedTickets
        [ScriptProperty]
        public int RedTickets { get; set; }

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

        // ShowInventory
        [ScriptMethod]
        public void ShowInventory()
        {
            Stand();
            inventoryScene ??= new InventoryScene(this);
            if (Session.Camera.Target == this)
                Session.Camera.FocusTarget();
            inventoryScene.SceneController.Push();
        }

        // SpeechBubbleSound
        [ScriptProperty(CodingContext.EntityDeclaration)]
        public Sound? SpeechBubbleSound { get; set; }

        // Stand
        [ScriptMethod()]
        public void Stand(bool forceRestart = false) => StateMachine.ChangeState(ActorStateNames.Stand, forceRestart);

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

        // UseSelectedItem
        public void UseSelectedItem()
        {
            if (Inventory.Junk.SelectedItem is Item item)
            {
                // Throwable
                if (item.MetaItem.Action == ItemAction.Throw)
                    PerformThrowAction();

                // Consume
                else if (item.MetaItem.Category == InventoryCategory.Consumables)
                    PerformConsumeAction();
            }
        }

        // WhiteTickets
        [ScriptProperty]
        public int WhiteTickets { get; set; }

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
