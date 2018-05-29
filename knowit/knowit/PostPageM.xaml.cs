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

        }
        private void Post_Click(object sender, ItemClickEventArgs args)
        {
            this.Frame.Navigate(typeof(PostPage));
        }
    }
}
