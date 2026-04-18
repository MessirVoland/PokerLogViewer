using System.Windows;
using Application = System.Windows.Application;

namespace PokerLogViewer
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var vm = new MainViewModel();

            var window = new MainWindow
            {
                DataContext = vm
            };

            window.Show();
        }
    }
}