using clientnew;
using HumaniChat.MVVM.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace HumaniChat.MVVM.ViewModel;

class MainViewModel: INotifyPropertyChanged
{
    private readonly Client _client;
    private string? currentMessage = "";

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<MessageModel> Messages { get; set; }
    public ObservableCollection<ContactModel> Contacts { get; set; }

    public ICommand SendMessageCommand { get; }
    public string? CurrentMessage
    {
        get => currentMessage;
        set
        {
            currentMessage = value;
            OnPropertyChanged();
        }
    }
    public MainViewModel()
    {
        Messages = new ObservableCollection<MessageModel>();
        Contacts = new ObservableCollection<ContactModel>();

        SendMessageCommand = new RelayCommand(o => SendMessage(), o => !string.IsNullOrWhiteSpace(CurrentMessage));

        _client = new Client();
        _client.Connect("192.168.55.3", 9001);
        _client.MessageReceived += OnMessageReceived;

        #region TestMessages
        Messages.Add(new MessageModel
        {
            Username = "Eve",
            UsernameColor = "#37054d",
            ImageSource = "https://e7.pngegg.com/pngimages/925/734/png-clipart-woman-pointing-left-side-while-smiling-woman-smile-female-information-woman-hand-people.png",
            Message = "Sup",
            Time = DateTime.Now,
            IsNativeOrigin = false,
            FirstMessage = true
        });

        for (var i = 0; i < 3; i++)
        {
            Messages.Add(new MessageModel
            {
                Username = "Eve",
                UsernameColor = "#37054d",
                ImageSource = "https://e7.pngegg.com/pngimages/925/734/png-clipart-woman-pointing-left-side-while-smiling-woman-smile-female-information-woman-hand-people.png",
                Message = "I'm so tired",
                Time = DateTime.Now,
                IsNativeOrigin = false,
                FirstMessage = false
            });
        }

        for (var i = 0; i < 4; i++)
        {

            Messages.Add(new MessageModel
            {
                Username = "Eve",
                UsernameColor = "#37054d",
                ImageSource = "https://e7.pngegg.com/pngimages/925/734/png-clipart-woman-pointing-left-side-while-smiling-woman-smile-female-information-woman-hand-people.png",
                Message = "lol",
                Time = DateTime.Now,
                IsNativeOrigin = true,
            });
        }

        Messages.Add(new MessageModel
        {
            Username = "Nana",
            UsernameColor = "#37054d",
            ImageSource = "https://e7.pngegg.com/pngimages/925/734/png-clipart-woman-pointing-left-side-while-smiling-woman-smile-female-information-woman-hand-people.png",
            Message = "Last",
            Time = DateTime.Now,
            IsNativeOrigin = true,
        });
        #endregion

        for (var i = 0; i < 5; i++)
        {
            Contacts.Add(new ContactModel
            {
                Username = $"Eve {i}",
                ImageSource = "https://e7.pngegg.com/pngimages/925/734/png-clipart-woman-pointing-left-side-while-smiling-woman-smile-female-information-woman-hand-people.png",
                Messages = Messages
            });

        }

    }

    private void OnMessageReceived(object? sender, string incomingMessage)
    {
        Messages.Add(new MessageModel
        {
            Username = "Unknown",
            UsernameColor = "#37054d",
            ImageSource = "https://e7.pngegg.com/pngimages/925/734/png-clipart-woman-pointing-left-side-while-smiling-woman-smile-female-information-woman-hand-people.png",
            Message = incomingMessage,
            Time = DateTime.Now,
            IsNativeOrigin = false,
            FirstMessage = false,
        });
    }

    private void SendMessage()
    {
        if (CurrentMessage == null)
        {
            return;
        }

        _client.SendMessage(CurrentMessage);
        Messages.Add(new MessageModel
        {
            Username = "You",
            UsernameColor = "#37054d",
            ImageSource = "https://e7.pngegg.com/pngimages/925/734/png-clipart-woman-pointing-left-side-while-smiling-woman-smile-female-information-woman-hand-people.png",
            Message = CurrentMessage,
            Time = DateTime.Now,
            IsNativeOrigin = false,
            FirstMessage = false,
        });
        CurrentMessage = "";
    }

    private void OnPropertyChanged([CallerMemberName] string? prop = null) =>
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
}