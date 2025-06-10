using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione.UI
{
    /// <summary>
    /// UIWillpowerMeter
    /// </summary>
    public sealed class UIWillpowerMeter : GameObject
    {
        private Actor? actor;
        private readonly ImageSprite[] emptyUnits;
        private readonly ImageSprite[] fullUnits;
        private readonly TextSprite labelText;

        // Constructor
        public UIWillpowerMeter(EngendroGame game)
            : base(game)
        {
            // Empty units
            emptyUnits = new ImageSprite[20];
            for (var i = 0; i < emptyUnits.Length; i++)
            {
                emptyUnits[i] = new ImageSprite(game, Atlases.UI.WillpowerUnitEmpty)
                {
                    Scale = ScaleInfo.UIElement.Small
                };
            }

            // Full units
            fullUnits = new ImageSprite[20];
            for (var i = 0; i < fullUnits.Length; i++)
            {
                fullUnits[i] = new ImageSprite(game, Atlases.UI.WillpowerUnitFull)
                {
                    Scale = ScaleInfo.UIElement.Small
                };
            }

            // Label text
            this.labelText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Terra,
                PivotOrigin = RectanglePoint.Middle,
                Position = Screen.SafeArea.GetPoint(RectanglePoint.Top, 0, 6),
                Scale = ScaleInfo.Text.VeryLarge,
                Text = TextRepository.GetValue("ActorProperty.Willpower.Name")
            };
        }

        #region Private members

        // Invalidate
        private void Invalidate()
        {
            const int spacing = 1;

            if (actor == null)
                return;

            var maximum = actor.Stats.GetWillpower();
            
            var imageWidth = emptyUnits[0].BoundingBox.Width;
            var totalWidth = (maximum * imageWidth) + ((maximum - 1) * spacing);
            var start = (Screen.NativeWidth - totalWidth) / 2;

            var y = 11;
            for (var i = 0; i < maximum; i++)
            {
                var x = start + (i * (imageWidth + spacing));
                emptyUnits[i].Position = new Vector2(x, y);
                fullUnits[i].Position = new Vector2(x, y);
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (actor == null)
                return;

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            labelText.Draw(gameTime);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera);
            for (var i = 0; i < actor.Stats.GetWillpower(); i++)
            {
                emptyUnits[i].Draw(gameTime);
            }

            for (var i = 0; i < actor.Willpower; i++)
            {
                fullUnits[i].Draw(gameTime);
            }

            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (actor == null)
                return;

            if (actor != null)
                Invalidate();
        }

        #endregion

        // Actor
        public Actor? Actor
        {
            get => actor;
            set
            {
                if (actor != value)
                {
                    actor = value;
                    Invalidate();
                }
            }
        }
    }
}
