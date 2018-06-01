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
using Windows.UI.Xaml;
using System.IO;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace knowit
{
    public sealed partial class PostPageM : Page
    {
        public static string res;
        private string username;
        private string password;
        ListItemViewModels myViewModels = ListItemViewModels.GetInstance();
        ChatWindowViewModel myChatModels = ChatWindowViewModel.GetInstance();

        private DispatcherTimer timer = new DispatcherTimer();
        private int tick_id = 0;

        public PostPageM()
        {
            InitializeComponent();
            NetworkControl.InitialWebSocket();

            //Control tile change time
            timer.Interval = new TimeSpan(0, 0, 3);
            timer.Tick += ChangeTile;
            timer.Start();
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

        private void ChangeTile(object sender, object e)
        {
            XmlDocument xmld = new XmlDocument();
            xmld.LoadXml(File.ReadAllText("Tile.xml"));
            var textList = xmld.GetElementsByTagName("text");
            var imgList = xmld.GetElementsByTagName("image");
            if(myViewModels.allPosts.Count == 0)
            {
                return;
            }else if(tick_id < myViewModels.allPosts.Count)
            {
                try
                {
                    textList[0].InnerText = myViewModels.allPosts[tick_id].poster_name;
                    textList[1].InnerText = myViewModels.allPosts[tick_id].post_name;
                    textList[2].InnerText = myViewModels.allPosts[tick_id].poster_name;
                    textList[3].InnerText = myViewModels.allPosts[tick_id].post_name;
                    textList[4].InnerText = myViewModels.allPosts[tick_id].poster_name;
                    textList[5].InnerText = myViewModels.allPosts[tick_id].post_name;
                    textList[6].InnerText = myViewModels.allPosts[tick_id].poster_name;
                    textList[7].InnerText = myViewModels.allPosts[tick_id].post_name;
                    //Send Tile notification
                    var update_notification = new TileNotification(xmld);
                    TileUpdateManager.CreateTileUpdaterForApplication().Update(update_notification);
                    TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
                    tick_id++;
                }catch(Exception eq) { }
            }

        }

    }
}
