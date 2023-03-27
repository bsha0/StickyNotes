using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StickyNotes.Core.Data;
using StickyNotes.Core.Entities;
using StickyNotes.Core.Mvvm;
using StickyNotes.UI.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace StickyNotes.UI.ViewModels
{
    public class MainWindowViewModel : ObservableObject, IRefresh
    {
        private readonly INoteItemRepository _repository;
        private readonly IDialogService _dialogService;

        #region NoteItems

        private ObservableCollection<DynamicViewModel<NoteItem>> noteItems = new();
        public ObservableCollection<DynamicViewModel<NoteItem>> NoteItems { get { return noteItems; } }
        public ICollectionView NoteItemsView { get; init; }

        #endregion

        #region SearchText
        private string searchText;

        public string SearchText
        {
            get { return searchText; }
            set
            {
                if (SetProperty(ref searchText, value))
                {
                    this.Refresh();
                }
            }
        }

        #endregion

        #region SearchCommand

        private ICommand searchCommand;
        public ICommand SearchCommand
        {
            get { return searchCommand ?? (searchCommand = new RelayCommand(Search)); }
        }

        private void Search()
        {
            this.Refresh();
        }

        #endregion

        #region AddItemCommand

        private ICommand addItemCommand;
        public ICommand AddItemCommand
        {
            get { return addItemCommand ?? (addItemCommand = new RelayCommand(AddItem)); }
        }

        private void AddItem()
        {
            var item = new NoteItem() { LastModificationTime = DateTime.Now };
            _repository.Add(item);
            var vm = new DynamicViewModel<NoteItem>(item);
            NoteItems.Add(vm);
            OpenItem(vm);
        }

        #endregion

        #region DeleteItemCommand

        private ICommand deleteItemCommand;
        public ICommand DeleteItemCommand
        {
            get { return deleteItemCommand ?? (deleteItemCommand = new RelayCommand<DynamicViewModel<NoteItem>>(DeleteItem)); }
        }

        private void DeleteItem(DynamicViewModel<NoteItem> item)
        {
            NoteItems.Remove(item);
            _repository.Delete(item.Model.Id);
            foreach (Window window in Application.Current.MainWindow.OwnedWindows)
            {
                if (window.DataContext is IDialogViewModel vm && vm.EqualsTo(item))
                {
                    window.Close();
                }
            }
        }

        #endregion

        #region DeleteItemCommand

        private ICommand openItemCommand;
        public ICommand OpenItemCommand
        {
            get { return openItemCommand ?? (openItemCommand = new RelayCommand<DynamicViewModel<NoteItem>>(OpenItem)); }
        }

        private void OpenItem(DynamicViewModel<NoteItem> item)
        {
            _dialogService.Show(typeof(NoteItemWindow).FullName, "New Item", item, (result) =>
            {
                if (string.IsNullOrEmpty(item.Model.Content))
                {
                    NoteItems.Remove(item);
                }
                ((dynamic)item).LastModificationTime = DateTime.Now;
                _repository.Update(item.Model);
            });
        }
        #endregion

        public MainWindowViewModel(INoteItemRepository repository, IDialogService dialogService)
        {
            _repository = repository;
            _dialogService = dialogService;

            foreach (var item in repository.GetAll())
            {
                NoteItems.Add(new DynamicViewModel<NoteItem>(item));
            }

            NoteItemsView = CollectionViewSource.GetDefaultView(NoteItems);
            NoteItemsView.Filter = (x =>
            {
                if (string.IsNullOrEmpty(SearchText))
                {
                    return true;
                }
                if (x is DynamicViewModel<NoteItem> item)
                {
                    if (!string.IsNullOrEmpty(item.Model.Content))
                    {
                        var doc = new FlowDocument();
                        var range = new TextRange(doc.ContentStart, doc.ContentEnd);
                        range.Load(new MemoryStream(Encoding.UTF8.GetBytes(item.Model.Content)), DataFormats.Xaml);
                        return range.Text.Contains(SearchText);
                    }
                }
                return false;
            });
        }

        #region IRefresh
        public void Refresh()
        {
            NoteItemsView.Refresh(); ;
        }
        #endregion
    }
}
