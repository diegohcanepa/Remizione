using Engendro;
using Engendro.Audio;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// UIPlayerSelector
    /// </summary>
    public class UIPlayerSelector : GameObject, IInputHandler
    {
        private readonly List<ImageSprite> icons = [];
        private readonly GameSession session;

        // Constructor
        public UIPlayerSelector(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            for (var i = 0; i < 5; i++)
            {
                var icon = new ImageSprite(Game)
                {
                };

                icons.Add(icon);
            }
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (icons.Count > 0 && session.Players.Count > 1)
            {
                Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp, BlendState.AlphaBlend, null);
                for (var i = 0; i < icons.Count; i++)
                {
                    icons[i].Draw(gameTime);
                }
                Game.SpriteBatch.End();
            }
        }

        #endregion

        // Invalidate
        public void Invalidate()
        {
            Vector2 InvalidateIcon(int i, Vector2 pos)
            {
                icons[i].Opacity = session.Players[i].IsPlayer ? 1.4f : .8f;
                icons[i].Scale = session.Players[i].IsPlayer ? ScaleInfo.UIElement.Large : ScaleInfo.UIElement.Medium;
                icons[i].Position = pos;
                icons[i].Image = Atlases.UI.FindImage(session.Players[i].Name + "Icon");
                pos.X += icons[i].BoundingBox.Width;
                return pos;
            }

            var pos = new Vector2(5);

            if (session.Player != null)
            {
                var index = session.Players.IndexOf(session.Player);
                if (index != -1)
                    pos = InvalidateIcon(index, pos);
            }

            for (var i = 0; i < icons.Count; i++)
            {
                if (i < session.Players.Count)
                {
                    if (!session.Players[i].IsPlayer)
                        pos = InvalidateIcon(i, pos);
                }
                else
                    return;
            }
        }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (session.Player != null && session.Players.Count > 1)
            {
                if (!InputManager.DefaultPlayer.Keyboard.IsShiftDown())
                {
                    if (InputBindings.PreviousPlayer.IsPressed(PlayerIndex.One))
                    {
                        session.Player = session.Players.NextItem(session.Player);
                        Sound.Play(SoundNames.UISelectPlayer);
                        return HandleInputResult.Handled;
                    }
                    else if (InputBindings.NextPlayer.IsPressed(PlayerIndex.One))
                    {
                        session.Player = session.Players.PreviousItem(session.Player);
                        Sound.Play(SoundNames.UISelectPlayer);
                        return HandleInputResult.Handled;
                    }
                }
            }

            return HandleInputResult.Unhandled;
        }
    }
}
