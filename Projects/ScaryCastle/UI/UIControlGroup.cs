using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ScaryCastle
{
    /// <summary>
    /// UIControlGroup
    /// </summary>
    public sealed class UIControlGroup : GameObject
    {
        private readonly List<UIButton> controlList = [];

        // Constructor
        public UIControlGroup(EngendroGame game)
            : base(game)
        {
            this.Controls = new ReadOnlyCollection<UIButton>(controlList);
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsVisible)
                return;

            for (var i = 0; i < controlList.Count; i++)
            {
                controlList[i].Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (!IsVisible)
                return;

            var layout = false;
            var bbox = RectangleF.Empty;

            for (var i = 0; i < controlList.Count; i++)
            {
                if (!layout)
                {
                    bbox = controlList[i].BoundingBox;
                }

                controlList[i].Update(gameTime);

                if (!layout && controlList[i].BoundingBox != bbox)
                    layout = true;
            }

            if (layout)
                Layout();
        }

        #endregion

        // Add
        public UIButton Add(InputBinding inputBinding)
        {
            UIButton control = new(Game, inputBinding)
            {
                PivotOrigin = RectanglePoint.RightBottom,
            };

            controlList.Add(control);

            Layout();

            return control;
        }

        // Controls
        public ReadOnlyCollection<UIButton> Controls { get; }

        // FirstControl
        public UIButton? FirstControl => controlList.Count == 0 ? null : controlList[0];

        // IsVisible
        public bool IsVisible { get; set; } = true;

        // Layout
        public void Layout()
        {
            if (LayoutStyle == UIControlGroupLayoutStyle.Vertically)
            {
                Utils.LayoutControlsVertically([.. controlList], Spacing);
            }
            else
            {
                Utils.LayoutControlsHorizontally([.. controlList], Spacing);
            }
        }

        // LayoutStyle
        public UIControlGroupLayoutStyle LayoutStyle
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Layout();
                }
            }
        }

        // RemoveAt
        public void RemoveAt(int index)
        {
            controlList.RemoveAt(index);
        }

        // Spacing
        public float Spacing
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    Layout();
                }
            }
        } = 2;
    }
}
