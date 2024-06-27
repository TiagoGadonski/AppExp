using AppExp.ViewModels;
using System.Windows;

namespace AppExp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(int numPlayers, int numBots)
        {
            InitializeComponent();
            DataContext = new GameViewModel(numPlayers, numBots);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Verifique se não há mais janelas abertas e se sim, encerre a aplicação.
            if (Application.Current.Windows.Count == 0)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
