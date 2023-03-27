using StickyNotes.Core.Mvvm;
using System;
using System.Windows;

namespace StickyNotes.UI.Mvvm
{
    public sealed class DialogService : IDialogService
    {
        public static readonly IDialogService Default = new DialogService();

        private Window CreateWindow(string typeName, string title, Action<DialogResult> callback)
        {
            var container = ((App)Application.Current).Container;
            var window = container.Resolve<Window>(typeName);
            window.Closed += (s, e) =>
            {
                callback?.Invoke(window.DialogResult == true ? DialogResult.OK : DialogResult.Cancel);
            };
            window.Title = title;
            if (window.Owner == null)
            {
                window.Owner = Application.Current.MainWindow;
            }
            var parent = window.Owner;
            var left = parent.Left + parent.Width;
            var top = parent.Top;
            foreach (Window child in parent.OwnedWindows)
            {
                var rect = new Rect(child.Left - 1, child.Top - 1, child.Width + 2, child.Height + 2);
                if (rect.Contains(new Point(left, top)))
                {
                    left += 15;
                    top += 15;
                }
            }
            window.Left = left;
            window.Top = top;

            return window;
        }

        public void Show(string name, string title, object parameter, Action<DialogResult> callback)
        {
            var window = CreateWindow(name, title, callback);
            if (window.DataContext is IDialogViewModel vm)
            {
                vm.OnDialogOpened(parameter);
            }
            window.Show();
        }

        public bool? ShowDialog(string name, string title, object parameter, Action<DialogResult> callback)
        {
            var window = CreateWindow(name, title, callback);
            if (window.DataContext is IDialogViewModel vm)
            {
                vm.OnDialogOpened(parameter);
            }
            return window.ShowDialog();
        }
    }
}
