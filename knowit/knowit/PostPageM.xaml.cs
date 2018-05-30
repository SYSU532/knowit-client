using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using knowit.ViewModels;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;

namespace knowit
{
    public sealed partial class PostPageM : Page
    {
        private string username;
        private string password;
        ListItemViewModels myViewModels = ListItemViewModels.GetInstance();
        public PostPageM()
        {
            this.InitializeComponent();

        }
        private void Post_Click(object sender, ItemClickEventArgs args)
        {
            this.Frame.Navigate(typeof(PostPage));
        }
        private async void InitializePost()
        {
            ObservableCollection<PostItem> posts = await NetworkControl.GetPostCollection(username, password);
            if(posts != null)
            {
                foreach (PostItem item in posts)
                {
                    string thumbs_num = item.thumbs.ToString();
                    myViewModels.AddPost(item.editor, item.title, thumbs_num, item.imageURL, item.videoURL);
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
    }
}
