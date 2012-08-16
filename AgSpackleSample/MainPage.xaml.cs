namespace AgSpackleSample
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;
    using AgWindowLibrary;

    public partial class MainPage : UserControl
    {
        Windows _windows;
        int _id;

        public MainPage()
        {
            InitializeComponent();

            _windows = new Windows();
            this.Loaded += (s, e) =>
            {
                var window = Window.GetWindow(this);
                window.Closing += (s2,e2) => _windows.Dispose();
            };

            Func<IntPtr, Window> TryFind = hwnd => 
                { 
                    foreach(Window window in Application.Current.Windows)
                    {
                        if( Hwnd.FindHwnd(window) == hwnd) return window;
                    }
                    return null;
                };

            Func<IntPtr,string> TryFindTitle = hwnd =>
                {
                    Window window = TryFind(hwnd);
                    return window != null ? window.Title : "";
                };
            _windows.WindowActivated += (s, e) =>
                {
                    Events.Items.Add("+ " + TryFindTitle(e.Hwnd));
                };

            _windows.WindowDeactivated += (s, e) =>
                {
                    Events.Items.Add("- " + TryFindTitle(e.Hwnd));
                };
            
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += (s,e) =>
            {
                Dispatcher.BeginInvoke(() => { textBlock1.Text = DateTime.Now.ToString(); });
            };
            timer.Start();
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

        private void NewModalDialogWindowButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new Window { Title = "Dialog Window" + _id, Width = 320, Height = 200 };
            var owner = Window.GetWindow(this);
            window.ShowModal(owner);
            ++_id;
        }
    }
}
