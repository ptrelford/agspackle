namespace AgSpackleSample
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using AgWindowLibrary;

    public partial class MainPage : UserControl
    {
        Windows _windows;
        int _id;

        public MainPage()
        {
            InitializeComponent();

            _windows = new Windows();

            Func<IntPtr, Window> Find = hwnd => 
                { 
                    foreach(Window window in Application.Current.Windows)
                    {
                        if( Hwnd.FindHwnd(window) == hwnd) return window;
                    }
                    throw new InvalidOperationException();
                };

            _windows.WindowActivated += (s, e) =>
                {
                    Events.Items.Add("+ " + Find(e.Hwnd).Title);
                };

            _windows.WindowDeactivated += (s, e) =>
                {
                    Events.Items.Add("- " + Find(e.Hwnd).Title);
                };
        }

        private void NewTransparentWindowButton_Click(object sender, RoutedEventArgs e)
        {
            var window = 
                new Window { 
                    Title = "Transparent Window "+_id, 
                    WindowStyle=WindowStyle.None, 
                    Width=320, 
                    Height=200 
                };
            window.Content = new TextBlock { Text=window.Title };
            var hwnd = Hwnd.FindHwnd(window);
            Hwnd.SetTransparency(hwnd, 128);
            window.Show();
            ++_id;
        }

        private void NewWindowNotInTaskBarButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new Window { Title = "Not In TaskBar"+_id, Width=320, Height=200  };
            var hwnd = Hwnd.FindHwnd(window);
            window.Show();
            window.Hide();
            Hwnd.RemoveFromTaskBar(hwnd);
            window.Show();
            ++_id;
        }
    }
}
