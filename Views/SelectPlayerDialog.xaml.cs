using AppExp.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace AppExp.Views
{
    public partial class SelectPlayerDialog : Window
    {
        public SelectPlayerDialog(ObservableCollection<Player> players)
        {
            InitializeComponent();
            Players = players;
            DataContext = this;
        }

        public ObservableCollection<Player> Players { get; }
        public Player SelectedPlayer { get; private set; }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            SelectedPlayer = (Player)PlayerListBox.SelectedItem;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
