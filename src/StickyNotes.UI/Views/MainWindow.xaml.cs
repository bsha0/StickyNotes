using StickyNotes.UI.ViewModels;
using System.Windows;
using System.Windows.Input;
using StickyNotes.Core.Mvvm;

namespace StickyNotes.UI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = Container.Default.Resolve<MainWindowViewModel>();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void OnCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
