namespace AgWindowLibrary
{
    using System;
    using System.Runtime.InteropServices;

    public abstract class WindowActivationEventArgs : EventArgs
    {
        private readonly IntPtr _hwnd;

        public WindowActivationEventArgs(IntPtr hwnd)
        {
            _hwnd = hwnd;
        }

        public IntPtr Hwnd { get { return _hwnd; } }
    }

    public class WindowDeactivatedEventArgs : WindowActivationEventArgs
    {
        public WindowDeactivatedEventArgs(IntPtr hwnd) : base(hwnd) { }
    }

    public class WindowActivatedEventArgs : WindowActivationEventArgs
    {
        public WindowActivatedEventArgs(IntPtr hwnd) : base(hwnd) { }
    }

    public class Windows : IDisposable
    {
        private HookProc _callback; // Prevent GC from collecting the delegate!
        private IntPtr _hHook = IntPtr.Zero;

        public Windows()
        {
            _callback = WindowHook;
            _hHook = SetWindowsHookEx(WH_CALLWNDPROC, _callback, IntPtr.Zero, GetCurrentThreadId());

        }

        public void Dispose()
        {
            if (_hHook != IntPtr.Zero)
                UnhookWindowsHookEx(_hHook);
        }

        public event EventHandler<WindowDeactivatedEventArgs> WindowDeactivated;
        public event EventHandler<WindowActivatedEventArgs> WindowActivated;

        [AllowReversePInvokeCalls]
        private IntPtr WindowHook(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code == HC_ACTION)
            {
                var messageInfo = new CWPSTRUCT();
                Marshal.PtrToStructure(lParam, messageInfo);

                if (messageInfo.message == WM_ACTIVATE)
                {
                    var hwnd = messageInfo.lparam;
                    if ((int)messageInfo.wparam == WA_INACTIVE)
                    {
                        var e = WindowActivated;
                        if (e != null)
                            e(this, new WindowActivatedEventArgs(hwnd));
                    }
                    else
                    {
                        var e = WindowDeactivated;
                        if (e != null)
                            e(this, new WindowDeactivatedEventArgs(hwnd));
                    }
                }

            }
            return CallNextHookEx(_hHook, code, wParam, lParam);
        }

        #region P/Invoke
        private const int HC_ACTION = 0;
        private const int WH_CALLWNDPROC = 4;
        private const int WA_INACTIVE = 0;
        private const int WM_ACTIVATE = 0x0006;

        [StructLayout(LayoutKind.Sequential)]
        public class CWPSTRUCT
        {
            public IntPtr lparam;
            public IntPtr wparam;
            public int message;
            public IntPtr hwnd;
        }

        delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(int hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool UnhookWindowsHookEx(IntPtr hhk);
        #endregion
    }
}
