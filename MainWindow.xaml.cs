using Ankara_Online.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Diagnostics;
using System.Linq;
using Windows.UI.ApplicationSettings;

namespace Ankara_Online
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            _navigationView.Loaded += NavigationView_Loaded;
            this.Activated += MainWindow_Activated;
            //homeViewItem.PointerPressed += HomePageItem_PointerPressed;
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 1200, Height = 900 });
            //FontAwesome6.Fonts.FontAwesomeFonts.LoadAllStyles(new Uri("ms-appx:///Assets//Fonts/"));
            this.contentFrame = new Frame();
            contentFrame.Navigate(typeof(HomePageView));

        }
        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var item = sender.MenuItems.OfType<NavigationViewItem>().First(x => (string)x.Content == (string)args.InvokedItem);
            NavigationView_Navigate(item as NavigationViewItem);
        }
        private void NavigationView_Navigate(NavigationViewItem item)
        {
            switch(item.Tag)
            {
                case "homePageView":
                    contentFrame.Navigate(typeof(HomePageView));
                    break;

                case "softwareSectorFileView":
                    contentFrame.Navigate(typeof(SectorFilesView));
                    break;

                case "DocumentsView":
                    contentFrame.Navigate(typeof(DocumentsView));
                    break;

                case "NOTAMSViews":
                    contentFrame.Navigate(typeof(NotamsView));
                    break;

                case "SettingsView":
                    contentFrame.Navigate(typeof(SettingsView));
                    break;
            }
        }

        private void contentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load page " +e.SourcePageType.FullName);
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            this.contentFrame.Navigate(typeof(HomePageView));
        }

        private void NavigationView_Loaded(object sender, RoutedEventArgs args)
        {
            this.contentFrame.Navigate(typeof(HomePageView));
        }
    }
}
