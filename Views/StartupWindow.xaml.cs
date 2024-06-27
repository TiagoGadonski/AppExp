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
            // Pega o número de jogadores e o número de bots do UI (isso precisa ser implementado no XAML)
            if (int.TryParse(NumPlayersTextBox.Text, out int numPlayers) && int.TryParse(NumBotsTextBox.Text, out int numBots))
            {
                // Inicia o jogo com os jogadores e bots especificados
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

            // Verifique se não há mais janelas abertas e se sim, encerre a aplicação.
            if (Application.Current.Windows.Count == 0)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
