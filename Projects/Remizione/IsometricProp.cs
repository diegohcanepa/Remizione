using Engendro;
using Engendro.Audio;
using EngendroAdventure.Scripting;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// IsometricProp
    /// </summary>
    public class IsometricProp : GameThing
    {
        #region Private fields

        private bool isRevealBoxDirty;
        private RectangleF revealBox;
        private readonly FloatTween revealTween = new();
        private readonly ImageSprite shadow;
        private Polygon? terrainPoly;

        #endregion

        #region Constructor

        // Constructor
        public IsometricProp(GameSession session, string name)
            : base(session, name)
        {
            this.Atlas = Atlases.Environment;
            
            // Shadow
            this.shadow = new ImageSprite(session.Game)
            {
                Opacity = ColorPalette.ShadowOpacity,
                PivotOrigin = RectanglePoint.Bottom,
            };
        }

        #endregion

        #region Private members

        // InvalidateShadowImage
        private void InvalidateShadowImage() => shadow.Image = Atlas?.GetImage(GetDefaultImageName() + "Shadow");

        // InvalidateTerrainArea
        private void InvalidateTerrainArea()
        {
            if (TerrainPolygon == null)
                return;

            int vertexCount = TerrainPolygon.Vertices.Count;
            var vertices = new Vector2[vertexCount];

            var offset = new Vector2(X - BoundingBox.Width / 2, Y - BoundingBox.Height);
            TerrainPolygon.GetVertices(vertices, offset);
            terrainPoly ??= new();
            terrainPoly.SetVertices(vertices);
        }

        // UpdateOpacityFactor
        private void UpdateOpacityFactor(GameTime gameTime)
        {
            const int tweenDuration = 200;

            if (Session.Player != null && RevealBox.Contains(Session.Player.Position))
            {
                if (Sprite.OpacityFactor == GameSettings.PropRevealOpacity)
                    return;

                if (!revealTween.IsRunning || revealTween.EndValue == 1)
                    revealTween.Start(TweenStyle.Linear, Sprite.OpacityFactor, .5f, tweenDuration);
            }
            else
            {
                if (Sprite.OpacityFactor == 1)
                    return;

                if (!revealTween.IsRunning || revealTween.EndValue == GameSettings.PropRevealOpacity)
                    revealTween.Start(TweenStyle.Linear, Sprite.OpacityFactor, 1, tweenDuration);
            }

            if (revealTween.IsRunning)
            {
                revealTween.Update(gameTime);
                Sprite.OpacityFactor = revealTween.CurrentValue;
            }
            else
                Sprite.OpacityFactor = 1;
        }

        #endregion

        #region Protected members

        // OnDrawShadow
        protected override void OnDrawShadow(GameTime gameTime) => shadow.Draw(gameTime);

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            InvalidateShadowImage();
            InvalidateTerrainArea();
        }

        // OnTransform
        protected override void OnTransform(TransformChange change)
        {
            base.OnTransform(change);

            shadow?.MatchTransform(Sprite);

            isRevealBoxDirty = true;

            InvalidateTerrainArea();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (!RevealArea.IsEmpty)
                UpdateOpacityFactor(gameTime);
        }

        #endregion

        // GetFootstepSound
        public Sound? GetFootstepSound(Vector2 position)
        {
            if (terrainPoly != null)
            {
                if (terrainPoly.Contains(position))
                    return TerrainSound;
            }

            return null;
        }

        // RevealArea
        [ScriptProperty]
        public Rectangle RevealArea { get; set; }

        // RevealBox
        public RectangleF RevealBox
        {
            get
            {
                if (isRevealBoxDirty)
                {
                    revealBox = RevealArea.IsEmpty ? RectangleF.Empty : this.GetAbsoluteBounds(RevealArea);
                    isRevealBoxDirty = false;
                }

                return revealBox;
            }
        }

        // TerrainPolygon
        [ScriptProperty]
        public Polygon? TerrainPolygon { get; set; }

        // TerrainSound
        [ScriptProperty]
        public Sound? TerrainSound { get; set; }
    }
}
