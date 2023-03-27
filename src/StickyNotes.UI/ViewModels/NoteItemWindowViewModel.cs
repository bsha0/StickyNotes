using CommunityToolkit.Mvvm.ComponentModel;
using StickyNotes.Core.Entities;
using StickyNotes.Core.Mvvm;

namespace StickyNotes.UI.ViewModels
{
    public class NoteItemWindowViewModel : ObservableObject, IDialogViewModel
    {
        #region NoteItem
        private DynamicViewModel<NoteItem> noteItem;
        public DynamicViewModel<NoteItem> NoteItem
        {
            get { return noteItem; }
            set { SetProperty(ref noteItem, value); }
        }
        #endregion

        #region IDialogViewModel
        public bool? OnDialogClosed()
        {
            return string.IsNullOrEmpty(NoteItem.Model.Content);
        }

        public void OnDialogOpened(object parameter)
        {
            if (parameter is DynamicViewModel<NoteItem> vm)
            {
                NoteItem = vm;
            }
        }

        public bool EqualsTo(object parameter)
        {
            return parameter == NoteItem;
        }
        #endregion
    }
}
