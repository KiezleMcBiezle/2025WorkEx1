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

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MessageInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true; // Prevent Enter key from making a newline or beeping

                string message = MessageInput.Text.Trim();

                if (!string.IsNullOrEmpty(message))
                {
                    // Create message TextBlock
                    var messageText = new TextBlock
                    {
                        Text = message,
                        FontFamily = new FontFamily("Comic Sans MS"),
                        FontSize = 21,
                        Foreground = Brushes.White,
                        TextWrapping = TextWrapping.Wrap,
                        Width = 250
                    };

                    // Create timestamp TextBlock
                    var timestampText = new TextBlock
                    {
                        Text = DateTime.Now.ToString("HH:mm"),
                        FontFamily = new FontFamily("Comic Sans MS"),
                        FontSize = 12,
                        Foreground = Brushes.WhiteSmoke,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Margin = new Thickness(0, 4, 0, 0)
                    };

                    // StackPanel to hold message and timestamp
                    var stack = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        Width = 250
                    };
                    stack.Children.Add(messageText);
                    stack.Children.Add(timestampText);

                    // Bubble border
                    var border = new Border
                    {
                        Background = Brushes.LightGreen,
                        CornerRadius = new CornerRadius(15),
                        Margin = new Thickness(100, 10, 0, 10),
                        Padding = new Thickness(10),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Child = stack
                    };

                    // Add to message container
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
    }
}
