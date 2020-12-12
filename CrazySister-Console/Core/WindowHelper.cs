using System;
using System.Runtime.InteropServices;

namespace Server.Core
{
    public static class WindowHelper
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        public static void WindowVisibility(bool val)
        {
            var handle = GetConsoleWindow();

            if (val)
                ShowWindow(handle, SW_SHOW);
            else
                ShowWindow(handle, SW_HIDE);
        }
    }
}
