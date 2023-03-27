namespace StickyNotes.Core.Mvvm
{
    public enum DialogResult
    {
        None,
        OK,
        Cancel
    }

    public interface IDialogService
    {
        void Show(string name, string title, object parameter, Action<DialogResult> callback);
        bool? ShowDialog(string name, string title, object parameter, Action<DialogResult> callback);
    }

    public interface IDialogViewModel
    {
        void OnDialogOpened(object parameter);
        bool? OnDialogClosed();
        bool EqualsTo(object parameter);
    }
}
