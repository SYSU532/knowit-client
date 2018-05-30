using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace knowit
{
    class PostItem
    {
        public string content;
        public string editor;
        public string imageURL;
        public List<KeyValuePair<String, String>> comments;
        public string videoURL;
        public int thumbs;
        public string title;
        public string id;
        public bool hasImage { get { return imageURL != ""; } }
        public bool hasVideo { get { return videoURL != ""; } }
        public PostItem(string id, string con, string ti, string edi, string img, string vid, int thu, List<KeyValuePair<String, String>> com)
        {
            this.id = id;
            content = con;
            title = ti;
            editor = edi;
            imageURL = img;
            videoURL = vid;
            thumbs = thu;
            comments = com;
        }
    }
}
