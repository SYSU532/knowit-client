
using System;
using System.Collections.Generic;
using Windows.Media.Core;
using Windows.Media.Streaming.Adaptive;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

namespace knowit
{
    public sealed partial class PostPage : Page
    {
        private string username;
        private string password;
        private string id;
        private int thumb_click = 0;
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
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    video?.MediaPlayer.Dispose();
                });
            }
            if (thumb_click == 1)
            {
                Dictionary<string, string> info = await NetworkControl.GiveThumbToPost(username, password, id);
            }
        }
        private async void InitializePost()
        {

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
            }

        }
        private void AddComment_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }

        private void thumb_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            int res = 0;
            int.TryParse(thumb_num.Text, out res);
            if(thumb_click == 0)
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
