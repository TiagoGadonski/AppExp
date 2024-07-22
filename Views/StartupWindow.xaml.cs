using System.Windows;

namespace AppExp.Views
{
    public partial class StartupWindow : Window
    {
        public StartupWindow()
        {
            InitializeComponent();
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(NumPlayersTextBox.Text, out int numPlayers) && int.TryParse(NumBotsTextBox.Text, out int numBots))
            {
                if ((numPlayers + numBots > 6)|| (numPlayers + numBots <= 1))
                {
                    MessageBox.Show("O número total de jogadores e bots devem ser entre 2 e 6.");
                    return;
                }

                MainWindow mainWindow = new MainWindow(numPlayers, numBots);
                mainWindow.Show();

                this.Close();
            }
            else
            {
                MessageBox.Show("Por favor, insira números válidos para jogadores e bots.");
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (Application.Current.Windows.Count == 0)
            {
                Application.Current.Shutdown();
            }
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = e.Uri.ToString(),
                UseShellExecute = true
            });
            e.Handled = true;
        }
    }
}
