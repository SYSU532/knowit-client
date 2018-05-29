using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace knowit.Models
{
    class ListItem
    {
        private int post_id;
        public string poster_name;
        public string post_name;
        public string thumbs_num;
        private string image_url = "";
        private string video_url = "";
        public ListItem(int post_id, string poster_name, string post_name, string thumbs_num, string image_url, string video_url)
        {
            this.post_id = post_id;
            this.poster_name = poster_name;
            this.post_name = post_name;
            this.thumbs_num = thumbs_num;
            this.image_url = image_url;
            this.video_url = video_url;
        }
    }
}
