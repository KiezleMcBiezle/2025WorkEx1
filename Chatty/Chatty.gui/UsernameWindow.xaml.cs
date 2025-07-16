using System.Windows;
using System.Windows.Input;

namespace ChatappUI
{
    public partial class UsernameWindow : Window
    {
        public string Username { get; set; }

        public UsernameWindow()
        {
            InitializeComponent();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void StartChat_Click(object sender, RoutedEventArgs e)
        {
            string enteredUsername = UsernameInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(enteredUsername))
            {
                MessageBox.Show("Please enter a valid username.", "Invalid Username", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Username = enteredUsername;
            DialogResult = true;  // Close dialog and return true to caller
        }
    }
}
