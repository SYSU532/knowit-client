using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using knowit.ViewModels;

namespace knowit
{
    public sealed partial class PostPageM : Page
    {
        ListItemViewModels myViewModels = ListItemViewModels.GetInstance();
        public PostPageM()
        {
            this.InitializeComponent();
            myViewModels.AddPost(1, "Jian Yang", "Not Hotdog", "5", "", "");
            myViewModels.AddPost(2, "Richard", "Pie Pieper", "2", "", "");
            myViewModels.AddPost(2, "Bighetti", "Nippler", "3", "", "");
        }
        private void Post_Click(object sender, ItemClickEventArgs args)
        {
            this.Frame.Navigate(typeof(PostPage));
        }
    }
}
