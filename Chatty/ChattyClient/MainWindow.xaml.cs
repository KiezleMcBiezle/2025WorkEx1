using System.Windows;
using System.Windows.Input;
using ChattyClient.ViewModel;

namespace ChattyClient
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var viewModel = DataContext as MainViewModel;
                if (viewModel?.SendMessageCommand.CanExecute(null) == true)
                {
                    viewModel.SendMessageCommand.Execute(null);
                }
            }
        }
    }
}