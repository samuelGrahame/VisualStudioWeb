using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Retyped.dom;
using Console = System.Console;

namespace VisualStudioWeb
{
    class Program
    {
        public static bool IsMouseDown = false;
        public static double MouseDownX;
        public static double MouseDownY;

        public static double CurrentMouseX;
        public static double CurrentMouseY;

        public static HTMLElement MouseDownElement = null;
        public static Dictionary<HTMLElement, Action<MouseEvent>> MouseDown = new Dictionary<HTMLElement, Action<MouseEvent>>();
        public static Dictionary<HTMLElement, Action<MouseEvent>> MouseMove = new Dictionary<HTMLElement, Action<MouseEvent>>();
        public static Dictionary<HTMLElement, Action<MouseEvent>> MouseUp = new Dictionary<HTMLElement, Action<MouseEvent>>();


        // Settings
        public static bool SnapToGrid = true;
        public static float SnapToGridX = 5;
        public static float SnapToGridY = 5;
        public static float MoveTransitionTimeMs = 5;

        private static void invokeEvent(MouseEvent ev, Dictionary<HTMLElement, Action<MouseEvent>> eventList)
        {
            CurrentMouseX = ev.clientX;
            CurrentMouseY = ev.clientY;

            Action<MouseEvent> eventAtt = null;
            if(IsMouseDown)
            {
                Console.WriteLine($"Mouse_Down: True - x:{CurrentMouseX} - y:{CurrentMouseY}");
            }

            if (eventList.TryGetValue(IsMouseDown ? MouseDownElement : ev.target.As<HTMLElement>(), out eventAtt))
            {
                eventAtt(ev);
            }
        }

        static void Main()
        {
            // Global Events.
            window.onmousedown = (ev) =>
            {
                IsMouseDown = true;

                MouseDownX = ev.clientX;
                MouseDownY = ev.clientY;

                MouseDownElement = ev.target.As<HTMLElement>();

                invokeEvent(ev, MouseDown);
            };
            window.onmousemove = (ev) =>
            {
                invokeEvent(ev, MouseMove);
            };
            window.onmouseup = (ev) =>
            {                
                IsMouseDown = false;
                invokeEvent(ev, MouseUp);
            };

            var resizeContainer = new ResizeContainer();

            resizeContainer.AttachElement(new HTMLInputElement() { type = "text" });
            resizeContainer.ApplySettings(new ControlSettings()
            {
                Style = 
                {
                    { "width", "100px" },
                    { "height", "100px" },
                    { "left", "100px" },
                    { "top", "100px" }                    
                },
                Settings =
                {
                    { "value", "TextBox1" }
                }
            });

            document.body.appendChild(resizeContainer.Container);



            var resizeContainer2 = new ResizeContainer();

            resizeContainer2.AttachElement(new HTMLButtonElement() { type = "button" });
            resizeContainer2.ApplySettings(new ControlSettings()
            {
                Style =
                {
                    { "width", "100px" },
                    { "height", "100px" },
                    { "left", "300px" },
                    { "top", "100px" }
                },
                Settings =
                {
                    { "textContent", "Button1" }
                }
            });

            document.body.appendChild(resizeContainer2.Container);


        }
    }
}
