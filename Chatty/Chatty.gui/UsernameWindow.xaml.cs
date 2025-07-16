using System.Windows;

namespace ChatappUI
{
    public partial class UsernameWindow : Window
    {
        public string? Username { get; private set; }

        public UsernameWindow()
        {
            InitializeComponent();
        }

        private void StartChat_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(UsernameInput.Text))
            {
                Username = UsernameInput.Text.Trim();
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter a username.");
            }
        }
    }
}
