using ChattyClient.Core;
using ChattyClient.Networking;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using clientnew;
using System.Net.Sockets;
using System.Net;

namespace ChattyClient.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> Messages { get; set; } = new();

        private string _username = "";
        public Class1 client1;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        private string _currentMessage = "";
        public string CurrentMessage
        {
            get => _currentMessage;
            set
            {
                _currentMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand ConnectCommand { get; }
        public ICommand SendMessageCommand { get; }
        public ICommand DisconnectCommand { get; }
        public ICommand ToggleSidebarCommand { get; }

        private bool _isConnected = false;
        private bool _isSidebarOpen = true;

        public string SidebarWidth => _isSidebarOpen ? "200" : "0";
        public string SidebarVisibility => _isSidebarOpen ? "Visible" : "Collapsed";
        public string ToggleButtonText => _isSidebarOpen ? "◀" : "▶";

        public MainViewModel()
        {
            ConnectCommand = new RelayCommand(o => Connect(), o => !_isConnected && !string.IsNullOrWhiteSpace(Username));
            SendMessageCommand = new RelayCommand(o => SendMessage(), o => _isConnected && !string.IsNullOrWhiteSpace(CurrentMessage));
            DisconnectCommand = new RelayCommand(o => Disconnect(), o => _isConnected);
            ToggleSidebarCommand = new RelayCommand(o => ToggleSidebar());
        }

        private void Connect()
        {

            try
            {
                var client = new TcpClient();
                client.Connect(IPAddress.Parse("192.168.55.3"), 9001);
                Class1 client2 = new Class1(client);
                client1 = client2;
                client2.messagerecieved += OnMessageReceived;

                // Add system message with timestamp
                string timestamp = DateTime.Now.ToString("HH:mm:ss");
                Messages.Add($"[{timestamp}] (SYSTEM) Connected as {Username}");

                _isConnected = true;
                OnPropertyChanged(nameof(ConnectCommand));
                OnPropertyChanged(nameof(SendMessageCommand));
                OnPropertyChanged(nameof(DisconnectCommand));
            }
            catch (Exception ex)
            {
                string timestamp = DateTime.Now.ToString("HH:mm:ss");
                Messages.Add($"[{timestamp}] (ERROR) Failed to connect: {ex.Message}");
            }
        }

        private void OnMessageReceived(object? sender, (string, string) input)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                string timestamp = DateTime.Now.ToString("HH:mm:ss");
                Messages.Add($"[{timestamp}] {input.Item2}");
            });
        }

        private void OnMessageReceived((string,string) input)
        {
            
        }

        private void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(CurrentMessage)) return;

            try
            {
                client1.send_message(CurrentMessage);

                // Add your own message with timestamp
                string timestamp = DateTime.Now.ToString("HH:mm:ss");
                Messages.Add($"[{timestamp}] ({Username}) {CurrentMessage}");

                CurrentMessage = "";
            }
            catch (Exception ex)
            {
                string timestamp = DateTime.Now.ToString("HH:mm:ss");
                Messages.Add($"[{timestamp}] (ERROR) Failed to send: {ex.Message}");
            }
        }

        private void Disconnect()
        {
            try
            {

                string timestamp = DateTime.Now.ToString("HH:mm:ss");
                Messages.Add($"[{timestamp}] (SYSTEM) Disconnected");

                _isConnected = false;
                OnPropertyChanged(nameof(ConnectCommand));
                OnPropertyChanged(nameof(SendMessageCommand));
                OnPropertyChanged(nameof(DisconnectCommand));
            }
            catch (Exception ex)
            {
                string timestamp = DateTime.Now.ToString("HH:mm:ss");
                Messages.Add($"[{timestamp}] (ERROR) Disconnect failed: {ex.Message}");
            }
        }

        private void ToggleSidebar()
        {
            _isSidebarOpen = !_isSidebarOpen;
            OnPropertyChanged(nameof(SidebarWidth));
            OnPropertyChanged(nameof(SidebarVisibility));
            OnPropertyChanged(nameof(ToggleButtonText));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? prop = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}