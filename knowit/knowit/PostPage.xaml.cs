
using System;
using System.Collections.Generic;
using Windows.Media.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace knowit
{
    public sealed partial class PostPage : Page
    {
        private string username;
        private string password;
        private string id;
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
                Run run = new Run
                {
                    Text = post["content"] as string
                };
                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(run);
                passage.Blocks.Add(paragraph);
                if ((string)post["image"] != "")
                {
                    video.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    var imageUrl = (string)post["image"];
                    var newSrc = new BitmapImage(new Uri(NetworkControl.GetFullPathUrl(imageUrl), UriKind.Absolute));
                    newSrc.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    image.Source = newSrc;
                }
                else if((string)post["media"] != "")
                {
                    image.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    var videoUrl = (string)post["media"];
                    var media = MediaSource.CreateFromUri(new Uri(NetworkControl.GetFullPathUrl(videoUrl), UriKind.Absolute));
                    video.Source = media;
                }
            }

        }
        private void AddComment_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }
    }
}
