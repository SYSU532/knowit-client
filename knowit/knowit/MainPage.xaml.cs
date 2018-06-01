﻿using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using System.ComponentModel;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.ApplicationModel.Core;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;
using Windows.UI.ViewManagement;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using knowit.ViewModels;
using Windows.UI.Core;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace knowit
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // ListItemViewModels myViewModels = ListItemViewModels.GetInstance();
        private string username;
        private string password;

        public MainPage()
        {
            this.InitializeComponent();
            //draw into the title bar
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            //remove the solid-colored backgrounds behind the caption controls and system back button
            var viewTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.ButtonBackgroundColor = Colors.Transparent;
            viewTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            viewTitleBar.ButtonForegroundColor = (Color)Resources["SystemBaseHighColor"];

            
        }
        private async void InitializeUser()
        {
            usernameText.Text = username;
            Dictionary<string, string> info = await NetworkControl.QueryUserInfo(username);
            string imageUrl = info.GetValueOrDefault<string, string>("imageUrl");
            var newSrc = new BitmapImage(new Uri(NetworkControl.GetFullPathUrl(imageUrl), UriKind.Absolute));
            newSrc.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            personPic.ProfilePicture = newSrc;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string[] user_info = e.Parameter as string[];
            username = user_info[0];
            password = user_info[1];
            InitializeUser();
        }
        private void nvAll_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            string[] info = new string[2];
            info[0] = username;
            info[1] = password;
            var item = args.InvokedItem;
            if(item.GetType() == typeof(Grid))
            {
                contentFrame.Navigate(typeof(UserPage), info);
            }
            switch (item)
            {
                case "注销":
                    contentFrame.Navigate(typeof(SigninPage));
                    Frame.Navigate(typeof(SigninPage));
                    break;
                case "创建帖子":
                    contentFrame.Navigate(typeof(CreatePostPage), info);
                    break;
                case "主页":
                    contentFrame.Navigate(typeof(PostPageM), info);
                    break;
            }
        }
        private void MoreInfoBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void nvAll_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            On_BackRequested();
        }
        private bool On_BackRequested()
        {
            bool navigated = false;

            // don't go back if the nav pane is overlayed
            if (nvAll.IsPaneOpen && (nvAll.DisplayMode == NavigationViewDisplayMode.Compact || nvAll.DisplayMode == NavigationViewDisplayMode.Minimal))
            {
                return false;
            }
            else
            {
                if (contentFrame.CanGoBack)
                {
                    contentFrame.GoBack();
                    navigated = true;
                }
            }
            return navigated;
        }
        private void nvAll_Loaded(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigated += On_Navigated;
            string[] temp = { username, password };
            contentFrame.Navigate(typeof(PostPageM), temp);
            foreach (NavigationViewItemBase item in nvAll.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "home")
                {
                    nvAll.SelectedItem = item;
                    break;
                }
            }
        }
        private void On_Navigated(object sender, NavigationEventArgs e)
        {
            nvAll.IsBackEnabled = contentFrame.CanGoBack;
            Dictionary<Type, string> lookup = new Dictionary<Type, string>()
            {
                {typeof(PostPageM), "home"},
                {typeof(UserPage), "user"},
                {typeof(CreatePostPage), "create"},
                {typeof(PostPage), "post"},
                {typeof(SigninPage), "signin"},
            };
            String stringTag = lookup[contentFrame.SourcePageType];

            // set the new SelectedItem  
            foreach (NavigationViewItemBase item in nvAll.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.Equals(stringTag))
                {
                    item.IsSelected = true;
                    break;
                }
            }
        }
    }
}
