﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using knowit.Models;
using knowit.ViewModels;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;

namespace knowit
{
    public sealed partial class PostPageM : Page
    {
        public static string res;
        private string username;
        private string password;
        ListItemViewModels myViewModels = ListItemViewModels.GetInstance();
        ChatWindowViewModel myChatModels = ChatWindowViewModel.GetInstance();
        public PostPageM()
        {
            InitializeComponent();
            NetworkControl.InitialWebSocket();
        }
        private void Post_Click(object sender, ItemClickEventArgs args)
        {
            ListItem post = args.ClickedItem as ListItem;
            string[] info = new string[3];
            info[0] = username;
            info[1] = password;
            info[2] = post.id;
            Frame.Navigate(typeof(PostPage), info);
        }
        private async void InitializePost()
        {
            myViewModels.allPosts.Clear();
            ObservableCollection<PostItem> posts = await NetworkControl.GetPostCollection(username, password);
            if(posts != null)
            {
                foreach (PostItem item in posts)
                {
                    string thumbs_num = item.thumbs.ToString();
                    myViewModels.AddPost(item.id, item.editor, item.title, thumbs_num, item.imageURL, item.videoURL);
                }
            }
            
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string[] user_info = e.Parameter as string[];
            username = user_info[0];
            password = user_info[1];
            InitializePost();
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            NetworkControl.closeWs();
        }
        private async void SendMessage_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            string mess = null;
            message.Document.GetText(Windows.UI.Text.TextGetOptions.AdjustCrlf, out mess);
            if (mess != "")
            {
                await NetworkControl.SendChatMessage(username, password, mess);
                message.Document.SetText(Windows.UI.Text.TextSetOptions.ApplyRtfDocumentDefaults, "");
            }
        }

    }
}
