using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Generic;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Popups;

namespace knowit
{
    public sealed partial class UserPage : Page
    {
        private string username;
        private string password;
        private StorageFile CurrentPic = null;
        public UserPage()
        {
            this.InitializeComponent();
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            string[] temp = e.Parameter as string[];
            username = temp[0];
            password = temp[1];
            Dictionary<string, string> info = await NetworkControl.QueryUserInfo(username);
            string imageUrl = info.GetValueOrDefault<string, string>("imageUrl");
            string phone = info.GetValueOrDefault<string, string>("phone");
            string email = info.GetValueOrDefault<string, string>("email");
            var newSrc = new BitmapImage(new Uri(NetworkControl.GetFullPathUrl(imageUrl), UriKind.Absolute));
            newSrc.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            personPic.ProfilePicture = newSrc;
            usernameBlock.Text = username;
            phoneBlock.Text = phone;
            emailBlock.Text = email;
        }
        

        private async void Commit_Change(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> info = await NetworkControl.UserInfoModify(username, password, phoneBlock.Text, emailBlock.Text, CurrentPic);
            await WebView.ClearTemporaryWebDataAsync();
            
            string status = info["code"];
            if(status != "1")
            {
                MessageDialog dialog = new MessageDialog("修改失败！");
                await dialog.ShowAsync();
            }
            else
            {
                string[] temp = new string[2];
                temp[0] = username;
                temp[1] = password;
                MessageDialog dialog = new MessageDialog("修改成功！");
                await dialog.ShowAsync();
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(MainPage), temp);
            }
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".gif");
            openPicker.FileTypeFilter.Add(".tiff");
            openPicker.FileTypeFilter.Add(".bmp");

            StorageFile file = await openPicker.PickSingleFileAsync();
            CurrentPic = file;
            if (file != null)
            {
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
                personPic.ProfilePicture = source;
            }
        }

        private void TextBlock_Tapped_1(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void TextBlock_Tapped_2(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
    }
}
