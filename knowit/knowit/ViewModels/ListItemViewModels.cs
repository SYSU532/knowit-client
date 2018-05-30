using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml.Media;

namespace knowit.ViewModels
{
    class ListItemViewModels
    {
        //单例模式
        private static ListItemViewModels instance;
        private ListItemViewModels() { }
        public static ListItemViewModels GetInstance()
        {
            if (instance == null)
            {
                instance = new ListItemViewModels();
            }
            return instance;
        }

        public ObservableCollection<Models.ListItem> allPosts = new ObservableCollection<Models.ListItem>();

        //添加帖子
        public void AddPost(string poster_name, string post_name, string thumbs_num, string image_url, string video_url)
        {
            this.allPosts.Add(new Models.ListItem(poster_name, post_name, thumbs_num, image_url, video_url));
        }
    }
}
