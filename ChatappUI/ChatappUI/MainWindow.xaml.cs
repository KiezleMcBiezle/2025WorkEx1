using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatappUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void MessageInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true; // Prevent beep sound on enter

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
                        Text = System.DateTime.Now.ToString("HH:mm"),  // e.g., 14:23
                        FontFamily = new FontFamily("Comic Sans MS"),
                        FontSize = 12,
                        Foreground = Brushes.WhiteSmoke,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Margin = new Thickness(0, 4, 0, 0)
                    };

                    // StackPanel to hold message and timestamp vertically
                    var stack = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        Width = 250
                    };

                    stack.Children.Add(messageText);
                    stack.Children.Add(timestampText);

                    // Border around the whole message+time
                    var border = new Border
                    {
                        Background = Brushes.LightGreen,
                        CornerRadius = new CornerRadius(15),
                        Margin = new Thickness(100, 10, 0, 10),
                        Padding = new Thickness(10),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Child = stack
                    };

                    MessageContainer.Children.Add(border);

                    MessageInput.Clear();

                    // Scroll to bottom after adding message
                    MessageContainer.UpdateLayout();
                    var scrollViewer = (ScrollViewer)MessageContainer.Parent;
                    scrollViewer.ScrollToBottom();
                }
            }
        }
        private bool isSidebarOpen = false;

        private void ToggleMenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (isSidebarOpen)
            {
                SidebarColumn.Width = new GridLength(50); // narrow sidebar
                MenuItemsPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                SidebarColumn.Width = new GridLength(200); // expanded sidebar
                MenuItemsPanel.Visibility = Visibility.Visible;
            }
            isSidebarOpen = !isSidebarOpen;
        }

    }
}
