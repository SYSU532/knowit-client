
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Media.Core;
using Windows.Media.Streaming.Adaptive;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

namespace knowit
{
    class CommentItem
    {
        public string username;
        public string comment;
        public CommentItem(string username, string comment)
        {
            this.username = username;
            this.comment = comment;
        }
    }
    class CommentItemViewModels
    {
        //单例模式
        private static CommentItemViewModels instance;
        private CommentItemViewModels() { }
        public static CommentItemViewModels GetInstance()
        {
            if (instance == null)
            {
                instance = new CommentItemViewModels();
            }
            return instance;
        }
        public ObservableCollection<CommentItem> allComments = new ObservableCollection<CommentItem>();
        //添加评论
        public void AddComment(string username, string content)
        {
            allComments.Add(new CommentItem(username, content));
        }
    }
    public sealed partial class PostPage : Page
    {
        private string username;
        private string password;
        private string id;
        private Boolean hasThumb = false;
        private int thumb_click = 0;
        CommentItemViewModels myViewModels = CommentItemViewModels.GetInstance();
        public PostPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string[] info = e.Parameter as string[];
            username = info[0];
            password = info[1];
            id = info[2];
            InitializePost();
        }
        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            
            if (video.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                video.Source = null;
            }
            if (thumb_click == 1 && hasThumb != true)
            {
                Dictionary<string, string> info = await NetworkControl.GiveThumbToPost(username, password, id);
            }
            else if(thumb_click == 0 && hasThumb == true)
            {
                Dictionary<string, string> info = await NetworkControl.CancelThumbToPost(username, password, id);
            }
        }
        private async void InitializePost()
        {
            myViewModels.allComments.Clear();
            hasThumb = await NetworkControl.CheckUserThumbOrNot(username, id);
            if (hasThumb) thumb_click = 1;
            Dictionary<string, object> post = await NetworkControl.GetPostFromID(username, password, id);
            if((string)post["code"] != "1")
            {
                //
            }
            else
            {
                title.Text = post["title"] as string;
                author.Text = post["editor"] as string;
                thumb_num.Text = post["thumbs"] as string;
                List<KeyValuePair<String, String>> commentDict = post["comment"] as List<KeyValuePair<String, String>>;
                Run run = new Run
                {
                    Text = post["content"] as string
                };
                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(run);
                passage.Blocks.Add(paragraph);
                if ((string)post["image"] != "")
                {
                    image.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    var imageUrl = (string)post["image"];
                    var newSrc = new BitmapImage(new Uri(NetworkControl.GetFullPathUrl(imageUrl), UriKind.Absolute));
                    newSrc.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    image.Source = newSrc;
                }
                else if((string)post["media"] != "")
                {
                    video.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    var videoUrl = (string)post["media"];
                    video.Source = MediaSource.CreateFromUri(new Uri(NetworkControl.GetFullPathUrl(videoUrl), UriKind.Absolute));
                }
                foreach(var comment in commentDict)
                {
                    myViewModels.AddComment(comment.Key, comment.Value);
                }
            }

        }
        private async void AddComment_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            string mess = null;
            comment.Document.GetText(Windows.UI.Text.TextGetOptions.AdjustCrlf, out mess);
            if (mess == "")
            {
                MessageDialog dialog = new MessageDialog("评论不能为空！");
                await dialog.ShowAsync();
            }
            else
            {
                myViewModels.AddComment(username, mess);
                Dictionary<string, string> info = await NetworkControl.PostComment(username, password, id, mess);
                comment.Document.SetText(Windows.UI.Text.TextSetOptions.ApplyRtfDocumentDefaults, "");
            }
        }

        private void thumb_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            int res = 0;
            int.TryParse(thumb_num.Text, out res);
            if (thumb_click == 0)
            {
                thumb_num.Text = (++res).ToString();
                thumb_click++;
            }
            else
            {
                thumb_num.Text = (--res).ToString();
                thumb_click--;
            }
        }
    }
}
