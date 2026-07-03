using Adberration;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScaryCastle
{
    /// <summary>
    /// UIPocketItemMeter
    /// </summary>
    public class UIPocketItemMeter : SessionGameObject<GameSession>
    {
        private bool isInitialized;
        private readonly Sprite icon;
        private readonly Vector2 iconScale = Vector2.One;
        private int lastKnownValue = -1;
        private readonly FloatTween rotationTween = new();
        private readonly Vector2Tween scaleTween = new();
        private readonly TextSprite valueText;

        #region Constructor

        // Constructor
        public UIPocketItemMeter(GameSession session, PocketItemType pocketItemType, Vector2 position, bool hideIfEmpty)
            : base(session)
        {
            this.PocketItemType = pocketItemType;
            this.HideIfEmpty = hideIfEmpty;

            // Icon
            this.icon = new()
            {
                PivotOrigin = RectanglePoint.RightBottom,
            };

            if (pocketItemType == PocketItemType.Coin)
            {
                icon.PivotOrigin = RectanglePoint.RightBottom;
                icon.Position = Screen.Area.GetPoint(RectanglePoint.RightBottom, position);
            }
            else
            {
                icon.PivotOrigin = RectanglePoint.LeftBottom;
                icon.Position = Screen.Area.GetPoint(RectanglePoint.LeftBottom, position);
            }

            icon.RenderImage = pocketItemType switch
            {
                PocketItemType.BronzeKey => Atlases.UI.BronzeKeyIcon,
                PocketItemType.Coin => Atlases.UI.CoinIcon,
                PocketItemType.GoldenKey => Atlases.UI.GoldenKeyIcon,
                _ => throw new System.NotImplementedException(),
            };

            // Score text
            this.valueText = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                Scale = ScaleInfo.Text.Giant
            };

            if (pocketItemType == PocketItemType.Coin)
            {
                valueText.PivotOrigin = RectanglePoint.Right;
                valueText.Position = icon.BoundingBox.GetPoint(RectanglePoint.Left, -1, 1);
            }
            else
            {
                valueText.PivotOrigin = RectanglePoint.Left;
                valueText.Position = icon.BoundingBox.GetPoint(RectanglePoint.Right, -1, 2);
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (lastKnownValue <= 0 && HideIfEmpty)
                return;

            Game.SpriteBatch.Begin(Game.Camera);
            icon.Draw(gameTime);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            valueText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (lastKnownValue != Count)
            {
                if (isInitialized)
                {
                    if (PocketItemType == PocketItemType.Coin)
                    {
                        Sound.Play(SoundNames.CollectCoin);
                    }
                    else
                    {
                        Sound.Play(SoundNames.PickupKey);
                    }
                }

                isInitialized = true;
                lastKnownValue = Count;
                valueText.Text = PocketItemType == PocketItemType.Coin ? $"{Count}" : $"x{Count}";

                if (!icon.Tweens.IsTweening)
                {
                    rotationTween.Start(TweenStyle.QuadraticInOut, 0, 15, 50, 6);
                    icon.Tweens.RotationTween = rotationTween;

                    scaleTween.Start(TweenStyle.QuadraticInOut, iconScale, iconScale * 1.3f, 150, 2);
                    icon.Tweens.ScaleTween = scaleTween;
                }
            }

            icon.Update(gameTime);
        }

        #endregion

        // Count
        public int Count => PocketItemType switch
        {
            PocketItemType.BronzeKey => Session.BronzeKeys,
            PocketItemType.Coin => Session.Coins,
            PocketItemType.GoldenKey => Session.GoldenKeys,
            _ => throw new System.NotImplementedException(),
        };

        // HideIfEmpty
        public bool HideIfEmpty { get; }

        // IconBoundingBox
        public RectangleF IconBoundingBox => icon.BoundingBox;

        // PocketItemType
        public PocketItemType PocketItemType { get; }
    }
}
