using AppExp.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace AppExp.Views
{
    public partial class SelectPlayerDialog : Window, INotifyPropertyChanged
    {
        private Player _selectedPlayer;

        public SelectPlayerDialog(ObservableCollection<Player> players)
        {
            InitializeComponent();
            Players = players;
            DataContext = this;
            OkCommand = new RelayCommand(_ => DialogResult = true);
        }

        public ObservableCollection<Player> Players { get; }

        public Player SelectedPlayer
        {
            get => _selectedPlayer;
            set
            {
                _selectedPlayer = value;
                OnPropertyChanged();
            }
        }

        public ICommand OkCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
