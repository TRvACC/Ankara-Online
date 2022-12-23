using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;
using WinUIEx;

namespace Ankara_Online
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WindowEx
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.CenterOnScreen();

            this.SetIcon("Assets/trvacc_icon_transparent.ico");

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
                    contentFrame.Navigate(typeof(TrainingView));
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

        private void NavigationView_Loaded(object sender, RoutedEventArgs args)
        {
            this.contentFrame.Navigate(typeof(HomePageView));
        }
        internal static IntPtr _hWnd;
    }
}
