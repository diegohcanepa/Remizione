using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione
{
    /// <summary>
    /// UIInteractPrompt
    /// </summary>
    public sealed class UIInteractPrompt : GameObject
    {
        private readonly UIButton button;
        private readonly ImageSprite coin;
        private readonly ImageSprite coinSlot;
        private readonly GameSession session;
        private GameThing? target;

        // Constructor
        public UIInteractPrompt(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            // Button
            this.button = new(Game, InputBindings.Interact)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Bottom, 0, -4)
            };

            // Coin
            this.coin = new(Game, Atlases.UI.CoinIcon)
            {
                PivotOrigin = RectanglePoint.Left,
                Scale = ScaleInfo.UIElement.Small
            };

            // Coin slot
            this.coinSlot = new(Game, Atlases.UI.CoinSlot)
            {
                PivotOrigin = RectanglePoint.Left,
                Scale = ScaleInfo.UIElement.Small
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (session.IsCurrentScene && target != null)
            {
                button.Draw(gameTime);

                if (target is SaintPeregrine statue && statue.RequiredCoins > 0 && statue.PropState != PropState.Unlocked)
                {
                    Game.SpriteBatch.Begin(button.Camera, SamplerState.PointClamp);

                    var pos = button.BoundingBox.GetPoint(RectanglePoint.Right, 2, 0);
                    
                    for (var i = 0; i < statue.RequiredCoins; i++)
                    {
                        var image = statue.PropAmount > i ? coin : coinSlot;
                        
                        image.Position = pos;
                        image.Draw(gameTime);

                        pos.X += image.BoundingBox.Width;
                    }

                    Game.SpriteBatch.End();
                }
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (session.IsCurrentScene && session.Player?.InteractiveTarget is GameThing currentTarget)
            {
                if (currentTarget != target)
                {
                    target = currentTarget;
                    button.Text = currentTarget.LocalizedDisplayName;
                    //Sound.Play(SoundNames.UIPrompt);
                }
            }
            else
            {
                button.Text = null;
                target = null;
            }

            button.Update(gameTime);
        }

        #endregion
    }
}
