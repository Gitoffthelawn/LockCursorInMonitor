﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace LockCursorInMonitor
{
    static class CursorLock
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public int X;
            public int Y;
        };

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(ref Win32Point pt);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern bool ClipCursor(RECT rcClip);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern bool GetClipCursor(ref RECT lprect);

        private static RECT rect = new RECT();
        private static bool locked = false;

        public static bool Locked 
        { 
            get => locked;
            set 
            {
                locked = value; 
            } 
        }
        /// <summary>
        /// Wether the ctrl key is pressed or not. This gets set by the event handlers GlobalHookCtrlDown and 
        /// GlobalHookCtrlUp. This is done because I'm not aware of any other way to check if ctrl is pressed in the 
        /// mouse event handlers.
        /// </summary>
        public static bool ControlKeyPressed { get; set; }

        public static void LockCursor()
        {
            // Get the position of the cursor
            System.Drawing.Point MousePoint = GetMousePosition();
            // Get the bounds of the screen that the cursor is on
            RECT bounds = Screen.GetWorkingArea(MousePoint);
            // Confine the cursor to that monitor
            ClipCursor(bounds);
            Locked = true;
            // Write the current value of rect.Right to the console
            Trace.WriteLine("rect.Right:");
            Trace.WriteLine(rect.Right);
            // This throws System.NullReferenceException: 'Object reference not set to an instance of an object.'
            bool getClip = GetClipCursor(ref rect);
            Trace.WriteLine("rect.Right:");
            Trace.WriteLine(rect.Right);
        }

        public static void UnlockCursor()
        {
            ClipCursor(null);
            Locked = false;
        }

        private static System.Drawing.Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            return new System.Drawing.Point(w32Mouse.X, w32Mouse.Y);
        }
    }
}
