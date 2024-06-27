using System.Windows;

namespace AppExp.Views
{
    public partial class InsertExplodingKittenDialog : Window
    {
        public int Position { get; private set; }

        public InsertExplodingKittenDialog(int maxPosition)
        {
            InitializeComponent();
            PositionTextBox.Text = "0"; // Default position
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(PositionTextBox.Text, out int position))
            {
                Position = position;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Por favor, insira um número válido.");
            }
        }
    }
}
