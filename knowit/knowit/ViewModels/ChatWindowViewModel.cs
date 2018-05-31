using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using knowit.Models;
using knowit.ViewModels;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Uwp.Helpers;

namespace knowit
{
    public class ChatWindowViewModel
    {
        private static ChatWindowViewModel instance;
        private ChatWindowViewModel() { }
        public static ChatWindowViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new ChatWindowViewModel();
            }
            return instance;
        }
        public ObservableCollection<MessageBase> allMessages = new ObservableCollection<MessageBase>();
        
        //添加消息
        public async void AddMessageSelf(string username, string message)
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                allMessages.Add(new Message
                {
                    Com = message,
                    IsSelf = true,
                    Name = username
                });
            });
        }
        public async void AddMessage(string username, string message)
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                allMessages.Add(new Message
                {
                    Com = message,
                    Name = username
                });
            });
        }
    }
}
