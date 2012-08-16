namespace AgWindowLibrary
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;

    public static class Hwnd
    {
        public static IntPtr FindHwnd(Window window)
        {
            var oldTitle = window.Title;
            var id = oldTitle + "(" + Guid.NewGuid().ToString() + ")";
            window.Title = id;
            var hwnd = FindWindowByCaption(IntPtr.Zero, id);
            window.Title = oldTitle;
            return hwnd;
        }

        public static void RemoveFromTaskBar(IntPtr hwnd)
        {
            // Note: the window must be hidden for this to take effect
            SetWindowLong(hwnd, GWL_EXSTYLE, (int)WS_EX_NOACTIVATE);
        }

        public static void SetTransparency(IntPtr hwnd, byte alpha)
        {
            // Note: the window must be in the hidden state to take effect
            SetWindowLong(hwnd, GWL_EXSTYLE, (int)(WS_EX_LAYERED + WS_EX_TRANSPARENT));
            SetLayeredWindowAttributes(hwnd, 0, alpha, LWA_ALPHA);
        }

        #region Win32 API
        private const int GWL_EXSTYLE = -20;
        private const UInt32 WS_EX_NOACTIVATE = 0x08000000;
        private const UInt32 WS_EX_LAYERED = 0x80000;
        private const UInt32 WS_EX_TRANSPARENT = 0x00000020;
        private const int LWA_ALPHA = 0x2;

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowEnabled(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);
        #endregion
    }
}