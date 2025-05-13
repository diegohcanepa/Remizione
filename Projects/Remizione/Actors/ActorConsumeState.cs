using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// ActorConsumeState
    /// </summary>
    public sealed class ActorConsumeState : ActorAnimatedState
    {
        private bool soundPlayed;

        // Constructor
        public ActorConsumeState(Actor owner)
            : base(owner, ActorStateNames.Consume, false)
        {
        }

        // CheckTransitions
        public override string? CheckTransitions()
        {
            if (!Owner.AnimationPlayer.IsPlaying)
                return ActorStateNames.Stand;
            else
                return base.CheckTransitions();
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            soundPlayed = false;
        }

        // Exit
        public override void Exit()
        {
            base.Exit();
            Owner.EndTurn();
            Item = null;
        }

        // Item
        public Item? Item { get; set; }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (!soundPlayed && Item != null && Owner.AnimationPlayer.Frame is SpriteFrame frame)
            {
                if (frame.Label == GameSettings.KeyFrame)
                {
                    var hp = Owner.HP;
                    var faith = Owner.Faith;

                    Item.Use();

                    if (Item.MetaItem.Sound != null)
                        Owner.PlaySound(Item.MetaItem.Sound);

                    var diffHP = Owner.HP - hp;
                    var diffFaith = Owner.Faith - faith;

                    if (diffHP > 0)
                        Owner.Session.HUD.Log.Show(LogVerb.Restore, $"{Localization.GetLocalizedValue(DerivedStat.Spirit)} + {diffHP}");

                    if (diffFaith > 0)
                        Owner.Session.HUD.Log.Show(LogVerb.Restore, $"{Localization.GetLocalizedValue(DerivedStat.Faith)} + {diffFaith}");

                    soundPlayed = true;
                }
            }
        }
    }
}
