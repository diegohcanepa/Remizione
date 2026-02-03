using Adberration.Scripting;
using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// PlayerTurnState
    /// </summary>
    public sealed class PlayerTurnState : CombatManagerState
    {
        private Script? awaitingScript;
        private int awaitScriptCooldown;
        private readonly TextSprite tip;
        private int tipNumber;
        private readonly string[] tips = new string[3];

        // PlayerTurnState
        public PlayerTurnState(CombatManager manager)
            : base(manager)
        {
            // Tips
            for (var i = 0; i < tips.Length; i++)
            {
                tips[i] = TextRepository.GetValue($"Combat.Tip{i+1}");
            }

            // Tip
            this.tip = new(manager.Session.Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftTop, 2, 2),
                Scale = ScaleInfo.Text.Giant
            };
        }

        #region Private members

        // RefreshTip
        private void RefreshTip(int patience)
        {
            if (tipNumber == 3)
                return;

            float progress = TimeInState / patience;
            var changeTip = false;

            if (progress < 0.5f)
            {
                if (tipNumber < 1)
                    changeTip = true;
            }
            else if (progress < 0.75f)
            {
                if (tipNumber < 2)
                    changeTip = true;
            }
            else
            {
                if (tipNumber < 3)
                    changeTip = true;
            }

            if (changeTip)
            {
                tipNumber++;
                tip.Text = tips[tipNumber - 1];
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (TimeInState > 2)
                tip.Draw(gameTime);
        }

        // OnEnter
        protected override void OnEnter()
        {
            awaitScriptCooldown = 100;
        }

        // OnHandleInput
        protected override HandleInputResult OnHandleInput(GameTime gameTime)
        {
            var context = Manager.Session.InteractionContext;

            if (InputManager.DefaultPlayer.Mouse.IsRightButtonPressed())
            {
                context.HeldItem = null;
                return HandleInputResult.Handled; 
            }

            if (context.Target != null)
            {
                if (InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
                {
                    if (context.HeldItem is Item item)
                    {
                        if (context.UseWithScript != null)
                        {
                            awaitingScript = context.UseWithScript;
                        }
                        else if (Manager.Session.ScriptLibrary.FindRoutine($"Use{item.Name}") is Script script)
                        {
                            awaitingScript = script;
                        }
                    }

                    if (awaitingScript != null)
                    {
                        context.HeldItem = null;
                        Manager.Session.AwaitScript(awaitingScript);
                    }
                }
            }

            return HandleInputResult.Handled;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (awaitingScript != null)
            {
                if (awaitScriptCooldown > 0)
                {
                    awaitScriptCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                }
                else if (!Manager.Session.IsAwaitingScript(awaitingScript))
                {
                    Manager.TransitionTo(new EnemyTurnState(Manager));
                }
            }
            else if (Manager.Enemy.Definition is ThingDefinition def)
            {
                if (TimeInState > def.Patience)
                    Manager.TransitionTo(new EnemyTurnState(Manager));
                else
                    RefreshTip(def.Patience);
            }
        }

        #endregion
    }
}
