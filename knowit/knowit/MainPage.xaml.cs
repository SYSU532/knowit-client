using System;
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
            /*myViewModels.AddPost(1, "Jian Yang", "Not Hotdog", "5", "", "");
            myViewModels.AddPost(2, "Richard", "Pie Pieper", "2", "", "");
            myViewModels.AddPost(2, "Bighetti", "Nippler", "3", "", "");*/
        }
        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {

        }
        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {

        }
        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {

        }
        private void nvAll_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var item = args.InvokedItem;
            if (item.GetType() == typeof(Grid))
            {
                contentFrame.Navigate(typeof(UserPage));
            }
            else if((string)item == "home")
            {
                contentFrame.Navigate(typeof(MainPage));
            }
        }
        /*private void Post_Click(object sender, ItemClickEventArgs args)
        {
            // open post detail page
            contentFrame.Navigate(typeof(PostPage));
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(PostPage), null);
                Window.Current.Content = frame;
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        }*/

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
            contentFrame.Navigate(typeof(PostPageM));
        }
        private void On_Navigated(object sender, NavigationEventArgs e)
        {
            nvAll.IsBackEnabled = contentFrame.CanGoBack;
        }
    }
}
