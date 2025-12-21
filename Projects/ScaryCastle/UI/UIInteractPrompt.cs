using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;

namespace ScaryCastle
{
    /// <summary>
    /// UIInteractPrompt
    /// </summary>
    public sealed class UIInteractPrompt : GameObject
    {
        private readonly UIButton button;
        private readonly TextSprite priceText;
        private readonly ImageSprite ticketIcon;
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

            // PriceText
            this.priceText = new(Game, Fonts.Common)
            {
                PivotOrigin = RectanglePoint.Center,
                Scale = ScaleInfo.Text.VeryLarge
            };

            // TicketIcon
            ticketIcon = new(Game, Atlases.UI.TicketPriceIcon)
            {
                PivotOrigin = RectanglePoint.Left,
                Scale = new(.65f)
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (session.IsCurrentScene && target != null)
            {
                button.Draw(gameTime);
                
                if (!priceText.IsEmpty)
                {
                    Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);
                    ticketIcon.Draw(gameTime);
                    priceText.Draw(gameTime);
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
                    button.Text = currentTarget.GetInteractPrompt() ?? currentTarget.LocalizedDisplayName;

                    if (target is IBuyable buyable && buyable.Price > 0)
                    {
                        ticketIcon.Position = button.BoundingBox.GetPoint(RectanglePoint.Right, -1, .8f);
                        priceText.Color = ColorPalette.Text.Default;
                        priceText.Text = buyable.Price.ToString(CultureInfo.InvariantCulture);
                        priceText.Position = ticketIcon.BoundingBox.GetPoint(RectanglePoint.Center, .25f, 0);
                    }
                    else
                    {
                        priceText.Text = null;
                    }

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
