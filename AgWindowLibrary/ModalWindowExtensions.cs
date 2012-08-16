namespace AgWindowLibrary
{
    using System.Windows;

    public static class ModalWindowExtensions
    {
        public static void ShowModal(this Window modalWindow, Window ownerWindow, bool showInTaskBar = false)
        {
            ModalWindow.Show(modalWindow, ownerWindow, showInTaskBar);
        }
    }
}

