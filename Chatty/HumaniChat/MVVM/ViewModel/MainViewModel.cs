using HumaniChat.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaniChat.MVVM.ViewModel
{
    class MainViewModel
    {
        public ObservableCollection<MessageModel> Messages { get; set; }
        public ObservableCollection<ContactModel> Contacts { get; set; }

        public MainViewModel()
        {
            Messages = new ObservableCollection<MessageModel>();
            Contacts = new ObservableCollection<ContactModel>();

            Messages.Add( new MessageModel
            {
                Username = "Eve",
                UsernameColor = "#37054d",
                ImageSource = "https://e7.pngegg.com/pngimages/925/734/png-clipart-woman-pointing-left-side-while-smiling-woman-smile-female-information-woman-hand-people.png",
                Message = "Sup",
                Time = DateTime.Now,
                IsNativeOrigin = false,
                FirstMessage = true
            });

            for (int i = 0; i < 3; i++)
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
            for (int i = 0; i < 4; i++)
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

            for (int i = 0;i < 5;i++) 
            {
                Contacts.Add(new ContactModel
                {
                    Username = $"Eve {i}",
                    ImageSource = "https://e7.pngegg.com/pngimages/925/734/png-clipart-woman-pointing-left-side-while-smiling-woman-smile-female-information-woman-hand-people.png",
                    Messages = Messages
                });

            }



        }
    }
}
