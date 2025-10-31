using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// UIControlGroup
    /// </summary>
    public sealed class UIControlGroup : GameObject
    {
        private readonly List<UITextButton> controlList = [];
        private UIControlGroupLayoutStyle layoutStyle;
        private float spacing = 2;

        // Constructor
        public UIControlGroup(EngendroGame game)
            : base(game)
        {
            this.Controls = new ReadOnlyCollection<UITextButton>(controlList);
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            for (var i = 0; i < controlList.Count; i++)
            {
                controlList[i].Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
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
                {
                    layout = true;
                }
            }

            if (layout)
            {
                Layout();
            }
        }

        #endregion

        // Add
        public UITextButton Add(string text, InputBinding? inputBinding)
        {
            return Add(text, inputBinding);
        }

        // Add
        public UITextButton Add(string text, InputBinding? gameInput, string? imageName)
        {
            UITextButton control = new(Game, gameInput)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Text = text
            };

            if (!string.IsNullOrWhiteSpace(imageName))
            {
                control.ImageName = imageName;
            }

            controlList.Add(control);

            Layout();

            return control;
        }

        // Controls
        public ReadOnlyCollection<UITextButton> Controls { get; }

        // FirstControl
        public UITextButton? FirstControl => controlList.Count == 0 ? null : controlList[0];

        // IsActiveInGameLoop
        public override bool IsActiveInGameLoop => IsVisible && base.IsActiveInGameLoop;

        // IsVisible
        public bool IsVisible { get; set; } = true;

        // Layout
        public void Layout()
        {
            if (layoutStyle == UIControlGroupLayoutStyle.Vertically)
            {
                Utils.LayoutControlsVertically(controlList.ToArray(), spacing);
            }
            else
            {
                Utils.LayoutControlsHorizontally(controlList.ToArray(), spacing);
            }
        }

        // LayoutStyle
        public UIControlGroupLayoutStyle LayoutStyle
        {
            get => layoutStyle;
            set
            {
                if (value != layoutStyle)
                {
                    layoutStyle = value;
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
            get => spacing;
            set
            {
                if (value != spacing)
                {
                    spacing = value;
                    Layout();
                }
            }
        }
    }
}
