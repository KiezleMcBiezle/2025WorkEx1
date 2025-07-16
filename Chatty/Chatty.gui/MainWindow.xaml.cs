using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ChatappUI
{
    public partial class MainWindow : Window
    {
        private bool isSidebarOpen = false;
        public string CurrentUsername { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            // Show username entry window
            var usernameWindow = new UsernameWindow();
            bool? result = usernameWindow.ShowDialog();

            if (result == true)
            {
                CurrentUsername = usernameWindow.Username;
            }
            else
            {
                Close(); // Exit if user doesn't enter a name
            }
        }

        private void MessageInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;

                string message = MessageInput.Text.Trim();

                if (!string.IsNullOrEmpty(message))
                {
                    // Username TextBlock
                    var usernameText = new TextBlock
                    {
                        Text = CurrentUsername ?? "You",
                        FontWeight = FontWeights.Bold,
                        FontFamily = new FontFamily("Comic Sans MS"),
                        FontSize = 14,
                        Foreground = Brushes.DarkGreen,
                        Margin = new Thickness(0, 0, 0, 4)
                    };

                    // Message TextBlock
                    var messageText = new TextBlock
                    {
                        Text = message,
                        FontFamily = new FontFamily("Comic Sans MS"),
                        FontSize = 21,
                        Foreground = Brushes.White,
                        TextWrapping = TextWrapping.Wrap,
                        Width = 250
                    };

                    // Timestamp TextBlock
                    var timestampText = new TextBlock
                    {
                        Text = DateTime.Now.ToString("HH:mm"),
                        FontFamily = new FontFamily("Comic Sans MS"),
                        FontSize = 12,
                        Foreground = Brushes.WhiteSmoke,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Margin = new Thickness(0, 4, 0, 0)
                    };

                    // Stack for all content
                    var stack = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        Width = 250
                    };

                    stack.Children.Add(usernameText);
                    stack.Children.Add(messageText);
                    stack.Children.Add(timestampText);

                    // Final message border with updated background
                    var border = new Border
                    {
                        Background = (Brush)new BrushConverter().ConvertFromString("#404040"),
                        CornerRadius = new CornerRadius(15),
                        Margin = new Thickness(100, 10, 0, 10),
                        Padding = new Thickness(10),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Child = stack
                    };

                    // Add message to container
                    MessageContainer.Children.Add(border);

                    MessageInput.Clear();

                    // Scroll to bottom
                    MessageContainer.UpdateLayout();
                    if (MessageContainer.Parent is ScrollViewer scrollViewer)
                    {
                        scrollViewer.ScrollToBottom();
                    }
                }
            }
        }

        private void ToggleMenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (isSidebarOpen)
            {
                SidebarColumn.Width = new GridLength(50);
                MenuItemsPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                SidebarColumn.Width = new GridLength(200);
                MenuItemsPanel.Visibility = Visibility.Visible;
            }

            isSidebarOpen = !isSidebarOpen;
        }

        private void ChangeUsernameButton_Click(object sender, RoutedEventArgs e)
        {
            var usernameWindow = new UsernameWindow();

            // Prefill the current username if UsernameWindow supports it
            usernameWindow.Username = CurrentUsername;

            bool? result = usernameWindow.ShowDialog();

            if (result == true && !string.IsNullOrWhiteSpace(usernameWindow.Username))
            {
                CurrentUsername = usernameWindow.Username;
                MessageBox.Show($"Username changed to: {CurrentUsername}", "Username Changed", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
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
    }
}
