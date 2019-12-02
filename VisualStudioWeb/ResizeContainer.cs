using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Retyped.dom;

namespace VisualStudioWeb
{
    public class ResizeContainer
    {
        public List<ResizeContainer> Children;
        public HTMLDivElement Container;
        protected HTMLElement AssignedElement;
        public ControlSettings ControlSettings;
        private float startingX;
        private float startingY;
        private float startingWidth;
        private float startingHeight;

        private List<HTMLElement> resizeableButtons = new List<HTMLElement>();

        private static ResizeContainer focusedResize;
        private static List<ResizeContainer> selectedResize = new List<ResizeContainer>();

        public static ResizeContainer FocusedResize
        {
            get { return focusedResize; }
            set {
                if(focusedResize != value)
                {
                    if(focusedResize != null)
                    {
                        if(!selectedResize.Contains(focusedResize))
                        {
                            foreach (var item in focusedResize.resizeableButtons)
                            {
                                item.style.visibility = "hidden";
                            }
                        }                        
                    }

                    focusedResize = value;

                    if (focusedResize != null)
                    {
                        foreach (var item in focusedResize.resizeableButtons)
                        {
                            item.style.visibility = null;
                        }
                        if (selectedResize.Contains(focusedResize))
                        {
                            selectedResize.Remove(focusedResize);
                        }
                    }
                }
            }
        }

        public ResizeContainer()
        {
            Container = new HTMLDivElement();
            Container.style.position = "absolute";
            Container.style.boxSizing = "border-box";
            Container.style.overflow = "hidden";
            Container.style.userSelect = "none";
            Container.style.transition = $"width {Program.MoveTransitionTimeMs}ms, height {Program.MoveTransitionTimeMs}ms, left {Program.MoveTransitionTimeMs}ms, top {Program.MoveTransitionTimeMs}ms";
            
            // Move
            Container.appendChild(createResizeButton("0", "0", (ev) =>
            {                
                Left = Snap(startingX - (float)(Program.MouseDownX - Program.CurrentMouseX), Program.SnapToGridX);
                Top = Snap(startingY - (float)(Program.MouseDownY - Program.CurrentMouseY), Program.SnapToGridY);
            }, "100%", "100%", "999", "transparent"));

            // Bottom Right
            Container.appendChild(createResizeButton("calc(100% - 10px)", "calc(100% - 10px)", (ev) =>
            {
                Width = Snap(startingWidth - (float)(Program.MouseDownX - Program.CurrentMouseX), Program.SnapToGridX);
                Height = Snap(startingHeight - (float)(Program.MouseDownY - Program.CurrentMouseY), Program.SnapToGridY);
            }));

            // Top Right
            Container.appendChild(createResizeButton("calc(100% - 10px)", "0", (ev) =>
            {
                Top = Snap(startingY - (float)(Program.MouseDownY - Program.CurrentMouseY), Program.SnapToGridY);
                Width = Snap(startingWidth - (float)(Program.MouseDownX - Program.CurrentMouseX), Program.SnapToGridX);
                Height = Snap(startingHeight - (Top - startingY), Program.SnapToGridY);
            }));

            // Top Left
            Container.appendChild(createResizeButton("0", "0", (ev) =>
            {
                Top = Snap(startingY - (float)(Program.MouseDownY - Program.CurrentMouseY), Program.SnapToGridY);                
                Height = Snap(startingHeight - (Top - startingY), Program.SnapToGridY);

                Left = Snap(startingX - (float)(Program.MouseDownX - Program.CurrentMouseX), Program.SnapToGridX);
                Width = Snap(startingWidth - (Left - startingX), Program.SnapToGridX);
            }));

            // Bottom Left
            Container.appendChild(createResizeButton("0", "calc(100% - 10px)", (ev) =>
            {
                Height = Snap(startingHeight - (float)(Program.MouseDownY - Program.CurrentMouseY), Program.SnapToGridY);

                Left = Snap(startingX - (float)(Program.MouseDownX - Program.CurrentMouseX), Program.SnapToGridX);
                Width = Snap(startingWidth - (Left - startingX), Program.SnapToGridX);
            }));

            // Top Middle
            Container.appendChild(createResizeButton("calc(50% - 5px)", "0", (ev) =>
            {
                Top = Snap(startingY - (float)(Program.MouseDownY - Program.CurrentMouseY), Program.SnapToGridY);
                Height = Snap(startingHeight - (Top - startingY), Program.SnapToGridY);
            }));

            // Middle Right
            Container.appendChild(createResizeButton("calc(100% - 10px)", "calc(50% - 5px)", (ev) =>
            {
                Width = Snap(startingWidth - (float)(Program.MouseDownX - Program.CurrentMouseX), Program.SnapToGridX);
            }));

            // Middle Left
            Container.appendChild(createResizeButton("0", "calc(50% - 5px)", (ev) =>
            {
                Left = Snap(startingX - (float)(Program.MouseDownX - Program.CurrentMouseX), Program.SnapToGridX);
                Width = Snap(startingWidth - (Left - startingX), Program.SnapToGridX);
            }));


            // Bottom Middle
            Container.appendChild(createResizeButton("calc(50% - 5px)", "calc(100% - 10px)", (ev) =>
            {                
                Height = Snap(startingHeight - (float)(Program.MouseDownY - Program.CurrentMouseY), Program.SnapToGridY);
            }));
        }        

        private static float Snap(float value, float grid)
        {
            if (Program.SnapToGrid)
                return (float)Math.Round(value / grid) * grid;
            else
                return value;
        }

        private HTMLDivElement createResizeButton(string _left, string _top, Action<MouseEvent> onMove, string overrideWidth = "10px", string overrideHeight = "10px", string zindex = "1000", string overrideColor = "white")
        {
            var resizeButton = new HTMLDivElement();
            resizeButton.style.position = "absolute";

            resizeButton.style.left = _left;
            resizeButton.style.top = _top;
            resizeButton.style.width = overrideWidth;
            resizeButton.style.height = overrideHeight;
            resizeButton.style.backgroundColor = overrideColor;
            resizeButton.style.zIndex = zindex;
            resizeButton.style.boxSizing = "border-box";

            if(overrideColor != "transparent")
            {
                resizeButton.style.border = "1px solid lightgrey";
                resizeButton.style.borderRadius = "1px";
                resizeButton.style.visibility = "hidden";

                resizeableButtons.Add(resizeButton);
            }
            resizeButton.tabIndex = -1;

            resizeButton.onblur = (ev) =>
            {                
                if (Container.children.Contains(ev.relatedTarget.As<HTMLElement>()))
                {
                    return;
                }
                FocusedResize = null;
                
            };

            resizeButton.onfocus = (ev) =>
            {
                FocusedResize = this;
                foreach (var item in resizeableButtons)
                {
                    item.style.visibility = null;
                }
            };

            Program.MouseDown[resizeButton] = (ev) =>
            {
                startingX = Left;
                startingY = Top;
                startingHeight = Height;
                startingWidth = Width;
            };

            Program.MouseMove[resizeButton] = (ev) =>
            {
                if (Program.IsMouseDown)
                {
                    ev.preventDefault();
                    ev.stopPropagation();
                    onMove(ev);
                }
            };

            return resizeButton;
        }

        private float _left;
        
        public float Left
        {
            get { return _left; }
            set {
                _left = value;
                var valueInPx = $"{value}px";
                Container.style.left = valueInPx;
                ApplySettings(new ControlSettings() { Style = { { "left", valueInPx } } }, true);
            }
        }

        private float _top;
        public float Top
        {
            get { return _top; }
            set
            {
                _top = value;
                var valueInPx = $"{value}px";
                Container.style.top = valueInPx;
                ApplySettings(new ControlSettings() { Style = { { "top", valueInPx } } }, true);
            }
        }

        private float _width;
        public float Width
        {
            get { return _width; }
            set
            {                
                _width = value;
                var valueInPx = $"{value}px";
                Container.style.width = valueInPx;
                ApplySettings(new ControlSettings() { Style = { { "width", valueInPx } } }, true);
            }
        }

        private float _height;
        public float Height
        {
            get { return _height; }
            set
            {
                _height = value;
                var valueInPx = $"{value}px";
                Container.style.height = valueInPx;
                ApplySettings(new ControlSettings() { Style = { { "height", valueInPx } } }, true);
            }
        }

        public void AttachElement(HTMLElement element)
        {
            if (AssignedElement != null)
                Container.removeChild(AssignedElement);
            AssignedElement = element;

            AssignedElement.style.position = "absolute";
            AssignedElement.style.boxSizing = "border-box";

            ControlSettings = new ControlSettings();
            Container.appendChild(AssignedElement);
        }

        public void ApplySettings(ControlSettings controlSettings, bool fromContainer = false)
        {
            if (AssignedElement == null || controlSettings == null || controlSettings.Settings == null)
                return;

            foreach (var item in controlSettings.Style)
            {
                if (!ControlSettings.Style.TryGetValue(item.Key, out dynamic value) || value != item.Value)
                {
                    ControlSettings.Style[item.Key] = item.Value;
                    if (fromContainer)
                        continue;

                    if (item.Key == "left")
                    {
                        AssignedElement.style.left = "0";
                        Left = Bridge.Script.ParseFloat(item.Value);
                    }
                    else if (item.Key == "top")
                    {
                        AssignedElement.style.top = "0";
                        Top = Bridge.Script.ParseFloat(item.Value);
                    }
                    else if (item.Key == "width")
                    {
                        AssignedElement.style.width = "100%";
                        Width = Bridge.Script.ParseFloat(item.Value);
                    }
                    else if (item.Key == "height")
                    {
                        AssignedElement.style.height = "100%";
                        Height = Bridge.Script.ParseFloat(item.Value);
                    }
                    else
                    {
                        AssignedElement.style[item.Key] = item.Value;
                    }
                    
                }
            }

            foreach (var item in controlSettings.Settings)
            {
                if (!ControlSettings.Settings.TryGetValue(item.Key, out dynamic value) || value != item.Value)
                {
                    ControlSettings.Settings[item.Key] = item.Value;                    
                    AssignedElement[item.Key] = item.Value;
                }
            }
        }

        public void LoadContainerSettings()
        {
            dynamic value;
            if (ControlSettings.Settings.TryGetValue("left", out value))
            {
                Left = value;
            }
            if (ControlSettings.Settings.TryGetValue("top", out value))
            {
                top = value;
            }
            if (ControlSettings.Settings.TryGetValue("width", out value))
            {
                Width = value;
            }
            if (ControlSettings.Settings.TryGetValue("height", out value))
            {
                Height = value;
            }

        }
    }
}
