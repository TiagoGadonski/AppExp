using System.Windows;

namespace AppExp.Views
{
    public partial class InputBox : Window
    {
        public string Message { get; set; }
        public string ResponseText { get; set; }

        public InputBox(string message)
        {
            InitializeComponent();
            Message = message;
            DataContext = this;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
