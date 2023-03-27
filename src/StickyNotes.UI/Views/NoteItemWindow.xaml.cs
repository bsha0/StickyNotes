using StickyNotes.Core.Mvvm;
using StickyNotes.UI.ViewModels;
using System.Windows;

namespace StickyNotes.UI.Views
{
    public partial class NoteItemWindow : Window
    {
        public NoteItemWindow()
        {
            InitializeComponent();
            this.DataContext = Container.Default.Resolve<NoteItemWindowViewModel>();
        }
    }
}
