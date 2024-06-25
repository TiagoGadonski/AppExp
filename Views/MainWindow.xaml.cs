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
    }
}
