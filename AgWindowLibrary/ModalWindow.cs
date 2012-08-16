namespace AgWindowLibrary
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    internal class ModalWindow
    {
        internal static void Show(Window modalWindow, Window ownerWindow, bool showInTaskBar = false)
        {
            var instance = new ModalWindow();
            instance.Invoke(modalWindow, ownerWindow, showInTaskBar);
        }

        private Action _close = () => { };

        private void Invoke(Window modalWindow, Window ownerWindow, bool showInTaskBar = false)
        {
            if (ownerWindow == null) throw new ArgumentNullException("parentWindow");

            modalWindow.Closing += WindowClosing;
            ownerWindow.Closing += WindowClosing;

            var ownerContent = WindowContent.GetWindowContent(ownerWindow);
            var grid = new Grid();
            var border = new Border { Background = new SolidColorBrush(Colors.Black), Opacity=0.5 };
            WindowContent.SetWindowContent(ownerWindow, grid);
            grid.Children.Add(ownerContent);
            grid.Children.Add(border);

            var ownerHwnd = Hwnd.FindHwnd(ownerWindow);
            bool isOwnerEnabled = Hwnd.IsWindowEnabled(ownerHwnd);
            Hwnd.EnableWindow(ownerHwnd, false);

            _close = () =>
            {
                modalWindow.Closing -= WindowClosing;
                ownerWindow.Closing -= WindowClosing;
                grid.Children.Remove(ownerContent);
                WindowContent.SetWindowContent(ownerWindow, ownerContent);
                Hwnd.EnableWindow(ownerHwnd, isOwnerEnabled);
                _close = () => { };
            };

            SetWindowStartupLocationAsCenterOwner(modalWindow, ownerWindow);

            modalWindow.TopMost = true;
            if (showInTaskBar == false)
            {
                var hwnd = Hwnd.FindHwnd(modalWindow);
                Hwnd.RemoveFromTaskBar(hwnd);
            }
            modalWindow.Show();
        }

        private void WindowClosing(object sender, ClosingEventArgs e)
        {
            _close();
        }

        private static void SetWindowStartupLocationAsCenterOwner(Window window, Window ownerWindow)
        {
            var x =
                window.Width == 0
                ? ownerWindow.Left + 16
                : ownerWindow.Left + ownerWindow.Width / 2 - window.Width / 2;
            var y =
                window.Height == 0
                ? ownerWindow.Top + 16
                : ownerWindow.Top + ownerWindow.Height / 2 - window.Height / 2;
            window.Left = x;
            window.Top = y;
        }
    }
}