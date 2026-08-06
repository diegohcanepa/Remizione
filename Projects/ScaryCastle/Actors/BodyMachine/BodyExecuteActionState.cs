using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// BodyExecuteActionState
    /// </summary>
    public sealed class BodyExecuteActionState : BodyAnimatedState
    {
        private bool animationFound;
        private bool eventDone;
        private static readonly string missText = TextRepository.GetValue("Misc.Miss");

        // Constructor
        public BodyExecuteActionState()
            : base(string.Empty, false)
        {
        }

        #region Private members

        // CanInflictDamage
        private bool CanInflictDamage(GameThing target)
        {
            if (target.CanBeHit() && Owner.AnimationPlayer.Frame?.IsTrigger == true)
            {
                if (Owner.IsInAttackLane(target))
                    return true;
            }

            return false;
        }

        // ResolveInPlaceAction
        private void ResolveInPlaceAction(IAction action)
        {
            if (action.InPlaceEffectType == InPlaceEffectType.None)
                return;

            if (action.InPlaceEffectType == InPlaceEffectType.Lightning)
            {
                if (Target != null && Owner.Room != null)
                {
                    var lightning = new LightningInvocation(action, Target);
                    Owner.Room.Children.Add(lightning);
                }
            }
        }

        // ResolveProjectileAction
        private void ResolveProjectileAction(IAction action)
        {
            if (action.Projectile == null)
                return;

            if (Owner.AnimationPlayer.Frame?.SpawnPoint is Vector2 actionPoint && actionPoint != Vector2.Zero)
            {
                var projectile = Owner.Session.ObjectPools.Projectiles.Get();
                var pos = Owner.GetAnchoredPosition(actionPoint);
                projectile.Launch(Owner, pos, Owner.Direction == Adberration.FacingDirection.Right ? Vector2.UnitX : -Vector2.UnitX, action.Projectile);
            }
        }

        // ResolveProximityAction
        private void ResolveProximityAction(IAction action)
        {
            if (Target != null && CanInflictDamage(Target))
            {
                var missChance = action.MissChance;

                if (Owner.Session.RunModifiers.IsActive(RunModifierNames.Darkness))
                {
                    if (Owner.PixelsMoved > GameRoom.PlayerLightBounds.Width / 2)
                        missChance += GameSettings.DarknessMissChancePenalty;
                }

                if (!missChance.Roll())
                    EffectDescriptor.Apply(action.EffectDescriptors, Owner, Target, EffectContext.Attack);
                else
                    Owner.ShowFlyOff(missText, Color.WhiteSmoke, 2000);

                //Owner.Session.InterruptAwaitingScript();
            }
        }

        // ResolveSelfAction
        private void ResolveSelfAction(IAction action)
        {
            ActionProcessor.Apply(action, Owner, null, EffectContext.Use);
        }

        #endregion

        #region Protected members

        // GetAnimationName
        protected override string GetAnimationName()
        {
            return Action?.AnimationName ?? string.Empty;
        }

        #endregion

        // Action
        public IAction? Action { get; set; }

        // Enter
        public override void Enter()
        {
            base.Enter();

            animationFound = Owner.ContainsAnimation(GetAnimationName());
            eventDone = !animationFound;

            if (Action?.SoundStart != null)
                Owner.PlaySound(Action.SoundStart);
        }

        // Exit
        public override void Exit()
        {
            base.Exit();
            Action = null;
            Target = null;

            if (Owner.IsPlayer)
                Owner.Session.ProcessTurn();
        }

        // Target
        public GameThing? Target { get; set; }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (Action != null && !eventDone && Owner.AnimationPlayer.Frame?.IsTrigger == true)
            {
                eventDone = true;

                if (Action.EnergyCost > 0 && Owner.Energy <= Action.EnergyCost)
                {
                    Sound.Play(SoundNames.Error);
                    var text = Localization.GetValue(MessageKind.NotEnoughFaith);
                    Owner.ShowFlyOff(text, ColorPalette.Text.TerraLight, 2500);
                    return;
                }

                if (Action.SoundTrigger != null)
                    Owner.PlaySound(Action.SoundTrigger);

                switch (Action.ActionKind)
                {
                    // InPlaceAction
                    case ActionKind.InPlace:
                        ResolveInPlaceAction(Action);
                        break;

                    // ProjectileAction
                    case ActionKind.Projectile:
                        ResolveProjectileAction(Action);
                        break;

                    // ProximityAction
                    case ActionKind.Proximity:
                        ResolveProximityAction(Action);
                        break;

                    // SelfAction
                    case ActionKind.Self:
                        ResolveSelfAction(Action);
                        break;

                    default:
                        break;
                }

                Action.Consume(Owner);

                return;
            }

            if (!animationFound || !Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<BodyStandState>();
        }
    }
}