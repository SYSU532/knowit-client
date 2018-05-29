using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace knowit
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SignupPage : Page
    {
        public SignupPage()
        {
            this.InitializeComponent();
            InitializeFrostedGlass(GlassHost);
        }

        private StorageFile thumbFile = null;

        private void InitializeFrostedGlass(UIElement glassHost)
        {
            Visual hostVisual = ElementCompositionPreview.GetElementVisual(glassHost);
            Compositor compositor = hostVisual.Compositor;
            var backdropBrush = compositor.CreateHostBackdropBrush();
            var glassVisual = compositor.CreateSpriteVisual();
            glassVisual.Brush = backdropBrush;
            ElementCompositionPreview.SetElementChildVisual(glassHost, glassVisual);
            var bindSizeAnimation = compositor.CreateExpressionAnimation("hostVisual.Size");
            bindSizeAnimation.SetReferenceParameter("hostVisual", hostVisual);
            glassVisual.StartAnimation("Size", bindSizeAnimation);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string initName = e.Parameter as string;
            if (initName != null)
            {
                UsernameBox.Text = initName;
            }
        }

        private Regex usernamePattern = new Regex("^[a-zA-Z][0-9a-zA-Z]\\w{4,16}");
        private Regex passwordPattern = new Regex("^[1-9a-zA-Z-_]{6,12}");
        private Regex emailPattern = new Regex("^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\\.[a-zA-Z0-9_-]+)+$");
        private Regex phonePattern = new Regex("^[1-9]\\d{10}");

        private void UsernameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var box = (TextBox)sender;
            if (box != null)
            {
                if (usernamePattern.IsMatch(box.Text))
                {
                    UsernameMsg.Text = "OK!";
                    UsernameMsg.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    UsernameMsg.Text = "Username Invalid!";
                    UsernameMsg.Foreground = new SolidColorBrush(Colors.Red);
                }
            }
        }


        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var box = (PasswordBox)sender;
            if (box != null)
            {
                if (passwordPattern.IsMatch(box.Password))
                {
                    PasswordMsg.Text = "OK!";
                    PasswordMsg.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    PasswordMsg.Text = "Password Invalid!";
                    PasswordMsg.Foreground = new SolidColorBrush(Colors.Red);
                }
            }
        }

        private void SecondPasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!passwordPattern.IsMatch(PasswordBox.Password))
            {
                SecondPasswordMsg.Text = "Password Invalid!";
                SecondPasswordMsg.Foreground = new SolidColorBrush(Colors.Red);
            }
            else if (PasswordBox.Password == SecondPasswordBox.Password)
            {
                SecondPasswordMsg.Text = "OK!";
                SecondPasswordMsg.Foreground = new SolidColorBrush(Colors.Green);
            }
            else
            {
                SecondPasswordMsg.Text = "Passwords Don't Match!";
                SecondPasswordMsg.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private void PhoneBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var box = (TextBox)sender;
            if (box != null)
            {
                if (phonePattern.IsMatch(box.Text))
                {
                    PhoneMsg.Text = "OK!";
                    PhoneMsg.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    PhoneMsg.Text = "Phone Number Invalid!";
                    PhoneMsg.Foreground = new SolidColorBrush(Colors.Red);
                }
            }
        }

        private void EmailBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var box = (TextBox)sender;
            if (box != null)
            {
                if (emailPattern.IsMatch(box.Text))
                {
                    EmailMsg.Text = "OK!";
                    EmailMsg.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    EmailMsg.Text = "Email Address Invalid!";
                    EmailMsg.Foreground = new SolidColorBrush(Colors.Red);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null) return;
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            if (!usernamePattern.IsMatch(UsernameBox.Text))
                builder.Append("Username Invalid!\n");
            if (!passwordPattern.IsMatch(PasswordBox.Password))
                builder.Append("Password Invalid!\n");
            if (PasswordBox.Password != SecondPasswordBox.Password)
                builder.Append("The Two Passwords Don't Match!\n");
            if (!emailPattern.IsMatch(EmailBox.Text))
                builder.Append("Email Invalid!\n");
            if (!phonePattern.IsMatch(PhoneBox.Text))
                builder.Append("Phone Number Invalid!\n");
            if (builder.Length != 0)
            {
                var alert = new MessageDialog(builder.ToString(), "Error!");
                await alert.ShowAsync();
            }
            else {
                if (thumbFile == null)
                {
                    thumbFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/jian-yang.png"));
                }
                var res = await NetworkControl.AttemptSignUp(UsernameBox.Text, PasswordBox.Password, SecondPasswordBox.Password,
                                                        PhoneBox.Text, EmailBox.Text, thumbFile);
                string msg = "";
                bool success = false;
                if (res["code"] == "-1")
                {
                    msg = "Connection to server appears to be down, please check.";
                }
                else if (res["code"] == "-2")
                {
                    msg = "Ughh... The server doesn't seem to be a Know-it Server, please check again.";
                }
                else if (res["code"] == "0")
                {
                    msg = res["errMessage"];
                }
                else if (res["code"] == "1")
                {
                    msg = "Success!";
                    success = true;
                }
                MessageDialog dialog = new MessageDialog(msg);
                await dialog.ShowAsync();
                if (success)
                {
                    string[] temp = new string[2];
                    temp[0] = UsernameBox.Text;
                    temp[1] = PasswordBox.Password;
                    Frame.Navigate(typeof(MainPage), temp);
                    //go to content page, passing a pair with username as key and password as value as parameter.
                }

            }
        }

        private async void FileSelect_Click(object sender, RoutedEventArgs e)
        {
            var fop = new FileOpenPicker();
            fop.FileTypeFilter.Add(".jpg");
            fop.FileTypeFilter.Add(".jpeg");
            fop.FileTypeFilter.Add(".png");
            fop.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            var file = await fop.PickSingleFileAsync();
            if (file != null)
            {

                IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);

                // Set the image source to the selected bitmap
                BitmapImage bitmapImage = new BitmapImage();

                await bitmapImage.SetSourceAsync(fileStream);
                AvatarImage.Source = bitmapImage;

                thumbFile = file;
            }
        }
    }
}
