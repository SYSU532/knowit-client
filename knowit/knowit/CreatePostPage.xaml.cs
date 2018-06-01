using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using knowit.ViewModels;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Media.Core;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace knowit
{
    public sealed partial class CreatePostPage : Page
    {
        private string username;
        private string password;
        private StorageFile media = null;
        public CreatePostPage()
        {
            InitializeComponent();

        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string[] user_info = e.Parameter as string[];
            username = user_info[0];
            password = user_info[1];
        }
        private async void Add_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".wmv");
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".mkv");
            openPicker.FileTypeFilter.Add(".avi");
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".gif");
            openPicker.FileTypeFilter.Add(".tiff");
            openPicker.FileTypeFilter.Add(".bmp");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if(file != null)
            {
                media = file;
                if(file.FileType == ".wmv" || file.FileType == ".mp4" || file.FileType == ".mkv" || file.FileType == ".avi")
                {
                    video.Visibility = Visibility.Visible;
                    video.Source = MediaSource.CreateFromStorageFile(file);
                }
                else
                {
                    image.Visibility = Visibility.Visible;
                    SoftwareBitmap softwareBitmap;
                    IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                    softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                    if (softwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 ||
                        softwareBitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
                    {
                        softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                    }
                    var source = new SoftwareBitmapSource();
                    await source.SetBitmapAsync(softwareBitmap);
                    image.Source = source;
                }
            }
        }

        private async void Create_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            string pass = null;
            passage.Document.GetText(Windows.UI.Text.TextGetOptions.AdjustCrlf, out pass);
            if(pass == "")
            {
                MessageDialog dialog = new MessageDialog("帖子内容不能为空！");
                await dialog.ShowAsync();
                return;
            }
            Dictionary<string, string> info = await NetworkControl.PublishPost(username, password, title.Text, pass, media);
            if(info["code"] != "1")
            {
                MessageDialog dialog = new MessageDialog("创建帖子失败！");
                await dialog.ShowAsync();
            }
            else
            {
                string[] temp = new string[2];
                temp[0] = username;
                temp[1] = password;
                MessageDialog dialog = new MessageDialog("创建帖子成功！");
                await dialog.ShowAsync();
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(MainPage), temp);
            }
        }
    }
}
