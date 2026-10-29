using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// UIMaterials
    /// </summary>
    public sealed class UIMaterials : SessionGameObject<GameSession>
    {
        #region Private fields

        private readonly List<Sprite> icons = [];
        private int lastKnownVersion = -1;
        private readonly Dictionary<Material, Sprite> materials = [];
        private Vector2 position;
        private readonly FloatTween rotationTween = new();
        private readonly Vector2Tween scaleTween = new();

        #endregion

        #region Constructor

        // Constructor
        public UIMaterials(GameSession session, Vector2 position)
            : base(session)
        {
            this.position = position;

            foreach (var enumValue in Enum.GetValues<Material>())
            {
                var icon = new Sprite(Atlases.UI.GetImage($"{enumValue}Icon"))
                {
                    PivotOrigin = RectanglePoint.Bottom,
                };

                materials[enumValue] = icon;
            }
        }

        #endregion

        #region Private members

        // Refresh
        private void Refresh()
        {
            lastKnownVersion = Session.PlayerData.Materials.Version;

            icons.Clear();
            var pos = position;

            foreach (var enumValue in Enum.GetValues<Material>())
            {
                var count = Session.PlayerData.Materials.GetCount(enumValue);
                if (count > 0)
                {
                    icons.Add(materials[enumValue]);
                    icons[^1].Position = pos;
                    icons[^1].RenderImage = Atlases.UI.FindImage($"Material{enumValue}{count}");
                    pos.X += icons[^1].BoundingBox.Width + 1;
                }
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            for (var i = 0; i < icons.Count; i++)
            {
                icons[i].Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (lastKnownVersion != Session.PlayerData.Materials.Version)
                Refresh();
        }

        #endregion
    }
}