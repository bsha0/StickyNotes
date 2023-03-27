using StickyNotes.Core.Data;
using StickyNotes.Core.Mvvm;
using StickyNotes.Data;
using StickyNotes.UI.Mvvm;
using StickyNotes.UI.ViewModels;
using StickyNotes.UI.Views;
using System.Reflection;
using System.Windows;
using Winforms = System.Windows.Forms;

namespace StickyNotes.UI
{
    public partial class App : Application
    {
        public IContainer Container { get; init; }

        public App()
        {
            this.Container = Core.Mvvm.Container.Default;
            this.Container.ConfigureContainer();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            this.MainWindow = CreateMainWindow();
            MainWindow.Show();
        }

        private Window CreateMainWindow()
        {
            var position = AppSettings.Default.WindowPosition;
            var window = Container.Resolve<MainWindow>();
            if (position.Width > 0)
            {
                window.WindowStartupLocation = WindowStartupLocation.Manual;
                window.Left = position.Left;
                window.Top = position.Top;
                window.Width = position.Width;
                window.Height = position.Height;
            }
            else
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            window.Loaded += (s, e) =>
            {
                CreateNotifyIcon();
            };

            window.Closed += (s, e) =>
            {
                AppSettings.Default.WindowPosition = new Rect(window.Left, window.Top, window.Width, window.Height);
                AppSettings.Default.Save();
            };
            return window;
        }

        private void CreateNotifyIcon()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var image = assembly.GetManifestResourceStream("StickyNotes.UI.Resources.Images.AppIcon.ico");
            var notifyIcon = new Winforms.NotifyIcon
            {
                BalloonTipTitle = "Sticky Notes",
                BalloonTipText = "Hello, Sticky Notes!",
                Text = "Sticky Notes",
                Icon = new System.Drawing.Icon(image),
                Visible = true
            };
            notifyIcon.ShowBalloonTip(1000);

            var contextMenu = new Winforms.ContextMenuStrip();
            notifyIcon.ContextMenuStrip = contextMenu;
            var openMenuItem = new Winforms.ToolStripMenuItem();
            openMenuItem.Text = "Open";
            openMenuItem.Click += (s, e) => { this.MainWindow.Show(); };
            contextMenu.Items.Add(openMenuItem);
            var closeMenuItem = new Winforms.ToolStripMenuItem();
            closeMenuItem.Text = "Close";
            closeMenuItem.Click += (s, e) => { this.Shutdown(); };
            contextMenu.Items.Add(closeMenuItem);
            notifyIcon.DoubleClick += (s, e) =>
            {
                this.MainWindow.Show();
            };
        }
    }

    public static class ContainerExtension
    {
        public static void ConfigureContainer(this IContainer container)
        {
            container.ConfigureServices().ConfigureViewAndViewModels().Build();
        }

        private static IContainer ConfigureServices(this IContainer container)
        {
            container.AddSingleton<IContainer>(container);
            container.AddSingleton<INoteItemRepository>(new NoteItemRepository("note.db3"));
            container.AddSingleton<IDialogService, DialogService>();

            return container;
        }

        private static IContainer ConfigureViewAndViewModels(this IContainer container)
        {
            container.AddTransient<MainWindow>();
            container.AddTransient<MainWindowViewModel>();

            container.AddTransient<NoteItemWindow>();
            container.AddTransient<NoteItemWindowViewModel>();

            return container;
        }
    }
}
