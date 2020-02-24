using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace GameSettings
{
    public class BorderlessMode
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        private static IntPtr window;

        private static readonly uint SWP_SHOWWINDOW = 0x0040;
        private static readonly uint SWP_NOMOVE = 0x0002;
        private static readonly int GWL_STYLE = -16;
        private static readonly int WS_BORDER = 1;

        private static readonly int WS_OVERLAPPED = 0x00000000;
        private static readonly int WS_THICKFRAME = 0x00040000;
        private static readonly int WS_CAPTION = 0x00C00000;
        private static readonly int WS_SYSMENU = 0x00080000;
        private static readonly int WS_MINIMIZEBOX = 0x00020000;

        private static readonly int WS_OVERLAPPEDWINDOW = WS_OVERLAPPED |
                                                          WS_CAPTION |
                                                          WS_SYSMENU |
                                                          WS_THICKFRAME |
                                                          WS_MINIMIZEBOX;

        public static void SetBorderlessWindow(Resolution resolution)
        {
            window = GetActiveWindow();
            var screenPosition = new Rect(0f, 0f, resolution.width, resolution.height);
            SetWindowLong(window, GWL_STYLE, WS_BORDER);
            SetWindowPos(window, 0,
                        (int)screenPosition.x,
                        (int)screenPosition.y,
                        (int)screenPosition.width,
                        (int)screenPosition.height,
                        SWP_SHOWWINDOW | SWP_NOMOVE);
        }

        public static void SetBorderedWindow(Resolution resolution)
        {
            window = GetActiveWindow();
            var screenPosition = new Rect(0f, 0f, resolution.width, resolution.height);
            SetWindowLong(window, GWL_STYLE, WS_OVERLAPPEDWINDOW);
            SetWindowPos(window, 0,
                        (int)screenPosition.x,
                        (int)screenPosition.y,
                        (int)screenPosition.width,
                        (int)screenPosition.height,
                        SWP_SHOWWINDOW | SWP_NOMOVE);
        }

#else
        public static void SetBorderlessWindow(Resolution resolution)
        {
            Debug.Log("Current platform doesn't support borderless window functions.");
        }

        public static void SetBorderedWindow(Resolution resolution)
        {
            Debug.Log("Current platform doesn't support borderless window functions.");
        }
#endif
    }
}
