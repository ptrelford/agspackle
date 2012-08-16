namespace AgWindowLibrary
{
    using System.Windows;
    using System.Windows.Controls;

    internal class WindowContent
    {
        internal static FrameworkElement GetWindowContent(Window window)
        {
            if (Application.Current.MainWindow == window)
            {
                var rootVisual = Application.Current.RootVisual;
                if (rootVisual is UserControl)
                {
                    return ((UserControl)rootVisual).Content as FrameworkElement;
                }
                else if (rootVisual is ContentControl)
                {
                    return ((ContentControl)rootVisual).Content as FrameworkElement;
                }
                else // Not supported
                {
                    return null;
                }
            }
            else
            {
                return window.Content;
            }
        }

        internal static void SetWindowContent(Window window, FrameworkElement content)
        {
            if (Application.Current.MainWindow == window)
            {
                var rootVisual = Application.Current.RootVisual;
                if (rootVisual is UserControl)
                {
                    ((UserControl)rootVisual).Content = content;
                }
                else if (rootVisual is ContentControl)
                {
                    ((ContentControl)rootVisual).Content = content;
                }
                else // Not supported
                {
                }
            }
            else
            {
                window.Content = content;
            }
        }
    }
}